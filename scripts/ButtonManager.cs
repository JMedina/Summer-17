using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    //We want button presses to be triggered after a short amount of time
    //instead of immediately, for the sake of the user. So we use a counter
    //to increment time.
    private int Counter = 0;
    public AudioClip soundFile;

    void writeToFile(string text)
    {
        StreamWriter writer = new StreamWriter("Assets/test.txt", true);
        writer.WriteLine(text);
        writer.Close();
    }

    void OnCollisionEnter(Collision col)
    {
        ++Counter;
        if (Counter >= 20)
        {
            GetComponent<AudioSource>().PlayOneShot(soundFile);
            Debug.Log(this.gameObject.name);
            writeToFile(this.gameObject.name);
            Counter = 0;
            SceneManager.LoadScene("Redirected walking scene");
        }
     }
}

