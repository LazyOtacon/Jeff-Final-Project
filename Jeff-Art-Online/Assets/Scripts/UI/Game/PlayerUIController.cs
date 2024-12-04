using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerUIController : NetworkBehaviour
{
    public GameObject childObject;  // The child object to get the parent of

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Death;
    [SerializeField] private GameObject TheBody;

    private const int MAX_HEALTH = 100;

    [Networked, HideInInspector] public bool IsDead { get; private set; }
    [Networked, HideInInspector] public int CurrentHealth { get; private set; }
    [Networked, HideInInspector] public NetworkString<_16> PlayerName { get; private set; }

    [Networked, HideInInspector] public Vector2 Respawnpos { get; private set; }

    private ChangeDetector changeDetector;

    private void Start()
    {
        FindAndSetDeathObjectInactive();
    }

    private void FindAndSetDeathObjectInactive()
    {
        // Find all objects with the "Death" tag
        GameObject[] deathObjects = GameObject.FindGameObjectsWithTag("Death");

        if (deathObjects.Length > 0)
        {
            // Assume the first "Death" object found is the one we need
            Death = deathObjects[0];

            if (HasInputAuthority) // Only affect the current player's "Death" object
            {
                Death.SetActive(false);
                Debug.Log("Death object has been set to inactive for the current player.");
            }
        }
        else
        {
            Debug.LogWarning("No object with the tag 'Death' found in the scene.");
        }
    }

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        CurrentHealth = MAX_HEALTH;
        PlayerName = GameManager.Instance.LocalPlayerName;
        Respawnpos = transform.position;
    }

    public override void Render()
    {
        foreach (string change in changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(CurrentHealth):
                    OnHealthChanged();
                    break;
                case nameof(PlayerName):
                    OnPlayerNameChanged();
                    break;
            }
        }
    }

    private void OnHealthChanged()
    {
        float percentage = (float)CurrentHealth / MAX_HEALTH;
        fillImage.fillAmount = percentage;

        healthText.text = $"{CurrentHealth}/{MAX_HEALTH}";

        if (CurrentHealth <= 0)
        {
            CurrentHealth = MAX_HEALTH;
            ToggleDeathObjectForOwner();
            DisableBodyForAllPlayersmessageRpc();

            Invoke("ToggleDeathObjectForOwner", 3f);
            Invoke("DisableBodyForAllPlayersmessageRpc", 3f);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void DisableBodyForAllPlayersmessageRpc()
    {
        this.gameObject.transform.position = Respawnpos;
        DisableBodyForAllPlayersRpc();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void DisableBodyForAllPlayersRpc()
    {
        if (TheBody != null)
        {
            bool isCurrentlyActive = TheBody.activeSelf;
            TheBody.SetActive(!isCurrentlyActive); // Disable the body for all players
            Debug.Log("TheBody has been disabled for all players.");
        }
        else
        {
            Debug.LogWarning("TheBody object is not assigned.");
        }
    }

    public void ToggleDeathObjectForOwner()
    {
        if (HasInputAuthority) // Check if this is the owner's screen
        {

            if (Death != null)
            {
                bool isCurrentlyActive = Death.activeSelf;
                Death.SetActive(!isCurrentlyActive); // Toggle the active state
                IsDead = !IsDead;
                Debug.Log($"Death object has been toggled to: {!isCurrentlyActive}");
            }
            else
            {
                Debug.LogWarning("Death object is not assigned in the inspector.");
            }
        }
    }



        private void OnPlayerNameChanged()
    {
        playerNameText.text = PlayerName.ToString();

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void SetPlayerNameRpc(NetworkString<_16> playerName)
    {
        PlayerName = playerName;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void ReducePlayerHealthRpc(int amount)
    {
        CurrentHealth -= amount;
    }
}
