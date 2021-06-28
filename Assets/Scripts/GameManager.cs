using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameObjectType{
    Tile,
    Character,
    Environment
}

public enum MatrixElementType
{
    Tile,
    Piece
}

public enum GameState
{
    Editor,
    Normal,
    SettingFight,
    Fighting,
    Pause
}

public enum CharacterObjectState
{
    Enabled,
    Disabled,
}

public enum CharacterState
{
    Alive,
    Dead
}

public enum CharacterAnimation
{
    Idle,
    Walking,
    CastAttack,
    Attack,
    Injured,
    Death
}


public enum CharacterType
{
    Player,
    Enemy,
    NPC
}

public enum SpellType
{
    MagicalDamage,
    PhysicalDamage,
    Heal
}
public enum TileState
{
    Enabled,
    Disabled,
    Empty,
    Hidden
}

public enum CharacterResourceType
{
    MaxHealthPoints,
    HealthPoints,
    HealthRecoveryPerSecond,
    MaxManaPoints,
    ManaPoints,
    MaxMovementPoints,
    MovementPoints
}


public class GameManager : MonoBehaviour
{
    public GameState gameState;

    public bool showGrid;

    public static GameManager sharedInstance;

    public MapMatrix mapTileMatrix;

    public Map currentMap;

    public Player currentPlayer;


    public Text fpsText;
    public Text averagefpsText;

    [HideInInspector]
    public bool showHealthBars;

    [Range(0, 255)]
    public int spritesAlpha;
    public bool transparentSprites = false;


    public List<Tile> visibleTiles = new List<Tile>();
    public List<CharController> visibleCharacters = new List<CharController>();
    public List<EnvironmentObject> visibleEnvironmentObjects = new List<EnvironmentObject>();

    [Header("Game Levels")]
    List<GameObject> gameLevels = new List<GameObject>();
    [Header("Highlight tiles")]
    public GameObject highlightPrefab;
    public List<GameObject> disabledHighlightsList = new List<GameObject>();
    public List<GameObject> enabledHighlightsList = new List<GameObject>();



    [Header("GameManagers")]
    public UIManager uiManager;
    public MouseClicksManager mouseClicksManager;


    private void Awake()
    {
        sharedInstance = this;
        uiManager = FindObjectOfType(typeof(UIManager)) as UIManager;
        mouseClicksManager = FindObjectOfType(typeof(MouseClicksManager)) as MouseClicksManager;
        this.currentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        


    }

    private void Update()
    {/*
        fps = (int)(1f / Time.unscaledDeltaTime);
        this.fpsText.text = fps.ToString();*/
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(FPS());
        SetGame();
    }

    IEnumerator FPS()
    {
        int fps = 0;
        int averageFPS = 0;
        int counter = 1;
        while (true)
        {
            fps = (int)(1f / Time.unscaledDeltaTime);
            this.fpsText.text = fps.ToString();


            averageFPS += fps;
            this.averagefpsText.text = (averageFPS / counter).ToString();

            counter++;
            yield return new WaitForSeconds(0.2f);

        }
    }




    public void SetGame()
    {
        if (GameObject.FindGameObjectWithTag("Map"))
        {
            this.currentMap = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();

            this.mapTileMatrix = this.currentMap.SetMapMatrix();
            if (this.currentMap == null || this.mapTileMatrix == null)
            {
                GetMapMatrix(GameObject.FindGameObjectWithTag("Map"));
            }
            SetPlayer();
            SetCharactersGroups();
            SetObjectsInContainers();
            CalculateMapIsometric();
            CalculateDisabledTiles();
            SetCharactersPositions();
            BattlefieldManager.sharedInstance.InstantiateBarriers();
            this.SetGameState(GameState.Normal);

        }
        else
        {
            Debug.Log("No se ha encontrado un mapa");
        }
    }



    public void SetPlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            this.currentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            /*this.currentPlayer.transform.parent.transform.SetParent(this.currentMap.Characters.gameObject.transform);
            if (currentMap.spawnTile != null)
            {
                this.currentPlayer.transform.localPosition = new Vector3(currentMap.spawnTile.transform.localPosition.x, currentMap.spawnTile.transform.localPosition.y, currentMap.spawnTile.transform.localPosition.y - 0.15f);
                this.mapTileMatrix.SetObjectAt(this.mapTileMatrix.GetObjectPosition(GameObjectType.Character, this.currentPlayer), GameObjectType.Character, this.currentPlayer);
                this.currentPlayer.GetComponent<CharController>().currentTile = this.mapTileMatrix.GetTileAt(this.mapTileMatrix.GetObjectPosition(GameObjectType.Character, this.currentPlayer));
                Debug.Log("Player position: " + this.mapTileMatrix.GetObjectPosition(GameObjectType.Character, this.currentPlayer));
                Debug.Log("Tile en posición del player: " + this.mapTileMatrix.GetTileAt(this.mapTileMatrix.GetObjectPosition(GameObjectType.Character, this.currentPlayer)));
            }
            else
            {
                //this.currentPlayer.transform.localPosition = new Vector3(this.mapTileMatrix[0, 0].gameObject.transform.localPosition.x, this.mapTileMatrix[0, 0].gameObject.transform.localPosition.y, this.mapTileMatrix[0, 0].gameObject.transform.localPosition.y - 0.15f);
                Debug.Log("eeeeeeeeeeeeeeeeeeeeeeeeeee");
            }*/
        }
        else
        {
            Debug.Log("No se ha encontrado un jugador");
        }

    }

    private void SetObjectsInContainers()
    {
        GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject NPC in NPCs)
        {
            NPC.transform.SetParent(this.currentMap.NPCs.transform);
        }

        GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("WorldObject");
        foreach (GameObject worldObject in worldObjects)
        {
            worldObject.transform.SetParent(this.currentMap.Environment.transform);
        }

        GameObject[] charactersGroups = GameObject.FindGameObjectsWithTag("CharactersGroup");
        foreach (GameObject charactersGroup in charactersGroups)
        {
            charactersGroup.transform.SetParent(this.currentMap.Characters.transform);
        }

    }


    private void SetCharactersGroups()
    {
        for (int i = 0; i < this.currentMap.Characters.transform.childCount; i++)
        {
            if (this.currentMap.Characters.transform.GetChild(i).GetComponent<CharactersGroup>())
            {
                this.currentMap.Characters.transform.GetChild(i).GetComponent<CharactersGroup>().SetGroup();
            }
            else
            {
                Debug.Log("No se ha encontrado el script de grupo del objeto: " + this.currentMap.Characters.transform.GetChild(i).name);
            }
        }
    }

    public void GetMapMatrix(GameObject map)
    {

        Debug.Log("Nope");
        if (map.GetComponent<Map>())
        {
            Map auxMap = map.GetComponent<Map>();
            this.currentMap = auxMap;
            //this.mapTileMatrix = new Tile[auxMap.columns, auxMap.rows];

            if (auxMap.mapMatrix != null)
            {
                this.mapTileMatrix = auxMap.mapMatrix;

                if (this.mapTileMatrix.tileMatrix.Count < 1 )
                {
                    for (int i = 0; i < this.currentMap.Floor.transform.childCount; i++)
                    {
                        if (this.currentMap.Floor.transform.GetChild(i).gameObject.GetComponent<Tile>() != null)
                        {
                            Tile auxTile = this.currentMap.Floor.transform.GetChild(i).gameObject.GetComponent<Tile>();
                            //this.mapTileMatrix.SetTileAt(auxTile.coordinates, auxTile);
                            this.mapTileMatrix.SetObjectAt((Vector2Int)auxTile.coordinates, GameObjectType.Tile, auxTile.gameObject);
                            
                        }
                    }
                    Debug.Log("tile matrix cargada");
                }
            }
            else
            {
                Debug.Log("Tile matrix inválida");
            }

            Debug.Log("Mapa tiene: " + this.mapTileMatrix.tileMatrix.Count + " casillas");

        }
        else
        {
            Debug.Log("Mapa no encontrado");
        }

    }

    public void CalculateDisabledTiles()
    {
        /*
        for (int i = 0; i < this.currentMap.Environment.transform.childCount; i++)
        {
            foreach (Tile tile in this.mapTileMatrix.tileMatrix.Values)
            {
                Vector2 auxWorldObjectLocalPosition = this.currentMap.Environment.transform.GetChild(i).transform.localPosition;
                Vector2 auxTilePosition = tile.gameObject.transform.localPosition;
                if (auxWorldObjectLocalPosition == auxTilePosition && this.currentMap.Environment.transform.GetChild(i).tag == "WorldObject")
                {
                    tile.SetTileState(TileState.Disabled);
                }
            }
        }*/
    }


    public void CalculateMapIsometric()
    {
        for (int i = 0; i < this.currentMap.Environment.transform.childCount; i++)
        {
            Transform childTransform = this.currentMap.Environment.transform.GetChild(i).transform;
            childTransform.localPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y, childTransform.localPosition.y);
        }

        for (int i = 0; i < this.currentMap.NPCs.transform.childCount; i++)
        {
            Transform childTransform = this.currentMap.NPCs.transform.GetChild(i).transform;
            childTransform.localPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y, childTransform.localPosition.y - 0.1f);

        }

        for (int i = 0; i < this.currentMap.Characters.transform.childCount; i++)
        {
            if (this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharactersGroup>())
            {
                foreach (CharController character in this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharactersGroup>().group)
                {
                    character.transform.localPosition = new Vector3(character.transform.localPosition.x, character.transform.localPosition.y, character.transform.localPosition.y - 0.1f);
                }
            }

            if (this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharController>())
            {
                Transform childTransform = this.currentMap.Characters.transform.GetChild(i).transform;
                childTransform.localPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y, childTransform.localPosition.y - 0.1f);
            }
        }
    }



    public void SetCharactersPositions()
    {
        /*
        for (int i = 0; i < this.currentMap.Characters.transform.childCount; i++)
        {
            Debug.Log("Contenedor de personajes tiene: " + this.currentMap.Characters.transform.childCount + " hijos");
            if (this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharController>())
            {
                this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharController>().GetCurrentTile();
            }

            if (this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharactersGroup>())
            {
                for (int j = 0; j < this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharactersGroup>().group.Count; j++)
                {
                    Debug.Log("Grupo de personajes tiene: " + this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharactersGroup>().group.Count + " hijos");
                    this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharactersGroup>().group[j].GetComponent<CharController>().GetCurrentTile();
                }

            }
        }*/
    }


    public void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Normal:
                TurnsManager.sharedInstance.ShowHUD(false);
                break;

            case GameState.SettingFight:
                TurnsManager.sharedInstance.ShowHUD(true);
                break;

            case GameState.Pause:
                break;

            case GameState.Editor:
                break;
        }
        this.gameState = state;
    }

    public void ShowGrid(bool show)
    {
        StartCoroutine(Grid(show));
    }

    IEnumerator Grid(bool show)
    {
        if (mapTileMatrix != null)
        {
            foreach (Tile tile in this.mapTileMatrix.tileMatrix.Values)
            {
                //tile.showGrid = show;
                tile.SetShowGrid(show);
            }

        }
        yield break;
    }


    public void SetTransparency(bool transparency)
    {
        float alpha = 0f;
        if (transparency)
        {
            alpha = (1f / 255f) * (float)this.spritesAlpha;
        }
        else
        {
            alpha = 1f;
        }

        this.transparentSprites = transparency;

        Transform auxCharactersContainer = this.currentMap.Characters.transform;
        for (int i = 0; i < auxCharactersContainer.childCount; i++)
        {
            foreach (CharController character in auxCharactersContainer.GetChild(i).GetComponent<CharactersGroup>().group)
            {
                SetAlpha(character.gameObject, alpha);
            }
        }

        Transform auxWorldObjectsContainer = this.currentMap.Environment.transform;
        for (int i = 0; i < auxWorldObjectsContainer.childCount; i++)
        {
            SetAlpha(auxWorldObjectsContainer.GetChild(i).gameObject, alpha);
        }
    }

    private void SetAlpha(GameObject auxObject, float alpha)
    {
        if (auxObject.GetComponent<SpriteRenderer>())
        {
            SpriteRenderer sr = auxObject.GetComponent<SpriteRenderer>();
            Color auxColor = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            sr.color = auxColor;
        }
        if (auxObject.transform.childCount > 0)
        {
            for (int i = 0; i < auxObject.transform.childCount; i++)
            {
                SetAlpha(auxObject.transform.GetChild(i).gameObject, alpha);
            }
        }

    }


    public void ShowHealthBars(bool show)
    {
        List<CharController> characters = new List<CharController>();

        GetCharacters(this.currentMap.Characters.transform, ref characters);

        foreach (CharController character in characters)
        {
            if (character.healthBar != null)
            {
                character.healthBar.ShowBar(show);
            }
        }

        this.showHealthBars = show;
    }
    

    private void GetCharacters(Transform parent, ref List<CharController> charList)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).GetComponent<CharController>())
            {
                if (!charList.Contains(parent.GetChild(i).GetComponent<CharController>()))
                {
                    charList.Add(parent.GetChild(i).GetComponent<CharController>());
                }
            }

            if (parent.GetChild(i).transform.childCount > 0)
            {
                GetCharacters(parent.GetChild(i).transform, ref charList);
            }
        }


    }



    public void HighlightTiles(Tile tile, Color? color = null)
    {
        Highlight(tile, color);
    }

    public void HighlightTiles(List<Tile> tilesList, Color? color = null)
    {
        foreach (var tile in tilesList)
        {
            Highlight(tile, color);
        }
        
    }

    public void HideAllHighlightedTiles()
    {
        Highlight(null, null, false);
    }
    public void HideHighlightedTiles(Tile tile)
    {
        Highlight(tile, null, false);
    }
    public void HideHighlightedTiles(List<Tile> tileList)
    {
        foreach (Tile tile in tileList)
        {
            Highlight(tile, null, false);
        }
        
    }

    private void Highlight(Tile tile = null, Color? color = null, bool show = true)
    {
        if (tile != null)
        {
            if (show)
            {
                GameObject auxHighlight = null;
                SpriteRenderer renderer = null;

                foreach (GameObject enabledHighlight in this.enabledHighlightsList)
                {
                    if (enabledHighlight.transform.parent == tile.transform)
                    {
                        enabledHighlight.SetActive(false);
                        auxHighlight = enabledHighlight;
                        enabledHighlight.SetActive(true);
                        break;
                    }
                }

                if (auxHighlight == null)
                {
                    if (this.disabledHighlightsList.Count > 0)
                    {
                        auxHighlight = this.disabledHighlightsList[0];
                        auxHighlight.SetActive(true);
                        this.disabledHighlightsList.Remove(auxHighlight);
                        this.enabledHighlightsList.Add(auxHighlight);
                    }
                    else
                    {
                        auxHighlight = Instantiate(this.highlightPrefab);
                        this.enabledHighlightsList.Add(auxHighlight);
                    }
                }


                auxHighlight.transform.SetParent(tile.transform);
                auxHighlight.transform.localPosition = Vector3.zero;

                if (auxHighlight.GetComponent<SpriteRenderer>())
                {
                    renderer = auxHighlight.GetComponent<SpriteRenderer>();
                }
                else if (auxHighlight.GetComponentInChildren<SpriteRenderer>())
                {
                    renderer = auxHighlight.GetComponentInChildren<SpriteRenderer>();
                }
                else
                {
                    Debug.Log("No se ha encontrado un renderer");
                }

                if (renderer != null)
                {
                    if (color.HasValue)
                    {
                        renderer.color = color.Value;
                    }
                    else
                    {
                        renderer.color = Color.white;
                    }
                }
            }
            else
            {
                foreach (GameObject auxHighlight in this.enabledHighlightsList)
                {
                    if (auxHighlight.transform.parent == tile.transform)
                    {
                        this.disabledHighlightsList.Add(auxHighlight);
                        auxHighlight.transform.SetParent(this.transform);
                        auxHighlight.SetActive(false);
                        this.enabledHighlightsList.Remove(auxHighlight);
                        break;
                    }
                }

            }

        }
        else
        {
            foreach (GameObject auxHighlight in this.enabledHighlightsList)
            {
                this.disabledHighlightsList.Add(auxHighlight);

                auxHighlight.transform.SetParent(this.transform);

                auxHighlight.SetActive(false);
            }
            this.enabledHighlightsList.Clear();
        }
    }


    public void ShowAmbientClouds(bool show)
    {
        AmbientClouds.sharedInstance.ShowClouds(show);
    }




    public bool Normal()
    {
        return this.gameState == GameState.Normal;
    }
    public bool IsSettingFight()
    {
        return this.gameState == GameState.SettingFight;
    }

    public bool IsFighting()
    {
        return this.gameState == GameState.Fighting;
    }

    public bool IsSettingFightOrFighting()
    {
        return this.IsFighting() || this.IsSettingFight();
    }

    public bool IsPlaying()
    {
        return this.gameState != GameState.Pause;
    }


    public Tile GetTileComponent(GameObject gObject)
    {
        if (gObject.GetComponent<Tile>())
        {
            return gObject.GetComponent<Tile>();
        }else if (gObject.GetComponentInChildren<Tile>())
        {
            return gObject.GetComponentInChildren<Tile>();
        }
        return null;
    }

    public CharController GetCharControllerComponent(GameObject gObject)
    {
        if (gObject.GetComponent <CharController>())
        {
            return gObject.GetComponent<CharController>();
        }
        else if (gObject.GetComponentInChildren<CharController>())
        {
            return gObject.GetComponentInChildren<CharController>();
        }
        return null;
    }
    public EnvironmentObject GetEnvironmentObjectComponent(GameObject gObject)
    {
        if (gObject.GetComponent<EnvironmentObject>())
        {
            return gObject.GetComponent<EnvironmentObject>();
        }
        else if (gObject.GetComponentInChildren<EnvironmentObject>())
        {
            return gObject.GetComponentInChildren<EnvironmentObject>();
        }
        return null;
    }
}
