using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowControls : MonoBehaviour
{

    public readonly int hashIdle = Animator.StringToHash("isIdling");
    public readonly int hashAttacking = Animator.StringToHash("isAttacking");
    public readonly int hashRunning = Animator.StringToHash("isRunning");
    public readonly int hashJumping = Animator.StringToHash("isJumping");
    public readonly int hashClimbing = Animator.StringToHash("isClimbing");
    public readonly int hashFalling = Animator.StringToHash("isFalling");
    public readonly int hashDying = Animator.StringToHash("Dying");
    public readonly int hashAlive = Animator.StringToHash("isAlive");

    Animator myAnimator;

    public float bowAttackAnimTime = .583f;
    [SerializeField] public RuntimeAnimatorController bowController;


    void Awake() 
    {
        
    }


    void Update()
    {
         
       
    }


    public void Attack()
    {
        myAnimator.SetBool(hashAttacking, true);      
    }
}
