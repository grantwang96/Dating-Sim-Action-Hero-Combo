using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNoise : MonoBehaviour {

    private float radius;
    public Damageable owner;

    private Collider2D coll;
    private SpriteRenderer srend;

    [SerializeField] private Color startColor;
    [SerializeField] private Coroutine noiseProcess;

	// Use this for initialization
	void Start () {
        transform.localScale = Vector3.zero;
        coll = GetComponent<Collider2D>();
        srend = GetComponent<SpriteRenderer>();
        coll.enabled = false;
        srend.enabled = false;
        owner = transform.parent.GetComponent<Damageable>();
	}

    public void Noise(float newRadius) {
        radius = newRadius;
        if(noiseProcess != null) { StopCoroutine(noiseProcess); }
        noiseProcess = StartCoroutine(Life());
    }

    IEnumerator Life() {
        float time = 0f;
        coll.enabled = true;
        srend.enabled = true;
        srend.color = startColor;

        transform.localScale = Vector3.zero;
        while (time < 1f) {

            time += Time.deltaTime * 10f;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * radius, time);
            yield return new WaitForEndOfFrame();
        }

        time = 0f;

        coll.enabled = false;

        while (time < 1f) {
            time += Time.deltaTime;
            srend.color = Color.Lerp(startColor, Color.clear, time);
            yield return new WaitForEndOfFrame();
        }

        noiseProcess = null;
    }
}
