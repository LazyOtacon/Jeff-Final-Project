using UnityEngine;
using Fusion;
using TMPro;
using System.Collections;

public class PlayerNetworkController : NetworkBehaviour, IBeforeUpdate
{
    public enum InputButtons
    {
        Jump = 0b1, // 1
        Shoot = 0b10, // 2
        Reload = 0b100,
    }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [SerializeField] private float groundCheckDistance = 0.35f;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundCheckLayer;

    [field: SerializeField] public GameObject LocalCamera { get; private set; }
    [field: SerializeField] public SpriteController SpriteController { get; private set; }
    [field: SerializeField] public WeaponController WeaponController { get; private set; }
    [field: SerializeField] public PlayerUIController UIController { get; private set; }

    private Rigidbody2D body;
    private AudioManager thisAudioManager;
    private bool canJump = false;

    private PlayerData playerData;

    public override void Spawned()
    {
        body = GetComponent<Rigidbody2D>();
        thisAudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        if (Object.HasInputAuthority)
        {
            LocalCamera.SetActive(true);
            StartCoroutine(SetName());
        }
    }

    private IEnumerator SetName()
    {
        yield return null;
        UIController.SetPlayerNameRpc(GameManager.Instance.LocalPlayerName);
    }

    public void Update()
    {
        PerformGroundCheck();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerData data))
        {
            // Movement logic
            body.velocity = new Vector2(data.horizontalInput * moveSpeed, body.velocity.y);

            // Jump logic
            if (canJump && data.networkButtons.IsSet(InputButtons.Jump))
            {
                if (Mathf.Abs(body.velocity.y) < 0.1f) // Ensures jump only applies when the vertical velocity is minimal
                {
                    body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    thisAudioManager.PlaySFX(thisAudioManager.playerJump);
                }
            }
        }

        SpriteController.UpdateFacing(body.velocity);
    }

    private void PerformGroundCheck()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(
            transform.position + Vector3.down * groundCheckDistance, 
            groundCheckRadius, 
            groundCheckLayer);

        canJump = groundCollider != null;
    }

    public void BeforeUpdate()
    {
        if (!UIController.IsDead && Object.HasInputAuthority)
        {
            playerData.horizontalInput = Input.GetAxis("Horizontal");

            // Map Jump, Shoot, and Reload inputs to playerData.networkButtons
            playerData.networkButtons.Set(InputButtons.Jump, Input.GetButton("Jump"));
            playerData.networkButtons.Set(InputButtons.Shoot, WeaponController.IsFiring);
            playerData.networkButtons.Set(InputButtons.Reload, Input.GetKeyDown(KeyCode.R));

            // Update WeaponController's gun rotation
            playerData.gunRotation = WeaponController.LocalAngle;
        }
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }
}