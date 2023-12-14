using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviourPun
{
    public GameObject healthPickupPrefab; // Prefab for health pickups
    public GameObject ammoPickupPrefab; // Prefab for ammo pickups
    public Transform[] spawnPoints; // Array of spawn points where pickups can appear
    public float spawnInterval = 10f; // Time interval between spawns
    public float initialDelay = 5f; // Initial delay before spawning starts
    public int maxPickups = 10; // Maximum number of pickups to spawn

    private int currentPickups = 0;
    private bool canSpawn = true;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating("SpawnPickup", initialDelay, spawnInterval);
        }
    }

    void SpawnPickup()
    {
        if (!canSpawn || currentPickups >= maxPickups || !PhotonNetwork.IsMasterClient)
            return;

        // Randomly select a spawn point
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Randomly determine whether to spawn health or ammo pickup
        GameObject pickupPrefab = Random.value < 0.5f ? healthPickupPrefab : ammoPickupPrefab;

        // Spawn the selected pickup at the random spawn point across the network
        PhotonNetwork.Instantiate(pickupPrefab.name, randomSpawnPoint.position, Quaternion.identity);

        currentPickups++;
    }

    // Call this method to enable or disable pickup spawning
    public void ToggleSpawn(bool enableSpawn)
    {
        canSpawn = enableSpawn;
    }
}
