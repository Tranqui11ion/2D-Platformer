using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float arrowSpeed = 7f;
    Rigidbody2D arrowRigidBody;
    float xSpeed;

    PlayerMovement player;
    void Start()
    {
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
            Destroy(gameObject);
        }
        
    }
}
