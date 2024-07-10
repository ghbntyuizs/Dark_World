using UnityEngine;

public class EnemySkeleton : EnemyBase
{
    protected override void SetWalkingAnimation(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }
}
