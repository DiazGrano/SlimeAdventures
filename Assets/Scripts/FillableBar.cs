using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillableBar : MonoBehaviour
{
    public bool showBar = false;

    public Transform fill;

    public SpriteRenderer fillSpriteRenderer;
    public Color fillColor;

    private int maxValue;
    private int currentValue;
    public SpriteRenderer backgroundSpriteRenderer;
    public Color backgroundColor;


    private void Start()
    {
        //this.fillColor = this.fillSpriteRenderer.color;
        this.backgroundColor = this.backgroundSpriteRenderer.color;

        ShowBar(showBar);
    }

    public Color FillColor {
        get {
            return this.fillColor;
        }
        set {
            this.fillColor = value;

            if (showBar)
            {
                this.fillSpriteRenderer.color = this.fillColor;
            }
        }
    }
    
    
    public Color BackgroundColor{
        get
        {
            return this.backgroundColor;
        }
        set
        {
            this.backgroundColor = value;

            if (showBar)
            {
                this.backgroundSpriteRenderer.color = this.backgroundColor;
            }
            
        }
    }

    

    [SerializeField]
    [Range(0f, 1f)]
    private float fillAmount;

    public void SetFillAmount(int maxValue, int currentValue)
    {
        this.maxValue = maxValue;
        this.currentValue = currentValue;



        float auxValue = 1f / (float)maxValue;
        this.fillAmount = auxValue * (float)currentValue;

        if (this.showBar)
        {
            this.fill.localScale = new Vector3(this.fillAmount, this.fill.localScale.y, 1);
        } 
    }

    public void ShowBar(bool show)
    {
        this.showBar = show;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(show);
        }

        if (this.showBar)
        {
            this.fillSpriteRenderer.color = this.fillColor;
            this.backgroundSpriteRenderer.color = this.backgroundColor;
            this.SetFillAmount(this.maxValue, this.currentValue);
        }

    }

}
