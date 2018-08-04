using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPosition = PlayerMovement.Instance.transform.position;
        newPosition.z = transform.position.z;
        transform.position = newPosition;
	}
}
