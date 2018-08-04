using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Vector2 spawnpoint;
    private float lifeTime;
    public float maxLifeTime;

    public float soundRadius;

    public SpriteRenderer srend;

    [SerializeField] private int _damage;
    public int damage {
        get { return _damage; }
        set { _damage = value; }
    }

	// Use this for initialization
	void Start () {
        spawnpoint = transform.position;
        lifeTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        lifeTime += Time.deltaTime;
        if(lifeTime > maxLifeTime) {
            Die();
        }
	}

    private void OnCollisionEnter2D(Collision2D collision) {

        try {
            Damageable dam = collision.transform.GetComponent<Damageable>();
            dam.TakeDamage(damage, spawnpoint);
        }
        catch {
            
        }

        Die();
    }

    private void Die() {
        Destroy(gameObject);
    }
}
