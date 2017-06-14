using System.Collections;
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

    //QUEST specific variables

    ////////////////////////////

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

    public void QUEST()
    {
        //I know these are used but I'm not quite sure what they represent...
        float[] P = new float[81];
        float[] Q = new float[41];
        float[] Q_0 = new float[41];
        float[][] S = new float[2][];
        S[0] = new float[81];
        S[1] = new float[81];

        //TODO : set termination rules

        //general variables
        int N = 20; // interval size?
        int N2 = N * 2;
        int sDev = 12; //S : standard deviation of prior density
        int numTrials = 32; //M

        //Weibull model variables
        float delta = 0.01f; //D : wiggle room to account for user error
        float b = 3.5f / 20f; //B : slope divided by interval size? 
        float epsilon = 1.5f; //E value shouuld be 1.5 for 2AFC but 1.15 for yes/no
        

        //logistic model variables
        float lambda = 0.0f; // lapse rate (no idea what this should be?? apparently zero creates bias) 
        float beta = 3.5f; //slope of psychometric
        float gamma = 0.5f; //G : value for 2AFC but should be 0 for yes/no



        for (int i = -N2; i < N2; i++) // i = intensity in units relative to the true threshold
        {
            //P[N2 + i] = 1 - (1 - gamma) * Mathf.Exp( Mathf.Pow(-10.0f , (b * (i + epsilon)))); // Weibull Psychometric

            P[N2 + i] = gamma + (1-gamma-lambda) / (1 + Mathf.Exp(-(i - maxGain) * beta)); //Logistic function

            if ( P[N2 + i] > 1-delta) { P[N2 + i] = 1 - delta; }//accounting for user error

            S[0][N2 - i] = Mathf.Log(1 - P[N2 + i]); //information is still within the log function
            S[1][N2 - i] = Mathf.Log(P[N2 + i]);
        }

        for (int t = -N; t < N; t++) //t = possible threshold values
        {
            Q_0[N + t] = -(t / sDev) ^ 2; //What does this mean?
            Q[N + t] = Q_0[N + t];
        }

        //print : the prior estimate is is S
        //input : T0 = prior estimate 
        float T0 = 0.0f; //PLACEHOLDER
        //input : T1 = actual threshold
        //DO WE NEED THIS PART??

        int x;
        for ( int k = 0; k < numTrials; k++)
        {
            //SUBRUITINE//
            x = -N; //x = the bayesian estimate of theshold
            for (int t = -N; t < N; t++) //t = possible threshold values
            {
                if ( Q[N+t] > Q[N + x])
                {
                    x = t;
                }
            }
            ////////////////////

            //int response = Mathf.FloorToInt(P[N2 + x + T0 - T1] + RandomNumber); //simulated user

            int response = 0; //PLACEHOLDER gather response from user by applying stimulus level x + T0

            //termination criteria probably evaluated here

            for (int t = -N; t < N; t++) //t = possible threshold values
            {
                Q[N + t] = Q[N + t] + S[response][N2+t-x];
            }

        }

        for (int t = -N; t < N; t++) //t = possible threshold values
        {
            Q[N + t] = Q[N + t] - Q_0[N + t];
        }

        //SUBRUITINE//
        x = -N; //x = the bayesian estimate of theshold
        for (int t = -N; t < N; t++) //t = possible threshold values
        {
            if (Q[N + t] > Q[N + x])
            {
                x = t;
            }
        }
        ////////////////////

        //max liklihood estimate = x + T0
        writeToFile("Assets/test.txt",  string.Format("{0:N4}", (x + T0) ));

    }

    public override void ApplyRedirection()
    {
        float deltaDir;
        deltaDir = redirectionManager.deltaDir;
        InjectRotation(currentGain * redirectionManager.deltaDir);
    }

}
