using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldManager : MonoBehaviour
{
    public static BattlefieldManager sharedInstance;

    public int minBattlefieldSize;
    public int maxBattlefieldSize;

    [Tooltip("Celdas disponibles para seleccionar al momento de prepararse para la batalla. \n Es la cantidad disponible para cada bando (o sea, la cantidad total será el doble)")]
    public int startingTilesAmount;
    private List<Tile> startingTilesList = new List<Tile>();

    public List<GameObject> barriersPrefabsList = new List<GameObject>();

    // Lista de tiles del campo de batalla
    public List<Tile> battlefield = new List<Tile>();

    public List<GameObject> instantiatedBarriersList = new List<GameObject>();
    public List<GameObject> currentBarriersList = new List<GameObject>();
    private List<Tile> currentBarriersTilesList = new List<Tile>();


    public List<Tile> disabledTiles = new List<Tile>();
    private List<GameObject> currentObjectsInBattlefield = new List<GameObject>();

    private List<CharController> hiddenCharacters = new List<CharController>();


    private GameManager gameManager;

    private void Awake()
    {
        sharedInstance = this;
        
    }
    private void Start()
    {
        this.gameManager = GameManager.sharedInstance;
    }

    public void GenerateBattlefield(Tile currentPosition)
    {
        EraseBattlefield();

        int x = (int)currentPosition.coordinates.x;
        int y = (int)currentPosition.coordinates.y;

        int auxBattlefieldSizeRows = Random.Range(minBattlefieldSize, maxBattlefieldSize);
        int auxBattlefieldSizeColumns = Random.Range(minBattlefieldSize, maxBattlefieldSize);

        for (int rows = 0; rows < auxBattlefieldSizeRows; rows++)
        {
            for (int columns = 0; columns < auxBattlefieldSizeColumns; columns++)
            {
                CheckTile(this.gameManager.mapTileMatrix.GetTileAt((Vector2Int)new Vector3Int(x - columns, y - rows, currentPosition.coordinates.z)), columns, auxBattlefieldSizeColumns, rows, auxBattlefieldSizeRows);

                CheckTile(this.gameManager.mapTileMatrix.GetTileAt((Vector2Int)new Vector3Int(x - columns, y + rows, currentPosition.coordinates.z)), columns, auxBattlefieldSizeColumns, rows, auxBattlefieldSizeRows);

                CheckTile(this.gameManager.mapTileMatrix.GetTileAt((Vector2Int)new Vector3Int(x + columns, y + rows, currentPosition.coordinates.z)), columns, auxBattlefieldSizeColumns, rows, auxBattlefieldSizeRows);

                CheckTile(this.gameManager.mapTileMatrix.GetTileAt((Vector2Int)new Vector3Int(x + columns, y - rows, currentPosition.coordinates.z)), columns, auxBattlefieldSizeColumns, rows, auxBattlefieldSizeRows);
            }
        }


        //GameManager.sharedInstance.CalculateMapIsometric();

        //SetAllColors();
    }

    public void InstantiateBarriers()
    {
        if (instantiatedBarriersList.Count < (maxBattlefieldSize * 8) - 1)
        {
            
            if (instantiatedBarriersList.Count > 0)
            {
                foreach (GameObject barrier in instantiatedBarriersList)
                {
                    Destroy(barrier);
                    instantiatedBarriersList.Clear();
                }
            }

            for (int i = 0; i < maxBattlefieldSize * 8; i++)
            {
                GameObject auxBarrier = Instantiate(barriersPrefabsList[Random.Range(0, barriersPrefabsList.Count)]);
                auxBarrier.GetComponentInChildren<SpriteRenderer>().enabled = false;
                auxBarrier.transform.SetParent(GameManager.sharedInstance.currentMap.Environment.transform);
                instantiatedBarriersList.Add(auxBarrier);
                //auxBarrier.SetActive(false);

            }
            Debug.Log("Barreras instanciadas");
        }
        else
        {
            Debug.Log("Barreras ya existentes");
        }

    }


    public void HideExternalObjects(GameObject targetGroup, GameObject playerGroup)
    {
        if (GameManager.sharedInstance.currentMap)
        {
            GameObject charactersContainer = GameManager.sharedInstance.currentMap.Characters.gameObject;
            for (int i = 0; i < charactersContainer.transform.childCount; i++)
            {
                if (charactersContainer.transform.GetChild(i).gameObject == targetGroup || charactersContainer.transform.GetChild(i).gameObject == playerGroup)
                {
                    continue;
                }
                else
                {
                    foreach (CharController auxChar in charactersContainer.transform.GetChild(i).gameObject.GetComponent<CharactersGroup>().group)
                    {
                        auxChar.SetCharacterObjectState(CharacterObjectState.Disabled);
                        hiddenCharacters.Add(auxChar);
                    }
                }
            }

            GameObject npcContainer = GameManager.sharedInstance.currentMap.NPCs.gameObject;
            for (int i = 0; i < npcContainer.transform.childCount; i++)
            {
                if (npcContainer.transform.GetChild(i).GetComponent<CharController>())
                {
                    npcContainer.transform.GetChild(i).GetComponent<CharController>().SetCharacterObjectState(CharacterObjectState.Disabled);
                    hiddenCharacters.Add(npcContainer.transform.GetChild(i).GetComponent<CharController>());
                }
            }
        }
        else
        {
            Debug.Log("No se ha encontrado un mapa");
        }
    }


    public List<Tile> GenerateStartingTiles()
    {
        this.startingTilesList.Clear();
        for (int i = 0; i < startingTilesAmount * 2; i++)
        {
            Tile auxRandomTile = battlefield[Random.Range(0, battlefield.Count)];
            while (startingTilesList.Contains(auxRandomTile) || auxRandomTile.tileState == TileState.Disabled || currentBarriersTilesList.Contains(auxRandomTile))
            {
                auxRandomTile = battlefield[Random.Range(0, battlefield.Count)];
            }
            if (i < startingTilesAmount)
            {
                auxRandomTile.SetTileState(TileState.Empty, Color.blue);
            }
            else
            {
                auxRandomTile.SetTileState(TileState.Empty, Color.red);
            }
            startingTilesList.Add(auxRandomTile);

        }
        return startingTilesList;
    }

    IEnumerator DisableTiles()
    {
        List<Tile> tilesWithObjects = new List<Tile>();
        foreach (GameObject gObject in this.currentObjectsInBattlefield)
        {
            tilesWithObjects.Add(this.gameManager.mapTileMatrix.GetObjectCurrentTile(GameObjectType.Environment, gObject));
        }
        foreach (Tile tile in this.gameManager.mapTileMatrix.tileMatrix.Values)
        {
            tile.SetFightColor();
        }/*
        foreach (Tile tile in GameManager.sharedInstance.mapTileMatrix.tileMatrix.Values)
        {
            if (tile.tileState == TileState.Enabled && !this.battlefield.Contains(tile) && !tilesWithObjects.Contains(tile))
            {
                tile.SetTileState(TileState.Disabled, Color.gray);
                tile.selfDisabled = true;
                this.disabledTiles.Add(tile);
                Debug.Log("mlem");
            }
        }*/
        Debug.Log("Rutina terminada");
        yield break;
    }

    public void SetAllColors()
    {

        StartCoroutine(DisableTiles());

        /*
        foreach (Tile tile in this.gameManager.mapTileMatrix.tileMatrix.Values)
        {
            tile.spriteRenderer.color = Color.gray;
            
        }*/
        /*
        foreach (Tile tile in battlefield)
        {
            tile.spriteRenderer.color = Color.white;
        }*/

        SetColor(GameManager.sharedInstance.currentMap.Environment.gameObject, Color.gray);

        foreach (GameObject auxObject in currentObjectsInBattlefield)
        {
            SetColor(auxObject, Color.white);
        }

        GameManager.sharedInstance.SetTransparency(GameManager.sharedInstance.transparentSprites);
    }

    public void CheckTile(Tile tile, int currentColumn, int maxColumns, int currentRow, int maxRows)
    {
        if (tile != null)
        {
            if (tile.tileState == TileState.Enabled)
            {
                EnvironmentObject eObject = null;
                if (eObject = gameManager.mapTileMatrix.GetEnvironmentObjectAt(tile))
                {
                    currentObjectsInBattlefield.Add(eObject.gameObject);
                }
                else
                {
                    battlefield.Add(tile);
                    if (currentColumn >= maxColumns - 1 || currentRow >= maxRows - 1)
                    {
                        currentBarriersTilesList.Add(tile);
                    }
                }
                
            }
        }
    }

    public bool IsInBattlefield(GameObjectType gOType, GameObject gObject)
    {
        switch (gOType)
        {
            case GameObjectType.Tile:
                Tile tile = this.gameManager.GetTileComponent(gObject);
                if (tile)
                {
                    return this.battlefield.Contains(tile);
                }
                break;

            case GameObjectType.Character:
                CharController cController = this.gameManager.GetCharControllerComponent(gObject);
                if (cController)
                {
                    return this.hiddenCharacters.Contains(cController);
                }
                break;

            case GameObjectType.Environment:
                return this.currentObjectsInBattlefield.Contains(gObject);
        }
        return false;
    }


    public void GenerateBarriers()
    {
        StartCoroutine(BarriersSpawnAnimation());
    }

    IEnumerator BarriersSpawnAnimation()
    {
        int counter = 0;
        foreach (Tile tile in currentBarriersTilesList)
        {

            GameObject auxBarrier = instantiatedBarriersList[counter];
            
            auxBarrier.transform.SetParent(GameManager.sharedInstance.currentMap.Environment.transform);
            auxBarrier.transform.localPosition = new Vector3(tile.transform.localPosition.x, tile.transform.localPosition.y, tile.transform.localPosition.y);

            auxBarrier.GetComponentInChildren<SpriteRenderer>().enabled = true;
            auxBarrier.GetComponentInChildren<Animator>().SetTrigger("Animation");

            currentBarriersList.Add(auxBarrier);

            tile.SetTileState(TileState.Disabled);
            disabledTiles.Add(tile);

            yield return new WaitForSeconds(0.1f);
            counter++;
        }
        // Set tile colors
        this.SetAllColors();
        yield break;


    }


    public void EraseBattlefield()
    {
        StopAllCoroutines();

        if (currentBarriersList.Count > 0)
        {
            foreach (GameObject barrier in currentBarriersList)
            {
                barrier.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            currentBarriersList.Clear();

        }

        if (currentBarriersTilesList.Count > 0)
        {
            currentBarriersTilesList.Clear();
        }

        if (disabledTiles.Count > 0)
        {
            foreach (Tile tile in disabledTiles)
            {
                tile.SetTileState(TileState.Enabled, Color.white);
                tile.selfDisabled = false;
            }
            disabledTiles.Clear();
        }

        if (battlefield.Count > 0)
        {
            foreach (Tile tile in this.gameManager.mapTileMatrix.tileMatrix.Values)
            {
                tile.spriteRenderer.color = Color.white;
            }
            battlefield.Clear();
        }

        SetColor(GameManager.sharedInstance.currentMap.Environment.gameObject, Color.white);

        if (currentObjectsInBattlefield.Count > 0)
        {
            currentObjectsInBattlefield.Clear();
        }

        if (startingTilesList.Count > 0)
        {
            foreach (Tile tile in startingTilesList)
            {
                tile.SetTileState(TileState.Enabled, Color.white);
            }
            startingTilesList.Clear();
        }

        if (hiddenCharacters.Count > 0)
        {
            foreach (CharController hiddenCharacter in hiddenCharacters)
            {
                hiddenCharacter.SetCharacterObjectState(CharacterObjectState.Enabled);
            }
            hiddenCharacters.Clear();
        }

        GameManager.sharedInstance.SetTransparency(GameManager.sharedInstance.transparentSprites);
    }


    public void SetColor(GameObject worldObject, Color color)
    {
        if (worldObject.GetComponent<SpriteRenderer>())
        {
            worldObject.GetComponent<SpriteRenderer>().color = color;
        }

        if (worldObject.transform.childCount > 0 && !instantiatedBarriersList.Contains(worldObject))
        {
            for (int i = 0; i < worldObject.transform.childCount; i++)
            {
                SetColor(worldObject.transform.GetChild(i).gameObject, color);
            }
        }
    }






}
