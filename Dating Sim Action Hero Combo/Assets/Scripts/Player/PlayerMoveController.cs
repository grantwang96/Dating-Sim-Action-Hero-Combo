﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MoveController {

    public static PlayerMoveController Instance { get; private set; }

    [SerializeField] private float _speed;

    private void Awake() {
        Instance = this;
    }
    
    protected override void ProcessMovement() {
        Vector2 moveInput = InputController.Instance.GameplayActionMap["Move"].ReadValue<Vector2>();
        Vector2 delta = moveInput * _speed * Time.deltaTime;
        _rigidbody.MovePosition((Vector2)transform.position + delta);

        UpdateMapPosition();
    }

    private void UpdateMapPosition() {
        IntVector3 position = LevelDataManager.Instance.WorldToArraySpace(transform.position);
        if(position != MapPosition) {
            UpdateMapSpacePosition(position);
        }
    }

    protected override void ProcessRotation() {
        if (CameraController.Instance.MainCamera == null) {
            return;
        }
        // TODO: pull this logic out and separate for controller support
        Vector2 mousePosition = InputController.Instance.GameplayActionMap["Look"].ReadValue<Vector2>();
        Vector2 worldMousePosition = CameraController.Instance.MainCamera.ScreenToWorldPoint(mousePosition);
        Vector2 dir = (worldMousePosition - (Vector2)PlayerUnit.Instance.transform.position).normalized;
        // ONLY change rotation if player has inputted some change
        if (!Mathf.Approximately(dir.x, 0f) || !Mathf.Approximately(dir.y, 0f)) {
            float angle = Vector2.SignedAngle(Vector2.up, dir);
            _rigidbody.MoveRotation(angle);
        }
    }
}
