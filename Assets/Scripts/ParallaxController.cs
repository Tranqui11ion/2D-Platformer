using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    public FloatReference speedMultiplier;
    private List<ParallaxImage> images;
    
    void FixedUpdate() {
        if (images != null)
        {
            foreach (var item in images)
            {
                item.MoveX(Time.deltaTime);
            }
        }    
    }

    void Start()
    {
        InitController();      
    }

    void InitController()
    {
        InitList();
        ScanForImages();

         foreach ( var item in images)
         {
            item.InitImage(speedMultiplier);
         }
    }
    
    void InitList()
    {
        if (images == null) images = new List<ParallaxImage>();
        else 
        {
            foreach (var item in images)
            {
                item.CleanUpImage();
            }
            images.Clear();
        }
    }
    void ScanForImages()
    {
        ParallaxImage pi;

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                pi = child.GetComponent<ParallaxImage>();
                if ( pi != null) images.Add(pi);
            }
        }
    }
 
}

[System.Serializable]
public class FloatReference
{
    [Range(0.01f, 5)]
    public float value = 1f;
}