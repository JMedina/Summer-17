using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

public class Utils : MonoBehaviour {

    public static void writeToFile(string path, string text)
    {
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(text);
        writer.Close();
    }


    public static string getLastLine(string path)
    {
        string lastLine;
        using (StreamReader reader = new StreamReader(path, Encoding.Default))
        {
            string[] lineList = File.ReadAllLines(path);
            lastLine = lineList.Last();
        }
        return lastLine;
    }

    public static void rotateRoom(GameObject room, GameObject arrow, GameObject player)
    {
        SteamVR_Fade.Start(Color.black, 0.1f);
        Vector3 axis = new Vector3(0, 1, 0);
        room.transform.RotateAround(player.transform.position, axis, 180f);
        arrow.transform.Rotate(new Vector3(0, 0, 1), 180f);
        SteamVR_Fade.Start(Color.clear, 1.2f);
    }
}
