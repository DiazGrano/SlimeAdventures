using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickAccessButton : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    public Sprite normalButtonSprite;
    public Sprite pressedButtonSprite;

    public Image buttonImage;

    public Canvas linkedWindow;


    public void OnPointerClick(PointerEventData eventData)
    {
        if (linkedWindow != null)
        {
            if (!this.linkedWindow.isActiveAndEnabled)
            {
                this.linkedWindow.enabled = true;
                RayCasterState(linkedWindow.gameObject, true);
            }
            else
            {
                this.linkedWindow.enabled = false;
                RayCasterState(linkedWindow.gameObject, false);
            }
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.buttonImage != null && this.pressedButtonSprite != null)
        {
            this.buttonImage.sprite = this.pressedButtonSprite;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.buttonImage != null && this.pressedButtonSprite != null)
        {
            this.buttonImage.sprite = this.normalButtonSprite;
        }
    }

    private void RayCasterState(GameObject parent, bool state)
    { 
        
        if (parent.GetComponent<GraphicRaycaster>() != null)
        {
            parent.GetComponent<GraphicRaycaster>().enabled = state;
        }

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            RayCasterState(parent.transform.GetChild(i).gameObject, state);
        }
        
    }
}
