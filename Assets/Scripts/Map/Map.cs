using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public int rows;
    public int columns;

    public Tilemap Floor;
    public Tilemap Environment;
    public Tilemap NPCs;
    public Tilemap Characters;
    public GameObject spawnTile;

    [SerializeField]
    public MapMatrix mapMatrix;



    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("SpawnTile") != null)
        {
            this.spawnTile = GameObject.FindGameObjectWithTag("SpawnTile");
        }
        if (this.mapMatrix == null)
        {
            if (this.GetComponent<MapMatrix>())
            {
                this.GetComponent<MapMatrix>();
            }
            else
            {
                Debug.Log("No se encontró la matrix del mapa");
            }
        }
        this.mapMatrix.map = this;
        
    }
    private void Start()
    {
        //this.SetMapMatrix();
    }

    public MapMatrix SetMapMatrix()
    {
        this.SetTiles();
        this.SetEnvironmentObjects();
        this.SetCharacters();

        return this.mapMatrix;
    }
    
    private void SetTiles()
    {
        Debug.Log("Elementos en floor: " + Floor.transform.childCount);
        for (int i = 0; i < Floor.transform.childCount; i++)
        {
            Tile tile = Floor.transform.GetChild(i).gameObject.GetComponent<Tile>();
            if (tile)
            {
                this.mapMatrix.SetObjectAt((Vector2Int)tile.coordinates, GameObjectType.Tile, tile.gameObject);
            }
        }
        Debug.Log("Matrix de tiles cargada con éxito");
    }

    private void SetEnvironmentObjects()
    {
        Debug.Log("Elementos en environment: " + Environment.transform.childCount);
        for (int i = 0; i < Environment.transform.childCount; i++)
        {
            GameObject gObject = Environment.transform.GetChild(i).gameObject;
            if (this.mapMatrix.SetObjectAt((Vector2Int)this.mapMatrix.GetObjectPosition(GameObjectType.Environment, gObject), GameObjectType.Environment, gObject))
            {
                this.mapMatrix.GetTileAt(this.mapMatrix.GetObjectPosition(GameObjectType.Environment, gObject)).SetTileState(TileState.Enabled, Color.gray);
            }
            else
            {
                Debug.Log("No se ha podido asignar la posición del objeto " + gObject.name);
            }
        }
        Debug.Log("Matrix de objetos de entorno cargada con éxito");
    }

    private void SetCharacters()
    {
        int charCount = 0;
        Debug.Log("Grupos: " + Characters.gameObject.transform.childCount);
        for (int i = 0; i < Characters.gameObject.transform.childCount; i++)
        {
            GameObject gObject = Characters.gameObject.transform.GetChild(i).gameObject;
            Debug.Log(gameObject.name);
            if (gObject.GetComponent<CharactersGroup>())
            {
                foreach (CharController character in gObject.GetComponent<CharactersGroup>().group)
                {
                    Debug.Log("Nombre de personaje: " + character.gameObject.name);
                    Debug.Log("Posición de personaje: " + this.mapMatrix.GetObjectPosition(GameObjectType.Character, character.gameObject));
                    this.mapMatrix.SetObjectAt(this.mapMatrix.GetObjectPosition(GameObjectType.Character, character.gameObject), GameObjectType.Character, character.gameObject);
                    character.currentTile = this.mapMatrix.GetTileAt(this.mapMatrix.GetObjectPosition(GameObjectType.Character, character.gameObject));
                    charCount++;
                }
            }
            else
            {
                this.mapMatrix.SetObjectAt(this.mapMatrix.GetObjectPosition(GameObjectType.Character, gObject), GameObjectType.Character, gObject);
                gObject.GetComponent<CharController>().currentTile = this.mapMatrix.GetTileAt(this.mapMatrix.GetObjectPosition(GameObjectType.Character, gObject));
                charCount++;
            }
        }
        Debug.Log("Elementos en Characters: " + charCount);



        Debug.Log("Matrix de personajes cargada con éxito");
    }
}
