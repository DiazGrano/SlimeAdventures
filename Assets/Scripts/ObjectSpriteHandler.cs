using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpriteHandler : MonoBehaviour
{
    [SerializeField]
    //SpriteRenderer objectSpriteRenderer;
    List<SpriteRenderer> objectSpriteRenderer;
    [SerializeField]
    Animator objectAnimator;

    //Color objectOriginalColor;
    List<Color> objectOriginalColor = new List<Color>();

    Color currentColor;

    private void Start()
    {
        this.objectSpriteRenderer = new List<SpriteRenderer>();
        this.objectOriginalColor = new List<Color>();
        if (this.GetComponent<SpriteRenderer>())
        {
            this.objectSpriteRenderer.Add(this.GetComponent<SpriteRenderer>());
        }
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            if (this.gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>())
            {
                this.objectSpriteRenderer.Add(this.gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>());
            }
        }
        foreach (SpriteRenderer sRenderer in this.objectSpriteRenderer)
        {
            this.objectOriginalColor.Add(sRenderer.color);
        }
        if (!this.objectAnimator)
        {
            if (!(this.objectAnimator = this.GetComponent<Animator>()))
            {
                if (!(this.objectAnimator = this.GetComponentInChildren<Animator>()))
                {
                    Debug.Log("No se ha encontrado el animator del objeto " + this.gameObject.name);
                }
            }
        }
    }



    public void TemporarilyChangeColor(Color color){
        if (this.currentColor != color)
        {
            if (color == Color.white)
            {
                ResetToOriginalColor();
            }
            else
            {
                this.currentColor = color;
                for (int i = 0; i < this.objectSpriteRenderer.Count; i++)
                {
                    this.objectSpriteRenderer[i].color = color;
                }
            }
        }
    }


    public void ResetToOriginalColor()
    {
        if (this.currentColor != Color.white)
        {
            for (int i = 0; i < this.objectSpriteRenderer.Count; i++)
            {
                this.objectSpriteRenderer[i].color = this.objectOriginalColor[i];
            }
            this.currentColor = Color.white;
        }
    }

    public void ChangeColorOverTime(float time, Color color)
    {
        StartCoroutine(ColorOverTimeCoroutine(time, color));
    }
    IEnumerator ColorOverTimeCoroutine(float time, Color color)
    {
        time = 0.5f;
        foreach (SpriteRenderer spriteRenderer in objectSpriteRenderer)
        {
            while (Color.gray != spriteRenderer.color)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.gray, Time.deltaTime / time);
                time -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            /*
            selfDisabled = true;
            SetTileState(TileState.Disabled, Color.gray);
            if (!bfManager.disabledTiles.Contains(this))
            {
                bfManager.disabledTiles.Add(this);
            }*/
        }
        

    }

}
