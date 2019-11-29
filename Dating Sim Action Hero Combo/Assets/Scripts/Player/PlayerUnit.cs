using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit {

    public static PlayerUnit Instance { get; private set; }

    [SerializeField] private Transform _front;
    
    public override Transform Transform => transform;
    public override Transform Front => _front;
    public override IntVector3 MapPosition => LevelDataManager.Instance.WorldToArraySpace(transform.position);

    [SerializeField] private float _speed;

    private void Awake() {
        Instance = this;
    }
    
    private void FixedUpdate()
    {
        ProcessMovement();
        ProcessRotation();
    }

    private void ProcessMovement() {
        Vector2 moveInput = InputController.Instance.MovementInput;
        Vector2 delta = moveInput * _speed * Time.deltaTime;
        _rigidbody.MovePosition((Vector2)transform.position + delta);
    }

    private void ProcessRotation() {
        Vector2 dir = InputController.Instance.RotationInput;
        // ONLY change rotation if player has inputted some change
        if (!Mathf.Approximately(dir.x, 0f) || !Mathf.Approximately(dir.y, 0f)) {
            float angle = Vector2.SignedAngle(Vector2.up, dir);
            _rigidbody.MoveRotation(angle);
        }
    }
}
