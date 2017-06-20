
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

    private SteamVR_TrackedObject trackedObj;
    public AudioSource firstVoiceover;
    public AudioSource secondVoiceover;
    public AudioClip thirdVoiceover;
    
    private static uint voiceoverSection = 1;




    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }


    void Update()
    {
        if (!firstVoiceover.isPlaying)
        {
            part1();
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
            GetComponent<AudioSource>().PlayOneShot(thirdVoiceover);
            if (voiceoverSection == 2 )
            {
                Debug.Log("selection : " + voiceoverSection);
                voiceoverSection = 3;
                rotateRoom();
            }
            else if (voiceoverSection == 3)
            {
                Debug.Log("selection : " + voiceoverSection);
                rotateRoom();
            } 
        }
    }


    void rotateRoom()
    {
		GetComponent<AudioSource>().PlayOneShot(thirdVoiceover);
        Vector3 axis = new Vector3(0, 1, 0);
        room.transform.RotateAround(player.transform.position, axis, 90f);
    }

}
