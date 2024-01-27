using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController[] animations = new RuntimeAnimatorController[2];
    [SerializeField] Animator animator;
    void Start()
    {
        animator.runtimeAnimatorController = animations[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartWalking()
    {
        if (animator.runtimeAnimatorController != animations[1])
        {
            animator.runtimeAnimatorController = animations[1];
        }
    }

    public void Idle()
    {
        if (animator.runtimeAnimatorController != animations[0])
        {
            animator.runtimeAnimatorController = animations[0];
        }
    }
}
