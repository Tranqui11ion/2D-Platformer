using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{

    public readonly int hashIdle = Animator.StringToHash("isIdling");
    public readonly int hashAttacking = Animator.StringToHash("isAttacking");
    public readonly int hashRunning = Animator.StringToHash("isRunning");
    public readonly int hashJumping = Animator.StringToHash("isJumping");
    public readonly int hashClimbing = Animator.StringToHash("isClimbing");
    public readonly int hashFalling = Animator.StringToHash("isFalling");
    public readonly int hashDying = Animator.StringToHash("Dying");
    public readonly int hashAlive = Animator.StringToHash("isAlive");

    string bowName = "bow";
    string swordName = "sword";
    string unarmedName = "hand";

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

    private CinemachineImpulseSource _myImpulseSource;

    [SerializeField] float lastAttack;
    BowControls bowControls;
    SwordControls swordControls;
    [SerializeField] GameObject arrow;
    [SerializeField] Transform bow;
    [SerializeField] string currentWeapon;
    [SerializeField] public RuntimeAnimatorController currentController;
    [SerializeField] public RuntimeAnimatorController unarmedController;
    [SerializeField] public RuntimeAnimatorController bowController;
    

    void Awake()
    {
        bowControls = GetComponent<BowControls>();
        swordControls = GetComponent<SwordControls>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        whatIsGround = LayerMask.GetMask("Ground");
        _myImpulseSource = GetComponent<CinemachineImpulseSource>();
        
    }

    void Start()
    {
        gravityScaleAtStart = myRigidbody.gravityScale;
        runSpeedAtStart = runSpeed;
        currentController = unarmedController;
        currentWeapon = unarmedName; 
    }
    // public CapsuleCollider2D GetPlayerCollider()
    // {
    //     return myBodyCollider;
    // }

    void Update()
    {
        if (!isAlive) { return; }

        if (lastAttack >= 0)
        {
            lastAttack -= Time.deltaTime;
        }
        else 
        {
            myAnimator.SetBool(hashAttacking, false);
            Run();
            CheckForHazards();
            SetJumpOrFall();
            ClimbLadder();
        }
    }

    IEnumerator ArrowDelay()
    {
        yield return new WaitForSeconds(.7f);
        ReleaseArrow();
    }

    void OnAttack(InputValue value)
    {
        if(!isAlive) { return;}
        if (currentWeapon == bowName)
        {
            currentController = bowController;
            //bowControls.Attack();
            myAnimator.SetBool(hashAttacking, true);
            lastAttack = bowControls.bowAttackAnimTime;
            StartCoroutine(ArrowDelay());
        }
        else if( currentWeapon == swordName)
        {
            swordControls.Attack();
        }  
        else
        {
            Debug.Log("No weapon equipped");
        } 
        
    }

    void ReleaseArrow()
    {
        Instantiate(arrow, bow.position, transform.rotation);
    }
    void OnSwitchWeapons()
    {
        if (currentWeapon == unarmedName)
        {
            currentWeapon = bowName;
            currentController = bowController;
            myAnimator.runtimeAnimatorController = currentController as RuntimeAnimatorController;
        }
        else
        {
            currentWeapon = unarmedName;
            currentController = unarmedController;
            myAnimator.runtimeAnimatorController = currentController as RuntimeAnimatorController;
        }

    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
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

    void CheckForHazards()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            Die();
        }
    }

    public void CheckIfHit()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")))
        {
            Die();
        }
    }
    void Run()
    {
        if (!isAlive) { return; }
        if (moveInput.x != 0)
        {
            CheckDirectionToFace(moveInput.x < 0);
            myFeetCollider.sharedMaterial = noFriction;
            //myRigidbody.bodyType = RigidbodyType2D.Dynamic; 
        }
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        if (myFeetCollider.IsTouchingLayers(whatIsGround))
        {
            bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
            myAnimator.SetBool(hashRunning, playerHasHorizontalSpeed);
        }




    }


    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!myFeetCollider.IsTouchingLayers(whatIsGround)) { return; }
        if (value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }


    void SetJumpOrFall()
    {
        float verticalSpeed = myRigidbody.velocity.y;
        bool playerHasVerticalSpeed = Mathf.Abs(verticalSpeed) > Mathf.Epsilon;


        if (!myFeetCollider.IsTouchingLayers(whatIsGround))
        {
            myAnimator.SetBool(hashJumping, verticalSpeed > 0);
            myAnimator.SetBool(hashFalling, verticalSpeed < 0);
            runSpeed = runSpeed / 2;
        }
        else
        {
            myAnimator.SetBool(hashJumping, false);
            myAnimator.SetBool(hashFalling, false);
            runSpeed = runSpeedAtStart;
        }

    }


    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool(hashClimbing, false);
            myAnimator.speed = 1;
            runSpeed = runSpeedAtStart;
            return;
        }

        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.gravityScale = 0f;
        myRigidbody.velocity = climbVelocity;
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            runSpeed = runSpeedAtStart;
            myAnimator.SetBool(hashClimbing, false);
            myAnimator.speed = 1;
            return;
        }
        else
        {
            bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
            if (playerHasVerticalSpeed)
            {
                myAnimator.SetBool(hashClimbing, true);
                runSpeed = runSpeedAtStart / 4;
                myAnimator.speed = 1;
            }
            else if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                myAnimator.SetBool(hashClimbing, true);
                runSpeed = runSpeedAtStart / 4;
                myAnimator.speed = 0;
            }
        }
    }

    void Die()
    {
        isAlive = false;
        myAnimator.SetTrigger(hashDying);
        myAnimator.SetBool(hashAlive, false);
        myRigidbody.bodyType = RigidbodyType2D.Static;
        myBodyCollider.enabled = false;
        myFeetCollider.enabled = false;
        _myImpulseSource.GenerateImpulse(1);
    }



}

