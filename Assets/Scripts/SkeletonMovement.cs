using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkeletonMovement : MonoBehaviour
{
    [SerializeField] bool justKilled = false;
    [SerializeField] bool isAlive = true;
    [SerializeField] bool isAttacking = false;
    [SerializeField] bool isRunning = false;

    [SerializeField] bool attackRight;
    [SerializeField] public bool IsFacingRight = true;
    [SerializeField] float moveSpeed = 1f;
    float startingMoveSpeed = 1f;

    
    SpriteRenderer mySpriteRenderer;
    Rigidbody2D myRigidbody;
    BoxCollider2D myBoxCollider;
    CircleCollider2D attackRangeCollider;
    Animator myAnimator;
    [SerializeField] GameObject player;
    PlayerMovement playerController;

    [SerializeField] float lastAttack; //time since this object last attacked
    float baseAttackTimer = 1.4f;
    [SerializeField] float attackTimer = 1.4f;
    [SerializeField] float attackDelayTimer = 0;

    


    // float animTransistionDelay = 2.5f;
    // float baseAnimTransistionDelay = 2.5f;

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
        myAnimator = GetComponent<Animator>();
        attackRangeCollider = GetComponent<CircleCollider2D>();
        
        isAlive = true;
        
    }

    void Update()
    {  
        lastAttack -= Time.deltaTime;
        if (!isAlive) { return; }
        // if (justKilled)
        // {
        //     isAlive = false;
        //     justKilled = false;
        //     StartCoroutine(Die());
        // }
        AttackPlayer();
        Move();              
    }


    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1.5f);
    }

    void Move()
    {
        if (lastAttack <= 0)
        {
            if (!attackRangeCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
            {
                myAnimator.SetBool("isRunning", true);
                myRigidbody.velocity = new Vector2(moveSpeed, 0f);
                CheckDirectionToFace(myRigidbody.velocity.x > 0);
            }    
            
        }     
    }

    
    void OnTriggerExit2D(Collider2D other) 
    {
        if (!myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) 
        {
            moveSpeed = -moveSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Projectile")
        {
            StartCoroutine(Die());    
        }
    }

    // void OnTriggerExit2D(Collider2D other) {
    //     if (myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player"))) 
    //     {
    //        StartCoroutine(Delay()); 
    //     }       
    // }

    
    void AttackPlayer()
    {   
        attackTimer -= Time.deltaTime;
        lastAttack -= Time.deltaTime;
        if (!myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player"))) 
        {
            if (lastAttack < 0 && lastAttack > -3)
            {
                StartCoroutine(Delay());
                myAnimator.SetBool("isAttacking1", false);
                isAttacking = false;
                return; 
            }
            
            
        }
        else
        {
        myAnimator.SetBool("isRunning", false);
            if(isAttacking)
            {
                if (attackTimer <= 0f)
                {
                    attackTimer = baseAttackTimer;
                    isAttacking = false;
                    myAnimator.SetBool("isAttacking1", false);
                    playerController.CheckIfHit();
                    
                }
                
            }
            else
            {
                if (lastAttack <= 0)
                {
                    lastAttack = 3f;
                    attackTimer = 1.25f;
                    isAttacking = true;
                    Debug.Log("Attacking");
                    myRigidbody.velocity = new Vector2(0f, 0f);             
                    myAnimator.SetBool("isAttacking1", true); 
                }   
                   
         

            }   
        }                            
    }

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
        if (gameObject != null)
        {
            isAlive = false;
            myRigidbody.velocity = new Vector2 (0f, 0f);
            myBoxCollider.enabled = false;
            attackRangeCollider.enabled = false;
            myAnimator.SetTrigger("Dying");
            yield return new WaitForSeconds(3f);
            StartCoroutine(FadeOut());
            Destroy(gameObject);
        }    
        
    }

    IEnumerator FadeOut()
     {
         float alphaVal = mySpriteRenderer.color.a;
         Color tmp = mySpriteRenderer.color;
 
         while (mySpriteRenderer.color.a < 1)
         {
             alphaVal += 0.01f;
             tmp.a = alphaVal;
             mySpriteRenderer.color = tmp;
 
             yield return new WaitForSeconds(0.05f); // update interval
         }
     }
    
}
