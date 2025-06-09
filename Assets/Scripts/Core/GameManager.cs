using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    // Ссылки на менеджеры дронов и ресурсов
    public DroneManager droneManager;
    public ResourceManager resourceManager;

    [Header("UI")]
    // UI-элементы управления
    public Slider droneCountSlider;       
    public Slider speedSlider;            
    public TMP_InputField spawnRateInput; 
    public Toggle pathToggle;     
    
    // Тексты для отображения счёта фракций
    public TMP_Text redScoreText;
    public TMP_Text blueScoreText;

    private int redScore = 0;
    private int blueScore = 0;

    public Slider speedSimSlider; // слайдер для управления скоростью всей симуляции (Time.timeScale)


    void Awake()
    {
        // Если уже есть другой экземпляр, уничтожаем этот
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Генерируем дронов при старте
        droneManager.GenerateDrones();

        // Подписываем UI на методы-обработчики
        UpdateUIFromDefaults();

        speedSimSlider.onValueChanged.AddListener(OnSpeedChanged);
        speedSimSlider.value = 1f; 
    }

    void UpdateUIFromDefaults()
    {
        // Подписываемся на изменения значений UI-элементов
        droneCountSlider.onValueChanged.AddListener(SetDroneCount);
        speedSlider.onValueChanged.AddListener(SetDroneSpeed);
        spawnRateInput.onValueChanged.AddListener(SetSpawnRate);
        pathToggle.onValueChanged.AddListener(SetPathVisibility);
    }

    // Обработчики изменения UI:

    void SetDroneCount(float value)
    {
        // Изменяем количество дронов на фракцию, пересоздаём их
        droneManager.SetDroneCountPerFaction((int)value);
    }

    void SetDroneSpeed(float value)
    {
        // Устанавливаем скорость дронов
        droneManager.SetDroneSpeed(value);
    }

    void SetSpawnRate(string value)
    {
        // Получаем текст из поля и устанавливаем интервал появления ресурсов
        if (float.TryParse(value, out float rate))
        {
            resourceManager.SetSpawnInterval(rate);
        }
    }

    void SetPathVisibility(bool enabled)
    {
        // Включаем/выключаем отрисовку пути дронов
        droneManager.TogglePathRendering(enabled);
    }

    public void ReportResourceCollected(Faction faction)
    {
        if (faction == Faction.Red) redScore++;
        else if (faction == Faction.Blue) blueScore++;

        // Обновляем UI-счёт
        redScoreText.text = $"Красные: {redScore}";
        blueScoreText.text = $"Синие: {blueScore}";
    }

    // Управление скоростью симуляции 
    void OnSpeedChanged(float value)
    {
        Time.timeScale = Mathf.Clamp(value, 0f, 5f);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
