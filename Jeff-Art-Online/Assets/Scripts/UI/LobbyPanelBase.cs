using UnityEngine;

public class LobbyPanelBase : MonoBehaviour
{
    public enum PanelType
    {
        CreateNicknamePanel,
        ConnectionPanel,
        WaitForConnectionPanel
    }

    [field: SerializeField]
    public PanelType Type { get; private set; }
    [SerializeField]
    protected Animator animator;
    protected LobbyUIManager lobbyUIManager;

    public virtual void Initialize(LobbyUIManager lobbyUIManager)
    {
        this.lobbyUIManager = lobbyUIManager;
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        animator.Play("Zoom In");
    }

    public void HidePanel()
    {
        StartCoroutine(Utils.AnimateAndSetStateOnFinish(gameObject, animator, "Zoom Out", false));
    }
}