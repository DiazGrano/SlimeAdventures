using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingA : MonoBehaviour
{
    public static PathFindingA sharedInstance;
    public bool diagonalEnabled = true;
    private List<Tile> openTiles = new List<Tile>();
    private List<Tile> closedTiles = new List<Tile>();
    private Tile current;
    private Tile goal;
    private Tile start;

    public int maxSteps = 100;
    public int currentSteps = 0;

    public Color pathColor;


    private GameManager gameManager;

    private List<Tile> path = new List<Tile>();

    private void Awake()
    {
        sharedInstance = this;
    }

    private void Start()
    {
        this.gameManager = GameManager.sharedInstance;
    }

    public List<Tile> FindPath(Tile goal, Tile start, CharController target = null, bool showPath = false)
    {
        this.gameManager = GameManager.sharedInstance;
        this.currentSteps = 0;

        if (GameManager.sharedInstance.currentMap == null)
        {
            GameManager.sharedInstance.GetMapMatrix(goal.gameObject.transform.parent.transform.parent.gameObject);
        }

        if (GameManager.sharedInstance.gameState == GameState.Normal)
        {
            //this.diagonalEnabled = true;
        }
        else
        {
            this.diagonalEnabled = false;
        }

        ShowPath(false);

        path.Clear();
        openTiles.Clear();
        closedTiles.Clear();
        this.goal = goal;
        this.start = start;

        openTiles.Add(this.start);

        while (openTiles.Count > 0)
        {
            current = FindLowestF(openTiles);
            openTiles.Remove(current);
            closedTiles.Add(current);

            if (target != null)
            {
                if (current == this.gameManager.mapTileMatrix.GetObjectCurrentTile(GameObjectType.Character, target.gameObject))
                {
                    path = FindParentPath(goal, start);
                    return path;
                }
            }
            if (current == goal)
            {
                path = FindParentPath(goal, start);

                if (showPath)
                {
                    ShowPath(showPath);
                }

                return path;
            }

            List<Tile> adjacents = FindAdjacents(current);

            foreach (Tile adjacent in adjacents)
            {
                // Si la casilla adyacente está deshabilitada, oculta, en la lista de casillas cerradas o si tiene una pieza sobre ella mientras se está en combate, entonces la casilla es ignorada
                if (adjacent.tileState == TileState.Disabled  || adjacent.tileState == TileState.Hidden || InList(closedTiles, adjacent) || !this.gameManager.mapTileMatrix.IsTileEmpty(adjacent) /*this.gameManager.mapTileMatrix.GetEnvironmentObjectAt(adjacent)*/ /*|| (GameManager.sharedInstance.gameState == GameState.Fighting && adjacent.pieceAtTile != null)*/)
                {
                    if (target != null && this.gameManager.mapTileMatrix.GetCharacterAt(adjacent) == target)
                    {
                    }
                    else
                    {
                        continue;
                    }
                    
                    
                }

                if (!InList(openTiles, adjacent))
                {
                    CalculateParameters(adjacent);
                    adjacent.parent = current;


                    if (!InList(openTiles, adjacent))
                    {
                        openTiles.Add(adjacent);
                    }

                }

            }

            this.currentSteps++;

            if (this.currentSteps >= this.maxSteps)
            {
                Debug.Log("Cantidad máxima de pasos alcanzada");
                break;
            }




        }
        Debug.Log("Destino inalcanzable");
        return null;
    }






    private List<Tile> FindAdjacents(Tile current)
    {
        List<Tile> auxList = new List<Tile>();

        int currentTileX = current.coordinates.x;
        int currentTileY = current.coordinates.y;
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (!diagonalEnabled)
                {
                    if ((x == -1 && y == 1) || (x == -1 && y == -1) || (x == 1 && y == 1) || (x == 1 && y == -1))
                    {
                        continue;
                    }
                }

                Tile auxTile = this.gameManager.mapTileMatrix.GetTileAt((Vector2Int)new Vector3Int(x + currentTileX, y + currentTileY, current.coordinates.z));
                if (auxTile != null)
                {
                    auxList.Add(auxTile);
                }
            }
        }

        return auxList;
    }


    private bool InList(List<Tile> list, Tile tile)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == tile)
            {
                return true;
            }
        }

        return false;
    }

    private Tile FindLowestF(List<Tile> list)
    {
        Tile lowestF;
        lowestF = list[0];
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].fCost < lowestF.fCost || list[i].fCost == lowestF.fCost && list[i].hCost < lowestF.hCost)
            {
                lowestF = list[i];
            }
        }

        return lowestF;
    }

    private void CalculateParameters(Tile tile)
    {

            int auxXG = (int)Mathf.Abs(tile.coordinates.x - start.coordinates.x);
            int auxYG = (int)Mathf.Abs(tile.coordinates.y - start.coordinates.y);
            int auxXH = (int)Mathf.Abs(tile.coordinates.x - goal.coordinates.x);
            int auxYH = (int)Mathf.Abs(tile.coordinates.y - goal.coordinates.y);
            int auxG = 0;
            int auxH = 0;

            for (int y = 0; y < auxYG; y++)
            {
                auxG++;
            }
            for (int x = 0; x < auxXG; x++)
            {
                auxG++;
            }

            for (int y = 0; y < auxYH; y++)
            {
                auxH++;
            }
            for (int x = 0; x < auxXH; x++)
            {
                auxH++;
            }

            tile.gCost = auxG;
            tile.hCost = auxH;
            tile.fCost = auxG + auxH;
    }


    private List<Tile> FindParentPath(Tile goal, Tile start)
    {
        List<Tile> path = new List<Tile>();
        Tile current = goal;
        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Add(start);
        path.Reverse();

        return path;
    }

    public void ShowPath(bool show)
    {
        if (show)
        {
            
            foreach (Tile tile in this.path)
            {
                if (tile != this.start)
                {
                    GameManager.sharedInstance.HighlightTiles(tile, this.pathColor);
                }
            }
        }
        else
        {

            GameManager.sharedInstance.HideAllHighlightedTiles();
        }

    }


}
