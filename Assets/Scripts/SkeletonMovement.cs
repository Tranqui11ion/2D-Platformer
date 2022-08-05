using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class SkeletonMovement : MonoBehaviour
{

    public readonly int hashIdle = Animator.StringToHash("isIdling");
    public readonly int hashAttack1 = Animator.StringToHash("isAttacking1");
    public readonly int hashAttack2 = Animator.StringToHash("isAttacking2");
    public readonly int hashRunning = Animator.StringToHash("isRunning");
    public readonly int hashDying = Animator.StringToHash("Dying");

    int currentAttackAnimation;


    [SerializeField] float moveTimer;
    [SerializeField] float idleTimer;
    [SerializeField] float lastChase;

    [SerializeField] bool justKilled = false;
    [SerializeField] bool isAlive = true;
    [SerializeField] bool isAttacking = false;
    [SerializeField] bool isRunning = false;
    [SerializeField] bool isIdle = false;
    [SerializeField] public bool isFollowingPlayer;

    [SerializeField] float agroRange = 1.2f;
    [SerializeField] bool attackRight;
    [SerializeField] public bool IsFacingRight = true;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float chaseSpeed = 1.5f;

    [SerializeField] float lastAttackDelay = 2f;
    float startingMoveSpeed = 1f;

    public readonly float runAnimationLength = .833f;
    public readonly float idleAnimationLength = .667f;
    public readonly float attackAnimationLength1 = 1.25f;
    public readonly float attackAnimationLength2 = .917f;

    float currentAttackAnimationLength;



    SpriteRenderer mySpriteRenderer;
    Rigidbody2D myRigidbody;
    BoxCollider2D myBoxCollider;
    CircleCollider2D attackRangeCollider;
    CapsuleCollider2D myCapsuleCollider;
    Animator myAnimator;
    [SerializeField] GameObject player;
    PlayerMovement playerController;

    [SerializeField] float lastAttack = -1f; //time since this object last attacked
    float baseAttackTimer = 1.4f;
    [SerializeField] float attackTimer = 1.4f;
    [SerializeField] float attackDelayTimer = 0;






    void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerMovement>();

    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        myAnimator = GetComponent<Animator>();
        attackRangeCollider = GetComponent<CircleCollider2D>();

        isAlive = true;

    }

    void Update()
    {
        if (!isAlive) { return; }
        lastChase -= Time.deltaTime;
        lastAttack -= Time.deltaTime;
        idleTimer -= Time.deltaTime;
        CheckForPlayer();
        CheckIfIdle();
        if (isAttacking) { AttackPlayer(); }
        if (isFollowingPlayer && !isAttacking && lastChase <= 0) {ChasePlayer();}
        if (!isIdle && !isAttacking && !isFollowingPlayer) { Move(); }
    }

    void CheckIfIdle()
    {
        if (idleTimer <= 0)
        {
            isIdle = false;
        }
        else
        {
            isIdle = true;
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1.5f);
    }

    // void Move()
    // {
    //     if (lastAttack <= 0)
    //     {
    //         if (!attackRangeCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
    //         {
    //             myAnimator.SetBool("isRunning", true);
    //             myRigidbody.velocity = new Vector2(moveSpeed, 0f);
    //             CheckDirectionToFace(myRigidbody.velocity.x > 0);
    //         }    

    //     }     
    // }
    void SetMoveTimer()
    {
        moveTimer = (float)Random.Range(3, 6) * runAnimationLength; //move animation is 1.0 sec so integer works well

    }

    void Move()
    {
        
            if (moveTimer < 0)
            {
                myAnimator.SetBool("isRunning", false);
                myRigidbody.velocity = new Vector2(0f, 0f);
                SetMoveTimer();

                int flip = Random.Range(1, 3); // Make direction change random
                Debug.Log("Setting flip: " + flip);
                if (flip == 1)
                {
                    Debug.Log("Flipped");
                    moveSpeed = -moveSpeed;
                }

                int idleMultiplier = Random.Range(3, 8);
                idleTimer = idleAnimationLength * idleMultiplier;     //.917 is animation length         
            }
            else
            {
                moveTimer -= Time.deltaTime;
                myAnimator.SetBool("isRunning", true);
                myRigidbody.velocity = new Vector2(moveSpeed, 0f);
                CheckDirectionToFace(myRigidbody.velocity.x > 0);

            }
        
    }

    void CheckForPlayer()
    {
        Transform target = player.transform;
        float distToPlayer = Math.Abs(transform.position.x - target.position.x);     
        if (Math.Abs(transform.position.y - target.position.y) < .6f && distToPlayer < agroRange && playerController.isGrounded )
        {
            Debug.Log("Detting isFollowing to True");
            isFollowingPlayer = true;
        }
        else if(distToPlayer < agroRange)
        {
            StopChasingPlayer();
        }
        else if ((Math.Abs(transform.position.y - target.position.y) > .6f))
        {
            StopChasingPlayer();
        }
    }
    void ChasePlayer()
    {
        Transform target = player.transform;
        myAnimator.SetBool("isRunning", true);

        //Enemy is to the left side of player
        if (transform.position.x < target.position.x)
        {     
            if(!IsFacingRight)
            {
                moveSpeed = -moveSpeed; 
            }
            myRigidbody.velocity = new Vector2(moveSpeed + .5f, 0f);    
        }
        //enemy is to the right of player
        else 
        {
            if(IsFacingRight)
            {
                moveSpeed = -moveSpeed; 
            }
            myRigidbody.velocity = new Vector2(moveSpeed - .5f, 0f);
        }
        
        CheckDirectionToFace(myRigidbody.velocity.x > 0);
        
    }

    void StopChasingPlayer()
    {
        isFollowingPlayer = false;
        lastChase = 1.5f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            //Stop enemy from running through walls
            if(isFollowingPlayer)
            {
                myRigidbody.velocity = new Vector2(0, 0f);  
                StopChasingPlayer();
            }
            moveSpeed = -moveSpeed;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered!!");
        if (other.tag == "Projectile")
        {
            StartCoroutine(Die());
        }
        if (attackRangeCollider.IsTouchingLayers(LayerMask.GetMask("Player")) && !isAttacking)
        {
            AttackPlayer();
        }

    }

    void SetAttackType()
    {
        int attack = Random.Range(1, 3);
        if (attack == 1)
        {
            Debug.Log("Setting attack 1");
            currentAttackAnimation = hashAttack1;
            currentAttackAnimationLength = attackAnimationLength1;
        }
        else
        {
            Debug.Log("Setting attack 2");
            currentAttackAnimation = hashAttack2;
            currentAttackAnimationLength = attackAnimationLength2;
        }
    }

    void AttackPlayer()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                attackTimer = -1f;
                isAttacking = false;
                myAnimator.SetBool(currentAttackAnimation, false);
                playerController.CheckIfHit();
            }

        }
        else if (lastAttack <= 0f)
        {
            SetAttackType();
            myAnimator.SetBool("isRunning", false);
            lastAttack = lastAttackDelay;
            attackTimer = currentAttackAnimationLength;
            isAttacking = true;
            Debug.Log("Attacking");
            myRigidbody.velocity = new Vector2(0f, 0f);
            myAnimator.SetBool(currentAttackAnimation, true);
        }
    }
    // void AttackPlayer()
    // {   
    //     
    //     if (!myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player"))) 
    //     {
    //         if (lastAttack < 0 && lastAttack > -3)
    //         {
    //             StartCoroutine(Delay());
    //             myAnimator.SetBool("isAttacking1", false);
    //             isAttacking = false;
    //             return; 
    //         }


    //     }
    //     else
    //     {
    //         myAnimator.SetBool("isRunning", false);
    //         if(isAttacking)
    //         {
    //             if (attackTimer <= 0f)
    //             {
    //                 attackTimer = baseAttackTimer;
    //                 isAttacking = false;
    //                 myAnimator.SetBool("isAttacking1", false);
    //                 playerController.CheckIfHit();

    //             }

    //         }
    //         else
    //         {
    //             if (lastAttack <= 0)
    //             {
    //                 lastAttack = 3f;
    //                 attackTimer = 1.25f;
    //                 isAttacking = true;
    //                 Debug.Log("Attacking");
    //                 myRigidbody.velocity = new Vector2(0f, 0f);             
    //                 myAnimator.SetBool("isAttacking1", true); 
    //             }   



    //         }   
    //     }                            
    // }

    void FlipSprite()
    {
        Vector3 scale = transform.localScale; //stores scale and flips x axis, "flipping" the entire gameObject around. (could rotate the player instead)
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }

    void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            FlipSprite();
    }

    public IEnumerator Die()
    {
        if (gameObject != null && isAlive)
        {
            isAlive = false;
            myRigidbody.velocity = new Vector2(0f, 0f);
            myBoxCollider.enabled = false;
            myCapsuleCollider.enabled = false;
            attackRangeCollider.enabled = false;
            myAnimator.SetTrigger("Dying");
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(FadeOut());
            Destroy(gameObject);
        }

    }

    [SerializeField] Color deathColor;
    //Split into another script later
    IEnumerator FadeOut()
    {
        Color color = mySpriteRenderer.material.color;

        float alphaVal = mySpriteRenderer.material.color.a;

        while (mySpriteRenderer.material.color.a > 0)
        {
            if (color.b > .25)
            {
                color.b -= .05f;
                color.g -= .05f;
            }
            alphaVal -= 0.01f;
            color.a = alphaVal;
            mySpriteRenderer.material.color = color;

            yield return new WaitForSeconds(0.01f); // update interval
        }
    }

}
