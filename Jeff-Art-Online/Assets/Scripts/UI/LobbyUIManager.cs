using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField]
    private LobbyPanelBase[] lobbyPanels;

    private void Start()
    {
        foreach (LobbyPanelBase panel in lobbyPanels)
        {
            panel.Initialize(this);
        }
    }

    public void ShowPanel(LobbyPanelBase.PanelType panelType)
    {
        foreach(LobbyPanelBase panel in lobbyPanels)
        {
            if (panel.Type == panelType)
            {
                panel.ShowPanel();
                return;
            }
        }
    }
}
