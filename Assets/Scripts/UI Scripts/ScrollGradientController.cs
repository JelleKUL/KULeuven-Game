using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollGradientController : MonoBehaviour
{
    [SerializeField]
    private Image leftGrad, rightGrad;
    [SerializeField]
    private float maxAlpha = 0.5f;

    public void UpdateGrad(Vector2 val)
    {
        if (leftGrad) leftGrad.color = new Color(leftGrad.color.r, leftGrad.color.g, leftGrad.color.b, val.x * maxAlpha);
        if (rightGrad) rightGrad.color = new Color(rightGrad.color.r, rightGrad.color.g, rightGrad.color.b, (1 - val.x) * maxAlpha);
    }
}
