using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundBox : MonoBehaviour, PooledObject
{
    [SerializeField] private float _growSpeed = 1f;
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField] [Range(0f, 1f)] private float _maxOpacity = 1f;

    private float _currentSize; // clamp 0 - 1
    private bool _finishedGrowing;
    private float _targetSize;
    private Vector3 _startPosition;
    private Unit _source; // can be null

    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private SpriteRenderer _renderer;

    public void Initialize(PooledObjectInitializationData initializationData) {
        WeaponSoundBoxInitializationData initData = initializationData as WeaponSoundBoxInitializationData;
        if(initData == null) {
            return;
        }
        _targetSize = initData.TargetSize;
        _startPosition = initData.StartPosition;
        _source = initData.Source;
    }
    
    private void FixedUpdate() {
        if (_finishedGrowing) {
            FadeOut();
        } else {
            Grow();
        }
    }

    private void Grow() {
        _currentSize += Time.deltaTime * _growSpeed;
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * _targetSize, _currentSize);
        if(_currentSize >= 1f) {
            _finishedGrowing = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        UnitSoundListener soundListener = collision.GetComponent<UnitSoundListener>();
        if(soundListener != null) {
            soundListener.OnSoundHeard(LevelDataManager.Instance.WorldToArraySpace(transform.position), _source);
        }
    }

    private void FadeOut() {
        if(Mathf.Approximately(_renderer.color.a, 0f)) {
            Despawn();
            return;
        }
        Color color = _renderer.color;
        color.a = (Mathf.Clamp(color.a - Time.deltaTime / _fadeTime, 0f, 1f));
        _renderer.color = color;
    }

    public void Spawn() {
        gameObject.SetActive(true);
        transform.position = _startPosition;
        _collider.enabled = true;
        Color opaque = _renderer.color;
        opaque.a = _maxOpacity;
        _renderer.color = opaque;
        _currentSize = 0f;
        transform.localScale = Vector3.zero;
        _finishedGrowing = false;
    }

    public void Despawn() {
        gameObject.SetActive(false);
        _collider.enabled = false;
        PooledObjectManager.Instance.ReturnPooledObject(this.name, this);
    }
}

public class WeaponSoundBoxInitializationData : PooledObjectInitializationData {
    public float TargetSize;
    public Vector3 StartPosition;
    public Unit Source;
}
