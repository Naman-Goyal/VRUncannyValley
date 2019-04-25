using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchCharacterScript : MonoBehaviour {
	public List<GameObject> avatarArray;
    public uint user_ID;

    int currAvatar = -1;

    private void Awake()
    {
        foreach (var i in avatarArray) {
            i.SetActive(false);
        }
    }

    private void Start()
    {
        currAvatar = Random.Range(0, avatarArray.Count);
        EnableChosen();
    }

    int GetNewAvatar()
    {
        avatarArray.RemoveAt(currAvatar);
        if (avatarArray.Count == 0)
            return -1;
        return Random.Range(0, avatarArray.Count);
    }

    void EnableChosen()
    {
        foreach (var i in avatarArray)
        {
            i.SetActive(false);
        }
        if (currAvatar == -1)
        {
            return;
        }
        avatarArray[currAvatar].SetActive(true);
    }

    // Meant to be called from an individual model
    public void NextAvatar()
    {
        currAvatar = GetNewAvatar();
        EnableChosen();

        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        var agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.Warp(new Vector3(24.28f, 0.26f, 26.02f));
    }
}
