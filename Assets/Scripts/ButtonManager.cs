using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    public AudioSource soundFile;
    public Nonparametric nonParametric;
    public GameObject room;
    public GameObject player;
    private int Counter = 0;
    private static bool needSound = false;

    private static string clicked = " ";
    
    void writeToFile(string text)
    {
        StreamWriter writer = new StreamWriter("Assets/test.txt", true);
        writer.WriteLine(text);
        writer.Close();

        
    }

    private void Awake()
    {
        //PEST
        //nonParametric.PESTInitialization();
        
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

        if (needSound)
        {
            soundFile.Play();
            needSound = false;
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
        if(col.gameObject.name == "Yes" || col.gameObject.name == "No" ) {
            clicked = col.gameObject.name;
        }
     }

    private void OnCollisionExit(Collision collision)
    {
        clicked = " ";
    }

   public void recordData()
    {

        if (clicked != " ")
        {

            needSound = true;
            Debug.Log(clicked);
            writeToFile(clicked);
            //STAIRCASE
            nonParametric.staircase();
            //PEST
            //nonParametric.PEST();

            //resetRoom();
            rotateRoom();
        }


    }
}

