using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public float spawnTime = 0.1f;

    public GameObject[] tetrisPiecePrefabs;

    public int spawnQueueSize = 5;
    private List<GameObject> spawnQueue = new List<GameObject>();

    private void Start()
    {
        if (spawnQueueSize <= 0)
            Debug.LogWarning("Spawn Queue Size is less than <= 0, this will most likely throw an error");
    }
    public void Spawn()
    {
        StartCoroutine(DelayedSpawn());
    }
    public GameObject InstantSpawn(string prefabName = null)
    {
        if (spawnQueue.Count == 0)
        {
            FillQueue();
        }

        GameObject tetrisPrefab;
        if (prefabName == null || prefabName == "")
        {
            tetrisPrefab = spawnQueue[0];
            spawnQueue.RemoveAt(0);
            FillQueue();
        }
        else
        {
            tetrisPrefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        }
        
        GameObject tetrisPieceInstance = Instantiate(tetrisPrefab, transform);
        tetrisPieceInstance.transform.position = spawnPoint.position;
        tetrisPieceInstance.name = tetrisPrefab.name; // IMPORTANTE! - name is reference back to the prefab name

        return tetrisPieceInstance;
    }

    private void FillQueue()
    {
        for (int i = spawnQueue.Count; i < spawnQueueSize; i++)
        {
            GameObject newPiece = GetRandomPiecePrefab();
            if (i > 0)
                if (newPiece == spawnQueue[0]) newPiece = GetRandomPiecePrefab();
            spawnQueue.Add(newPiece);
        }
    }

    private GameObject GetRandomPiecePrefab()
    {
        return tetrisPiecePrefabs[Random.Range(0, tetrisPiecePrefabs.Length)];
    }

    public IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(spawnTime);
        InstantSpawn();
    }
}
