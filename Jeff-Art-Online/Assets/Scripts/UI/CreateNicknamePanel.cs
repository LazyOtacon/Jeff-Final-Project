using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNicknamePanel : LobbyPanelBase
{
    [Header("CreateNicknamePanel Variables")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton;

    private const int MinNameLength = 2;
    private const int MaxNameLength = 10;

    public override void Initialize(LobbyUIManager lobbyUIManager)
    {
        base.Initialize(lobbyUIManager);
        
        confirmButton.interactable = false;
        confirmButton.onClick.AddListener(OnClickedConfirm);

        inputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    private void OnClickedConfirm()
    {
        string nickname = inputField.text;

        GameManager.Instance.LocalPlayerName = nickname;
        lobbyUIManager.ShowPanel(PanelType.ConnectionPanel);

        HidePanel();
    }

    private void OnInputValueChanged(string value)
    {
        confirmButton.interactable = value.Length >= MinNameLength && value.Length <= MaxNameLength;
    }
}
