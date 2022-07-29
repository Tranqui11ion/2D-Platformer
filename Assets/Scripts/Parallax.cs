using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeReference] Vector2 parallaxEffectMultiplier;
    private float length, startpos;
    public GameObject cam;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("State Driven Camera");
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
       
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffectMultiplier.x), cam.transform.position.y * (1 - parallaxEffectMultiplier.y));
        float dist = (cam.transform.position.x * parallaxEffectMultiplier);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (temp > startpos + length)
        {
            startpos += length;
        }
        else if (temp < startpos - length)
        {
            startpos -= length;
        }

    }
}
