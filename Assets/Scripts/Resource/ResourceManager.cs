using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour
{
    public GameObject resourcePrefab;     // Префаб ресурса
    public float spawnInterval = 3f;      // Интервал появления ресурсов
    public Vector2 spawnAreaMin = new(-8f, -4f);  // Минимальные координаты области спавна
    public Vector2 spawnAreaMax = new(8f, 4f);    // Максимальные координаты области спавна

    private Coroutine spawnRoutine;       

    void Start()
    {
        // Запускаем бесконечный цикл спавна ресурсов
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
        // Выбираем случайную позицию внутри заданной области
        Vector2 pos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        Instantiate(resourcePrefab, pos, Quaternion.identity);
    }

    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;

        // Перезапускаем корутину с новым интервалом
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnLoop());
    }
}
