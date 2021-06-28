#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MapLayers
{
    Floor,
    Environment,
    NPCs,
    Characters
}
public enum TileBrushState
{
    Enabled,
    Disabled
}

public enum TileBrushStyle
{
    SingleTile,
    Section
}

public class AuxTileList
{
    public int id;
    public List<GameObject> tileList = new List<GameObject>();
}

public class MapEditor : MonoBehaviour
{
    public static MapEditor sharedInstance;


    //public GameObject tile;
    [Header("Prefabs de celdas a usar para crear mapa automático")]
    public List<GameObject> tilesPrefabs = new List<GameObject>();

    [Header("Prefab de contenedor de mapa a usar para crear mapa")]
    public GameObject mapContainerPrefab;

    [Header("Mapa actual")]
    public GameObject currentMapContainer;
    public Map currentMap;


    private Tile[,] mapTileMatrix;

    [Header("Nombre del mapa a generar/guardar")]
    public string mapName = "EmptyMap";

    [Header("Tamaño del mapa a generar")]
    public int rows;
    public int columns;

    
    [Header("Hacer mapa procedural")]
    // En caso de seleccionar el hacer el mapa en modo proceduralMap,
    // el sprite en la posición 0 tiene menor probabilidad de aparecer que 
    // el srite de la última posición de la lista de sprites
    public bool proceduralMap = false;
    [Range(0.02f, 1f)]
    public float perlinNoiseScale = 0.3f;

    




    [Header("Dibujar manualmente")]
    public TileBrushState brushState;
    public TileBrushStyle brushStyle;

    [Header("Seleccionar layer a editar")]
    public MapLayers selectedLayer;
    [HideInInspector]
    public Tilemap selectedTilemap;

    [Header("Objeto a dibujar en mapa")]
    public GameObject tileToDraw;



    [Header("Modo de edición de celdas del mapa")]
    [Tooltip("Se usa seleccionando un modo de edición de celda y haciendo click derecho sobre la celda deseada")]
    public TileState tileMode = TileState.Enabled;










    [HideInInspector]
    public int toolbarTab;
    public string currentTab;





    private Coroutine optimizeCoroutine = null;
    private Coroutine deoptimizeCoroutine = null;


    int tileCounter = 0;
    int cantidadCeldas = 0;
    float individualTileCreationProgress = 0f;
    float totalTileCreationProgress = 0f;


    private void Start()
    {
        //GameManager.sharedInstance.SetGameState(GameState.Editor);
    }


    private void OnValidate()
    {
        if (this.currentMap != null)
        {
            switch (selectedLayer)
            {
                case MapLayers.Floor:
                    this.selectedTilemap = this.currentMap.Floor;
                    break;
                case MapLayers.Environment:
                    this.selectedTilemap = this.currentMap.Environment;
                    break;
                case MapLayers.NPCs:
                    this.selectedTilemap = this.currentMap.NPCs;
                    break;
                case MapLayers.Characters:
                    this.selectedTilemap = this.currentMap.Characters;
                    break;
            }
        }
    }

    void Awake()
    {
        sharedInstance = this;
    }


    public void GenerateMap()
    {
        if (currentMapContainer)
        {
            bool answer = EditorUtility.DisplayDialog("Confirmación de generación de mapa", "Ya se ha generado un mapa, si selecciona continuar,\n se eliminará  el mapa actual", "Continuar", "Cancelar");
            if (answer)
            {
                Destroy(currentMapContainer);
                StartCoroutine(CreateTiles());
            }
        }
        else
        {
            StartCoroutine(CreateTiles());
        }
    }


    IEnumerator CreateTileRow(int id, int columns, GameObject container)
    {
        float posX = 0;
        float posY = 0;

        float initialPosX = (-0.5f * id);
        float initialPosY = (0.25f * id);


        for (int j = 0; j < columns; j++)
        {

            Vector3 position = new Vector3(initialPosX + posX, initialPosY + posY, 0f);

            Vector3Int coordinate = selectedTilemap.WorldToCell(position);


            DrawTile(coordinate);

            /*
            // Procedural
            if (proceduralMap)
            {
                tile.currentTileSprite = tile.tileSprites[GetProceduralTile(tile, j, id)];
            }//-- Procedural
            else
            {
                //tile.currentTileSprite = tile.tileSprites[Random.Range(0, tile.tileSprites.Count)];
            }*/



            posX += 0.5f;
            posY += 0.25f;

            tileCounter++;

            totalTileCreationProgress += individualTileCreationProgress;
            EditorUtility.DisplayProgressBar("Creando mapa", "Celda: " + tileCounter + "/" + cantidadCeldas, totalTileCreationProgress);

            yield return new WaitForFixedUpdate();
            //yield return new WaitForSeconds(time);
        }
        if (tileCounter == cantidadCeldas)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Mapa creado", "El mapa ha sido creado exitosamente", "Continuar");
        }

        yield break;
    }

    public void DrawTile(Vector3Int coordinate)
    {
        if (tileToDraw != null && currentMap != null)
        {

            Tilemap tilemap = this.selectedTilemap;
            Map map = this.currentMap;

            if (/*!map.mapMatrix.CheckTileExist(coordinate)*/!map.mapMatrix.CheckIfObjectExists((Vector2Int)coordinate, GameObjectType.Tile))
            {
                Vector3 localPosition = tilemap.GetCellCenterLocal(coordinate);
                localPosition.z = localPosition.y;

                GameObject tileToDraw = this.tileToDraw;

                GameObject auxObject = Instantiate(tileToDraw, tilemap.gameObject.transform);
                auxObject.transform.localPosition = localPosition;

                Tile auxTile = auxObject.GetComponent<Tile>();
                auxTile.coordinates = coordinate;


                if (proceduralMap)
                {
                    auxTile.currentTileSprite = auxTile.tileSprites[GetProceduralTile(auxTile, coordinate.x, coordinate.y)];
                }
                else
                {
                    auxTile.currentTileSprite = auxTile.tileSprites[Random.Range(0, auxTile.tileSprites.Count)];
                }

                //auxTile.currentTileSprite = auxTile.tileSprites[Random.Range(0, auxTile.tileSprites.Count)];


                auxTile.spriteRenderer.sprite = auxTile.currentTileSprite;

                //map.tileMatrix.SetTileAt(coordinate, auxTile);
                map.mapMatrix.SetObjectAt((Vector2Int)coordinate, GameObjectType.Tile, auxTile.gameObject);
            }
        }
    }




    IEnumerator CreateTiles()
    {
        GameObject auxMapContainer = Instantiate(this.mapContainerPrefab);

        this.currentMapContainer = auxMapContainer;
        this.currentMap = this.currentMapContainer.GetComponent<Map>();
        this.currentMapContainer.name = this.mapName;
        this.currentMap.rows = this.rows;
        this.currentMap.columns = this.columns;
        GameObject auxTilesContainer = this.currentMap.Floor.gameObject;

        auxTilesContainer.transform.localPosition = new Vector3(0, 0, 100);

        this.selectedTilemap = this.currentMap.Floor;

        // TileMatrix auxTileMatrix = ScriptableObject.CreateInstance<TileMatrix>();

        // this.currentMap.tileMatrix = auxTileMatrix;


        if (this.rows > 0 && this.columns > 0)
        {
            this.tileCounter = 0;
            this.cantidadCeldas = (this.rows * this.columns);
            this.individualTileCreationProgress = 1f / this.cantidadCeldas;
            this.totalTileCreationProgress = 0f;
            EditorUtility.DisplayProgressBar("Creando mapa", "Celda: 0/" + this.cantidadCeldas, 0f);

            for (int i = 0; i < this.rows; i++)
            {
                StartCoroutine(CreateTileRow(i, this.columns, auxTilesContainer));
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Mapa creado", "El mapa ha sido creado exitosamente", "Continuar");
        }

        



        


        yield break;
    }


    private int GetProceduralTile(Tile tile, int x, int y)
    {
        int weight = 0;
        for (int i = 1; i <= tile.tileSprites.Count; i++)
        {
            weight += i;
        }


        float proceduralValue = Mathf.PerlinNoise(x * this.perlinNoiseScale, y * this.perlinNoiseScale);

        float auxProb = 1f / (float)weight;
        float auxProbSum = 0f;
        for (int i = 1; i <= tile.tileSprites.Count; i++)
        {
            auxProbSum += ((float)i * auxProb);
            if (proceduralValue <= auxProbSum)
            {
                return i - 1;
            }
        }
        return 0;
    }


    public void CreateNewMapPrefab()
    {
        if (this.currentMapContainer)
        {
            string localPath = "Assets/Prefabs/Map/" + this.mapName + ".prefab";

            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            //GameObject generatedPrefab = PrefabUtility.SaveAsPrefabAsset(currentMapContainer, localPath);
            GameObject generatedPrefab = PrefabUtility.SaveAsPrefabAsset(currentMapContainer, localPath);


            this.mapName = generatedPrefab.name;
            Debug.Log("Mapa creado correctamente");
            Debug.Log("Ruta: " + localPath);

            EditorUtility.DisplayDialog("Acción exitosa", "El prefab del mapa actual \n ha sido creado correctamente", "Continuar");
        }
        else
        {
            EditorUtility.DisplayDialog("Alerta", "Ningún mapa ha sido seleccionado", "Continuar");
        }
    }

    public void SaveMapChanges()
    {
        if (this.currentMapContainer)
        {
            string localPath = "Assets/Prefabs/Map/" + this.mapName + ".prefab";
            GameObject generatedPrefab = PrefabUtility.SaveAsPrefabAsset(currentMapContainer, localPath);
            this.mapName = generatedPrefab.name;
            Debug.Log("Mapa guardado correctamente");

            EditorUtility.DisplayDialog("Acción exitosa", "El los cambios en el prefab del mapa \n actual han sido guardados correctamente", "Continuar");

        }
        else
        {
            EditorUtility.DisplayDialog("Alerta", "Ningún mapa ha sido seleccionado", "Continuar");
        }
    }


    public void OptimizeGame()
    {
        if (optimizeCoroutine != null)
        {
            StopCoroutine(optimizeCoroutine);
        }
        StartCoroutine(Optimize());
    }

    IEnumerator Optimize()
    {
        if (this.currentMapContainer)
        {
            EditorUtility.DisplayProgressBar("Optimizando", "Optimizando mapa", 0f);
            float childCount = this.currentMap.Floor.transform.childCount;
            float individualProgress = 1f / childCount;
            float totalProgress = 0f;
            for (int i = 0; i < this.currentMap.Floor.transform.childCount; i++)
            {
                Tile auxTile = this.currentMap.Floor.transform.GetChild(i).gameObject.GetComponent<Tile>();
                switch (auxTile.tileState)
                {
                    case TileState.Enabled:
                        break;

                    case TileState.Disabled:
                        break;

                    case TileState.Hidden:
                        auxTile.spriteRenderer.sprite = null;
                        auxTile.spriteRenderer.enabled = false;
                        break;
                }

                totalProgress += individualProgress;
                EditorUtility.DisplayProgressBar("Optimizando", "Optimizando mapa", totalProgress);
                yield return new WaitForFixedUpdate();
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Acción exitosa", "El mapa ha sido optimizado correctamente", "Continuar");
        }
        else
        {
            EditorUtility.DisplayDialog("Alerta", "Ningún mapa ha sido seleccionado", "Continuar");
        }

        yield break;
    }

    public void DeoptimizeGame()
    {
        if (deoptimizeCoroutine != null)
        {
            StopCoroutine(deoptimizeCoroutine);
        }
        StartCoroutine(Deoptimize());
    }

    IEnumerator Deoptimize()
    {
        if (this.currentMapContainer)
        {
            float childCount = this.currentMap.Floor.transform.childCount;
            float individualProgress = 1f / childCount;
            float totalProgress = 0f;
            for (int i = 0; i < this.currentMap.Floor.transform.childCount; i++)
            {
                Tile auxTile = this.currentMap.Floor.transform.GetChild(i).gameObject.GetComponent<Tile>();
                switch (auxTile.tileState)
                {
                    case TileState.Enabled:
                        break;
                    case TileState.Disabled:
                        break;
                    case TileState.Hidden:
                        auxTile.spriteRenderer.enabled = true;
                        break;
                }


                totalProgress += individualProgress;
                EditorUtility.DisplayProgressBar("Desoptimizando", "Desoptimizando mapa", totalProgress);
                yield return new WaitForFixedUpdate();
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Acción exitosa", "El mapa ha sido desoptimizado correctamente", "Continuar");
        }
        else
        {
            EditorUtility.DisplayDialog("Alerta", "Ningún mapa ha sido seleccionado", "Continuar");
        }
        yield break;
    }

    public void DeleteCurrentMap()
    {
        if (currentMapContainer)
        {
            bool answer = EditorUtility.DisplayDialog("Confirmación de eliminación de mapa", "Se eliminará el mapa actual \n ¿Desea continuar?", "Continuar", "Cancelar");
            if (answer)
            {
                Destroy(currentMapContainer);
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Alerta", "Ningún mapa ha sido seleccionado", "Continuar");
        }
    }





    public void SetGame()
    {
        if (GameObject.FindGameObjectWithTag("Map"))
        {
            GetMapMatrix(GameObject.FindGameObjectWithTag("Map"));
            //SetCharactersGroups();
            SetObjectsInContainers();
            CalculateMapIsometric();
            CalculateDisabledTiles();
            SetCharactersPositions();
        }
        else
        {
            Debug.Log("No se ha encontrado un mapa");
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



    public void GetMapMatrix(GameObject map)
    {

        if (map.GetComponent<Map>())
        {
            this.mapTileMatrix = new Tile[map.GetComponent<Map>().columns, map.GetComponent<Map>().rows];
            this.currentMap = map.GetComponent<Map>();
            int counter = 0;

            for (int i = 0; i < map.GetComponent<Map>().rows; i++)
            {
                for (int j = 0; j < map.GetComponent<Map>().columns; j++)
                {
                    this.mapTileMatrix[j, i] = map.GetComponent<Map>().Floor.transform.GetChild(counter).GetComponent<Tile>();
                    counter++;
                }
            }

            Debug.Log("Mapa tiene: " + this.mapTileMatrix.Length + " casillas");

        }
        else
        {
            Debug.Log("Mapa no encontrado");
        }

    }

    public void CalculateDisabledTiles()
    {

        for (int i = 0; i < this.currentMap.Environment.transform.childCount; i++)
        {
            foreach (Tile tile in this.mapTileMatrix)
            {
                Vector2 auxWoldObjectPosition = this.currentMap.Environment.transform.GetChild(i).transform.localPosition;
                Vector2 auxTilePosition = tile.gameObject.transform.localPosition;
                if (auxWoldObjectPosition == auxTilePosition)
                {
                    tile.SetTileState(TileState.Disabled);
                }
            }
        }
    }


    public void CalculateMapIsometric()
    {
        for (int i = 0; i < this.currentMap.Environment.transform.childCount; i++)
        {
            Transform childTransform = this.currentMap.Environment.transform.GetChild(i).transform;
            childTransform.localPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y, childTransform.localPosition.y - 0.1f);
        }

        for (int i = 0; i < this.currentMap.NPCs.transform.childCount; i++)
        {
            Transform childTransform = this.currentMap.NPCs.transform.GetChild(i).transform;
            childTransform.localPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y, childTransform.localPosition.y - 0.2f);

        }

        for (int i = 0; i < this.currentMap.Characters.transform.childCount; i++)
        {
            if (this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharactersGroup>())
            {
                foreach (CharController character in this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharactersGroup>().group)
                {
                    character.transform.localPosition = new Vector3(character.transform.localPosition.x, character.transform.localPosition.y, character.transform.localPosition.y - 0.2f);
                }
            }

            if (this.currentMap.Characters.transform.GetChild(i).gameObject.GetComponent<CharController>())
            {
                Transform childTransform = this.currentMap.Characters.transform.GetChild(i).transform;
                childTransform.localPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y, childTransform.localPosition.y - 0.2f);
            }
        }
    }



    public void SetCharactersPositions()
    {
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
        }
    }


    private void OnApplicationQuit()
    {
        EditorUtility.ClearProgressBar();
    }

}
#endif
