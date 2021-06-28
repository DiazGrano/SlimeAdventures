using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellUI : MonoBehaviour, IPointerClickHandler
{
    public Sprite normalButtonSprite;
    public Sprite pressedButtonSprite;

    public Image buttonImage;

    public SpellData spellData;

    public Image spellIcon;
    public int spellID;

    private CharController player;

    private ResourceCostOptions resourceCostOptions;


    private void Start()
    {
        if (this.spellData != null)
        {
            if (this.spellData.spellIcon != null)
            {
                this.spellIcon.sprite = this.spellData.spellIcon;
            }
            this.spellID = this.spellData.spellId;
        }
        player = GameManager.sharedInstance.currentPlayer.GetComponent<CharController>();
        resourceCostOptions = GameManager.sharedInstance.uiManager.resourceCostOptions;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.spellData != null)
        {
            if (GameManager.sharedInstance.gameState == GameState.Fighting && TurnsManager.sharedInstance.IsCharacterTurn(player))
            {
                //ResourceCostOptions.sharedInstance.ShowSpellCastOptions(0, 0, null, false);
                //UIManager.resourceCostOptions.ShowSpellCastOptions(0, 0, null, false);
                Debug.Log(resourceCostOptions);
                resourceCostOptions.ShowSpellCastOptions(0, 0, null, false);
                if (this.spellData.spellCost <= player.characterStats.CharacterResource(CharacterResourceType.ManaPoints))
                {

                    SpellsManager.sharedInstance.SpellSelected(this);


                    if (this.buttonImage != null && this.pressedButtonSprite != null)
                    {
                        this.buttonImage.sprite = this.pressedButtonSprite;
                    }
                    List<Tile> auxTilesInRange = SpellRangeHelper.sharedInstance.ShowRange(Player.sharedInstance.characterController.currentTile.coordinates, this.spellData.spellMinRange, this.spellData.spellMaxRange, true, SpellRangeType.SpellRange);
                    FightsManager.sharedInstance.tilesInSpellRange = auxTilesInRange;


                    //SpellRangeHelper.sharedInstance.ShowRange(Player.sharedInstance.currentTile.coordinates, this.spellData.spellMinRange, this.spellData.spellMaxRange);

                }
                else
                {
                    Debug.Log("Mana insuficiente");
                }

            }
            else
            {
                Debug.Log("No es el turno del jugador");
            }
        }
    }



    public void SetButton()
    {
        if (this.spellData != null)
        {
            if (this.buttonImage != null && this.normalButtonSprite != null)
            {
                //ResourceCostOptions.sharedInstance.ShowSpellCastOptions(0, 0, null, false);
                resourceCostOptions.ShowSpellCastOptions(0, 0, null, false);
                //UIManager.resourceCostOptions.ShowSpellCastOptions(0, 0, null, false);

                SpellRangeHelper.sharedInstance.HideRange();
                FightsManager.sharedInstance.tilesInSpellRange = null;
                this.buttonImage.sprite = this.normalButtonSprite;
            }
        }
        
    }

}
