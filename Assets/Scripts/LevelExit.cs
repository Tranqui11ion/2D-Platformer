using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] float levelLoadDelay = 1.5f;

    

    void OnTriggerEnter2D(Collider2D other) 
    {
        StartCoroutine(LoadNextLevel());      
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; 
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }   
        
        SceneManager.LoadScene(nextSceneIndex);
    }
    // [SerializeField] Color deathColor;
    // //Split into another script later
    // IEnumerator FadeOut()
    // {
        
    //     Color color = mySpriteRenderer.material.color;

    //     float alphaVal = mySpriteRenderer.material.color.a;

    //     while (mySpriteRenderer.material.color.a > 0)
    //     {
    //         if (color.b > .25)
    //         {
    //             color.b -= .05f;
    //             color.g -= .05f;
    //         }
    //         alphaVal -= 0.01f;
    //         color.a = alphaVal;
    //         mySpriteRenderer.material.color = color;

    //         yield return new WaitForSeconds(0.01f); // update interval
    //     }
    // }
}
