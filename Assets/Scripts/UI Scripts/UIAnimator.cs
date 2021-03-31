using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    [SerializeField]
    private bool animate = true;
    [SerializeField]
    private float animationDuration = 1f;
    [SerializeField]
    private AnimationCurve relativeScaleCurve;

    private Vector3 startScale;

    private void Awake()
    {
        startScale = transform.localScale;
    }

    void OnEnable()
    {
        if (animate)
        {
            transform.localScale = Vector3.zero;
            StartCoroutine(AnimateScale(animationDuration));
        }
    }

    IEnumerator AnimateScale(float duration)
    {
        float journey = 0f;
        while (journey <= duration)
        {
            journey += Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);

            transform.localScale = startScale * relativeScaleCurve.Evaluate(percent);

            yield return null;
        }
    }
}
