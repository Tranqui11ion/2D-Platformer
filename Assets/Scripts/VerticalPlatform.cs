using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private float waitTime;
    private float flipTime;
    private bool isFlipped;

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    void Update() 
    {
        flipTime -= Time.deltaTime;
        if (isFlipped) 
        { 
            if (flipTime <= 0)
            {
                isFlipped = false;
                effector.rotationalOffset = 0f;
            }
            else 
            { 
                return; 
            }
        }
        if(Input.GetKeyUp(KeyCode.DownArrow) ||Input.GetKeyUp(KeyCode.W) ) 
        {
            waitTime = 0.5f;
        }
        if(Input.GetKey(KeyCode.DownArrow) ||Input.GetKey(KeyCode.W) )    
        {
            if(waitTime <= 0)
            {
                effector.rotationalOffset = 180f;
                waitTime = 0.5f;
                isFlipped = true;
                flipTime = 1f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
}
