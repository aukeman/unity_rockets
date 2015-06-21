using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float maxBankAngle;

	public float maxRotationalSpeed = 1.0f;
	public float thrustFactor = 0.01f;

	public float maximumSpeed = 1.0f;

	public float drag = 0.01f;

	public ParticleSystem engineExhaust;

	private Vector3 positionVector = new Vector3 (0.0f, 0.0f, 0.0f);
	private Vector3 velocityVector = new Vector3 (0.0f, 0.0f, 0.0f);
	private Vector3 rotationVector = new Vector3( 0.0f, 0.0f, 0.0f );
	private float maxExhaustLifetime = 0.0f;

	private float bankSpeed = 0.0f;
	private float bankAngle = 0.0f;

	private float rotationalAcceleration = 0.0f;

	private float rotationalSpeed = 0.0f;

	private float rotationAngle = 0.0f;

	private float maximumSpeedSquared = 0f;

	private float dragFactor = 1f;


	// Use  this for initialization
	void Awake(){
		if (engineExhaust != null) {
			engineExhaust.enableEmission = false;
			maxExhaustLifetime = engineExhaust.startLifetime;
		}

		this.maximumSpeedSquared = Mathf.Pow(this.maximumSpeed, 2.0f);
		this.dragFactor = 1.0f - this.drag;
	}

	void FixedUpdate(){
		float rotateAxis = Input.GetAxis ("Horizontal");
		float thrustAxis = Mathf.Max( 0f, Input.GetAxis ("Vertical") );

		rotationalSpeed = Mathf.SmoothDamp (rotationalSpeed, maxRotationalSpeed * rotateAxis, ref rotationalAcceleration, 0.1f);

		rotationAngle += rotationalSpeed;

		if ( rotationAngle < 0.0f ) { rotationAngle += 360.0f; }
		if (360.0f < rotationAngle) { rotationAngle -= 360.0f; } 


		bankAngle = Mathf.SmoothDampAngle (bankAngle, -maxBankAngle * rotateAxis, ref bankSpeed, 0.5f);

		rotationVector.Set (0f, rotationAngle, bankAngle);

		transform.rotation = Quaternion.Euler(rotationVector);

		velocityVector.x += Mathf.Sin (rotationAngle*Mathf.Deg2Rad) * thrustAxis * thrustFactor;
		velocityVector.z += Mathf.Cos (rotationAngle*Mathf.Deg2Rad) * thrustAxis * thrustFactor;

		if (0.0f < thrustAxis) {

			if ( maximumSpeedSquared < velocityVector.sqrMagnitude )
			{
				float speedLimitFactor = (maximumSpeed / velocityVector.magnitude);
				
				velocityVector.x *= speedLimitFactor;
				velocityVector.y *= speedLimitFactor;
			}

			if (engineExhaust != null) {
				engineExhaust.enableEmission = true;
				engineExhaust.startLifetime = maxExhaustLifetime * thrustAxis;
			} 
		}else {

			velocityVector.x *= dragFactor;
			velocityVector.z *= dragFactor;

			engineExhaust.enableEmission = false;
		}

		positionVector.x += velocityVector.x;
		positionVector.z += velocityVector.z;
		
		transform.position = positionVector;

	}
}
