using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRange : MonoBehaviour
{

    GameObject player;
    PlayerMovement playerController;
    SkeletonMovement skeletonController;
    public float agroRange = 1.2f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        skeletonController = GetComponent<SkeletonMovement>();
        playerController = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }   
}
