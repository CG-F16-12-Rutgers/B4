using UnityEngine;
using System.Collections;

public class EnemyShooting : MonoBehaviour {

	public float maximumDamage = 120f;
	public float minimumDamage = 45f;
	public AudioClip shotClip;
	public float flashIntensity = 3f;
	public float fadeSpeed = 10f;
	public float range = 100f;

	private Animator anim;
	private HashIDs hash;				
	private LineRenderer laserShotLine;					
	private Light laserShotLight;						
	private SphereCollider col;							
	private Transform player;							
	private PlayerHealth playerHealth;				
	private bool shooting;
	private float scaledDamage;

	Ray shootRay;
	RaycastHit shootHit;

	void Awake ()
	{
		anim = GetComponent<Animator>();
		laserShotLine = GetComponentInChildren<LineRenderer>();
		laserShotLight = laserShotLine.gameObject.GetComponent<Light>();
		col = GetComponent<SphereCollider>();
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		playerHealth = player.gameObject.GetComponent<PlayerHealth>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

		// The line renderer and light are off to start.
		laserShotLine.enabled = false;
		laserShotLight.intensity = 0f;

		scaledDamage = maximumDamage - minimumDamage;
	}


	void Update ()
	{
		float shot = anim.GetFloat(hash.shotFloat);

		if(shot > 0.5f && !shooting)
			// ... shoot
			Shoot();

		if(shot < 0.5f)
		{
			shooting = false;
			laserShotLine.enabled = false;
		}

		laserShotLight.intensity = Mathf.Lerp(laserShotLight.intensity, 0f, fadeSpeed * Time.deltaTime);
	}


	void OnAnimatorIK (int layerIndex)
	{
		float aimWeight = anim.GetFloat(hash.aimWeightFloat);

		anim.SetIKPosition(AvatarIKGoal.RightHand, player.position + Vector3.up * 1.5f);

		anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
	}
		

	void Shoot ()
	{
		shooting = true;

		// The fractional distance from the player, 1 is next to the player, 0 is the player is at the extent of the sphere collider.
		float fractionalDistance = (col.radius - Vector3.Distance(transform.position, player.position)) / col.radius;

		// The damage is the scaled damage, scaled by the fractional distance, plus the minimum damage.
		float damage = scaledDamage * fractionalDistance + minimumDamage;

		shootRay.origin = laserShotLine.transform.position;
		shootRay.direction = player.transform.position - transform.position;

		laserShotLine.SetPosition(0, laserShotLine.transform.position);

		if(Physics.Raycast (shootRay, out shootHit, range))
		{
			PlayerHealth playerHealth = shootHit.collider.GetComponent<PlayerHealth> ();

			if(playerHealth != null)
			{
				playerHealth.TakeDamage(damage);
			}
			laserShotLine.SetPosition (1, shootHit.point);
		}
		else
		{
			laserShotLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
		}

		//laserShotLine.SetPosition(1, player.position + Vector3.up * 1.5f);

		laserShotLine.enabled = true;

		laserShotLight.intensity = flashIntensity;

		AudioSource.PlayClipAtPoint(shotClip, laserShotLight.transform.position);

	}
}
