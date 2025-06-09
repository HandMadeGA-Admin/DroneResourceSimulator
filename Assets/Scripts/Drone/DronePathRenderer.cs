using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DronePathRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private DroneController drone;

    void Awake()
    {
        // Получаем компоненты LineRenderer и DroneController
        lineRenderer = GetComponent<LineRenderer>();
        drone = GetComponent<DroneController>();
    }

    void Update()
    {
        if (drone == null || !drone.CurrentTarget.HasValue)
        {
            lineRenderer.positionCount = 0;  // Линия очищается
            return;
        }

        // Устанавливаем линию из двух точек: текущая позиция дрона и позиция цели
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, drone.CurrentTarget.Value);
    }

    void OnDisable()
    {
        // При отключении компонента очищаем линию
        if (lineRenderer != null)
            lineRenderer.positionCount = 0;
    }
}
