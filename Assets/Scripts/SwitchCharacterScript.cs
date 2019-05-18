using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchCharacterScript : MonoBehaviour {
	public List<GameObject> avatarArray;
    [System.NonSerialized]
    public uint user_ID;

    public TestIDSaver testIDsaver;

    int currAvatar = -1;

    private void Awake()
    {
        foreach (var i in avatarArray) {
            i.SetActive(false);
        }
    }

    private void Start()
    {

        user_ID = testIDsaver.testID;
        currAvatar = -1;
        EnableChosen();
        StartCoroutine(WaitForEnter());
    }

    private void OnDestroy()
    {
        testIDsaver.testID = testIDsaver.testID + 1;
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
        agent.Warp(new Vector3(10.696f, 8.5349f, 13.778f));
    }

    IEnumerator WaitForEnter()
    {
        while (true)
        {
            yield return null;
            if (Input.GetKeyDown("space"))
            {
                currAvatar = Random.Range(0, avatarArray.Count);
                EnableChosen();
                yield return true;
                break;
            }

        }
    }
}
