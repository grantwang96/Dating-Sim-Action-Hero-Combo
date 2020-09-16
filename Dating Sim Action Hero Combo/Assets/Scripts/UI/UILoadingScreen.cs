using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UILoadingScreen : UIObject
{
    public event Action OnShowComplete;
    public event Action OnHideComplete;

    [SerializeField] private Image _background;
    [SerializeField] private Color _backgroundColor;
    [SerializeField][Range(0f, 5f)] private float _fadeInTime;
    [SerializeField][Range(0f, 5f)] private float _fadeOutTime;
    
    public override void Display() {
        base.Display();
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeBackground(_fadeInTime, Color.clear, _backgroundColor, OnShowComplete));
    }

    public override void Hide() {
        base.Hide();
        StopAllCoroutines();
        _background.color = _backgroundColor;
        StartCoroutine(FadeBackground(_fadeOutTime, _backgroundColor, Color.clear, OnHideFinish));
    }

    public void HideInstant() {
        _background.color = Color.clear;
        gameObject.SetActive(false);
    }

    private void OnHideFinish() {
        OnHideComplete?.Invoke();
        gameObject.SetActive(false);
    }

    private IEnumerator FadeBackground(float fadeTime, Color startColor, Color endColor, Action onCompleteAction) {
        float time = 0f;
        while(time < fadeTime) {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            _background.color = Color.Lerp(startColor, endColor, time / fadeTime);
        }
        _background.color = endColor;
        onCompleteAction?.Invoke();
    }
}
