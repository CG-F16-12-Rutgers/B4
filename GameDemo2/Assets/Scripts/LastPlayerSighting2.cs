using UnityEngine;
using System.Collections;

public class LastPlayerSighting2 : MonoBehaviour
{
	public Vector3 position = new Vector3(1000f, 1000f, 1000f);			
	public Vector3 resetPosition = new Vector3(1000f, 1000f, 1000f);	
	public float lightHighIntensity = 0.25f;						
	public float lightLowIntensity = 0f;								// The directional light's intensity when the alarms are on.
	public float fadeSpeed = 7f;										// How fast the light fades between low and high intensity.
	public float musicFadeSpeed = 1f;									// The speed at which the 


	private AlarmLight alarm;										// Reference to the AlarmLight script.
	private Light mainLight;											// Reference to the main light.
	private AudioSource audio;	
	private AudioSource panicAudio;										// Reference to the AudioSource of the panic msuic.
	private AudioSource[] sirens;										// Reference to the AudioSources of the megaphones.


	void Awake ()
	{

		audio = GetComponent<AudioSource> ();

		panicAudio = transform.FindChild("secondaryMusic").GetComponent<AudioSource>();

		// Find an array of the siren gameobjects.
		GameObject[] sirenGameObjects = GameObject.FindGameObjectsWithTag(Tags.siren);

		// Set the sirens array to have the same number of elements as there are gameobjects.
		sirens = new AudioSource[sirenGameObjects.Length];

		// For all the sirens allocate the audio source of the gameobjects.
		for(int i = 0; i < sirens.Length; i++)
		{
			sirens[i] = sirenGameObjects[i].GetComponent<AudioSource>();
		}
	}


	void Update ()
	{
		MusicFading();
	}

	void MusicFading ()
	{
		// If the alarm is not being triggered...
		if(position != resetPosition)
		{
			// ... fade out the normal music...
			audio.volume = Mathf.Lerp(audio.volume, 0f, musicFadeSpeed * Time.deltaTime);

			// ... and fade in the panic music.
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0.8f, musicFadeSpeed * Time.deltaTime);
		}
		else
		{
			// Otherwise fade in the normal music and fade out the panic music.
			audio.volume = Mathf.Lerp(audio.volume, 0.8f, musicFadeSpeed * Time.deltaTime);
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0f, musicFadeSpeed * Time.deltaTime);
		}
	}
}
