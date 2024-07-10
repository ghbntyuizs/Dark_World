using UnityEngine;

public class EnemyGoblin : EnemyBase
{
    protected override void SetWalkingAnimation(bool isWalking)
    {
        animator.SetBool("isRun", isWalking);
    }
}
