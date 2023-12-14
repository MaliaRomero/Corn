using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviour
{

    public string enemyPrefabPath;
    public float maxEnemies = 1;
    public float spawnRadius;
    public float spawnCheckTime;
    private float lastSpawnCheckTime;
    private List<GameObject> curEnemies = new List<GameObject>();

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (Time.time - lastSpawnCheckTime > spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        // remove any dead enemies from the curEnemies list
        for (int x = 0; x < curEnemies.Count; ++x)
        {
            if (!curEnemies[x])
                curEnemies.RemoveAt(x);
        }
        // if we have maxed out our enemies, return
        if (curEnemies.Count >= maxEnemies)
            return;

        //replace for  enemy navmesh
        Vector3 randomInCircle = Random.insideUnitCircle * spawnRadius;
        GameObject enemy = PhotonNetwork.Instantiate(enemyPrefabPath, transform.position + randomInCircle, Quaternion.identity);


        // otherwise, spawn an enemy
        curEnemies.Add(enemy);
    }


}
