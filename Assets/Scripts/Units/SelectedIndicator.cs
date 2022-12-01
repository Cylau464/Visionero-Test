using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Units
{
    public class SelectedIndicator : MonoBehaviour
    {
        [SerializeField] private DecalProjector _decalProjector;
        [SerializeField] private float _fadeTime = .1f;

        private Material _material;
        private const string ALPHA_PROPERTY_NAME = "_Alpha";

        private void Awake()
        {
            _material = _decalProjector.material = new Material(_decalProjector.material);
            _material.SetFloat(ALPHA_PROPERTY_NAME, 0f);
        }

        public void Enable()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(1f));
        }

        public void Disable()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(0f));
        }

        private IEnumerator Fade(float targetAlpha)
        {
            float t = 0f;
            float startAlpha = _material.GetFloat(ALPHA_PROPERTY_NAME);

            while (t < 1f)
            {
                t += Time.deltaTime / _fadeTime;
                _material.SetFloat(ALPHA_PROPERTY_NAME, Mathf.Lerp(startAlpha, targetAlpha, t));

                yield return null;
            }
        }
    }
}