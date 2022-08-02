using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smithy : MonoBehaviour
{
    public readonly int hashHammer = Animator.StringToHash("isHammering");

    [SerializeField] bool isIdle;
    [SerializeField] bool isHammering;
    [SerializeField] Animator myAnimator;
    private float defaultHammerTime = 5.835f;
    private float defaultIdleTime = 4.585f;
    [SerializeField] float hammerTime;
    [SerializeField] float idleTime = 4.585f;
    
    
    void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        isIdle = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isHammering)
        {
            hammerTime -= Time.deltaTime;
            if(hammerTime <=0)
            {
                isHammering = false;
                idleTime = defaultIdleTime;
                isIdle = true;
            }
        }    
        else if (isIdle)
        {
            idleTime -= Time.deltaTime;
            if(idleTime <=0)
            {
                isHammering = true;
                hammerTime = defaultHammerTime;
                isIdle = false;
            }
        }    
        Animate();    
    }

    void Animate()
    {
        if (isIdle)
        {
            if (myAnimator.GetBool(hashHammer)) { myAnimator.SetBool(hashHammer, false); }
        }
        else if (isHammering)
        {
            if (!myAnimator.GetBool(hashHammer)) { myAnimator.SetBool(hashHammer, true); }
        }
    }
}
