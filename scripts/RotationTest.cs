using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redirection;
using System.Text;
using System.IO;
using System.Linq;

public class RotationTest : Redirector
{
    public GameObject target;
    public Vector3 userPosition;

    private Transform prefabHUD = null;
    Transform instanceHUD;

    float rotGain = 10;
    float adjustmentAmount = 1f;
    bool lowering = true;


    void staircase() {

        StreamReader reader = new StreamReader("Assets/test.txt", Encoding.Default);
        string lastLine = File.ReadAllLines("file.txt").Last();

        if (lastLine == "Yes"){
            adjustmentAmount = (lowering == true) ? adjustmentAmount : (adjustmentAmount / 2);
            lowering = true;
        }

        if (lastLine == "No"){
            adjustmentAmount = (lowering == false) ? adjustmentAmount : (adjustmentAmount / 2);
            lowering = false;
        }

        rotGain = (lowering == true) ? (rotGain - adjustmentAmount) : (rotGain + adjustmentAmount);
    }



    public override void ApplyRedirection()
    {

        float deltaDir;
        //Vector3 targetPosition;
        //float angleToTarget;
        //targetPosition = Utilities.FlattenedPos3D(userPosition);
        //angleToTarget = Utilities.GetSignedAngle(redirectionManager.currDir, targetPosition - redirectionManager.currPos);


        deltaDir = redirectionManager.deltaDir;
        //if (Mathf.Abs(angleToTarget) >= Mathf.Deg2Rad * 10){
        InjectRotation(rotGain * redirectionManager.deltaDir);
        //}

        /* public void SetHUD()
         {
             if (prefabHUD == null)
                 prefabHUD = Resources.Load<Transform>("TwoOneTurnResetter HUD");
                 instanceHUD = Instantiate(prefabHUD);
                 instanceHUD.parent = redirectionManager.headTransform;
                 instanceHUD.localPosition = instanceHUD.position;
                 instanceHUD.localRotation = instanceHUD.rotation;
         } */

    }
}
