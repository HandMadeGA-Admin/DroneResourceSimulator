using UnityEngine;
using System.Collections.Generic;

public class DroneManager : MonoBehaviour
{
    // ������� ��� ������ 
    public GameObject redDronePrefab;
    public GameObject blueDronePrefab;

    // ������� ���
    public Transform redBase;
    public Transform blueBase;

    // ���������� ������
    public int dronesPerFaction = 3;

    // �������� �������� ������ 
    public float droneSpeed = 3f;

    // ������ ���� ������������ ������
    private List<DroneController> drones = new();

    // ����� �������� ������ 
    public void GenerateDrones()
    {
        ClearAllDrones();

        // ������ �������� ���������� ������ ��� ������ �������
        for (int i = 0; i < dronesPerFaction; i++)
        {
            SpawnDrone(redDronePrefab, redBase, Faction.Red);
            SpawnDrone(blueDronePrefab, blueBase, Faction.Blue);
        }
    }

    // ����� ��� ������ �����
    void SpawnDrone(GameObject prefab, Transform baseTransform, Faction faction)
    {
        GameObject obj = Instantiate(prefab, baseTransform.position, Quaternion.identity);
        DroneController ctrl = obj.GetComponent<DroneController>();

        // ������������� � ����� ����, ������� � ��������
        ctrl.homeBase = baseTransform;
        ctrl.faction = faction;
        ctrl.speed = droneSpeed;

        drones.Add(ctrl);
    }

    public void SetDroneCountPerFaction(int count)
    {
        dronesPerFaction = count;
        GenerateDrones();
    }

    public void SetDroneSpeed(float speed)
    {
        droneSpeed = speed;
        foreach (var d in drones)
            d.speed = speed;
    }

    public void TogglePathRendering(bool enabled)
    {
        foreach (var d in drones)
        {
            if (d.TryGetComponent<DronePathRenderer>(out var path))
            {
                path.enabled = enabled;
            }
        }
    }

    void ClearAllDrones()
    {
        foreach (var d in drones)
            if (d != null) Destroy(d.gameObject);
        drones.Clear();
    }
}
