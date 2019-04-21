using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GetDataScript : MonoBehaviour {
    public uint id;
    public string model_name;

    public FoveInterface2 fove;

    private List<string []> output;

	// Use this for initialization
	void Start () {
        //get ID from above
        // open the file
        var folder = Path.Combine(Application.persistentDataPath, id.ToString() + "_" + model_name);

        output.Add(GetFirstRow());
	}
	
	// Update is called once per frame
	void Update () {
        string[] row = new string[5];
        row[0] = id.ToString();
        row[1] = fove.GetPupilDilation().ToString();
        row[2] = GetBrightness();
        row[3] = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).ToString();
        row[4] = ((int) fove.CheckEyesClosed()).ToString();
	}

    string[] GetFirstRow()
    {
        //TestID, pupil dilation, brightness, time, blinks
        string[] row = new string[5];
        row[0] = "Test ID";
        row[1] = "Pupil dilation";
        row[2] = "Brightness";
        row[3] = "Time epoch";
        row[4] = "Eyes closed";

        return row;
    }

    string GetBrightness()
    {
        return "";
    }

}
