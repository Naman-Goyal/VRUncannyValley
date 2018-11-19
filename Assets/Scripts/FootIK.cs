/// <summary>
/// Taken from links from the youtube video 
/// https://www.youtube.com/watch?v=NKIZCPenIas
/// Modifications made - name of animations referred to, and recommenting
/// </summary>

using UnityEngine;
using System;
  
[RequireComponent(typeof(Animator))]  


public class FootIK : MonoBehaviour {
	//To access the Animator class
	protected Animator avatar;

	//To control whether to use the FootIK
	public bool ikActive = false;

	//To control how much will affect the transform of the IK
	public float transformWeight = 1.0f;

	//To make change the value smoothly
	public float smooth = 10;

	//The position of my left foot
	public Transform footL;

	//A Offset to the foot not jam on the floor
	public Vector3 footLoffset;

	//I will use to control when affect the position during animation
	public float weightFootL;

	//The position of my right foot
	public Transform footR;

	//A Offset to the foot not jam on the floor
	public Vector3 footRoffset;

	//I will use to control when affect the position during animation
	public float weightFootR;

	//I'll save the Raycast hit position of the feet
	private Vector3 footPosL;
	private Vector3 footPosR;

	//To access my Collider
	private CapsuleCollider myCollider;

	//Default [center] of my collider
	private Vector3 defCenter;

	//Default [Height] of my collider
	private float defHeight;

	//[LayerMask] to define with layer my foot [RayCast] will collide
	public LayerMask rayLayer;

    private int speedHash;

    ResponsiveMotion responsiveMotion;

	// Use this for initialization
	void Start () 
	{
		avatar = GetComponent<Animator>();
		myCollider = GetComponent<CapsuleCollider>();

		defCenter = myCollider.center;
		defHeight = myCollider.height;

        speedHash = Animator.StringToHash("Speed");

        responsiveMotion = gameObject.GetComponent<ResponsiveMotion>();
	}
		
	void OnAnimatorIK(int layerIndex)
	{
		if(avatar)
		{	
			if(ikActive)
			{
                //Change the [transformWeight] value to 1 smoothly
                if (transformWeight != 1.0f){
					transformWeight = Mathf.Lerp(transformWeight, 1.0f, Time.deltaTime * smooth);
					if(transformWeight >= 0.99){
						transformWeight = 1.0f;
					}
				}

				//If the situation of the player is [Idle]
				if(avatar.GetFloat(speedHash) < .1){

					//Set how much will affect the IK [transform]
					avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, transformWeight);
					avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, transformWeight);
					avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, transformWeight);
					avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, transformWeight);

					IdleIK();
				}

				//If the situation of the player is [Walk] or [Run]
				else if(avatar.GetFloat(speedHash) >= .1)
                {

					//Set how much will affect the IK [transform]
					avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, transformWeight * weightFootL);
					avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, transformWeight * weightFootL);
					avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, transformWeight * weightFootR);
					avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, transformWeight * weightFootR);

					WalkRunIK();
				}
			}
			else
			{	
				//Change the [transformWeight] value to 0 smoothly
				if(transformWeight != 0.0f){
					transformWeight = Mathf.Lerp(transformWeight, 0.0f, Time.deltaTime * smooth);
					if(transformWeight <= 0.01){
						transformWeight = 0.0f;
					}
				}

				//Set how much will affect the IK [transform]
				avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, transformWeight);
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, transformWeight);
				avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, transformWeight);
				avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, transformWeight);
			}
		}
	}


	void IdleIK(){
		RaycastHit hit;

		footPosL = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);

        // See how far the ground is
		if(Physics.Raycast(footPosL + Vector3.up, Vector3.down, out hit, 2.0f, rayLayer))
		{
			//Set the new position / rotation of IK
			avatar.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + footLoffset);
			avatar.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(footL.forward, hit.normal),  hit.normal));

			//Save the collision position
			footPosL = hit.point;
		}				

		footPosR = avatar.GetIKPosition(AvatarIKGoal.RightFoot);

        // See how far ground is
		if(Physics.Raycast(footPosR + Vector3.up, Vector3.down, out hit, 2.0f, rayLayer))
		{
            // Set the new position / rotation of IK
			avatar.SetIKPosition(AvatarIKGoal.RightFoot,hit.point + footRoffset);
			avatar.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.LookRotation(Vector3.ProjectOnPlane(footR.forward, hit.normal),  hit.normal));

			//Save the collision position
			footPosR = hit.point;
		}
	}
    void WalkRunIK()
    {
        RaycastHit hit;

        footPosL = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);

        // See how far the ground is
        if (Physics.Raycast(footPosL + Vector3.up, Vector3.down, out hit, 2.0f, rayLayer))
        {
            //Set the new position / rotation of IK
            avatar.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + footLoffset);
            avatar.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(footL.forward, hit.normal), hit.normal));

            //Save the collision position
            footPosL = hit.point;
        }

        footPosR = avatar.GetIKPosition(AvatarIKGoal.RightFoot);

        // See how far ground is
        if (Physics.Raycast(footPosR + Vector3.up, Vector3.down, out hit, 2.0f, rayLayer))
        {
            // Set the new position / rotation of IK
            avatar.SetIKPosition(AvatarIKGoal.RightFoot, hit.point + footRoffset);
            avatar.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(footR.forward, hit.normal), hit.normal));

            //Save the collision position
            footPosR = hit.point;
        }
    }

	void Update () 
	{ 
		if(ikActive){
			//If the situation of the player is [Idle] and [ikActive] is [true]
			if(avatar.GetFloat(speedHash) < .1)
            {
				IdleUpdateCollider();
			}
			//If the situation of the player is [Walk] or [Run]
			else if(avatar.GetFloat(speedHash) >= .1)
            {
				WalkRunUpdateCollider();
			}
		}else{
			//Reset the values of my Collider
			myCollider.center = new Vector3(0, Mathf.Lerp(myCollider.center.y, defCenter.y, Time.deltaTime * smooth) ,0);
			myCollider.height = Mathf.Lerp(myCollider.height, defHeight, Time.deltaTime * smooth);
		}
	}

	void IdleUpdateCollider () 
	{	
		//Create this value to calculate the height difference of the feet
		float dif;

		//Calculate the height difference of the feet
		dif = Mathf.Abs(footPosL.y - footPosR.y);

		//Change the Collider values depending on the value [dif]
		myCollider.center = new Vector3(0, Mathf.Lerp(myCollider.center.y, defCenter.y + dif, Time.deltaTime) ,0);
		myCollider.height = Mathf.Lerp(myCollider.height, defHeight - (dif / 2), Time.deltaTime);
	}

	void WalkRunUpdateCollider () 
	{
		//Create this value to use the [RaycastHit]
		RaycastHit hit;

		//Creating this value to save the height of the floor of the position I am
		Vector3 myGround = Vector3.zero;

		//Create this value to calculate the height difference
		Vector3 dif = Vector3.zero;

		//Check the height of the floor of the position I am
		if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 3.0f, rayLayer))
		{
			myGround = hit.point;
		}

		//RayCast to check the height of the position where I'm going
		if(Physics.Raycast(transform.position + (((transform.forward * (myCollider.radius))) + (myCollider.attachedRigidbody.velocity * Time.deltaTime)) + Vector3.up, Vector3.down, out hit, 2.0f, rayLayer))
		{
			//Calculate the height difference between the height of the position I'm with the height of the position where I'm going
			dif = hit.point - myGround;
			//Não deixar o valor ser menor que 0
			//Do not let the value be less than 0
			if(dif.y < 0){dif *= -1;}
		}
		//If the [dif] is less than 0.5
		if(dif.y < 0.5f){
			//Change the Collider values depending on the value [dif]
			myCollider.center = new Vector3(0, Mathf.Lerp(myCollider.center.y, defCenter.y + dif.y, Time.deltaTime * smooth) ,0);
			myCollider.height = Mathf.Lerp(myCollider.height, defHeight - (dif.y / 2), Time.deltaTime * smooth);
		//If the [dif] is not less than 0.5
		}else{
			//Reset the values of my Collider
			myCollider.center = new Vector3(0, Mathf.Lerp(myCollider.center.y, defCenter.y, Time.deltaTime * smooth) ,0);
			myCollider.height = Mathf.Lerp(myCollider.height, defHeight, Time.deltaTime * smooth);
		}
		
	}
}
