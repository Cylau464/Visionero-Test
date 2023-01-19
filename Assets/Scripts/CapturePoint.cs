using States.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CapturePoint : MonoBehaviour
{
    [SerializeField] private float _captureDuration = 20f;
    [field: SerializeField] public float CaptureRadius { get; private set; } = 5f;
    [SerializeField] private SphereCollider _captureTrigger;
    [SerializeField] private LayerMask _allyUnitsMask;
    [SerializeField] private LayerMask _enemyUnitsMask;
    [Space]
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _captureProgressBar;
    [SerializeField] private TMP_Text _progressText;

    private float _captureProgress;
    private List<CharacterStateMachine> _allyUnitsCapturing = new List<CharacterStateMachine>();
    private List<CharacterStateMachine> _enemyUnitsCapturing = new List<CharacterStateMachine>();

    public bool IsCaptured { get; private set; }

    public Action<CapturePoint> OnCaptured;

    private void Start()
    {
        _captureTrigger.radius = CaptureRadius;
        _canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (IsCaptured == false)
        {
            if (_enemyUnitsCapturing.Count > 0)
            {
                if(_allyUnitsCapturing.Count <= 0)
                    Capture();
            }
            else if (_captureProgress > 0f)
            {
                if(_allyUnitsCapturing.Count > 0)
                    Uncapture();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _allyUnitsMask) != 0)
            AddAllyUnit(other.GetComponent<CharacterStateMachine>());
        else if ((1 << other.gameObject.layer & _enemyUnitsMask) != 0)
            AddEnemyUnit(other.GetComponent<CharacterStateMachine>());
    }

    private void OnTriggerExit(Collider other)
    {
        if ((1 << other.gameObject.layer & _allyUnitsMask) != 0)
            RemoveAllyUnit(other.GetComponent<CharacterStateMachine>());
        else if ((1 << other.gameObject.layer & _enemyUnitsMask) != 0)
            RemoveEnemyUnit(other.GetComponent<CharacterStateMachine>());
    }

    private void AddAllyUnit(CharacterStateMachine unit)
    {
        unit.OnDead += RemoveAllyUnit;
        _allyUnitsCapturing.Add(unit);
    }

    private void AddEnemyUnit(CharacterStateMachine unit)
    {
        unit.OnDead += RemoveEnemyUnit;
        _enemyUnitsCapturing.Add(unit);
    }

    private void RemoveAllyUnit(CharacterStateMachine unit)
    {
        unit.OnDead -= RemoveAllyUnit;
        _allyUnitsCapturing.Remove(unit);
    }
    
    private void RemoveEnemyUnit(CharacterStateMachine unit)
    {
        unit.OnDead -= RemoveEnemyUnit;
        _enemyUnitsCapturing.Remove(unit);
    }

    private void Capture()
    {
        if (_captureProgress <= 0f)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(1f));
        }

        _captureProgress += Time.deltaTime / _captureDuration;
        _captureProgress = Mathf.Clamp01(_captureProgress);
        _captureProgressBar.value = _captureProgress;
        _progressText.text = Mathf.CeilToInt(_captureProgress * 100f).ToString();

        if (_captureProgress >= 1f)
        {
            IsCaptured = true;
            OnCaptured?.Invoke(this);
        }
    }

    private void Uncapture()
    {
        _captureProgress -= Time.deltaTime / _captureDuration;
        _captureProgress = Mathf.Clamp01(_captureProgress);
        _captureProgressBar.value = _captureProgress;
        _progressText.text = Mathf.CeilToInt(_captureProgress * 100f).ToString();

        if (_captureProgress <= 0f)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(0f));
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float t = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (t < 1f)
        {
            t += Time.deltaTime / _fadeTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }
    }
}