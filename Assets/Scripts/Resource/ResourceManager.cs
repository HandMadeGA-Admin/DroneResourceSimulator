using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour
{
    public GameObject resourcePrefab;     // ������ �������
    public float spawnInterval = 3f;      // �������� ��������� ��������
    public Vector2 spawnAreaMin = new(-8f, -4f);  // ����������� ���������� ������� ������
    public Vector2 spawnAreaMax = new(8f, 4f);    // ������������ ���������� ������� ������

    private Coroutine spawnRoutine;       

    void Start()
    {
        // ��������� ����������� ���� ������ ��������
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnResource();               
            yield return new WaitForSeconds(spawnInterval); 
        }
    }

    void SpawnResource()
    {
        // �������� ��������� ������� ������ �������� �������
        Vector2 pos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        Instantiate(resourcePrefab, pos, Quaternion.identity);
    }

    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;

        // ������������� �������� � ����� ����������
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnLoop());
    }
}
