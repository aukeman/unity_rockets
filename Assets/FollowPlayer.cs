﻿using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public Transform target;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - target.transform.position;	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = target.transform.position + offset; 
	}
}
