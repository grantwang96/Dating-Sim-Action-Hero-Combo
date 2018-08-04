using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {

    [SerializeField] protected int _health;
    public int health { get { return _health; } }

    // stores the location information for the grid
    private int xPos; public int XPos { get { return xPos; } }
    private int yPos; public int YPos { get { return YPos; } }
    public void SetPosition(int newX, int newY) { xPos = newX; yPos = newY; }

    private void Start() {
        if(GameManager.Instance != null) {
            GameManager.Instance.grid[xPos, yPos] = this;
        }
    }

    public virtual void TakeDamage(int damage) {
        _health -= damage;

        if(_health <= 0) {
            Die();
        }
    }

    protected virtual void Die() {
        Debug.Log(transform.name + " died!");
    }
}
