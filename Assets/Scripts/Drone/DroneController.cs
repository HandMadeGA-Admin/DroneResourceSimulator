using System.Collections;
using UnityEngine;
using System.Linq;

public class DroneController : MonoBehaviour
{
    // �������� ����������� �����
    public float speed = 3f;

    // ����� ���� 
    public Transform homeBase;

    // �������
    public Faction faction;

    // ������ �������� ��� ������� ��������
    public GameObject deliveryEffectPrefab;

    // ������� ��������� �����
    private DroneState state = DroneState.Searching;

    // ������� ������ ��� �����
    private ResourceNode currentResource;

    // ������� �������
    private Vector3? targetPosition;

    // ��������� ��� ��������� ������������
    public float avoidRadius = 1.2f;
    public float avoidStrength = 1.5f;

    // ������ �� SpriteRenderer ��� ��������� ���������
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
            Debug.LogWarning("Indicator SpriteRenderer �� ������ � �������� �������� �����!");
        }
        UpdateStateColor();
    }

    void Update()
    {
        switch (state)
        {
            case DroneState.Searching:
                FindResource(); // ���� ������
                break;

            case DroneState.FlyingToResource:
                MoveToTarget(); // ����� � �������
                if (ReachedTarget())
                {
                    state = DroneState.Collecting;
                    UpdateStateColor();
                    StartCoroutine(CollectResource()); // �������� ������
                }
                break;

            case DroneState.Collecting:
                break;

            case DroneState.Returning:
                MoveToTarget();
                if (ReachedTarget())
                {
                    // ��������� ������
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

        // �������� ��������� ��������� ������
        currentResource = allResources.OrderBy(r => Vector3.Distance(transform.position, r.transform.position)).First();
        currentResource.isTaken = true; 

        targetPosition = currentResource.transform.position; 
        state = DroneState.FlyingToResource; 
        UpdateStateColor();
    }

    void MoveToTarget()
    {
        if (!targetPosition.HasValue) return;

        // ����������� �������� � ���� (��������������� ������)
        Vector3 moveDir = (targetPosition.Value - transform.position).normalized;

        // ������ ��� ��������� ������������ � ������� �������
        Vector3 avoidDir = GetAvoidanceDirection();

        // �������� ����������� � ������ ���������
        Vector3 finalDir = (moveDir + avoidDir * avoidStrength).normalized;

        // ���������� ����� � �������� ����������� � �������� ���������
        transform.position += finalDir * speed * Time.deltaTime;
    }

    bool ReachedTarget()
    {
        // ���������, ������ �� ���� ������� ���� (������ 0.1 �����)
        return targetPosition.HasValue && Vector3.Distance(transform.position, targetPosition.Value) < 0.1f;
    }

    IEnumerator CollectResource()
    {
        // ����� ����� ������� (2 �������)
        yield return new WaitForSeconds(2f);

        if (currentResource != null)
        {
            Destroy(currentResource.gameObject); // ������� ������ ������� � �����
            GameManager.Instance.ReportResourceCollected(faction); // �������� ��������� ���� � ����� �������
            currentResource = null;
        }

        // ����� ��������� �������� ������ ����, ���� ���� ������� ������������
        Vector2 offset = Random.insideUnitCircle * 1.5f;
        targetPosition = homeBase.position + new Vector3(offset.x, offset.y, 0);

        // ������ ��������� �� �����������
        state = DroneState.Returning;
        UpdateStateColor();
    }

    IEnumerator UnloadResource()
    {
        if (deliveryEffectPrefab != null)
        {
            // ������������� ������ ��������
            var effect = Instantiate(deliveryEffectPrefab, homeBase.position, Quaternion.identity);
        }

        // ����� ��������� (0.5 �������)
        yield return new WaitForSeconds(0.5f);

        targetPosition = null;
        state = DroneState.Searching;
        UpdateStateColor();
    }

    Vector3 GetAvoidanceDirection()
    {
        Vector3 avoid = Vector3.zero;

        // �������� ��� ����� �� �����
        var drones = FindObjectsOfType<DroneController>();

        foreach (var other in drones)
        {
            if (other == this) continue; // ���������� ������ ����

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < avoidRadius)
            {
                // ��������� ������ ������������ �� ������� �����
                Vector3 away = (transform.position - other.transform.position).normalized;
                avoid += away * (1f - dist / avoidRadius);
            }
        }

        return avoid;
    }

    private void UpdateStateColor()
    {
        if (stateRenderer == null) return;

        // ��������� ���� ���������� 
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
