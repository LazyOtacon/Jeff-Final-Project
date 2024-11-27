using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPanel : LobbyPanelBase
{
    [SerializeField] private TMP_InputField gameNameField;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button joinRandomButton;
    [SerializeField] private Button backButton;

    private const int MINIMUM_ROOM_NAME_LENGTH = 2;

    private NetworkRunnerController networkRunnerController;

    public override void Initialize(LobbyUIManager lobbyUIManager)
    {
        base.Initialize(lobbyUIManager);

        hostButton.onClick.AddListener(() => Connect(GameMode.Host, gameNameField.text));
        joinButton.onClick.AddListener(() => Connect(GameMode.Client, gameNameField.text));
        joinRandomButton.onClick.AddListener(JoinRandomGame);
        backButton.onClick.AddListener(OnBackButtonClicked);

        networkRunnerController = GameManager.Instance.NetworkRunnerController;
    }
    private void Connect(GameMode mode, string roomName)
    {
        Debug.Log($"{mode} attemping to connect to {roomName}");
        if (roomName.Length >= MINIMUM_ROOM_NAME_LENGTH)
        {
            networkRunnerController.StartGame(mode, roomName);
            HidePanel();
        }
        else
        {
            Debug.LogWarning($"Room name \"{roomName}\" is too short.");
        }
    }

    private void JoinRandomGame()
    {
        networkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
        HidePanel();
    }

    private void OnBackButtonClicked()
    {
        lobbyUIManager.ShowPanel(PanelType.CreateNicknamePanel);
        HidePanel();
    }

    private void OnDestroy()
    {
        hostButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
        joinRandomButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
    }
}
