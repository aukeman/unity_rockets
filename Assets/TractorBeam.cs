using UnityEngine;
using System.Collections;

public class TractorBeam : MonoBehaviour {

	public ParticleSystem beam;
	public ParticleSystem pulses;

	private float beamParticleLifetime;

	private bool active;

	private ParticleSystem.Particle[] emptyParticles;

	// Use this for initialization
	void Awake () {
		beamParticleLifetime = beam.startLifetime;
		beam.enableEmission = false;
		pulses.enableEmission = false;
		active = false;

		emptyParticles = new ParticleSystem.Particle[0];

	}

	public void On()
	{
		beam.startLifetime = beamParticleLifetime;
		beam.enableEmission = true;
		pulses.enableEmission = true;
		active = true;
	}

	public void Off()
	{
		active = false;
		pulses.enableEmission = false;

		pulses.SetParticles (emptyParticles, 0);

		StartCoroutine ("RetractBeam");
		//StartCoroutine ("FadePulses");
	}

/*	IEnumerator FadePulses()
	{
		int increment = 255 / 5;

		pulses.GetParticles (pulseParticles);
		
		for (int i = 0; !active && i < 5; ++i) {
			beam.startLifetime -= increment;
			
			for (int pIdx = 0; pIdx < pulseParticles.Length; ++pIdx){
				Color c = pulseParticles[pIdx].color;
				c.a -= increment;
				c.a = 0;

				pulseParticles[pIdx].color = c;
			}

			yield return null;
		}

		if (!active) {
			for (int pIdx = 0; pIdx < pulseParticles.Length; ++pIdx){
				Color c = pulseParticles[pIdx].color;
				c.a = 0;
				pulseParticles[pIdx].color = c;
			}
		}
	}*/

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
