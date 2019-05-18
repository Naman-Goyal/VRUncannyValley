using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GetDataScript : MonoBehaviour {
    uint id;
    public string model_name;

    public FoveInterface2 fove;

    private List<string []> output;

    Texture2D tex;
    Rect rectangle;

	void OnEnable () {
        //get ID from above
        // open the file
        output = new List<string[]>();

        id = gameObject.transform.parent.gameObject.GetComponent<SwitchCharacterScript>().user_ID;
        output.Add(GetFirstRow());
	}

    private void Update()
    {
        StartCoroutine(getData());
    }

    IEnumerator getData()
    {
        yield return new WaitForEndOfFrame();
        string[] row = new string[7];
        row[0] = id.ToString();
        row[1] = fove.GetPupilDilation().ToString();
        row[2] = GetBrightness();
        row[3] = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).ToString();
        row[4] = ((int)fove.CheckEyesClosed()).ToString();
        row[5] = gameObject.GetComponent<NavMeshMove>().currPhase.ToString();
        row[6] = model_name;

        output.Add(row);
    }

    private void OnDisable()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < output.Count; i++)
        {
            sb.AppendLine(string.Join(",", output[i]));
        }

        //var file = Path.Combine(Application.persistentDataPath, id.ToString() + "_" + model_name + ".csv");
        var file = "Assets/SaveData/" + id.ToString() + "_" + model_name + ".csv";
        Debug.Log(file.ToString());
        StreamWriter outStream = File.CreateText(file);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    string[] GetFirstRow()
    {
        //TestID, pupil dilation, brightness, time, blinks
        string[] row = new string[7];
        row[0] = "Test ID";
        row[1] = "Pupil dilation";
        row[2] = "Brightness";
        row[3] = "Time epoch";
        row[4] = "Eyes closed";
        row[5] = "Phase";
        row[6] = "Model name";

        return row;
    }

    string GetBrightness()
    {
        tex = new Texture2D(4, 4, TextureFormat.RGB24, false);
        rectangle = new Rect(0, 0, 4, 4);
        var oldRT = RenderTexture.active;

        tex.ReadPixels(rectangle, 0, 0);
        tex.Apply();

        

        RenderTexture.active = oldRT;
        return CalcBrightness().ToString();
    }

    double CalcBrightness()
    {
        var pixels = tex.GetPixels();

        double brightness = 0;

        for (int i = 0; i < pixels.Length; i++)
        {
            //(0.2126*R + 0.7152*G + 0.0722*B)
            brightness += 0.2126 * pixels[i].r + 0.7152 * pixels[i].g + 0.0722 * pixels[i].b;
        }

        return brightness /= pixels.Length;
    }

}
