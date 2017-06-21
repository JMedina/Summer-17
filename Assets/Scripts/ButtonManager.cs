using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    public AudioClip soundFile;
    //public RotationTest rotationTest;
    public Nonparametric nonParametric;
    public GameObject room;
    public GameObject player;
    private int Counter = 0;
    
    void writeToFile(string text)
    {
        StreamWriter writer = new StreamWriter("Assets/test.txt", true);
        writer.WriteLine(text);
        writer.Close();
    }

    void Update()
    {
        if (Input.GetKeyDown("y"))
        {
            writeToFile("Yes");
            nonParametric.staircase();
            rotateRoom();
        }
        if (Input.GetKeyDown("n"))
        {
            writeToFile("No");
            nonParametric.staircase();
            rotateRoom();
        }
    }


    /*void resetRoom()
    {
        Quaternion rotation = Quaternion.Euler(0, player.transform.rotation.y, 0);
        room.transform.rotation = rotation;
    }*/

    void rotateRoom()
    {
        Vector3 axis = new Vector3(0, 1, 0);
        room.transform.RotateAround(player.transform.position, axis, 90f);
    }


    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "Yes" || col.gameObject.name == "No") {
            ++Counter;
            if (Counter >= 100)
            {
                GetComponent<AudioSource>().PlayOneShot(soundFile);
                Debug.Log(col.gameObject.name);
                writeToFile(col.gameObject.name);
                nonParametric.staircase();
                //resetRoom();
                rotateRoom();
                Counter = 0;
            }    
        }
     }
 }

