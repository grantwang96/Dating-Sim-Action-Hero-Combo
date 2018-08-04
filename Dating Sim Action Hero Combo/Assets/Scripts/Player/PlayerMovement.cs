using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private Rigidbody2D rbody;

	// Use this for initialization
	void Start () {
        Instance = this;
        rbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.up = PlayerInput.Instance.lookVector;

        if (PlayerInput.Instance.switchingOutfits) { return; }

        Vector2 input = PlayerInput.Instance.moveVector;
        rbody.velocity = input * runSpeed;
    }
}
