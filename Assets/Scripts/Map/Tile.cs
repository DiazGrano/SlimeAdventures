using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour/*, IPointerDownHandler, IPointerEnterHandler */
{
    public bool enabledTile = true;
    public Tile parent;
    public int gCost; // Costo del nodo inicial al nodo
    public int hCost; // Costo del nodo al nodo final
    public int fCost; // suma de G y H
    
    public TileState tileState = TileState.Enabled;

    public Vector3Int coordinates = new Vector3Int();

    public MatrixPiece pieceAtTile = null;
    
    public List<Sprite> tileSprites = new List<Sprite>();
    public Sprite currentTileSprite;
    public Sprite emptyTileSprite;

    public bool showGrid = false;
    public bool isVisible = false;

    public SpriteRenderer spriteRenderer;
    public LineRenderer grid;

    public bool selfDisabled = false;

    public Color tileColor = Color.white;

    private GameManager gManager;

    private BattlefieldManager bfManager;

    public ObjectSpriteHandler spriteHandler;

    private void Awake()
    {
        if (this.GetComponent<SpriteRenderer>())
        {
            this.spriteRenderer = this.gameObject.transform.GetComponent<SpriteRenderer>();
        }
        else if (this.GetComponentInChildren<SpriteRenderer>())
        {
            this.spriteRenderer = this.gameObject.transform.GetComponentInChildren<SpriteRenderer>();
        }
        else
        {
            Debug.Log("No se encontró renderer");
        }

        

    }

    private void Start()
    {
        this.gManager = GameManager.sharedInstance;
        this.bfManager = BattlefieldManager.sharedInstance;

        if (this.GetComponent<ObjectSpriteHandler>())
        {
            this.spriteHandler = this.GetComponent<ObjectSpriteHandler>();
        }
        else if(this.GetComponentInChildren<ObjectSpriteHandler>())
        {
            this.spriteHandler = this.GetComponentInChildren<ObjectSpriteHandler>();
        }
        else
        {
            Debug.Log("No se ha encontrado el sprite handler");
        }
        

        if (this.grid == null)
        {
            if (this.GetComponentInChildren<LineRenderer>())
            {
                this.grid = this.GetComponentInChildren<LineRenderer>();
            }
            
        }

        if (this.showGrid && this.tileState == TileState.Enabled && this.grid != null)
        {
            this.grid.enabled = true;
        }
        else
        {
            this.grid.enabled = false;
        }
    }

    public void AddTemporalTileColor(Color? color = null)
    {
        if (color.HasValue && color.Value != Color.white)
        {
            Color auxColor;

            float r = Mathf.Abs(this.tileColor.r - color.Value.r);
            float g = Mathf.Abs(this.tileColor.g - color.Value.g);
            float b = Mathf.Abs(this.tileColor.b - color.Value.b);
            float a = Mathf.Abs(this.tileColor.a - color.Value.a);
            auxColor = new Color(r, g, b, a);
            this.spriteRenderer.color = this.tileColor - auxColor;
        }
        else
        {
            this.spriteRenderer.color = this.tileColor;
        }
    }



    public void SetTileState(TileState state = TileState.Enabled, Color? permanentColor = null)
    {
        if (permanentColor.HasValue)
        {
            this.tileColor = permanentColor.Value;
        }


        switch (state)
        {
            case TileState.Enabled:
                this.spriteRenderer.enabled = true;
                this.spriteRenderer.sprite = currentTileSprite;
                break;

            case TileState.Disabled:
                this.spriteRenderer.enabled = true;
                this.spriteRenderer.sprite = currentTileSprite;
                break;

            case TileState.Hidden:
                this.spriteRenderer.sprite = null;
                this.spriteRenderer.enabled = false;
                break;

            case TileState.Empty:
                this.spriteRenderer.enabled = true;
                this.spriteRenderer.sprite = emptyTileSprite;
                break;
        }
        this.spriteRenderer.color = this.tileColor;
        this.tileState = state;

        ShowGrid(this.showGrid);

    }

    public void ShowGrid(bool show)
    {
        if (this.grid != null && this.grid.enabled != show /* && this.isVisible*/)
        {
            this.grid.enabled = show;
        }
        else if ((this.tileState == TileState.Disabled || this.tileState == TileState.Empty) && this.grid.enabled)
        {
            this.grid.enabled = show;
        }
        
    }
    public void SetShowGrid(bool show)
    {
        if (this.grid != null)
        {
            this.showGrid = show;
            if (this.grid.enabled != show && this.isVisible)
            {
                this.grid.enabled = show;
            }
        }
    }

   

    private void OnBecameInvisible()
    {
        if (GameManager.sharedInstance != null)
        {
            this.isVisible = false;
            this.ShowGrid(false);

            if (GameManager.sharedInstance.visibleTiles.Contains(this))
            {
                GameManager.sharedInstance.visibleTiles.Remove(this);
            }
        }
            

    }

    private void OnBecameVisible()
    {

            this.isVisible = true;
            this.ShowGrid(showGrid);
        if (GameManager.sharedInstance != null)
        {
            if (!GameManager.sharedInstance.visibleTiles.Contains(this))
            {
                GameManager.sharedInstance.visibleTiles.Add(this);
            }
            if (this.gManager.IsFighting() /*|| GameManager.sharedInstance.gameState == GameState.SettingFight*/)
            {
                if (!BattlefieldManager.sharedInstance.battlefield.Contains(this) && !this.bfManager.disabledTiles.Contains(this) /*&& this.tileState == TileState.Enabled*/)
                {
                    this.selfDisabled = true;
                    this.SetTileState(TileState.Disabled, Color.gray);
                    BattlefieldManager.sharedInstance.disabledTiles.Add(this);
                }
            }
            else if (this.selfDisabled)
            {
                this.selfDisabled = false;
                this.SetTileState(TileState.Enabled);
            }
        }
        


        


    }





    public void SetFightColor()
    {
        if (this.gManager.IsSettingFightOrFighting())
        {
            if (!this.bfManager.IsInBattlefield(GameObjectType.Tile, this.gameObject))
            {
                StartCoroutine(NotInBattlefieldColorTransition());
            }
        }
    }

    IEnumerator NotInBattlefieldColorTransition()
    {
        float time = 0.5f;
        while (Color.gray != this.spriteRenderer.color)
        {

            this.spriteRenderer.color = Color.Lerp(this.spriteRenderer.color, Color.gray, Time.deltaTime / time);
            time -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        this.selfDisabled = true;
        this.SetTileState(TileState.Disabled, Color.gray);
        if (!this.bfManager.disabledTiles.Contains(this))
        {
            this.bfManager.disabledTiles.Add(this);
        }
        
    }
}
