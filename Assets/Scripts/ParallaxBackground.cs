using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeReference] Vector2 parallaxEffectMultiplier;
    [SerializeField] GameObject cam;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = cam.transform;
        lastCameraPosition = cameraTransform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
        lastCameraPosition = cameraTransform.position;
    }
}
