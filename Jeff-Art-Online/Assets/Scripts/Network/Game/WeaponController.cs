using Fusion;
using UnityEngine;

public class WeaponController : NetworkBehaviour, IBeforeUpdate
{
    public float LocalAngle { get ; private set; }

    [Header("Aim Parameters")]
    [SerializeField] private Camera localCamera;
    [SerializeField] private Transform root;
    [SerializeField] private Transform gunPivot;
    [Header("Shoot Parameters")]
    [SerializeField] private NetworkPrefabRef bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float rateOfFire = 0.2f;

    [HideInInspector] public bool IsFiring { get; set; }

    [Networked] private float RotationAngle { get; set; }
    [Networked] private TickTimer ShootCooldown { get; set; }

    public void Update()
    {
        IsFiring |= Input.GetButton("Fire1");
    }

    public void BeforeUpdate()
    {
        if (Object.HasInputAuthority)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = 0;

            Vector3 direction = localCamera.ScreenToWorldPoint(mouseScreenPos) - gunPivot.position;
            direction *= root.localScale.x;

            float angleInDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            LocalAngle = angleInDeg;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerData data))
        {
            RotationAngle = data.gunRotation;
            gunPivot.rotation = Quaternion.Euler(0, 0, RotationAngle);

            CheckShootInput(data);
        }
    }

    private void CheckShootInput(PlayerData data)
    {
        if (HasStateAuthority && ShootCooldown.ExpiredOrNotRunning(Runner))
        {
            if (data.networkButtons.IsSet(PlayerNetworkController.InputButtons.Shoot))
            {
                ShootCooldown = TickTimer.CreateFromSeconds(Runner, rateOfFire);

                Runner.Spawn(bulletPrefab,
                    bulletSpawn.position,
                    bulletSpawn.rotation,
                    Object.InputAuthority,
                    (runner, obj) => { 
                        obj.GetComponent<BulletController>().Direction = root.localScale.x; 
                    });
            }
        }
    }
}
