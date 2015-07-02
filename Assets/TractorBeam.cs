using UnityEngine;
using System.Collections;

public class TractorBeam : MonoBehaviour {

	public delegate void TractorBeamEvent(GameObject target);
	
	public ParticleSystem beam;

	public float beamForce = 0.5f;

	private float beamParticleLifetime;

	private bool active;

	private float beamRange;

	// Use this for initialization
	void Awake () {
		beamParticleLifetime = beam.startLifetime;
		beam.enableEmission = false;
		active = false;

		beamRange = beam.startLifetime * beam.startSpeed;
	}

	public void On()
	{
		beam.startLifetime = beamParticleLifetime;
		beam.enableEmission = true;
		active = true;
	}

	public void Off()
	{
		active = false;
			StartCoroutine ("RetractBeam");
	}

	void FixedUpdate()
	{
		RaycastHit rayCastHit;
		if (active && 
		    Physics.SphereCast (transform.position,
			                    0.5f,
			                    transform.forward,
			                   out rayCastHit,
		                        beamRange)) {

			Vector3 force = (transform.position - rayCastHit.point).normalized * beamForce;

			rayCastHit.collider.attachedRigidbody.AddForceAtPosition( force, rayCastHit.point );
		}


		                    
	}

	IEnumerator RetractBeam()
	{
		float increment = beamParticleLifetime * 0.1f;


		for ( int i = 0; !active && i < 10; ++i )
		{
			beam.startLifetime -= increment;
			yield return null;
		}

		if (!active) {
			beam.enableEmission = false;
		}
	}
}
