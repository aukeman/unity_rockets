using UnityEngine;
using System.Collections;

public class TractorBeam : MonoBehaviour {

	public delegate void TractorBeamEvent(GameObject target);
	
	public event TractorBeamEvent Captured;
	public event TractorBeamEvent Released;
	
	public ParticleSystem beam;
	public ParticleSystem pulses;

	private Collider captureVolume;
	
	private GameObject capturedObject;
	private Vector3 capturePointLocal;

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

		captureVolume = GetComponent<Collider> ();
		captureVolume.enabled = false;
	}

	void FixedUpdate()
	{

		if (capturedObject != null) {
			capturedObject.transform.position = transform.TransformPoint( capturePointLocal );
		}

		RaycastHit rayCastHit;
		blocked = Physics.SphereCast (transform.position, // + transform.forward*1f, 
		                              0.5f,
		                              transform.forward,
		                              out rayCastHit,
		                              40.0f);
		
		if (blocked) {
			captureVolume.enabled = false;
		} else if (active) {
			captureVolume.enabled = true;
		}
	}

	public void On()
	{
		captureVolume.enabled = !blocked;
		beam.startLifetime = beamParticleLifetime;
		beam.enableEmission = true;
		active = true;
	}

	public void Off()
	{
		captureVolume.enabled = false;
		active = false;

		Release ();

		StartCoroutine ("RetractBeam");
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("OnTriggerEnter", other);
		if (!blocked && active) 
		{
			Capture(other.gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		Debug.Log ("OnTriggerExit", other);
		if (!blocked && active && capturedObject != null) {
			Release();
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (!blocked && active && capturedObject == null) {
			Capture(other.gameObject);
		} else if (blocked && active && capturedObject != null) {
			Release();
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

	void Capture(GameObject captured)
	{			
		TurnOnPulses();
		capturedObject = captured;
		capturePointLocal = transform.InverseTransformPoint( captured.transform.position ); 

		if  (Captured != null )
		{
			Captured(captured);
		}
	}

	void Release ()
	{
		if (capturedObject != null) {
			GameObject releasedObject = capturedObject;

			TurnOffPulses ();
			capturedObject = null;
			if (Released != null) {
				Released (releasedObject);
			}
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
