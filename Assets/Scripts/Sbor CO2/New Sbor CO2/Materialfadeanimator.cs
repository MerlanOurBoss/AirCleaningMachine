using System.Collections;
using UnityEngine;

public class MaterialFadeAnimator : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;
    [SerializeField] private string floatProperty = "_secondColorInfluence";
    [SerializeField] private float stepPerSecond = 0.05f;
    [SerializeField] private float tickInterval = 1f;

    private Coroutine _running;

    public void SetTarget(Material material) => targetMaterial = material;

    /// <summary>Плавно меняет float-параметр материала от текущего значения к endValue.</summary>
    public void FadeTo(float endValue)
    {
        if (_running != null)
            StopCoroutine(_running);
        _running = StartCoroutine(FadeRoutine(endValue));
    }

    private IEnumerator FadeRoutine(float endValue)
    {
        if (targetMaterial == null) yield break;

        float current = targetMaterial.GetFloat(floatProperty);
        float direction = Mathf.Sign(endValue - current);
        if (direction == 0f) yield break;

        while (direction > 0 ? current < endValue : current > endValue)
        {
            current += direction * stepPerSecond;
            targetMaterial.SetFloat(floatProperty, current);
            yield return new WaitForSeconds(tickInterval);
        }

        targetMaterial.SetFloat(floatProperty, endValue);
    }
}