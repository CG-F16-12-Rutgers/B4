using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	public Transform target;
	public AudioClip shoutingClip;
	public float turnSmoothing = 15f;
	public float speedDampTime = 0.1f;
	public bool aim;
	public Transform spine;
	public float aimingZ = 240f; 
	public float aimingX = 5f;
	public float aimingY = -50f;
	//The point in the ray we do from our camera, basically how far the character looks
	public float point = 30; 

	private Animator anim;
	private HashIDs hash;
	Rigidbody playerRigidbody;
	private Transform cam;
	private Vector3 camForward;
	private Vector3 move;

	float turnAmount;
	float forwardAmount;
	Vector3 lookPos;

	float autoTurnThreshold = 10;
	float autoTurnSpeed = 20;
	float movingTurnSpeed = 360;
	float stationaryTurnSpeed = 180;

	AudioSource footStepAudio;

	void Awake ()
	{
		if(Camera.main != null)
		{
			cam = Camera.main.transform;
		}

		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

		anim.SetLayerWeight(1, 1f);
		anim.SetLayerWeight(2, 1f);
		anim.SetLayerWeight(3, 1f);
		anim.SetLayerWeight(4, 1f);

		playerRigidbody = GetComponent<Rigidbody> ();

		footStepAudio = GetComponent<AudioSource> ();
	}


	void FixedUpdate ()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		bool sneak = Input.GetButton("Sneak");

		MovementManagement(h, v, sneak);

		bool shout = Input.GetButtonDown("Attract");

		anim.SetBool(hash.shoutingBool, shout);

		AudioManagement(h,v, shout);
	}
		
	void LateUpdate()
	{
		if(aim)
		{
			//pass the new rotation to the IK bone
			Vector3 eulerAngleOffset = Vector3.zero;
			eulerAngleOffset = new Vector3(aimingX, aimingY, aimingZ);

			//do a ray from the center of the camera and forward
			Ray ray = new Ray(cam.position, cam.forward);

			//find where the character should look
			Vector3 lookPosition = ray.GetPoint(point);

			//and apply the rotation to the bone
			spine.LookAt(lookPosition);
			spine.Rotate(eulerAngleOffset, Space.Self);
		}
	}

	void MovementManagement (float horizontal, float vertical, bool sneaking)
	{
		anim.SetBool(hash.sneakingBool, sneaking);

		if (!aim) 
		{
			camForward = Vector3.Scale (cam.forward, new Vector3 (1, 0, 1)).normalized;

			move = vertical * camForward + horizontal * cam.right;
		}
		else
		{
			move = Vector3.zero;

			Vector3 dir = lookPos - transform.position;
			dir.y = 0;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

			anim.SetFloat(hash.speedFloat, vertical);
			anim.SetFloat(hash.angularSpeedFloat, horizontal);
		}

		if (move.magnitude > 1)
			move.Normalize ();

		bool walkToggle = Input.GetKey (KeyCode.LeftShift) || aim;

		//the walk multiplier determines if the character is running or walking
		float walkMultiplier = 1;

		if(walkToggle) {
			walkMultiplier = 0.5f;
		}

		lookPos = cam != null ? transform.position + cam.forward * 100 : transform.position + transform.forward * 100;

		move *= walkMultiplier;

		Move (move, aim);
	}

	void Move(Vector3 movement, bool aiming)
	{
		if (movement.magnitude > 1)
			movement.Normalize ();

		Vector3 localMove = transform.InverseTransformDirection (movement);

		turnAmount = Mathf.Atan2 (localMove.x, localMove.z);
		//Our forward amount is our localmove forward
		forwardAmount = localMove.z;

		if(!aiming)
		{
			TurnTowardsCameraForward();
			ApplyExtraTurnRotation ();
		}

		anim.SetFloat (hash.speedFloat, forwardAmount, 0.1f, Time.deltaTime);
		anim.SetFloat (hash.angularSpeedFloat, turnAmount, 0.1f, Time.deltaTime);
	}

	void ApplyExtraTurnRotation ()
	{
		float turnSpeed = Mathf.Lerp (stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
		transform.Rotate (0, turnAmount * turnSpeed * Time.deltaTime, 0);
	}

	void TurnTowardsCameraForward()
	{
		//If the absolute value of the forward amount is less than .01
		if(Mathf.Abs(forwardAmount) < .01f)
		{
			//Find our look angle
			Vector3 lookDelta = transform.InverseTransformDirection(lookPos - transform.position);
			float lookAngle = Mathf.Atan2(lookDelta.x,lookDelta.z) * Mathf.Rad2Deg;

			//and if it's higher than our turn threshold
			if(Mathf.Abs(lookAngle) > autoTurnThreshold)
			{
				//correct the character's rotation
				turnAmount += lookAngle * autoTurnSpeed * .001f;
			}
		}
	}

	void AudioManagement (float horizontal, float vertical, bool shout)
	{
		if(anim.GetCurrentAnimatorStateInfo(0).fullPathHash == hash.locomotionState)
		{
			if(!footStepAudio.isPlaying && horizontal != 0 && vertical !=0)
				footStepAudio.Play();
		}
		else
			footStepAudio.Stop();

		if(shout)
			AudioSource.PlayClipAtPoint(shoutingClip, transform.position);
	}
}
