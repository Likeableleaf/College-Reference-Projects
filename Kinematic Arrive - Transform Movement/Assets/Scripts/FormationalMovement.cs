using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationalMovement : MonoBehaviour
{

	[SerializeField] private Transform formationLeader;

	[SerializeField] private float formationDistanceOffset;
	[SerializeField] private float formationRotationOffset;
	[SerializeField] private float finalRotation;

	[SerializeField] private Transform trans;
	[SerializeField] private Rigidbody rb;

	private float moveSpeed;
	private float turnSpeed;
	private float radiusOfSatisfaction;

	private void Start()
	{
		moveSpeed = 5f;
		turnSpeed = 5f;
		radiusOfSatisfaction = 1f;
	}

	private void Update()
	{
		
		formation();
	}


	private void formation()
    {
		// Project point forward from leader's forward facing vector
		Vector3 projectedPoint = formationLeader.forward * formationDistanceOffset;

		// Rotate that point to find this character's spot in the formation
		Vector3 positionInFormation = Quaternion.Euler(0f, formationRotationOffset, 0f) * projectedPoint;

		positionInFormation += formationLeader.position;

		if (Vector3.Distance(trans.position, positionInFormation) > radiusOfSatisfaction)
		{

			// Calculate vector to the position in the formation
			Vector3 towards = positionInFormation - trans.position;

			// Normalize vector to standardize movement speed
			towards.Normalize();
			towards *= moveSpeed;
			rb.velocity = towards;

			// Face player along movement vector
			Quaternion targetRotation = Quaternion.LookRotation(towards);
			trans.rotation = Quaternion.Lerp(trans.rotation, targetRotation, turnSpeed * Time.deltaTime);
		}
		else
		{

			Vector3 rotationInfo = Quaternion.Euler(0f, finalRotation, 0f) * (formationLeader.forward);
			Quaternion lastRotation = Quaternion.LookRotation(rotationInfo);
			trans.rotation = lastRotation;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}

	
}
