
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingSequence : MonoBehaviour {

    public Transform player;
    public Transform painting;
    public Transform feet;
    public GameObject arrow;
    public GameObject room;

    public SteamVR_Controller.Device left;
    public SteamVR_Controller right;

    public AudioSource firstVoiceover;
    public AudioSource secondVoiceover;
    public AudioSource thirdVoiceover;
    public AudioSource fourthVoiceover;
    public AudioSource selection;

    static uint voiceoverSection = 1;
    static bool hasPressedReticule = false;
    static bool needFourthVoiceover = false;
    static bool needDing = false;

    static string clicked;


    private void Awake()
    {
        firstVoiceover.Play();
        voiceoverSection = 2;
    }

    void Update()
    {
        if (voiceoverSection == 2 && !firstVoiceover.isPlaying)
        {
            playSecondVoiceover();
        }

        else if(voiceoverSection == 3 && !secondVoiceover.isPlaying)
        {
            playThirdVoiceover();
        }

        else if(voiceoverSection == 4 && !thirdVoiceover.isPlaying)
        {
            playFourthVoiceover();
        }

        if (needDing)
        {
            selection.Play();
            needDing = false;
        }
        Debug.Log(voiceoverSection);
    }


    public void playSecondVoiceover()
    {
        if (Vector3.Distance(player.position, feet.position) < 2)
        {
            secondVoiceover.Play();
            print("trying to play 2");
            voiceoverSection = 3;
            hasPressedReticule = false;
        }
    }


    public void playThirdVoiceover()
    {
        if(hasPressedReticule)
        {
            thirdVoiceover.Play();
            print("trying to play 3");
            voiceoverSection = 4;
        }
    }


    public void playFourthVoiceover()
    {
        if (needFourthVoiceover)
        {
            fourthVoiceover.Play();
            print("trying to play 4");
            needFourthVoiceover = false;
            voiceoverSection = 5;
        }
    }

    public void goBack()
    {
        if (Input.GetKeyDown("b"))
        {
            if(voiceoverSection > 1)
            {
                --voiceoverSection;
            }
        }
    }


    void OnCollisionEnter(Collision col)
    {
        hasPressedReticule = true; 
        clicked = col.gameObject.name;  

    }

    public void rotateRoom()
    {
        if (clicked == "Same" || clicked == "Different")
        {
            needFourthVoiceover = true;

            if (voiceoverSection == 4 && !thirdVoiceover.isPlaying)
            {
                playFourthVoiceover();
            }

            needDing = true;
            SteamVR_Fade.Start(Color.black, 0.1f);
            Vector3 axis = new Vector3(0, 1, 0);
            room.transform.RotateAround(player.transform.position, axis, 180f);
            arrow.transform.Rotate(new Vector3(0, 0, 1), 180f);
            SteamVR_Fade.Start(Color.clear, 1.2f);
        }
    }

}
