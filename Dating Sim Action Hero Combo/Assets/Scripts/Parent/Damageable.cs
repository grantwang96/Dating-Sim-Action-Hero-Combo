using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {

    [SerializeField] protected int _health;
    public int health { get { return _health; } }

    // stores the location information for the grid
    [SerializeField] protected int xPos; public int XPos { get { return xPos; } }
    [SerializeField] protected int yPos; public int YPos { get { return yPos; } }
    public void SetPosition(int newX, int newY) {
        GameManager.Instance.grid[xPos, yPos] = null;
        xPos = newX;
        yPos = newY;
    }

    protected virtual void Start() {
        xPos = GameManager.GetGridSpaceX(transform.position.x);
        yPos = GameManager.GetGridSpaceY(transform.position.y);
        if(GameManager.Instance != null) {
            GameManager.Instance.grid[xPos, yPos] = this;
        }
    }

    public virtual void TakeDamage(int damage, Vector2 sourcePoint) {
        _health -= damage;

        if(_health <= 0) {
            Die();
        }
    }

    protected virtual void Die() {
        Debug.Log(transform.name + " died!");
    }
}
