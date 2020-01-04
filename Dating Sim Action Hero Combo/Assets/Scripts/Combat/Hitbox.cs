using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hitbox : MonoBehaviour
{
    public event Action<Collider2D> OnHitBoxTriggered;
    
    private void OnCollisionEnter2D(Collision2D collision) {
        OnHitBoxTriggered?.Invoke(collision.collider);
    }
}
