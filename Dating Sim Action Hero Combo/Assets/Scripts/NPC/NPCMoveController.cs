using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCMoveController : MoveController
{
    [SerializeField] protected NPCNavigator _navigator;
    protected float _moveSpeed;
    
    public void SetSpeed(float speed) {
        _moveSpeed = speed;
    }

    protected override void ProcessMovement() {
        Vector2 dir = _navigator.MoveInput;
        Vector2 newPosition = (Vector2)transform.position + dir.normalized * _moveSpeed * Time.deltaTime;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.MovePosition(newPosition);
        ProcessMapSpace();
    }

    protected virtual void ProcessMapSpace() {
        IntVector3 position = LevelDataManager.Instance.WorldToArraySpace(transform.position);
        if(MapPosition != position) {
            UpdateMapSpacePosition(position);
        }
    }

    protected override void ProcessRotation() {
        SetRotation(_navigator.LookInput);
    }

    private void SetRotation(Vector2 dir) {
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        float lerpedAngle = Mathf.LerpAngle(_rigidbody.rotation, angle, 0.33f);
        if(Mathf.Abs(angle - lerpedAngle) < 0.1f) {
            lerpedAngle = angle;
        }
        _rigidbody.MoveRotation(lerpedAngle);
    }
}
