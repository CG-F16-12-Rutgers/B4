using UnityEngine;
using System.Collections;

public class LoadNextLevel2 : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") 
		{
			Application.LoadLevel("SkyCity") ;
		}
	}
}
