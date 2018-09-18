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
        PlayerDamageable.Instance.OnPlayerDeath += OnPlayerDeath;
	}
	
	// Update is called once per frame
	void Update () {
        transform.up = PlayerInput.Instance.lookVector;

        if (PlayerInput.Instance.switchingOutfits) { return; }

        Vector2 input = PlayerInput.Instance.moveVector;
        // rbody.velocity = input * runSpeed;
        rbody.MovePosition(rbody.position + (input * runSpeed * Time.deltaTime));
    }

    private void OnPlayerDeath() {
        rbody.velocity = Vector2.zero;
        rbody.isKinematic = true;
        this.enabled = false;
    }
}
