using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Units
{
    public abstract class HealthIndicator : MonoBehaviour
    {
        [SerializeField] protected Slider _healthBar;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected float _fadeOutTime = .25f;

        protected void OnUnitTakeDamage(int currentHealth)
        {
            _healthBar.value = GetTotalHealth();

            if (_healthBar.value == 0f)
                StartCoroutine(FadeOut());
        }

        protected void InitializeBar()
        {
            _healthBar.value = _healthBar.maxValue = GetTotalMaxHealth();
        }

        protected abstract int GetTotalHealth();
        protected abstract int GetTotalMaxHealth();

        private IEnumerator FadeOut()
        {
            float t = 0f;
            float startAlpha = _canvasGroup.alpha;

            while (t < 1f)
            {
                t += Time.deltaTime / _fadeOutTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);

                yield return null;
            }
        }
    }
}