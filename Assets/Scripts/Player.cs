using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float maxBankAngle;
	public float bankSpeedFactor = 0.5f;

	public float rotateSpeedFactor = 0.1f;
	public float maxRotationalSpeed = 1.0f;

	public float thrustFactor = 1f;
	public float maximumSpeed = 1.0f;

	public ParticleSystem engineExhaust;

	public TractorBeam tractorBeam;

	private float maxExhaustLifetime = 0.0f;

	private float bankSpeed = 0.0f;
	private float bankAngle = 0.0f;

	private Vector3 rotationVector = new Vector3( 0.0f, 0.0f, 0.0f );

	private float rotationalAcceleration = 0.0f;
	private float rotationalSpeed = 0.0f;
	private float rotationAngle = 0.0f;

	private float maximumSpeedSquared = 0f;

	private Rigidbody rigidBody;

	// Use  this for initialization
	void Awake(){
		engineExhaust.enableEmission = false;
		maxExhaustLifetime = engineExhaust.startLifetime;

		this.maximumSpeedSquared = Mathf.Pow(this.maximumSpeed, 2.0f);

		this.rigidBody = GetComponent<Rigidbody> ();

		this.tractorBeam.Captured += TractorBeamCapture;
		this.tractorBeam.Released += TractorBeamRelease;
	}

	void FixedUpdate(){
		float rotateAxis = Input.GetAxis ("Horizontal");
		float thrustAxis = Mathf.Max( 0f, Input.GetAxis ("Vertical") );

		rotationalSpeed = Mathf.SmoothDamp (rotationalSpeed, maxRotationalSpeed * rotateAxis, ref rotationalAcceleration, rotateSpeedFactor);

		rotationAngle += rotationalSpeed;

		if ( rotationAngle < 0.0f ) { rotationAngle += 360.0f; }
		if (360.0f < rotationAngle) { rotationAngle -= 360.0f; } 

		bankAngle = Mathf.SmoothDampAngle (bankAngle, -maxBankAngle * rotateAxis, ref bankSpeed, bankSpeedFactor);

		rotationVector.Set (0f, rotationAngle, bankAngle);

		transform.rotation = Quaternion.Euler(rotationVector);

		if (0.0f < thrustAxis) {

			rigidBody.AddForce( transform.forward * thrustAxis * thrustFactor );

			if ( maximumSpeedSquared < rigidBody.velocity.sqrMagnitude )
			{
				Vector3 velocity = rigidBody.velocity;

				float speedLimitFactor = (maximumSpeed / velocity.magnitude);

				velocity.x *= speedLimitFactor;
				velocity.z *= speedLimitFactor;

				rigidBody.velocity = velocity;
			}

			engineExhaust.enableEmission = true;
			engineExhaust.startLifetime = maxExhaustLifetime * thrustAxis;
		}else {
			engineExhaust.enableEmission = false;
		}
	}

	void Update()
	{
		bool fireDown = Input.GetButtonDown ("Fire1");
		bool fireUp = Input.GetButtonUp ("Fire1");
		
		if (fireDown) {
			tractorBeam.On();
		} else if (fireUp) {
			tractorBeam.Off();
		}
	}

	void TractorBeamCapture( GameObject captured )
	{
//		this.rigidBody.mass += captured.GetComponent<Rigidbody> ().mass;
	}

	void TractorBeamRelease( GameObject released )
	{
//		this.rigidBody.mass -= released.GetComponent<Rigidbody> ().mass;
	}
}
