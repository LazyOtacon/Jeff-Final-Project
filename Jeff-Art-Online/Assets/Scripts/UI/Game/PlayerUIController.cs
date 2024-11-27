using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text healthText;

    private const int MAX_HEALTH = 100;

    [Networked, HideInInspector] public int CurrentHealth { get; private set; }
    [Networked, HideInInspector] public NetworkString<_16> PlayerName { get; private set; }

    private ChangeDetector changeDetector;

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        CurrentHealth = MAX_HEALTH;
        PlayerName = GameManager.Instance.LocalPlayerName;
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
