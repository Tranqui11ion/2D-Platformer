using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] bool isAlive = true;
	[SerializeField] public bool IsFacingRight;
	[SerializeField] public bool isJumping;
	

	public float LastOnGroundTime { get; private set; }

    [Header("Movement")]
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 3f;
    // [SerializeField] float acceleration = 7f;
    // [SerializeField] float decceleration = 7f;
    // [SerializeField] float velPower = 0.9f;



    
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator; 
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart; 
    float runSpeedAtStart;
    
    [Header("Ground Detection")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] PhysicsMaterial2D withFriction;
    [SerializeField] PhysicsMaterial2D noFriction;

    
    
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        runSpeedAtStart = runSpeed;
    
        whatIsGround = LayerMask.GetMask("Ground");
    }

    // public CapsuleCollider2D GetPlayerCollider()
    // {
    //     return myBodyCollider;
    // }

    void Update()
    {  
        if (!isAlive) { return;}
        
        Run();
        SetJumpOrFall();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return;}
        moveInput = value.Get<Vector2>();
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


    void Run()
    {    
        if (!isAlive) { return;}  
        if (moveInput.x != 0)
        { 
			CheckDirectionToFace(moveInput.x < 0);
            myFeetCollider.sharedMaterial = noFriction;
            //myRigidbody.bodyType = RigidbodyType2D.Dynamic; 
        }
        Vector2 playerVelocity = new Vector2 (moveInput.x * runSpeed, myRigidbody.velocity.y);
            myRigidbody.velocity = playerVelocity;

        if (myFeetCollider.IsTouchingLayers(whatIsGround))
            {
                bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
                myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
            }
        

         
         
    }


    void OnJump(InputValue value)
    {
        if (!isAlive) { return;}
        if (!myFeetCollider.IsTouchingLayers(whatIsGround)) { return;}
        if(value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }
  

    void SetJumpOrFall()
    {
        float verticalSpeed =  myRigidbody.velocity.y;
        bool playerHasVerticalSpeed = Mathf.Abs(verticalSpeed) > Mathf.Epsilon;
        
        
        if (!myFeetCollider.IsTouchingLayers(whatIsGround))
        {
            myAnimator.SetBool("isJumping", verticalSpeed > 0);
            myAnimator.SetBool("isFalling", verticalSpeed < 0);
            runSpeed = runSpeed/2;
        }
        else
        {
            myAnimator.SetBool("isJumping", false);
            myAnimator.SetBool("isFalling", false);
            runSpeed = runSpeedAtStart;
        }
        
    }  

    
    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
        { 
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            myAnimator.speed = 1;
            runSpeed = runSpeedAtStart;
            return;
        }
        
        Vector2 climbVelocity = new Vector2 (myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.gravityScale = 0f;
        myRigidbody.velocity = climbVelocity;
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) 
        { 
            myRigidbody.gravityScale = gravityScaleAtStart;
            runSpeed = runSpeedAtStart;
            myAnimator.SetBool("isClimbing", false);
            myAnimator.speed = 1;
            return;
        }
        else
        {
            bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
            if (playerHasVerticalSpeed )
            {
                myAnimator.SetBool("isClimbing", true);
                runSpeed = runSpeedAtStart/4;
                myAnimator.speed = 1;
            }
            else if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                myAnimator.SetBool("isClimbing", true);
                runSpeed = runSpeedAtStart/4;
                myAnimator.speed = 0;
            }  
        }    
    }

    void Die()
    {
        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
        }
    }

    

}

