using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2.0f;
    
    [Header("Limitations")]
    public int totalQuota = 30;       // Total maksimal musuh selama game/level
    public int maxConcurrent = 5;     // Maksimal musuh yang ada di layar sekaligus

    private int _spawnedCount = 0;    // Sudah berapa yang lahir
    private List<GameObject> _activeEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Berjalan terus selama kuota masih ada
        while (_spawnedCount < totalQuota)
        {
            // Cek List: Hapus musuh yang sudah hancur/null dari list
            _activeEnemies.RemoveAll(item => item == null);

            // LOGIKA LIMITASI:
            // Hanya spawn jika jumlah yang aktif kurang dari limit concurrent
            if (_activeEnemies.Count < maxConcurrent)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
        
        Debug.Log("Semua kuota musuh telah habis!");
    }

    void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        _activeEnemies.Add(newEnemy);
        _spawnedCount++;
        
        Debug.Log($"Spawned: {_spawnedCount}/{totalQuota} | Active: {_activeEnemies.Count}");
    }
}