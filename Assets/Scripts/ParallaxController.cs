using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    [SerializeField] HorizontalDirection horizontalDirection;
    [SerializeField] VerticalDirection verticalDirection;
    [SerializeField] MoveType moveType;
    [SerializeField] float lowerYBounds;
    [SerializeField] float upperYBounds;
    
    [Header ("Only for follow transform")]
    [SerializeField] Transform transformToFollow;
    
    public FloatReference speedMultiplier;
    
    private List<ParallaxImage> images;
    private float lastX;
    private float lastY;

    void FixedUpdate()
    {
        if (images == null) { return; }

        if ( moveType == MoveType.OverTime) MoveOverTime();
        else if ( moveType == MoveType.FollowTransform)
        {
             FollowTransformX();
             if (transformToFollow.position.y > lowerYBounds && upperYBounds < transformToFollow.position.y){FollowTransformY();}
             
        }
        
    }

    void MoveOverTime()
    {
        if (horizontalDirection == HorizontalDirection.Fix) { return; }
            foreach (var item in images)
            {
                item.MoveX(Time.deltaTime);
            }
    }

    void FollowTransformX()
    {
        if (horizontalDirection == HorizontalDirection.Fix) { return; }

        float distance = lastX - transformToFollow.position.x;
        if (Mathf.Abs(distance) < .001f) { return; }
        foreach (var item in images)
        {
            item.MoveX(distance);
        }
        lastX = transformToFollow.position.x;
    }

    void FollowTransformY()
    {
        if (verticalDirection == VerticalDirection.Fix) { return; }

        float distance = lastY - transformToFollow.position.y;
        if (Mathf.Abs(distance) < .001f) { return; }
        foreach (var item in images)
        {
            item.MoveY(distance);
        }
        lastY = transformToFollow.position.y;
    }

    void Start()
    {
        InitController();
    }

    void InitController()
    {
        InitList();
        ScanForImages();

        foreach (var item in images)
        {
            item.InitImage(speedMultiplier, horizontalDirection, verticalDirection, moveType == MoveType.FollowTransform);
        }

        if (moveType == MoveType.FollowTransform)
        {
            lastX = transformToFollow.position.x;
            lastY = transformToFollow.position.y;
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
                if (pi != null) images.Add(pi);
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

public enum HorizontalDirection
{
    Fix,
    Left,
    Right
}

public enum MoveType 
{
    OverTime,
    FollowTransform
}

public enum VerticalDirection
{
    Fix,
    Up,
    Down
}
