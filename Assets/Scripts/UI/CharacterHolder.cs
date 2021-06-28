using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHolder : MonoBehaviour
{
    public Image characterSprite;

    public Image fill;

    public Color fillColorAlive = Color.white;
    public Color fillColorDead = Color.gray;

    public CharController character;

    public void SetColor(bool alive = true)
    {
        if (alive)
        {
            fill.color = fillColorAlive;
        }
        else{
            fill.color = fillColorDead;
            fill.fillAmount = 1f;
        }
    }
}
