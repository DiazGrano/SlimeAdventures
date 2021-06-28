using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillableBottle : MonoBehaviour
{
    [SerializeField]
    private Image fill = null;
    [SerializeField]
    private Color fillColor = Color.blue;

    [SerializeField]
    [Range(0f, 1f)]
    private float fillAmount;

    private void OnValidate()
    {
        this.fill.color = this.fillColor;
        this.fill.fillAmount = this.fillAmount;
    }

    public void SetFillAmount(int maxValue, int currentValue)
    {
        float auxValue = 1f / (float)maxValue;
        this.fillAmount = auxValue * (float)currentValue;
        this.fill.fillAmount = this.fillAmount;
    }
}
