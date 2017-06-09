using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    public AudioClip soundFile;
    public RotationTest rotationTest;
    public Transform room;
    public Transform player;
    private int Counter = 0;
    
    void writeToFile(string text)
    {
        StreamWriter writer = new StreamWriter("Assets/test.txt", true);
        writer.WriteLine(text);
        writer.Close();
    }

    void resetRoom()
    {
        Quaternion rotation = Quaternion.Euler(0, player.rotation.y, 0);
        room.rotation = rotation;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "Yes" || col.gameObject.name == "No") {
            ++Counter;
            if (Counter >= 20)
            {
                GetComponent<AudioSource>().PlayOneShot(soundFile);
                Debug.Log(col.gameObject.name);
                writeToFile(col.gameObject.name);
                rotationTest.staircase();
                resetRoom();
                Counter = 0;
            }    
        }
     }
 }

