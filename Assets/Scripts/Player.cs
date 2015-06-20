using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float maxBankAngle;

	public float rotationSpeed = 1.0f;

	public ParticleSystem engineExhaust;

	private Vector3 rotationVector = new Vector3( 0.0f, 0.0f, 0.0f );
	private float maxExhaustLifetime = 0.0f;

	private float rotationAngle = 0.0f;

	// Use this for initialization
	void Awake(){
		if (engineExhaust != null) {
			engineExhaust.enableEmission = false;
			maxExhaustLifetime = engineExhaust.startLifetime;
		}
	}

	void FixedUpdate(){
		float rotateAxis = Input.GetAxis ("Horizontal");
		float thrustAxis = Mathf.Max( 0f, Input.GetAxis ("Vertical") );


		rotationAngle += (rotationSpeed * rotateAxis);

		rotationVector.Set (0f, rotationAngle, -maxBankAngle * rotateAxis);

		transform.rotation = Quaternion.Euler(rotationVector);

		if (engineExhaust != null) {
			if (0.0f < thrustAxis) {
				engineExhaust.enableEmission = true;
				engineExhaust.startLifetime = maxExhaustLifetime * thrustAxis;
			} else {
				engineExhaust.enableEmission = false;
			}
		}
	}
}
