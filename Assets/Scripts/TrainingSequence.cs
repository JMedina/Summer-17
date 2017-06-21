
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingSequence : MonoBehaviour {

    public Transform player;
    public Transform painting;
    public GameObject room;

    public SteamVR_Controller.Device left;
    public SteamVR_Controller right;

    public AudioSource firstVoiceover;
    public AudioSource secondVoiceover;
    public AudioSource thirdVoiceover;
    public AudioSource selection;
    
    private static uint voiceoverSection = 1;

    static bool thirdVoiceNeeded = false;
    static bool selectionNeeded = false;


    void Awake()
    {
      
    }


    void Update()
    {
        if (!firstVoiceover.isPlaying)
        {
            part1();
        }
        
        if (thirdVoiceNeeded)
        {
            PlayThird();
            thirdVoiceNeeded = false;
        }

        if (selectionNeeded)
        {
            PlayDing();
            selectionNeeded = false;
        }
    }


    public void part1()
    {
        //if looking in the direction of the painting, tell them to click the reticule
        if (voiceoverSection == 1)
        {
            float dot = Vector3.Dot(player.forward, (painting.position - player.position).normalized);
            if (dot > 0.85f) {
                voiceoverSection = 2;
                //GetComponent<AudioSource>().PlayOneShot(secondVoiceover);
                secondVoiceover.Play();

            }
        }
    }


    void OnCollisionEnter(Collision col)
    {
        //The first time, will tell user to keep playing until they are ready to start.
        //Subsequent times will just rotate the room as usual.
        if (col.gameObject.name == "Yes" || col.gameObject.name == "No")
        {
            selectionNeeded = true;

            if (voiceoverSection == 2 )
            {
                //Debug.Log("selection : " + voiceoverSection);
                thirdVoiceNeeded = true;
                voiceoverSection = 3;
                rotateRoom();
            }
            else if (voiceoverSection == 3)
            {
                //Debug.Log("selection : " + voiceoverSection);
                rotateRoom();
            } 
        }
    }

    void PlayThird()
    {
        thirdVoiceover.Play();
    }

    void PlayDing()
    {
        selection.Play();
    }


    void rotateRoom()
    {
		//GetComponent<AudioSource>().PlayOneShot(thirdVoiceover);
        Vector3 axis = new Vector3(0, 1, 0);
        room.transform.RotateAround(player.transform.position, axis, 90f);
    }

}
