using System.Collections;
using UnityEngine;
using System.Linq;

public class DroneController : MonoBehaviour
{
    // Скорость перемещения дрона
    public float speed = 3f;

    // Точка базы 
    public Transform homeBase;

    // Фракция
    public Faction faction;

    // Префаб партикла для эффекта доставки
    public GameObject deliveryEffectPrefab;

    // Текущие состояния дрона
    private DroneState state = DroneState.Searching;

    // Текущий ресурс для сбора
    private ResourceNode currentResource;

    // Целевая позиция
    private Vector3? targetPosition;

    // Параметры для избегания столкновений
    public float avoidRadius = 1.2f;
    public float avoidStrength = 1.5f;

    // Ссылка на SpriteRenderer для индикации состояния
    public SpriteRenderer stateRenderer;

    public bool HasTarget => targetPosition.HasValue;

    public Vector3? CurrentTarget
    {
        get
        {
            if (state == DroneState.FlyingToResource || state == DroneState.Returning)
                return targetPosition;
            else
                return null;
        }
    }

    void Awake()
    {
        if (stateRenderer == null)
        {
            Debug.LogWarning("Indicator SpriteRenderer не найден в дочерних объектах дрона!");
        }
        UpdateStateColor();
    }

    void Update()
    {
        switch (state)
        {
            case DroneState.Searching:
                FindResource(); // Ищем ресурс
                break;

            case DroneState.FlyingToResource:
                MoveToTarget(); // Летим к ресурсу
                if (ReachedTarget())
                {
                    state = DroneState.Collecting;
                    UpdateStateColor();
                    StartCoroutine(CollectResource()); // Собираем ресурс
                }
                break;

            case DroneState.Collecting:
                break;

            case DroneState.Returning:
                MoveToTarget();
                if (ReachedTarget())
                {
                    // Выгружаем ресурс
                    state = DroneState.Unloading;
                    UpdateStateColor();
                    StartCoroutine(UnloadResource());
                }
                break;

            case DroneState.Unloading:
                break;
        }
    }

    void FindResource()
    {
        var allResources = FindObjectsOfType<ResourceNode>().Where(r => !r.isTaken).ToList();
        if (allResources.Count == 0)
            return;

        // Выбираем ближайший свободный ресурс
        currentResource = allResources.OrderBy(r => Vector3.Distance(transform.position, r.transform.position)).First();
        currentResource.isTaken = true; 

        targetPosition = currentResource.transform.position; 
        state = DroneState.FlyingToResource; 
        UpdateStateColor();
    }

    void MoveToTarget()
    {
        if (!targetPosition.HasValue) return;

        // Направление движения к цели (нормализованный вектор)
        Vector3 moveDir = (targetPosition.Value - transform.position).normalized;

        // Вектор для избежания столкновений с другими дронами
        Vector3 avoidDir = GetAvoidanceDirection();

        // Итоговое направление с учётом избежания
        Vector3 finalDir = (moveDir + avoidDir * avoidStrength).normalized;

        // Перемещаем дрона в итоговом направлении с заданной скоростью
        transform.position += finalDir * speed * Time.deltaTime;
    }

    bool ReachedTarget()
    {
        // Проверяем, достиг ли дрон позиции цели (меньше 0.1 юнита)
        return targetPosition.HasValue && Vector3.Distance(transform.position, targetPosition.Value) < 0.1f;
    }

    IEnumerator CollectResource()
    {
        // Время сбора ресурса (2 секунды)
        yield return new WaitForSeconds(2f);

        if (currentResource != null)
        {
            Destroy(currentResource.gameObject); // Удаляем объект ресурса с сцены
            GameManager.Instance.ReportResourceCollected(faction); // Сообщаем менеджеру игры о сборе ресурса
            currentResource = null;
        }

        // Задаём случайное смещение вокруг базы, куда дрон полетит возвращаться
        Vector2 offset = Random.insideUnitCircle * 1.5f;
        targetPosition = homeBase.position + new Vector3(offset.x, offset.y, 0);

        // Меняем состояние на возвращение
        state = DroneState.Returning;
        UpdateStateColor();
    }

    IEnumerator UnloadResource()
    {
        if (deliveryEffectPrefab != null)
        {
            // Воспроизводим эффект доставки
            var effect = Instantiate(deliveryEffectPrefab, homeBase.position, Quaternion.identity);
        }

        // Время разгрузки (0.5 секунды)
        yield return new WaitForSeconds(0.5f);

        targetPosition = null;
        state = DroneState.Searching;
        UpdateStateColor();
    }

    Vector3 GetAvoidanceDirection()
    {
        Vector3 avoid = Vector3.zero;

        // Получаем все дроны на сцене
        var drones = FindObjectsOfType<DroneController>();

        foreach (var other in drones)
        {
            if (other == this) continue; // Пропускаем самого себя

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < avoidRadius)
            {
                // Вычисляем вектор отталкивания от другого дрона
                Vector3 away = (transform.position - other.transform.position).normalized;
                avoid += away * (1f - dist / avoidRadius);
            }
        }

        return avoid;
    }

    private void UpdateStateColor()
    {
        if (stateRenderer == null) return;

        // Обновляем цвет индикатора 
        switch (state)
        {
            case DroneState.Searching:
                stateRenderer.color = Color.white;
                break;
            case DroneState.FlyingToResource:
                stateRenderer.color = Color.yellow;
                break;
            case DroneState.Collecting:
                stateRenderer.color = Color.gray;
                break;
            case DroneState.Returning:
                stateRenderer.color = Color.cyan;
                break;
            case DroneState.Unloading:
                stateRenderer.color = Color.green;
                break;
        }
    }
}
