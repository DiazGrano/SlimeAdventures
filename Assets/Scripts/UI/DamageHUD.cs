using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageHUD : MonoBehaviour
{
    public static DamageHUD sharedInstance;

    public Canvas damageHUDCanvas;

    public GameObject damageHUDPrefab;
    public TextMeshProUGUI damageText;


    public Vector3 offset;

    public Color damageFontColor;
    public Color healFontColor;
    public Color zeroDamageFontColor;


    private void Awake()
    {
        sharedInstance = this;
    }

    public void ShowDamage(int amount, Tile targetTile)
    {
        GameObject auxDamageHUDPrefab = Instantiate(this.damageHUDPrefab, this.damageHUDCanvas.transform);

        Vector3 worldPosition = new Vector3(targetTile.transform.position.x, targetTile.transform.position.y, 0f);

        auxDamageHUDPrefab.transform.position = CameraController.sharedInstance.cam.WorldToScreenPoint(worldPosition) + offset;
        damageText = auxDamageHUDPrefab.GetComponentInChildren<TextMeshProUGUI>();
        damageText.text = amount.ToString();

        if (amount > 0)
        {
            damageText.color = healFontColor;
        }
        else if (amount < 0)
        {
            damageText.color = damageFontColor;
        }
        else
        {
            damageText.color = zeroDamageFontColor;
        }
    }
}
