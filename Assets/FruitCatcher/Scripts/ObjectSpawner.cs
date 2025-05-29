using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Pengaturan Spawn")]
    public float spawnInterval = 2f; // Interval waktu spawn dalam detik


    [Header("Titik Spawn")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Object-object")]
    public List<GameObject> objects = new List<GameObject>();

    private float nextSpawnTime;

    void Start()
    {
        // Cek apakah ada spawn points dan buah
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("Tidak ada spawn points yang diatur pada " + gameObject.name);
        }

        if (objects.Count == 0)
        {
            Debug.LogWarning("Tidak ada buah yang diatur pada " + gameObject.name);
        }

        // Set waktu spawn pertama
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        // Cek apakah sudah waktunya spawn dan ada data yang valid
        if (Time.time >= nextSpawnTime && CanSpawn())
        {
            SpawnObject();
            // Set waktu spawn berikutnya
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private bool CanSpawn()
    {
        // Cek apakah ada spawn points dan buah yang tersedia
        return spawnPoints.Count > 0 && objects.Count > 0;
    }

    private void SpawnObject()
    {
        // Pilih spawn point secara acak
        int randomSpawnIndex = Random.Range(0, spawnPoints.Count);
        Transform selectedSpawnPoint = spawnPoints[randomSpawnIndex];

        // Pilih buah secara acak
        int randomObjectIndex = Random.Range(0, objects.Count);
        GameObject selectedObject = objects[randomObjectIndex];

        // Spawn buah di posisi spawn point yang dipilih
        if (selectedSpawnPoint != null && selectedObject != null)
        {
            Instantiate(selectedObject, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
            Debug.Log("Spawn " + selectedObject.name + " di " + selectedSpawnPoint.name);
        }
        else
        {
            Debug.LogError("Spawn point atau buah bernilai null!");
        }
    }


}