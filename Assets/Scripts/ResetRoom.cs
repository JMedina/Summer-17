using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRoom : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    public Transform room;
    public Transform player;


    // Use this for initialization
    void Start () {
        Quaternion rotation = Quaternion.Euler(0, player.rotation.y, 0);
        room.rotation = rotation;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
