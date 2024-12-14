using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float lifetime = 0.8f;

    [SerializeField] CapsuleCollider2D bulletCollider;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask hitboxMask;

    [Networked] private TickTimer LifetimeTimer { get; set; }
    [Networked] private NetworkBool HitObject { get; set; }
    [Networked, HideInInspector] public float Direction { get; set; }

    [field: SerializeField] public PlayerUIController UIController { get; private set; }

    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    private AudioManager thisAudioManager;

    private void Awake()
    {
        thisAudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public override void Spawned()
    {
        LifetimeTimer = TickTimer.CreateFromSeconds(Runner, lifetime);
        transform.localScale = new Vector3(Direction, 1, 1);
    }

    public override void Render()
    {
        if (!LifetimeTimer.ExpiredOrNotRunning(Runner))
        {
            transform.Translate(Direction * moveSpeed * Runner.DeltaTime * transform.right, Space.World);
        }

        if (!HitObject)
        {
            CheckGroundHit();
            CheckPlayerHit();
        }

        if (HitObject || LifetimeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }

    private void CheckGroundHit()
    {
        Collider2D overlappedCollider =
            Runner.GetPhysicsScene2D().OverlapCapsule(
                bulletCollider.transform.position,
                bulletCollider.size,
                bulletCollider.direction,
                0,
                groundMask);

        HitObject = overlappedCollider != null;
    }

    private void CheckPlayerHit()
    {
        Runner.LagCompensation.OverlapBox(
            bulletCollider.transform.position,
            bulletCollider.bounds.extents,
            bulletCollider.transform.rotation,
            Object.InputAuthority,
            hits,
            hitboxMask);

        if (hits.Count > 0)
        {
            foreach(LagCompensatedHit hit in hits)
            {
                NetworkObject player = hit.Hitbox.GetComponentInParent<NetworkObject>();

                bool hitSelf = player.InputAuthority.PlayerId == Object.InputAuthority.PlayerId;
                if (!hitSelf)
                {
                    if (player.GetComponentInChildren<PlayerUIController>().IsDead == false)
                    {
                        if (Runner.IsServer)
                        {
                            thisAudioManager.PlaySFX(thisAudioManager.playerHit);
                            player.GetComponentInChildren<PlayerUIController>().ReducePlayerHealthRpc(damage);
                        }
                        HitObject = true;
                       
                    }
                    break;
                }
            }
        }
    }
}
