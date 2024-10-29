using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] playerSpawns;  // Titik spawn untuk setiap pemain

    [SerializeField]
    private GameObject playerPrefab;   // Prefab pemain

    private List<GameObject> players = new List<GameObject>();  // Daftar pemain yang sedang aktif

    void Start()
    {
        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            SpawnPlayer(i);
        }
    }

    private void SpawnPlayer(int playerIndex)
    {
        if (playerIndex >= playerSpawns.Length)
        {
            Debug.LogError("Player index is out of bounds for spawn points.");
            return;
        }

        var spawnPoint = playerSpawns[playerIndex];
        var player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        // Setup player properties and health management
        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath += () => StartCoroutine(RespawnPlayerAtSpawnPoint(playerIndex)); // Event death
            playerHealth.SetSpawnPoint(spawnPoint); // Mengatur spawn point untuk player
        }

        players.Add(player);
    }

    private IEnumerator RespawnPlayerAtSpawnPoint(int playerIndex)
    {
        yield return new WaitForSeconds(3f);  // Delay sebelum respawn

        // Hapus pemain lama
        if (players[playerIndex] != null)
        {
            Destroy(players[playerIndex]);
        }

        // Respawn pemain baru di titik spawn
        players[playerIndex] = Instantiate(playerPrefab, playerSpawns[playerIndex].position, playerSpawns[playerIndex].rotation);

        // Re-bind event untuk pemain yang baru di-spawn
        var playerHealth = players[playerIndex].GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath += () => StartCoroutine(RespawnPlayerAtSpawnPoint(playerIndex));
            playerHealth.SetSpawnPoint(playerSpawns[playerIndex]); // Mengatur spawn point
        }
    }
}
