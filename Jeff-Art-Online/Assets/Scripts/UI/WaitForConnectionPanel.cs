using UnityEngine;
using UnityEngine.UI;

public class WaitForConnectionPanel : LobbyPanelBase
{
    [SerializeField] private Button cancelButton;

    private NetworkRunnerController networkRunnerController;

    private void Start()
    {
        networkRunnerController = GameManager.Instance.NetworkRunnerController;

        networkRunnerController.OnRunnerStartGame += OnStartGame;
        networkRunnerController.OnPlayerJoinSuccess += OnPlayerJoinedSuccess;

        cancelButton.onClick.AddListener(networkRunnerController.ShutdownRunner);

        gameObject.SetActive(false);
    }

    private void OnStartGame()
    {
        ShowPanel();
    }

    private void OnPlayerJoinedSuccess()
    {
        HidePanel();
    }

    private void OnDestroy()
    {
        networkRunnerController.OnRunnerStartGame -= OnStartGame;
        networkRunnerController.OnPlayerJoinSuccess -= OnPlayerJoinedSuccess;

        cancelButton.onClick.RemoveAllListeners();
    }
}
