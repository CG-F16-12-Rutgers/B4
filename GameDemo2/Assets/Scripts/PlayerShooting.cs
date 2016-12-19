using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

	public int damagePerShot = 20;
	public float timeBetweenBullets = 0.15f;
	public float range = 100f;
	public float aimingWeight;
	public ParticleSystem gunParticles;

	float timer;
	Ray shootRay;
	RaycastHit shootHit;

	LineRenderer gunLine;
	AudioSource gunAudio;
	Light gunLight;
	float effectsDisplayTime = 0.2f;
	private Animator anim;	
	private HashIDs hash;
	private Transform player;
	private PlayerMovement playerMove;
	private Transform cam;

	void Awake ()
	{
		anim = GetComponent<Animator>();
		gunLine = GetComponentInChildren <LineRenderer> ();
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		gunLight = GetComponentInChildren<Light> ();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		playerMove = GetComponent<PlayerMovement> ();

		if (Camera.main != null) 
		{
			cam = Camera.main.transform;
		}
	}


	void FixedUpdate ()
	{
		if (Input.GetButton ("Aim")) {
			aimingWeight = 1;
			anim.SetBool ("Aim", true);
			playerMove.aim = true;

			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

			anim.SetFloat (hash.speedFloat, horizontal);
			anim.SetFloat (hash.angularSpeedFloat, vertical);

			timer += Time.deltaTime;

			if (Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0) {
				Shoot ();
			}

			if (timer >= timeBetweenBullets * effectsDisplayTime) {
				DisableEffects ();
			}
		}
		else
		{
			aimingWeight = 0;
			anim.SetBool ("Aim", false);
			playerMove.aim = false;
		}

		Vector3 normalState = new Vector3 (0, 0, -2f);
		Vector3 aimingState = new Vector3 (0, 0, -0.5f);

		Vector3 pos = Vector3.Lerp (normalState, aimingState, aimingWeight);

		cam.transform.localPosition = pos;

	}

	public void DisableEffects ()
	{
		gunLine.enabled = false;
		gunLight.enabled = false;
	}


	void Shoot ()
	{
		//gunParticles.Emit (1);

		timer = 0f;

		//gunAudio.Play ();
		//AudioSource.PlayClipAtPoint(shotClip, laserShotLight.transform.position);

		gunLight.enabled = true;

		anim.SetTrigger ("Fire");

		//gunParticles.Stop ();
		//gunParticles.Play ();

		gunLine.enabled = true;
		gunLine.SetPosition (0, gunLine.transform.position);

		shootRay.origin = gunLine.transform.position;
		shootRay.direction = gunLine.transform.forward;

		if(Physics.Raycast (shootRay, out shootHit, range))
		{
			print (shootHit.collider.name);

			EnemyHealth enemyHealth = shootHit.collider.GetComponentInParent<EnemyHealth> ();

			if(enemyHealth != null)
			{
				enemyHealth.TakeDamage (damagePerShot, shootHit.point);
			}
			gunLine.SetPosition (1, shootHit.point);
		}
		else
		{
			gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
		}
	}

}
