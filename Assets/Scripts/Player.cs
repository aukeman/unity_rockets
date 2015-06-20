using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	Vector3 rotationVector = new Vector3( 0.0f, 0.0f, 0.0f );

	// Use this for initialization
	void Start () {
	
	}

	void FixedUpdate(){
		float axis = Input.GetAxis ("Horizontal");

		rotationVector.y = axis;

		transform.Rotate (rotationVector);
	
	}
}
