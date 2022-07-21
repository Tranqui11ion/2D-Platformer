using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkeletonMovement : MonoBehaviour
{
    [SerializeField] bool isAlive = true;
    [SerializeField] bool isAttacking = false;
    [SerializeField] public bool IsFacingRight = true;
    [SerializeField] float moveSpeed = 1f;
    float startingMoveSpeed = 1f;
    [SerializeField] CapsuleCollider2D playerCollider;
    Rigidbody2D myRigidbody;
    BoxCollider2D myBoxCollider;
    Animator myAnimator;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
        isAlive = true;
        
    }

    void Update()
    {
        Debug.Log("This runs");
        if (!isAlive) { return; }
        Debug.Log("This runs too");
        Move();
        //Die();
    }

    void Move()
    {
        Debug.Log("Move() runs");
        if (!isAttacking)
        {
            Debug.Log("if statement in Move() runs");
            myRigidbody.velocity = new Vector2(moveSpeed, 0f);
            CheckDirectionToFace(myRigidbody.velocity.x > 0);
            myAnimator.SetBool("isRunning", true);
        }   
        
         
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) 
        {
            moveSpeed = -moveSpeed;
            return;
        }
        else if(myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            myAnimator.SetBool("isRunning", false);
            StartCoroutine( AttackPlayer() );
        }
        
    }
    
    IEnumerator AttackPlayer()
    {   
        isAttacking = true;
        if(myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
            while(isAlive)
            {
                //Die();
                Debug.Log("Attacking");
                myRigidbody.velocity = new Vector2(0f, 0f);
                int r = Random.Range(1,3);
                if (r == 1 )
                {
                    //Debug.Log("Attack 1");
                    myAnimator.SetBool("isAttacking1", true);
                }
                else
                {
                    //Debug.Log("Attack 2");
                    myAnimator.SetBool("isAttacking2", true);
                }    
                myAnimator.SetBool("isAttacking1", false);
                myAnimator.SetBool("isAttacking2", false);      
                yield return new WaitForSeconds(3);
                
            }
        isAttacking = false;
    }

    void FlipSprite()
    {
        Vector3 scale = transform.localScale; //stores scale and flips x axis, "flipping" the entire gameObject around. (could rotate the player instead)
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
    }

    public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			FlipSprite();
	}

    void Die()
    {
        if(myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
        }
    }

}
