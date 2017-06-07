using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConstantStimuli : Redirector {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    int MAX_NUM_TRIALS = 3;
    int FREQ_TEST = 3;

    public override void ApplyRedirection()
    {
        
        //rotGains tests 0-max (increasing rotation)
        //rotMins tests min-0 (decreasing rotation)
        float[] rotGains = new float[MAX_NUM_TRIALS+1];
        float[] rotMins = new float[MAX_NUM_TRIALS+1];

        float[] frequency = new float[MAX_NUM_TRIALS * 2];

        float[] rotGainsResponses = new float[MAX_NUM_TRIALS];
        float[] rotMinsResponses = new float[MAX_NUM_TRIALS];

        //make sure we get the extremeties 
        rotGains[0] = redirectionManager.MAX_ROT_GAIN + 1.0f;
        rotMins[0] = redirectionManager.MIN_ROT_GAIN;

        rotGains[1] = 0.0f;
        rotMins[1] = 0.0f;

        System.Random rnd = new System.Random();

        

        for (int i = 2; i < MAX_NUM_TRIALS; i++)
        {
            
            //random distribution
            //rotGains[i] = (float)rnd.NextDouble() * redirectionManager.MAX_ROT_GAIN;
            //rotMins[i] = ( (float)rnd.NextDouble() - 1.0f ) * redirectionManager.MIN_ROT_GAIN

            //uniform distribution
            float step = (redirectionManager.MAX_ROT_GAIN + 1.0f) / ( MAX_NUM_TRIALS - 1.0f);
            rotGains[i] = (i - 1) * step;
            step = redirectionManager.MIN_ROT_GAIN / MAX_NUM_TRIALS;
            rotMins[i] = (i - 1) * step;          

        }


        int flag = 0;
        
        while (flag == 0) {

            int k = rnd.Next(-MAX_NUM_TRIALS, MAX_NUM_TRIALS);
                    
            if (frequency[k+MAX_NUM_TRIALS] < FREQ_TEST) {

                if (k < 0) {
                  
                    //Debug.Log("I'm trying to test " + rotMins[-1 * k]);
                    //InjectRotation(rotMins[-1 * k] * redirectionManager.deltaDir);
                } else {
                    int j = -1 * k;
                                       
                    //Debug.Log("I'm trying to test " + rotGains[k]);
                    //InjectRotation(rotGains[k] * redirectionManager.deltaDir);
                }

                //record responses

                frequency[k+MAX_NUM_TRIALS]++;

            }

            int count = 0;

            for (int i = -MAX_NUM_TRIALS; i < MAX_NUM_TRIALS; i++) {
                if (frequency[i + MAX_NUM_TRIALS] >= FREQ_TEST) {
                    count++;
                }    

            }

            if (count == MAX_NUM_TRIALS * 2) { flag = 1; }

        }

        Debug.Log("DONE");

    }
}
