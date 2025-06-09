using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    // ������ �� ��������� ������ � ��������
    public DroneManager droneManager;
    public ResourceManager resourceManager;

    [Header("UI")]
    // UI-�������� ����������
    public Slider droneCountSlider;       
    public Slider speedSlider;            
    public TMP_InputField spawnRateInput; 
    public Toggle pathToggle;     
    
    // ������ ��� ����������� ����� �������
    public TMP_Text redScoreText;
    public TMP_Text blueScoreText;

    private int redScore = 0;
    private int blueScore = 0;

    public Slider speedSimSlider; // ������� ��� ���������� ��������� ���� ��������� (Time.timeScale)


    void Awake()
    {
        // ���� ��� ���� ������ ���������, ���������� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // ���������� ������ ��� ������
        droneManager.GenerateDrones();

        // ����������� UI �� ������-�����������
        UpdateUIFromDefaults();

        speedSimSlider.onValueChanged.AddListener(OnSpeedChanged);
        speedSimSlider.value = 1f; 
    }

    void UpdateUIFromDefaults()
    {
        // ������������� �� ��������� �������� UI-���������
        droneCountSlider.onValueChanged.AddListener(SetDroneCount);
        speedSlider.onValueChanged.AddListener(SetDroneSpeed);
        spawnRateInput.onValueChanged.AddListener(SetSpawnRate);
        pathToggle.onValueChanged.AddListener(SetPathVisibility);
    }

    // ����������� ��������� UI:

    void SetDroneCount(float value)
    {
        // �������� ���������� ������ �� �������, ���������� ��
        droneManager.SetDroneCountPerFaction((int)value);
    }

    void SetDroneSpeed(float value)
    {
        // ������������� �������� ������
        droneManager.SetDroneSpeed(value);
    }

    void SetSpawnRate(string value)
    {
        // �������� ����� �� ���� � ������������� �������� ��������� ��������
        if (float.TryParse(value, out float rate))
        {
            resourceManager.SetSpawnInterval(rate);
        }
    }

    void SetPathVisibility(bool enabled)
    {
        // ��������/��������� ��������� ���� ������
        droneManager.TogglePathRendering(enabled);
    }

    public void ReportResourceCollected(Faction faction)
    {
        if (faction == Faction.Red) redScore++;
        else if (faction == Faction.Blue) blueScore++;

        // ��������� UI-����
        redScoreText.text = $"�������: {redScore}";
        blueScoreText.text = $"�����: {blueScore}";
    }

    // ���������� ��������� ��������� 
    void OnSpeedChanged(float value)
    {
        Time.timeScale = Mathf.Clamp(value, 0f, 5f);
    }

    public void QuitGame()
    {
        Debug.Log("����� �� ����...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
