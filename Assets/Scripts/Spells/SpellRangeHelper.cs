using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellRangeType
{
    SpellRange,
    SpellAOE
}
public class SpellRangeHelper : MonoBehaviour
{
    public static SpellRangeHelper sharedInstance;

    [SerializeField]
    private Color spellRangeTileColor = Color.blue;
    [SerializeField]
    private Color spellAOETileColor = Color.red;

    private GameManager gManager;
    private List<Tile> spellRangeTiles = new List<Tile>();

    private List<Tile> spellAOETiles = new List<Tile>();

    private void Awake()
    {
        sharedInstance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        this.gManager = GameManager.sharedInstance;
    }

    public List<Tile> ShowRange(Vector3Int currentCoordinate, int minRange, int maxRange, bool show = true, SpellRangeType type = SpellRangeType.SpellRange)
    {

        HideRange(type);

        for (int row = 0; row < maxRange; row++)
        {
            for (int column = 0; column < maxRange; column++)
            {
                if (column >= minRange)
                {
                    Vector3Int coordinate1 = currentCoordinate + new Vector3Int(Mathf.Clamp(column - row, 0, maxRange), row, 0);
                    Tile auxTile = this.gManager.currentMap.mapMatrix.GetTileAt((Vector2Int)coordinate1);
                    SetTile(auxTile, show, type);

                    Vector3Int coordinate3 = currentCoordinate + new Vector3Int(Mathf.Clamp(column - row, 0, maxRange), -row, 0);
                    Tile auxTile3 = this.gManager.currentMap.mapMatrix.GetTileAt((Vector2Int)coordinate3);
                    SetTile(auxTile3, show, type);

                    Vector3Int coordinate2 = currentCoordinate + new Vector3Int(Mathf.Clamp(-column + row, -maxRange, 0), -row, 0);
                    Tile auxTile2 = this.gManager.currentMap.mapMatrix.GetTileAt((Vector2Int)coordinate2);
                    SetTile(auxTile2, show, type);

                    Vector3Int coordinate4 = currentCoordinate + new Vector3Int(Mathf.Clamp(-column + row, -maxRange, 0), row, 0);
                    Tile auxTile4 = this.gManager.currentMap.mapMatrix.GetTileAt((Vector2Int)coordinate4);
                    SetTile(auxTile4, show, type);
                }
            }
        }

        if (type == SpellRangeType.SpellRange)
        {
            return this.spellRangeTiles;
        }
        else if (type == SpellRangeType.SpellAOE)
        {
            return this.spellAOETiles;
        }

        return null;
    }



    public void HideRange(SpellRangeType type = SpellRangeType.SpellRange)
    {
        switch (type)
        {
            case SpellRangeType.SpellRange:

                GameManager.sharedInstance.HideAllHighlightedTiles();

                this.spellRangeTiles.Clear();
                this.spellAOETiles.Clear();

                break;

            case SpellRangeType.SpellAOE:

                foreach (Tile tile in this.spellAOETiles)
                {
                    if (this.spellRangeTiles.Contains(tile))
                    {

                        GameManager.sharedInstance.HighlightTiles(tile, this.spellRangeTileColor);
                    }
                    else
                    {
                        GameManager.sharedInstance.HideHighlightedTiles(tile);
                    }
                }
                this.spellAOETiles.Clear();

                break;
        }
    }

    private void SetTile(Tile tile, bool show, SpellRangeType type = SpellRangeType.SpellRange)
    {

        if (tile != null)
        {
            if (tile.tileState == TileState.Enabled)
            {
                if (type == SpellRangeType.SpellRange)
                {
                    if (/*tile.spriteRenderer.color == Color.white*/ tile.tileState == TileState.Enabled)
                    {
                        if (show)
                        {
                            GameManager.sharedInstance.HighlightTiles(tile, this.spellRangeTileColor);
                        }

                        if (!this.spellRangeTiles.Contains(tile))
                        {
                            this.spellRangeTiles.Add(tile);
                        }
                    }
                }
                else
                {
                    if (/*tile.spriteRenderer.color != Color.gray*/ tile.tileState == TileState.Enabled)
                    {
                        if (show)
                        {
                            GameManager.sharedInstance.HighlightTiles(tile, this.spellAOETileColor);
                        }
                        if (!this.spellAOETiles.Contains(tile))
                        {
                            this.spellAOETiles.Add(tile);
                        }
                        
                    }
                }

            }
        }


    }
}
