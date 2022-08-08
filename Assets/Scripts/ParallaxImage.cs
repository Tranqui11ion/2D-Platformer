using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxImage : MonoBehaviour
{

    public float speedX = 0;
    public int spawnCount = 2;

    private Transform[] controlledTransforms;
    private float imageWidth;
    private float minLeftX;
    private FloatReference speedMultiplier;

    public void MoveX(float moveBy)
    {
        moveBy *= speedX * speedMultiplier.value;

        for (int i = 0; i < controlledTransforms.Length; i++)
        {
            Vector3 newPos = controlledTransforms[i].position;
            newPos.x -= moveBy;
            controlledTransforms[i].position = newPos;
        }
        CheckAndReposition();
    }

    public void CleanUpImage()
    {
        if ( controlledTransforms != null)
        {
            for (int i = 0; i < controlledTransforms.Length; i++)
            {
                Destroy(controlledTransforms[i].gameObject);
            }
        }
    }

    public void InitImage(FloatReference speedMultiplier)
    {
        this.speedMultiplier = speedMultiplier;
        controlledTransforms = new Transform[spawnCount + 1];
        controlledTransforms[0] = transform;

        imageWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        minLeftX = transform.position.x - imageWidth - 0.5f;

        for (int i = 0; i < controlledTransforms.Length; i++)
        {
            controlledTransforms[i] = PrepareCopyAt(transform.position.x + imageWidth * i);
        }
    }

    private Transform PrepareCopyAt(float posX)
    {
        GameObject go = Instantiate(gameObject, new Vector3(posX, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
        Destroy(go.GetComponent<ParallaxImage>());

        return go.transform;
    }

    private void CheckAndReposition()
    {
        for (int i = 0; i < controlledTransforms.Length; i++)
        {
            if(controlledTransforms[i].position.x < minLeftX)
            {
                Vector3 newPos = controlledTransforms[i].position;
                newPos.x = GetRightmostTransform().position.x + imageWidth;
                controlledTransforms[i].position = newPos;   
            }
        }
    }

    private Transform GetRightmostTransform()
    {
        float currentMaxX = float.NegativeInfinity;
        Transform currentTransform = null;

        for (int i = 0; i < controlledTransforms.Length; i++)
        {
            if (currentMaxX < controlledTransforms[i].position.x)
            {
                currentMaxX = controlledTransforms[i].position.x;
                currentTransform = controlledTransforms[i];
            }
        }

        return currentTransform;
    }

}    
