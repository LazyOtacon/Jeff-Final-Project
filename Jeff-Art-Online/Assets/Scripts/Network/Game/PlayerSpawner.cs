using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] NetworkPrefabRef playerPrefab = NetworkPrefabRef.Empty;
    [SerializeField] Transform[] spawnPositions;

    private void SpawnPlayer(PlayerRef playerRef)
    {
        int playerSpawnIndex = playerRef.PlayerId % spawnPositions.Length;
        Vector3 spawnPosition = spawnPositions[playerSpawnIndex].position;

        NetworkObject obj = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, playerRef);
        Runner.SetPlayerObject(playerRef, obj);
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            SpawnPlayer(player);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject obj))
            {
                Runner.Despawn(obj);
            }

            Runner.SetPlayerObject(player, null);
        }
    }
}
