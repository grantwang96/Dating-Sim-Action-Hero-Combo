using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCMoveController : MoveController
{
    private Transform _lookTarget;

    protected bool _isMoving;
    protected float _moveSpeed;
    protected List<IntVector3> _currentPath = new List<IntVector3>();
    protected Vector2 _currentDestination;

    public event Action OnArrivedTargetDestination;
    
    public void SetDestination(float speed, IntVector3 destination) {
        _currentPath.Clear();
        Mapservice.GetPathToDestination(MapPosition, destination, out _currentPath);
        _moveSpeed = speed;
        _isMoving = true;
        UpdateCurrentDestination();
    }

    public void ClearDestination() {
        _currentPath.Clear();
        _isMoving = false;
    }

    public void SetLookTarget(Transform target) {
        _lookTarget = target;
    }

    protected override void ProcessMovement() {
        if (!_isMoving) { return; }
        Vector2 dir = _currentDestination - (Vector2)transform.position;
        Vector2 newPosition = (Vector2)transform.position + dir.normalized * _moveSpeed * Time.deltaTime;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.MovePosition(newPosition);
        ProcessMapSpace();
        if (Vector2.Distance(transform.position, _currentDestination) < 0.1f) {
            UpdateCurrentDestination();
        }
    }

    protected virtual void ProcessMapSpace() {
        IntVector3 position = LevelDataManager.Instance.WorldToArraySpace(transform.position);
        if(MapPosition != position) {
            UpdateMapSpacePosition(position);
        }
    }

    protected override void ProcessRotation() {
        Vector2 dir = transform.forward;
        if(_lookTarget != null) {
            dir = (_lookTarget.position - transform.position).normalized;
            SetRotation(dir);
        } else if (_isMoving) {
            dir = (_currentDestination - (Vector2)transform.position).normalized;
            SetRotation(dir);
        }
    }

    private void SetRotation(Vector2 dir) {
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        float lerpedAngle = Mathf.LerpAngle(_rigidbody.rotation, angle, 0.33f);
        if(Mathf.Abs(angle - lerpedAngle) < 0.1f) {
            lerpedAngle = angle;
        }
        _rigidbody.MoveRotation(lerpedAngle);
    }

    protected void UpdateCurrentDestination() {
        if (_currentPath.Count == 0) {
            ArrivedFinalDestination();
            return;
        }
        _currentDestination = LevelDataManager.Instance.ArrayToWorldSpace(_currentPath[0].x, _currentPath[0].y);
        _currentPath.RemoveAt(0);
    }

    protected void ArrivedFinalDestination() {
        _isMoving = false;
        _currentPath.Clear();
        OnArrivedTargetDestination?.Invoke();
    }
}
