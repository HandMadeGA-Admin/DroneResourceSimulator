using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DronePathRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private DroneController drone;

    void Awake()
    {
        // �������� ���������� LineRenderer � DroneController
        lineRenderer = GetComponent<LineRenderer>();
        drone = GetComponent<DroneController>();
    }

    void Update()
    {
        if (drone == null || !drone.CurrentTarget.HasValue)
        {
            lineRenderer.positionCount = 0;  // ����� ���������
            return;
        }

        // ������������� ����� �� ���� �����: ������� ������� ����� � ������� ����
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, drone.CurrentTarget.Value);
    }

    void OnDisable()
    {
        // ��� ���������� ���������� ������� �����
        if (lineRenderer != null)
            lineRenderer.positionCount = 0;
    }
}
