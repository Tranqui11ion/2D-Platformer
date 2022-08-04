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

    public void FlipOffset()
    {
        effector.rotationalOffset = 180f;
        isFlipped = true;
        flipTime = .5f;
    }

    public IEnumerator DelayAndFlipOffset()
    {
        yield return new WaitForSeconds(.5f);
        FlipOffset();
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
            
        }
    }
}
