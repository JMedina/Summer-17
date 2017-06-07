using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    //We want button presses to be triggered after a short amount of time
    //instead of immediately, for the sake of the user. So we use a counter
    //to increment time.
    private int topCounter = 0;
    private int bottomCounter = 0;

    public AudioClip confirm;


    void writeToFile(string text)
    {
        StreamWriter writer = new StreamWriter("Assets/test.txt", true);
        writer.WriteLine(text);
        writer.Close();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Top")
        {
            ++topCounter;
            if (topCounter >= 15)
            {
                GetComponent<AudioSource>().PlayOneShot(confirm);
                Debug.Log("I Want MORE");
                writeToFile("I Wanted MORE");
                topCounter = 0;
                SceneManager.LoadScene("Redirected walking scene");
            }

        }
        else if (col.gameObject.name == "Bottom")
        {
            ++bottomCounter;
            if (bottomCounter >= 15)
            {
                GetComponent<AudioSource>().PlayOneShot(confirm);
                Debug.Log("I Want LESS");
                writeToFile("I Wanted LESS");
                bottomCounter = 0;
                SceneManager.LoadScene("Redirected walking scene");
            }
        }   
    }
}
