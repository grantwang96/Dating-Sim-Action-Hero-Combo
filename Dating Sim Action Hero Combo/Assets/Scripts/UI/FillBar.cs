using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    [SerializeField] private Image _fillBar;
    [SerializeField] private RectTransform _container;
    [SerializeField] private float _lerpSpeed;

    [SerializeField] private float _percentage;
    [SerializeField] private bool _isLerping;

    private void Start() {
        UpdateBarSize();
    }

    private void Update() {
        if (_isLerping) {
            UpdateBarSize();
        }
    }

    public void UpdateValue(float percentage) {
        _percentage = Mathf.Clamp(percentage, 0f, 1f);
        _isLerping = true;
    }

    public void UpdateValueInstant(float percentage) {
        _percentage = Mathf.Clamp(percentage, 0f, 1f);
        _isLerping = false;
        float targetSize = _container.sizeDelta.x * _percentage;
        _fillBar.rectTransform.sizeDelta = new Vector2(targetSize, _container.sizeDelta.y);
    }

    private void UpdateBarSize() {
        float targetSize = _container.sizeDelta.x * _percentage;
        Vector2 fillBarSize = _fillBar.rectTransform.sizeDelta;
        if(Mathf.Approximately(fillBarSize.x, targetSize)) {
            _isLerping = false;
            return;
        }
        float change = _lerpSpeed * Time.deltaTime;
        change = Mathf.Min(change, Mathf.Abs(targetSize - fillBarSize.x));
        if (fillBarSize.x > targetSize) {
            change *= -1;
        }
        _fillBar.rectTransform.sizeDelta = new Vector2(fillBarSize.x + change, _container.sizeDelta.y);
    }
}
