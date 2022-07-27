using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordControls : MonoBehaviour
{
    // Start is called before the first frame update
    public readonly int hashIdle = Animator.StringToHash("isIdling");
    public readonly int hashAttacking = Animator.StringToHash("isAttacking");
    public readonly int hashRunning = Animator.StringToHash("isRunning");
    public readonly int hashJumping = Animator.StringToHash("isJumping");
    public readonly int hashClimbing = Animator.StringToHash("isClimbing");
    public readonly int hashFalling = Animator.StringToHash("isFalling");
    public readonly int hashDying = Animator.StringToHash("Dying");
    public readonly int hashAlive = Animator.StringToHash("isAlive");

    [SerializeField] public float lastSwordAttack;
    [SerializeField] public RuntimeAnimatorController bowController;

    Animator myAnimator;

    void Awake() 
    {
        myAnimator.runtimeAnimatorController = bowController as RuntimeAnimatorController;      
    }


    void Update()
    {
        if (lastSwordAttack >= 0)
        {
            lastSwordAttack -= Time.deltaTime;
        }
    }


    public void Attack()
    {

        myAnimator.SetBool(hashAttacking, true);
        
    }
}
