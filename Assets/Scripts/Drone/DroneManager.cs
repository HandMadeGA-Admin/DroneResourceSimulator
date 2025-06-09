using UnityEngine;
using System.Collections.Generic;

public class DroneManager : MonoBehaviour
{
    // Префабы для дронов 
    public GameObject redDronePrefab;
    public GameObject blueDronePrefab;

    // Позиции баз
    public Transform redBase;
    public Transform blueBase;

    // Количество дронов
    public int dronesPerFaction = 3;

    // Скорость движения дронов 
    public float droneSpeed = 3f;

    // Список всех контроллеров дронов
    private List<DroneController> drones = new();

    // Метод создания дронов 
    public void GenerateDrones()
    {
        ClearAllDrones();

        // Создаём заданное количество дронов для каждой фракции
        for (int i = 0; i < dronesPerFaction; i++)
        {
            SpawnDrone(redDronePrefab, redBase, Faction.Red);
            SpawnDrone(blueDronePrefab, blueBase, Faction.Blue);
        }
    }

    // Метод для спавна дрона
    void SpawnDrone(GameObject prefab, Transform baseTransform, Faction faction)
    {
        GameObject obj = Instantiate(prefab, baseTransform.position, Quaternion.identity);
        DroneController ctrl = obj.GetComponent<DroneController>();

        // Устанавливаем у дрона базу, фракцию и скорость
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
