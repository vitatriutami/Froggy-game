using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Pengaturan Spawn")]
    public float baseSpawnInterval = 2.4f;
    private float spawnTimer;
    private float currentSpawnInterval;

    [Header("Titik Spawn")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Object-object")]
    public List<GameObject> objects = new List<GameObject>();

    // Status mode
    private bool hasApproachedDoubleMode = false; // 55%-50%
    private bool isInPreDoubleWave = false; // 40-30
    private bool isInDoubleMode = false; // <30
    private bool isInTripleMode = false; // <20

    private int lastSpawnIndex = -1;

    void Start()
    {
        ResetSpawner();
    }

public bool allowPostGameoverSpawn = true;

    void Update()
    {
        // if (GameManager.Instance == null || GameManager.Instance.isGameOver) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGameOver && !allowPostGameoverSpawn) return;

        float remainingTime = GameManager.Instance.GetRemainingTime();
        float totalTime = GameManager.Instance.timeLimit;

        // Fase-fase waktu
        if (!hasApproachedDoubleMode && remainingTime <= totalTime * 0.55f)
        {
            currentSpawnInterval = baseSpawnInterval * 0.85f;
            hasApproachedDoubleMode = true;
            Debug.Log("🚸 Mendekati double mode, spawn dipercepat sedikit.");
        }

        if (!isInPreDoubleWave && remainingTime <= 40f && remainingTime > 30f)
        {
            currentSpawnInterval = baseSpawnInterval * 0.7f;
            isInPreDoubleWave = true;
            Debug.Log("🌊 Pre-double mode: mulai spawn 2 objek bergelombang.");
        }

        if (!isInDoubleMode && remainingTime <= 30f)
        {
            currentSpawnInterval = baseSpawnInterval * 0.6f;
            isInDoubleMode = true;
            isInPreDoubleWave = false; // pastikan tidak jalan bareng
            Debug.Log("🚀 Double mode aktif!");
        }

        if (!isInTripleMode && remainingTime <= 25f)
        {
            currentSpawnInterval = baseSpawnInterval * 0.5f;
            isInTripleMode = true;
            isInDoubleMode = false; // hentikan double mode biasa
            Debug.Log("🔥 Triple spawn aktif!");
        }

        // Spawn berdasarkan fase
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval && CanSpawn())
        {
            if (isInTripleMode)
                SpawnTripleObjects();
            else if (isInPreDoubleWave)
                SpawnWaveyTwoObjects();
            else
                SpawnSingleObjectSmart();

            spawnTimer = 0f;
        }
    }

    public void ResetSpawner()
    {
        currentSpawnInterval = baseSpawnInterval;
        spawnTimer = 0f;

        hasApproachedDoubleMode = false;
        isInPreDoubleWave = false;
        isInDoubleMode = false;
        isInTripleMode = false;
    }

    private bool CanSpawn()
    {
        return spawnPoints.Count >= 3 && objects.Count > 0;
    }

    private void SpawnSingleObjectSmart()
    {
        int randomIndex = GetSmartSpawnIndex();
        Transform point = spawnPoints[randomIndex];
        GameObject obj = GetRandomObject();

        Instantiate(obj, point.position, point.rotation);
        Debug.Log($"🎯 Spawn {obj.name} di {point.name}");
    }

    private void SpawnWaveyTwoObjects()
    {
        if (spawnPoints.Count < 2) return;

        int i1 = Random.Range(0, spawnPoints.Count);
        int i2;
        do { i2 = Random.Range(0, spawnPoints.Count); } while (i2 == i1);

        Vector3 p1 = spawnPoints[i1].position;
        Vector3 p2 = spawnPoints[i2].position + Vector3.up * 1.5f;

        Instantiate(GetRandomObject(), p1, Quaternion.identity);
        Instantiate(GetRandomObject(), p2, Quaternion.identity);

        Debug.Log($"🌊 Spawn 2 objek di titik {i1} dan {i2} (bergelombang)");
    }

    private void SpawnTripleObjects()
    {
        if (spawnPoints.Count < 3) return;

        List<int> selectedIndexes = GetUniqueIndexes(3, spawnPoints.Count);

        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = spawnPoints[selectedIndexes[i]].position + Vector3.up * (i * 0.7f);
            Instantiate(GetRandomObject(), spawnPos, Quaternion.identity);
        }

        Debug.Log($"🔥 Triple spawn di titik {selectedIndexes[0]}, {selectedIndexes[1]}, {selectedIndexes[2]}");
    }

    private List<int> GetUniqueIndexes(int count, int max)
    {
        List<int> indexes = new List<int>();
        while (indexes.Count < count)
        {
            int rand = Random.Range(0, max);
            if (!indexes.Contains(rand))
                indexes.Add(rand);
        }
        return indexes;
    }

    private int GetSmartSpawnIndex()
    {
        int index;
        do
        {
            index = Random.Range(0, spawnPoints.Count);
        } while (index == lastSpawnIndex && spawnPoints.Count > 1);

        lastSpawnIndex = index;
        return index;
    }

    private GameObject GetRandomObject()
    {
        return objects[Random.Range(0, objects.Count)];
    }
}
