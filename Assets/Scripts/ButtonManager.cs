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

    public GameObject arrow;

    private int Counter = 0;
    private static bool needSound = false;
    
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
        //nonParametric.PESTSubroutine();
        //nonParametric.setStimLevel(0.0f);
        //nonParametric.setResponse(-1);
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

    void rotateRoom()
    {
        Vector3 axis = new Vector3(0, 1, 0);
        room.transform.RotateAround(player.transform.position, axis, 90f);
    }


    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "Yes" || col.gameObject.name == "No") {
            ++Counter;
            if (Counter >= 50)
            {
                needSound = true;
                Debug.Log(col.gameObject.name);
                writeToFile(col.gameObject.name);
                nonParametric.staircase();  //<- staircase
                //nonParametric.PEST();     //<- PEST

                rotateRoom();
                Counter = 0;

                arrow.transform.Rotate(new Vector3(0, 0, 1), 180f);
            }    
        }
     }
 }

