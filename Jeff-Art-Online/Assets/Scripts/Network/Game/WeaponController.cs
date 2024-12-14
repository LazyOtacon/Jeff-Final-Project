using Fusion;
using System.Collections;
using UnityEngine;
using TMPro;

public class WeaponController : NetworkBehaviour, IBeforeUpdate
{
    public float LocalAngle { get; private set; }

    [Header("Aim Parameters")]
    [SerializeField] private Camera localCamera;
    [SerializeField] private Transform root;
    [SerializeField] private Transform gunPivot;

    [Header("Shoot Parameters")]
    [SerializeField] private NetworkPrefabRef bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float rateOfFire = 0.2f;
    private AudioManager thisAudioManager;

    [HideInInspector] public bool IsFiring { get; set; }

    [Networked] private float RotationAngle { get; set; }
    [Networked] private TickTimer ShootCooldown { get; set; }

    [Header("Reload Parameters")]
    [SerializeField] private float reloadTime;
    [SerializeField] private int bulletAmount;

    [Networked] private int currentBullet { get; set; } // Networked property

    [SerializeField] private bool isReloading;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI ammoText; // Reference for the UI element

    public override void Spawned()
    {
        // Initialize networked properties
        currentBullet = bulletAmount;

        // Get AudioManager
        thisAudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        // Update the UI
        UpdateAmmoUI();
    }

    public void Update()
    {
        IsFiring |= Input.GetButton("Fire1");

        if (isReloading)
        {
            Invoke("ReloadTime", reloadTime);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerData data))
        {
            RotationAngle = data.gunRotation;
            gunPivot.rotation = Quaternion.Euler(0, 0, RotationAngle);
            CheckShootInput(data);
            CheckReloadInput(data);
        }
    }

    private void CheckShootInput(PlayerData data)
    {
        if (HasStateAuthority && ShootCooldown.ExpiredOrNotRunning(Runner))
        {
            if (data.networkButtons.IsSet(PlayerNetworkController.InputButtons.Shoot) && currentBullet > 0)
            {
                ShootCooldown = TickTimer.CreateFromSeconds(Runner, rateOfFire);
                currentBullet--; // Update networked property
                RPC_UpdateAmmoUI(currentBullet, bulletAmount); // Notify clients
                thisAudioManager.PlaySFX(thisAudioManager.gunShoot);

                Runner.Spawn(bulletPrefab,
                    bulletSpawn.position,
                    bulletSpawn.rotation,
                    Object.InputAuthority,
                    (runner, obj) =>
                    {
                        obj.GetComponent<BulletController>().Direction = root.localScale.x;
                    });
            }
        }
    }

    private void CheckReloadInput(PlayerData data)
    {
        if (HasStateAuthority)
        {
            if (data.networkButtons.IsSet(PlayerNetworkController.InputButtons.Reload))
            {
                isReloading = true;
                thisAudioManager.PlaySFX(thisAudioManager.gunReload);
            }
        }
    }

    void ReloadTime()
    {
        if (isReloading)
        {
            currentBullet = bulletAmount; // Reset ammo count
            RPC_UpdateAmmoUI(currentBullet, bulletAmount); // Notify clients
            isReloading = false;
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {currentBullet}/{bulletAmount}";
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateAmmoUI(int current, int total)
    {
        currentBullet = current;
        bulletAmount = total;
        UpdateAmmoUI();
    }
    public void BeforeUpdate()
    {
        if (Object.HasInputAuthority)
        {
            // Get the mouse position on the screen
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = 0; // Ensure the z-coordinate is zero for 2D calculations

            // Convert the screen position to world position relative to the gun pivot
            Vector3 direction = localCamera.ScreenToWorldPoint(mouseScreenPos) - gunPivot.position;

            // Adjust the direction based on the player's facing direction
            direction *= root.localScale.x;

            // Calculate the angle in degrees from the direction vector
            float angleInDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Set the local angle for aiming and apply it to the weapon's rotation
            LocalAngle = angleInDeg;
        }
    }
}
