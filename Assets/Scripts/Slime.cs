using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] float moveTimer;
    [SerializeField] bool justKilled = false;
    [SerializeField] bool isAlive = true;
    [SerializeField] bool isRunning = false;

    [SerializeField] public bool IsFacingRight = true;
    [SerializeField] float moveSpeed = .5f;
    float startingMoveSpeed = .5f;


    SpriteRenderer mySpriteRenderer;
    Rigidbody2D myRigidbody;
    BoxCollider2D myBoxCollider;
    CircleCollider2D attackRangeCollider;
    Animator myAnimator;
    [SerializeField] GameObject player;
    PlayerMovement playerController;

    [SerializeField] float lastAttack; //time since this object last attacked
    




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
        
        if (!isAlive) { return; }
        lastAttack += Time.deltaTime;
        moveTimer -= Time.deltaTime;
        if (lastAttack > 3f){ AttackPlayer(); }
        
        Move();
    }

    void SetMoveTimer()
    {
        moveTimer = Random.Range(1.0f, 3.0f);
    }

    IEnumerator Delay(float val)
    {
        yield return new WaitForSeconds(val);
    }

    void Move()
    {
        if (moveTimer < 0)
        {
            Delay(1.5f);
            SetMoveTimer();
            FlipSprite();
        }
        else
        {
            myAnimator.SetBool("isRunning", true);
            myRigidbody.velocity = new Vector2(moveSpeed, 0f);
            CheckDirectionToFace(myRigidbody.velocity.x > 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            StartCoroutine(Die());
        }
        else if (other.tag == "Player")
        {
            AttackPlayer();
        }
        else if (other.tag == "Ground" || other.tag == "Slope")
        {
            moveSpeed = -moveSpeed;
        }
    }

    void AttackPlayer()
    {
        if (attackRangeCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
           lastAttack = 0f;
           playerController.CheckIfHit();
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
        if (gameObject != null && isAlive)
        {
            isAlive = false;
            myRigidbody.bodyType = RigidbodyType2D.Static;
            myRigidbody.velocity = new Vector2(0f, 0f);
            myBoxCollider.enabled = false;
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
