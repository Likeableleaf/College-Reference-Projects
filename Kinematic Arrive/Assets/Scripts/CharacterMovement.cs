using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

	[SerializeField] private Transform trans;
	[SerializeField] private Rigidbody rb;
	[SerializeField] private Transform target;


	private float maxSpeed;
	private float radiusOfSat;

	void Start () {
		maxSpeed = 5f;
		radiusOfSat = 2f;
	}
	
	// Update is called once per frame
	void Update () {

		// Calculate vector from character to target
		Vector3 towards = target.position - trans.position;
		trans.rotation = Quaternion.LookRotation (towards);

		// If we haven't reached the target yet
		if (towards.magnitude > radiusOfSat) {

			// Normalize vector to get just the direction
			towards.Normalize ();
			towards *= maxSpeed;

			// Move character
			rb.velocity = towards;
		}
	}
}
