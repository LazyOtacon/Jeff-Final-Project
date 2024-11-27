using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    [SerializeField] private Transform sprite;
    [SerializeField] private Animator animator;

    private bool isFacingRight = true;

    private readonly int walkParamHash = Animator.StringToHash("Walk");
    private readonly int shootParamHash = Animator.StringToHash("Shoot");

    public void UpdateAnimState(Vector2 speed, bool shooting)
    {
        bool moving = speed.sqrMagnitude != 0;

        animator.SetBool(walkParamHash, moving);
        animator.SetBool(shootParamHash, shooting);
    }

    public void UpdateFacing(Vector2 speed)
    {
        if (speed.x < 0) isFacingRight = false;
        else if (speed.x > 0) isFacingRight = true;

        float localXScale = isFacingRight ? 1f : -1f;
        Vector3 localScale = new Vector3(localXScale, 1, 1);

        sprite.localScale = localScale;
    }
}
