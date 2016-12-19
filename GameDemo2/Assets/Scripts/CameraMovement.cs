using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	/*
	public Transform target;
	public float smoothing = 5f;

	Vector3 offset;

	void Start()
	{
		offset = transform.position - target.position;
	}

	void FixedUpdate()
	{
		Vector3 targetCamPos = target.position + offset;

		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);

	}*/

	public Transform target;
	public float rotSpeed = 1.5f;

	private float _rotY;
	private Vector3 _offset;

	void Start()
	{
		_rotY = transform.eulerAngles.y;
		_offset = target.position - transform.position;
	}

	void LateUpdate()
	{
		float horInput = Input.GetAxis ("Horizontal");
		if (horInput != 0) {
			_rotY += horInput * rotSpeed;
		} else {
			_rotY += Input.GetAxis ("Mouse X") * rotSpeed * 3;
		}

		Quaternion rotation = Quaternion.Euler (0, _rotY, 0);
		transform.position = target.position - (rotation * _offset);
		transform.LookAt (target);
	}
}
