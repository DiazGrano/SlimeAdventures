using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class MapMatrix : MonoBehaviour
{
    [SerializeField]
    public Dictionary<Vector2Int, Tile> tileMatrix = new Dictionary<Vector2Int, Tile>();
    public Dictionary<Vector2Int, CharController> charactersMatrix = new Dictionary<Vector2Int, CharController>();
    public Dictionary<Vector2Int, EnvironmentObject> environmentMatrix = new Dictionary<Vector2Int, EnvironmentObject>();
    public Map map;
    public GameManager gameManager;
    

    private void Start()
    {
        this.gameManager = GameManager.sharedInstance;
    }

    public Tile GetTileAt(Vector2Int coordinate)
    {
        this.tileMatrix.TryGetValue(coordinate, out Tile tile);

        if (tile != null)
        {
            return tile;
        }
        return null;
    }

    public CharController GetCharacterAt(Tile tile)
    {
        if (this.charactersMatrix.TryGetValue((Vector2Int)tile.coordinates, out CharController characterController))
        {
            return characterController;
        }
        else
        {
            return null;
        }
    }

    public EnvironmentObject GetEnvironmentObjectAt(Tile tile)
    {
        if (this.environmentMatrix.TryGetValue((Vector2Int)tile.coordinates, out EnvironmentObject gObject))
        {
            return gObject;
        }
        else
        {
            return null;
        }
    }


    public bool IsTileEmpty(Tile tile)
    {
        if (this.GetCharacterAt(tile) || this.GetEnvironmentObjectAt(tile))
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public Tile GetObjectCurrentTile(GameObjectType gObjectType, GameObject gObject)
    {
        if (gObject != null)
        {
            return this.GetTileAt(this.GetObjectPosition(gObjectType, gObject));
        }
        return null;
    }

    public Vector2Int GetObjectPosition(GameObjectType gObjectType, GameObject gObject)
    {
        if (gObject != null)
        {
            switch (gObjectType)
            {
                case GameObjectType.Tile:
                    return (Vector2Int)map.Floor.WorldToCell(gObject.transform.position);

                case GameObjectType.Character:
                    return (Vector2Int)map.Floor.WorldToCell(gObject.transform.position);

                case GameObjectType.Environment:
                    return (Vector2Int)map.Floor.WorldToCell(gObject.transform.position);
            }
        }
        return new Vector2Int(-999999, -999999);
    }


    public bool SetObjectAt(Vector2Int coordinate, GameObjectType gObjectType, GameObject gObject, bool force = false)
    {
        if (gObject)
        {
            if (this.CheckIfObjectExists(coordinate, gObjectType))
            {
                if (force)
                {
                    this.RemoveObjectAt(coordinate, gObjectType);
                }
                else
                {
                    return false;
                }
            }

            switch (gObjectType)
            {
                case GameObjectType.Tile:
                    if (gObject.GetComponent<Tile>())
                    {
                        this.tileMatrix.Add(coordinate, gObject.GetComponent<Tile>());
                        return true;
                    }
                    break;

                case GameObjectType.Character:
                    if (gObject.GetComponent<CharController>())
                    {
                        this.charactersMatrix.Add(coordinate, gObject.GetComponent<CharController>());
                        return true;
                    }
                    break;

                case GameObjectType.Environment:
                    if (gObject.GetComponent<EnvironmentObject>())
                    {
                        this.environmentMatrix.Add(coordinate, gObject.GetComponent<EnvironmentObject>());
                        gObject.GetComponent<EnvironmentObject>().coordinate = coordinate;
                        return true;
                    }
                    else
                    {
                        Debug.Log("El objeto " + gObject.name + " no cuenta con un script de environment object");
                    }
                    break;
            }
        }
        return false;
    }

    public void DestroyObjectAt(Vector2Int coordinate, MatrixElementType elementType = MatrixElementType.Tile)
    {
        Tile tile;
        Debug.Log("i");
        switch (elementType)
        {
            case MatrixElementType.Tile:
                if (this.tileMatrix.ContainsKey(coordinate))
                {
                    tile = GetTileAt(coordinate);
                    Destroy(tile.gameObject);
                    this.tileMatrix.Remove(coordinate);
                    Debug.Log("e");
                }
                break;
            case MatrixElementType.Piece:
                if ((tile = this.GetTileAt(coordinate)) != null)
                {
                    //tile.pieceAtTile = null;
                }
                break;
        }
    }

    public bool RemoveObjectAt(Vector2Int coordinate, GameObjectType gObjectType)
    {
        if (this.CheckIfObjectExists(coordinate, gObjectType))
        {
            switch (gObjectType)
            {
                case GameObjectType.Tile:
                    this.tileMatrix.Remove(coordinate);
                    return true;

                case GameObjectType.Character:
                    this.charactersMatrix.Remove(coordinate);
                    return true;

                case GameObjectType.Environment:
                    this.environmentMatrix.Remove(coordinate);
                    return true;
            }
        }
        return false;
    }


    public bool CheckIfObjectExists(Vector2Int coordinate, GameObjectType gObjectType)
    {
        switch (gObjectType)
        {
            case GameObjectType.Tile:
                return this.tileMatrix.ContainsKey(coordinate);

            case GameObjectType.Character:
                return this.charactersMatrix.ContainsKey(coordinate);

            case GameObjectType.Environment:
                return this.environmentMatrix.ContainsKey(coordinate);

            default:
                return false;
        }
    }

    public void ResetTilesColors(List<Tile> tileList)
    {
        if (tileList != null)
        {
            foreach (Tile tile in tileList)
            {
                if (tile.spriteHandler != null)
                {
                    tile.spriteHandler.ResetToOriginalColor();
                }
                else
                {
                    Debug.Log("No se encontró el sprite handler de " + tile.gameObject.name);
                }
            }
        }
    }

    public void SetTilesColors(List<Tile> tileList, Color color)
    {
        if (tileList != null && color != null)
        {
            foreach (Tile tile in tileList)
            {
                if (!this.IsTileEmpty(tile))
                {
                    continue;
                }
                if (tile.spriteHandler != null)
                {
                    tile.spriteHandler.TemporarilyChangeColor(color);
                }
                else
                {
                    Debug.Log("No se encontró el sprite handler de " + tile.gameObject.name);
                }
            }
        }
    }
}
