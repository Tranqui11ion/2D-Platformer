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
        Vector3 temp = new Vector3(cam.transform.position.x * (1 - parallaxEffectMultiplier.x), cam.transform.position.y * (1 - parallaxEffectMultiplier.y));
        Vector3 dist = new Vector3(cam.transform.position.x * parallaxEffectMultiplier.x, cam.transform.position.y * parallaxEffectMultiplier.y);

        transform.position = new Vector3(startpos + dist.x, dist.y, transform.position.z);

        if (temp.x > startpos + length)
        {
            startpos += length;
        }
        else if (temp.x < startpos - length)
        {
            startpos -= length;
        }

    }
}
