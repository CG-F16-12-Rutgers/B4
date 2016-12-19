using UnityEngine;
using System.Collections;

public class LoadNextLevel : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") 
		{
			Application.LoadLevel("Scene1") ;
		}
	}
}
