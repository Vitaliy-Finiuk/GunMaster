using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PrCharacterIK : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = true;
    public Transform leftHandTarget = null;

    private void Start() => 
        animator = GetComponent<Animator>();

    private void OnAnimatorIK()
    {
        if (animator)
        {
            if (ikActive)
            {
                if (leftHandTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
                }
            }

            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}