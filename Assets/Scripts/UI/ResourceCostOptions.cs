using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceCostOptions : MonoBehaviour
{
    public GameObject resourceCostObject;
    public static ResourceCostOptions sharedInstance;
    public Canvas spellOptionsCanvas;
    public TextMeshProUGUI spellCostText;
    private RectTransform rTransform;
    public Tile selectedTile;

    private SpellsManager sManager;

    private int cost;

    public Player player;
    private Tile tile;
    CharacterResourceType resourceType;

    private List<Tile> auxAOE;


    private void Awake()
    {
        sharedInstance = this;
    }

    private void Start()
    {
        sharedInstance = this;
        resourceCostObject = gameObject;
        if (GetComponent<Canvas>() != null)
            {
                this.spellOptionsCanvas = GetComponent<Canvas>();
                this.spellOptionsCanvas.enabled = false;
                
            }
            else
            {
                Debug.Log("Error: No se ha encontrado un canvas de opciones de hechizo");
            }
        if (this.spellOptionsCanvas == null)
        {
            if (GetComponent<Canvas>() != null)
            {
                this.spellOptionsCanvas = GetComponent<Canvas>();
                this.spellOptionsCanvas.enabled = false;
                
            }
            else
            {
                Debug.Log("Error: No se ha encontrado un canvas de opciones de hechizo");
            }
        }
        this.player = GameManager.sharedInstance.currentPlayer.GetComponent<Player>();

        if (this.spellOptionsCanvas != null)
        {
            this.rTransform = GetComponent<RectTransform>();
            this.spellOptionsCanvas.enabled = false;

        }

        this.sManager = SpellsManager.sharedInstance;
    }

    public int resourceCost
    {
        get {
            return resourceCost;
        }

        set {
            spellCostText.text = value.ToString();
        }
    }


    public void ShowSpellCastOptions(CharacterResourceType type, int resourceCost, Tile tile, bool show = true)
    {
        this.selectedTile = tile;
        this.resourceCost = resourceCost;
        cost = resourceCost;
        Vector2 position = Vector2.zero;

        if (tile != null)
        {
            this.tile = tile;
            position = CameraController.sharedInstance.cam.WorldToScreenPoint(tile.transform.position);

            switch (type)
            {
                case CharacterResourceType.MaxHealthPoints:
                    break;
                case CharacterResourceType.HealthPoints:
                    break;
                case CharacterResourceType.HealthRecoveryPerSecond:
                    break;
                case CharacterResourceType.MaxManaPoints:
                    break;
                case CharacterResourceType.ManaPoints:

                    this.auxAOE = SpellRangeHelper.sharedInstance.ShowRange(tile.coordinates, this.sManager.selectedSpell.spellData.spellAOEMin, this.sManager.selectedSpell.spellData.spellAOEMax, true, SpellRangeType.SpellAOE);
                    
                    if (FightsManager.sharedInstance.tilesInAOE != null)
                    {
                        FightsManager.sharedInstance.tilesInAOE.Clear();
                    }
                    else
                    {
                        FightsManager.sharedInstance.tilesInAOE = new List<Tile>();
                    }
                    foreach (Tile auxTile in this.auxAOE)
                    {
                        FightsManager.sharedInstance.tilesInAOE.Add(auxTile);
                    }
                    //FightsManager.sharedInstance.tilesInAOE = this.auxAOE;
                    break;
                case CharacterResourceType.MaxMovementPoints:
                    break;
                case CharacterResourceType.MovementPoints:

                    break;
            } 
        }

        resourceCostObject.transform.position = new Vector2(position.x, position.y + this.rTransform.rect.height);
        this.spellOptionsCanvas.enabled = show;
        this.resourceType = type;
    }

    public void TriggerAction(bool proceed)
    {
        switch (this.resourceType)
        {
            case CharacterResourceType.MaxHealthPoints:
                break;
            case CharacterResourceType.HealthPoints:
                break;
            case CharacterResourceType.HealthRecoveryPerSecond:
                break;
            case CharacterResourceType.MaxManaPoints:
                break;
            case CharacterResourceType.ManaPoints:
                if (proceed)
                {
                    SpellsManager.sharedInstance.CastSpell(this.selectedTile);
                }
                else
                {
                    SpellsManager.sharedInstance.CastSpell(null);
                }
                FightsManager.sharedInstance.tilesInAOE = null;
                break;
            case CharacterResourceType.MaxMovementPoints:
                break;
            case CharacterResourceType.MovementPoints:
                if (proceed)
                {
                    this.player.MovePlayer(this.tile, null, this.cost);
                }
                else
                {
                    PathFindingA.sharedInstance.ShowPath(false);
                }
                break;
        }

        ShowSpellCastOptions(0, 0, null, false);



    }


}
