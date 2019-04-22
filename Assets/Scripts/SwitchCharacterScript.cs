using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchCharacterScript : MonoBehaviour {

	public GameObject avatar1, avatar2, avatar3, avatar4, avatar5;
	// public GameObject[] avatarArray;

	int whichAvatarIsOn = 1;

	void Start () {
		GameObject[] avatarArray = { avatar1, avatar2, avatar3, avatar4, avatar5 };

		int number = Random.Range(1, 6);
		avatar1.gameObject.SetActive (false);
		avatar2.gameObject.SetActive (false);
		avatar3.gameObject.SetActive (false);
		avatar4.gameObject.SetActive (false);
		avatar5.gameObject.SetActive (false);
		avatarArray[number].SetActive (true);
	}

	public void SwitchAvatar(int n)
	{
			GameObject[] avatarArray = { avatar1, avatar2, avatar3, avatar4, avatar5 };
			avatar1.gameObject.SetActive (false);
			avatar2.gameObject.SetActive (false);
			avatar3.gameObject.SetActive (false);
			avatar4.gameObject.SetActive (false);
			avatar5.gameObject.SetActive (false);
			avatarArray[n].SetActive (true);
	}


    void Update()
    {
        if (Input.GetKeyDown("space"))	//get some message



        {
			int number = Random.Range(1, 6);
            SwitchAvatar(number);
            Debug.Log(number);
        }
    }

}
