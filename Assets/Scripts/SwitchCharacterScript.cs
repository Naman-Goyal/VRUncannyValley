using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchCharacterScript : MonoBehaviour {
	public List<GameObject> avatarArray;
    public uint user_ID;

    int currAvatar = 0;

    private void Awake()
    {
        foreach (var i in avatarArray) {
            i.SetActive(false);
        }
    }

    private void Start()
    {
        NextAvatar();
    }

    int GetNewAvatar()
    {
        avatarArray.RemoveAt(currAvatar);
        return Random.Range(0, avatarArray.Count);
    }

    void EnableChosen()
    {
        foreach (var i in avatarArray)
        {
            i.SetActive(false);
        }
        avatarArray[currAvatar].SetActive(true);
    }

    // Meant to be called from an individual model
    public void NextAvatar()
    {
        currAvatar = GetNewAvatar();
        EnableChosen();
    }
}
