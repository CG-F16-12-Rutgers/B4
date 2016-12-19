 using UnityEngine;
using System.Collections;

public class CCTVPlayerDetection : MonoBehaviour {

	private GameObject player;
	private LastPlayerSighting lastPlayerSighting;


	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag(Tags.player);
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	}


	void OnTriggerStay (Collider other)
	{
		if(other.gameObject == player)
		{
			Vector3 relPlayerPos = player.transform.position - transform.position;
			RaycastHit hit;

			if(Physics.Raycast(transform.position, relPlayerPos, out hit))
				// If the raycast hits the player...
			if(hit.collider.gameObject == player)
				// ... set the last global sighting of the player to the player's position.
				lastPlayerSighting.position = player.transform.position;
		}
	}

}
