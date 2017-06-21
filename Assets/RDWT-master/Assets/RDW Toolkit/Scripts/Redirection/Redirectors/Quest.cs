/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redirection;
using System.Text;
using System.IO;
using System.Linq;

public class Quest : Redirector
{

    public static float minGain = -.99f;
    public static float maxGain = .49f;
    static float currentGain = .5f;

    public static uint maxTrials;
    static uint trialNum;

    public GameObject yesButton;
    public GameObject noButton;


    private string getLastLine(string path)
    {
        string lastLine;
        using (StreamReader reader = new StreamReader("Assets/test.txt", Encoding.Default))
        {
            lastLine = File.ReadAllLines("file.txt").Last();
        }
        return lastLine;
    }

    void writeToFile(string path, string text)
    {
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(text);
        writer.Close();
    }

    public void quest()
    {
        int interval = 20;
        float stDev = 12;
        float error = 0.01f;
        float G = 0.5f;
        float B = 3.5f / interval;
        float E = 1f / 5f;
        uint maxTrials = 32;
        float priorEstimate = 0f;

        float[] probabilityDensity = new float[4*interval];
        float[] successFunction = new float[4*interval];
        float[] failureFunction = new float[4*interval];
        float[] questFunction = new float[interval];
        float[] q0 = new float[interval];

        for(int X = -2*interval; X < 2*interval; ++X)
        {
            probabilityDensity[2*interval + X] = Math.min(1 - error, 1 - (1-G) * Math.Exp(-10 ^ (B*X*E)));
            successFunction[2*interval - X] = Math.Log(    P[2*interval + X]);
            failureFunction[2*interval - X] = Math.Log(1 - P[2*interval + X]); 
        } 

        for(int T = -interval; T < interval; ++T)
        {
            q0[interval + T] = Math.Pow (-(T/stDev), 2.0f);
            questFunction[interval + T] = q0[interval + T];            
        }

        for(int trialNum = 0; trialNum < maxTrials; ++trialNum)
        {
            goto subroutine; main:
            int Result = 0;//REPLACE THIS WITH ACTUAL TEST
            
            for(int T = -interval; T < interval; ++T)
            {
                questFunction[interval + T] += q0[interval + T];
            }
        }


        subroutine:
            X = -interval;
            for (int t = -interval; t < interval; ++t)
            {
                if (questFunction[interval + t] > questfunction[interval + X])
                {
                    X = t;
                }
            }
            goto main;


    }




    public override void ApplyRedirection()
    {
        float deltaDir;
        deltaDir = redirectionManager.deltaDir;
        InjectRotation(currentGain * redirectionManager.deltaDir);
    }

}
*/