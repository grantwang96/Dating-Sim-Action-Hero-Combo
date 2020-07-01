using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MoveController {

    [SerializeField] private float _speed;

    protected override void ProcessMovement() {
        Vector2 moveInput = InputController.Instance.MovementInput;
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
        // TODO: pull this logic out and separate for controller support
        Vector2 mousePosition = InputController.Instance.MousePositionInput;
        Vector2 dir = (mousePosition - (Vector2)PlayerUnit.Instance.transform.position).normalized;
        // ONLY change rotation if player has inputted some change
        if (!Mathf.Approximately(dir.x, 0f) || !Mathf.Approximately(dir.y, 0f)) {
            float angle = Vector2.SignedAngle(Vector2.up, dir);
            _rigidbody.MoveRotation(angle);
        }
    }
}
