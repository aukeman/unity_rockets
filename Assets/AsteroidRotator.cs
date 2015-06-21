using UnityEngine;
using System.Collections;

public class AsteroidRotator : MonoBehaviour {

	public float minRotationSpeed = 0.1f;
	public float maxRotationSpeed = 1.0f;

	// Use this for initialization
	void Start () {

		GetComponent<Rigidbody> ().angularVelocity = 
			new Vector3 (Random.Range (minRotationSpeed, maxRotationSpeed),
			             Random.Range (minRotationSpeed, maxRotationSpeed),
			             Random.Range (minRotationSpeed, maxRotationSpeed));;
	
	}
}
