using UnityEngine;
using System.Collections;

public class TractorBeam : MonoBehaviour {

	public ParticleSystem beam;
	public ParticleSystem pulses;

	private float beamParticleLifetime;

	private bool active;
	private bool blocked;

	private ParticleSystem.Particle[] emptyParticles;

	// Use this for initialization
	void Awake () {
		beamParticleLifetime = beam.startLifetime;
		beam.enableEmission = false;
		pulses.enableEmission = false;
		active = false;
		blocked = false;

		emptyParticles = new ParticleSystem.Particle[0];

	}

	void Update()
	{
		RaycastHit rayCastHit;
		blocked = Physics.SphereCast (transform.position, // + transform.forward*1f, 
		                              0.5f,
		                              transform.forward,
		                              out rayCastHit,
		                              40.0f);

		if (blocked) {
			Debug.Log("hit: " + rayCastHit.collider);
		}

	}

	public void On()
	{
		beam.startLifetime = beamParticleLifetime;
		beam.enableEmission = true;
		pulses.enableEmission = !blocked;
		active = true;
	}

	public void Off()
	{
		active = false;
		TurnOffPulses ();

		StartCoroutine ("RetractBeam");
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("OnTriggerEnter", other);
		if (!blocked && active) 
		{
			Debug.Log ("OnTriggerEnter, turn on pulses");
			TurnOnPulses();
		}
	}

	void OnTriggerExit(Collider other)
	{
		Debug.Log ("OnTriggerExit", other);
		TurnOffPulses ();
	}

	void OnTriggerStay(Collider other)
	{
		if (!blocked && active && !ArePulsesOn ()) {
			TurnOnPulses();
		}
	}

	void TurnOnPulses()
	{
		pulses.enableEmission = true;
	}

	void TurnOffPulses()
	{
		pulses.enableEmission = false;
		pulses.SetParticles (emptyParticles, 0);
	}

	bool ArePulsesOn()
	{
		return pulses.enableEmission;
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
