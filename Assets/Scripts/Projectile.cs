using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float arrowSpeed = 7f;
    Rigidbody2D arrowRigidBody;
    CapsuleCollider2D myCapsuleCollider;
    float xSpeed;

    PlayerMovement player;
    void Start()
    {
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        arrowRigidBody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed = player.transform.localScale.x * arrowSpeed;

        if (xSpeed < 0)
        {
            Vector3 scale = transform.localScale; //stores scale and flips x axis, "flipping" the entire gameObject around. (could rotate the player instead)
            scale.x *= -1;
            transform.localScale = scale;
        }    
    }

    void Update()
    {
        arrowRigidBody.velocity = new Vector2 (xSpeed, 0f);
    }

    void FlipSprite()
    {
        Vector3 scale = transform.localScale; //stores scale and flips x axis, "flipping" the entire gameObject around. (could rotate the player instead)
		scale.x *= -1;
		transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy")
        {
            Debug.Log("Connected with Enemy");
            Destroy(gameObject);
        }
        else if (myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) 
        {
            StartCoroutine(ArrowDestroyWithDelay());
        }
        
    }

    IEnumerator ArrowDestroyWithDelay()
    {
        Debug.Log("Connected with Ground");
        int LayerGround = LayerMask.NameToLayer("Ground");
        gameObject.layer = LayerGround;
        myCapsuleCollider.isTrigger = false;
        arrowRigidBody.velocity = new Vector2(0f, 0f);
        arrowRigidBody.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);       
    }
}
