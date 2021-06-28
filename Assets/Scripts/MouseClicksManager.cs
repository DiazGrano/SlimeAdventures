using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MouseClicksManager : MonoBehaviour
{
   // public static MouseClicksManager sharedInstance;

    private GameManager gameManager;
    private Camera cam;
    private Player player;

    private ResourceCostOptions resourceCostOptions;

    private void Awake()
    {
        //sharedInstance = this;
    }

    private void Start()
    {
        this.gameManager = GameManager.sharedInstance;
        this.player = GameManager.sharedInstance.currentPlayer.GetComponent<Player>();
        this.cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        resourceCostOptions = GameManager.sharedInstance.uiManager.resourceCostOptions;
    }

    private void Update()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (IsPointerOverUIObject())
                {
                    return;
                }
                else
                {
                    PointerClicked();
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                else
                {
                    PointerClicked();
                }
            }
        }
    }


    private void PointerClicked()
    {
        RaycastHit2D hit = Cast2DRayOnMousePosition();
        if (hit)
        {
            switch (hit.collider.tag)
            {
                case "Enemy":

                    CharController selectedTarget = hit.collider.gameObject.GetComponent<CharController>();

                    switch (gameManager.gameState)
                    {
                        case GameState.Normal:

                            //player.TargetCharacterClicked(hit.collider.gameObject);
                            SelectedCharacterOptions.sharedInstance.ShowSelectedCharacterOptions(selectedTarget);

                            break;

                        case GameState.SettingFight:
                            break;

                        case GameState.Fighting:
                            if (TurnsManager.sharedInstance.IsCharacterTurn(player.characterController))
                            {
                                if (FightsManager.sharedInstance.tilesInSpellRange.Contains(selectedTarget.currentTile))
                                {

                                    //ResourceCostOptions.sharedInstance.ShowSpellCastOptions(CharacterResourceType.ManaPoints, SpellsManager.sharedInstance.selectedSpell.spellData.spellCost, selectedTarget.currentTile, true);

                                   resourceCostOptions.ShowSpellCastOptions(CharacterResourceType.ManaPoints, SpellsManager.sharedInstance.selectedSpell.spellData.spellCost, selectedTarget.currentTile, true);






                                    //SpellsManager.sharedInstance.CastSpell(selectedTarget.currentTile);
                                }
                                else
                                {
                                    SpellsManager.sharedInstance.CastSpell(null);
                                }
                                
                            }
                            break;

                        case GameState.Pause:
                            break;
                    }

                    break;
            }
        }
        else
        {
            if (SelectedCharacterOptions.sharedInstance.characterOptionsOpen())
            {
                SelectedCharacterOptions.sharedInstance.ForceClose();
            }
            Tile selectedTile = GameManager.sharedInstance.currentMap.mapMatrix.GetTileAt((Vector2Int)GetMouseCoordinate());
            if (selectedTile != null)
            {
                switch (this.gameManager.gameState)
                {
                    case GameState.Normal:

                        if (selectedTile.tileState == TileState.Enabled)
                        {
                            player.MovePlayer(selectedTile);
                        }
                        break;

                    case GameState.SettingFight:
                        if (selectedTile.tileState == TileState.Empty)
                        {
                            player.MovePlayer(selectedTile);
                        }
                        break;

                    case GameState.Fighting:
                        if (TurnsManager.sharedInstance.IsCharacterTurn(player.characterController))
                        {
                            if (selectedTile.tileState == TileState.Enabled)
                            {
                                
                                if (SpellsManager.sharedInstance.selectedSpell != null)
                                {
                                    if (FightsManager.sharedInstance.tilesInSpellRange.Contains(selectedTile))
                                    {
                                        //ResourceCostOptions.sharedInstance.ShowSpellCastOptions(CharacterResourceType.ManaPoints, SpellsManager.sharedInstance.selectedSpell.spellData.spellCost, selectedTile);
                                        resourceCostOptions.ShowSpellCastOptions(CharacterResourceType.ManaPoints, SpellsManager.sharedInstance.selectedSpell.spellData.spellCost, selectedTile);

                                    }
                                    else
                                    {
                                        SpellsManager.sharedInstance.CastSpell(null);
                                    }

                                }
                                else
                                {
                                    if (/*selectedTile.spriteRenderer.color == Color.white*/ true)
                                    {
                                        List<Tile> path = PathFindingA.sharedInstance.FindPath(selectedTile, this.player.characterController.currentTile, null, false);
                                        if (path != null)
                                        {
                                            if (path.Count - 1 <= player.characterController.characterStats.CharacterResource(CharacterResourceType.MovementPoints))
                                            {
                                                PathFindingA.sharedInstance.ShowPath(true);
                                                //ResourceCostOptions.sharedInstance.ShowSpellCastOptions(CharacterResourceType.MovementPoints, path.Count - 1, selectedTile);
                                                resourceCostOptions.ShowSpellCastOptions(CharacterResourceType.MovementPoints, path.Count - 1, selectedTile);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    private RaycastHit2D Cast2DRayOnMousePosition(float maxLength = 1000f)
    {
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = new Ray(mouseWorldPosition, Vector2.zero);

        return Physics2D.Raycast(ray.origin, ray.direction, maxLength);
    }



    private Vector3Int GetMouseCoordinate()
    {
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        return gameManager.currentMap.Floor.WorldToCell(mouseWorldPosition);
    }

    public bool IsPointerOverUIObject()
    {
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) || EventSystem.current.currentSelectedGameObject != null)
        {
            return true;
        }

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}
