using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] NetworkPrefabRef playerPrefab = NetworkPrefabRef.Empty;
    [SerializeField] Transform[] spawnPositions;
    private AudioManager thisAudioManager;

    private void Awake()
    {
        thisAudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
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
            thisAudioManager.PlaySFX(thisAudioManager.playerJoin);
            SpawnPlayer(player);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject obj))
            {
                thisAudioManager.PlaySFX(thisAudioManager.playerLeave);
                Runner.Despawn(obj);
            }

            Runner.SetPlayerObject(player, null);
        }
    }
}
