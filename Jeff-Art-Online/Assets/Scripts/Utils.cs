using System.Collections;
using UnityEngine;

public static class Utils
{
    public static IEnumerator AnimateAndSetStateOnFinish(GameObject parent, Animator animator, string clip, bool state = true)
    {
        animator.Play(clip);
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationLength);
        parent.SetActive(state);
    }
}
