using Fusion;
using UnityEngine;

public class PlayerRespawnController : NetworkBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private Transform[] SpawnPoints; // Array of spawn points
    [SerializeField] private float RespawnDelay = 3f; // Delay before respawning the player
    private PlayerUIController playerUIController; // Reference to PlayerUIController
    private AudioManager thisAudioManager;

    private void Start()
    {
        thisAudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        // If the Player is already in the scene, attempt to find the PlayerUIController.
        if (Player != null)
        {
            playerUIController = Player.GetComponent<PlayerUIController>();
        }

        // If PlayerUIController isn't assigned, find it in the scene
        if (playerUIController == null)
        {
            playerUIController = FindObjectOfType<PlayerUIController>();
        }

        // Subscribe to the OnPlayerDeath event from PlayerUIController
        if (playerUIController != null)
        {

        }
        else
        {
            Debug.LogWarning("PlayerUIController is not assigned or found in the scene.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe when the object is destroyed to prevent memory leaks
        if (playerUIController != null)
        {

        }
    }

    private void HandlePlayerDeath()
    {
        // Start the respawn process when the player dies
        Debug.Log("Player has died. Starting respawn process.");
        thisAudioManager.PlaySFX(thisAudioManager.playerDeath);
        Invoke(nameof(RespawnPlayer), RespawnDelay);
    }

    public void RespawnPlayer()
    {
        // Respawn the player at a random spawn point
        if (HasInputAuthority)
        {
            Vector3 spawnPosition = SpawnPoints[Random.Range(0, SpawnPoints.Length)].position;
            Player.transform.position = spawnPosition;

            // Reactivate the player
            Player.SetActive(true);

            // Optionally, reset health or other UI elements

            // Reactivate death object
            if (playerUIController != null)
            {
                playerUIController.ToggleDeathObjectForOwner();
            }

            Debug.Log("Player respawned.");
        }
    }
}
