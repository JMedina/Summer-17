using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;
    private Vector3 hitPoint;

    public GameObject reticulePrefab;
    private GameObject reticule;
    private Transform reticuleTransform;
    public TrainingSequence trainingSequence;

    public ButtonManager buttonManager;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start()
    {
        reticule = Instantiate(reticulePrefab);
        reticuleTransform = reticule.transform;
    }

    private void ShowReticule(RaycastHit hit)
    {
        reticule.SetActive(true);
        reticuleTransform.position = hitPoint;
        reticuleTransform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
    }


    void Update()
    {
        // If the touchpad is held down
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {

            reticule.SetActive(false);
            RaycastHit hit;

            // Shoot ray from controller. If it hits something, store point where it hit and show reticule
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
            {
                hitPoint = hit.point;
                ShowReticule(hit);
            }
        }

        //If you release the touchpad, check to see if you hit a button
        else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (buttonManager)
            {
                buttonManager.resetRoom();
            }
            else if (trainingSequence)
            {
                trainingSequence.rotateRoom();
            }
        }
        
        // Hide reticule when touchpad isn't being pressed
        else
        {
            reticule.SetActive(false);
            
        }
    }
}
