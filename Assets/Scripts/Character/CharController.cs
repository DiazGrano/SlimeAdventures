using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharController : MonoBehaviour
{
    public CharacterObjectState characterObjectState;
    public CharacterType characterType;
    public CharacterState characterState;
    
    public Stats characterStats;

    public SplashArt splashArt;
    public Tile currentTile;
    //public MatrixPiece matrixPiece;
    public CharactersGroup currentGroup;
    public bool isCharacterTurn;

    public SpriteRenderer sRenderer;
    public Animator anim;
    public FillableBar healthBar;

    public ObjectSpriteHandler spriteHandler;

    private void Awake()
    {
        if (this.sRenderer == null)
        {
            if (this.GetComponent<SpriteRenderer>())
            {
                this.sRenderer = this.GetComponent<SpriteRenderer>();
            }
            else if (this.GetComponentInChildren<SpriteRenderer>())
            {
                this.sRenderer = this.GetComponentInChildren<SpriteRenderer>();
            }
            else
            {
                Debug.Log("No se ha encontrado un sprite renderer");
            }
        }

        if (this.anim == null)
        {
            if (this.GetComponent<Animator>() != null)
            {
                this.anim = this.GetComponent<Animator>();
            }
            else if (this.GetComponentInChildren<Animator>() != null)
            {
                this.anim = this.GetComponentInChildren<Animator>();
            }
            else
            {
                Debug.Log("No se ha encontrado un animator");
            }
        }

        if (this.characterStats == null)
        {
            if (this.GetComponent<Stats>())
            {
                this.characterStats = this.GetComponent<Stats>();
            }
            else
            {
                Debug.Log("No se ha encontrado el script de stats");
            }
        }

        if (!this.spriteHandler)
        {
            if (!(this.spriteHandler = this.GetComponent<ObjectSpriteHandler>()))
            {
                if (!(this.spriteHandler = this.GetComponentInChildren<ObjectSpriteHandler>()))
                {
                    Debug.Log("No se ha encontrado el sprite handler del objeto " + this.gameObject.name);
                }
            }
        }
        
        /*
        if (this.matrixPiece == null)
        {
            if (this.GetComponent<MatrixPiece>())
            {
                this.matrixPiece = this.GetComponent<MatrixPiece>();
            }
        }*/
        

    }

    private void Start()
    {
        if (this.healthBar != null)
        {
            this.healthBar.FillColor = Color.red;
            this.healthBar.SetFillAmount(this.characterStats.CharacterResource(CharacterResourceType.MaxHealthPoints), this.characterStats.CharacterResource(CharacterResourceType.HealthPoints));
        }
        //CheckCharacterState();
        StartCoroutine(HealthRecoveryPerSecond());


    }



    public void SetCharacterAnimation(CharacterAnimation animation)
    {
        if (this.anim != null)
        {
            switch (animation)
            {
                case CharacterAnimation.Idle:
                    this.anim.SetBool("Idle", true);
                    this.anim.SetBool("Walking", false);
                    break;
                case CharacterAnimation.Walking:
                    this.anim.SetBool("Idle", false);
                    this.anim.SetBool("Walking", true);
                    this.anim.SetFloat("Speed", this.characterStats.GetMovementSpeed());
                    break;
                case CharacterAnimation.CastAttack:
                    this.anim.SetTrigger("CastAttack");
                    break;
                case CharacterAnimation.Attack:
                    this.anim.SetTrigger("Attack");
                    break;
                case CharacterAnimation.Injured:
                    this.anim.SetTrigger("Injured");
                    break;
                case CharacterAnimation.Death:
                    this.anim.SetTrigger("Death");
                    break;
            }
        }
    }

    public void SetCharacterObjectState(CharacterObjectState state)
    {
        switch (state)
        {
            case CharacterObjectState.Enabled:
                this.healthBar.ShowBar(GameManager.sharedInstance.showHealthBars);
                this.sRenderer.enabled = true;
                break;
            case CharacterObjectState.Disabled:
                this.healthBar.ShowBar(false);
                this.sRenderer.enabled = false;
                break;
        }
        this.characterObjectState = state;
    }

    public void SetCharacterState(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Alive:
                this.healthBar.ShowBar(GameManager.sharedInstance.showHealthBars);
                this.sRenderer.enabled = true;
                break;
            /*
            case CharacterState.Enabled:
                this.healthBar.gameObject.SetActive(true);
                this.sRenderer.enabled = true;
                break;
            case CharacterState.Disabled:
                this.healthBar.gameObject.SetActive(false);
                this.sRenderer.enabled = false;
                break;*/
            case CharacterState.Dead:
                this.healthBar.ShowBar(false);
                this.sRenderer.enabled = false;
                break;
        }

        this.characterState = state;
    }


    public void GetCurrentTile()
    {
        GameObject auxTilesContainer = GameManager.sharedInstance.currentMap.Floor.gameObject;

        for (int i = 0; i < auxTilesContainer.transform.childCount; i++)
        {
            if (new Vector2(auxTilesContainer.transform.GetChild(i).transform.localPosition.x, auxTilesContainer.transform.GetChild(i).transform.localPosition.y) == new Vector2(this.transform.localPosition.x, this.transform.localPosition.y))
            {
                this.currentTile = auxTilesContainer.transform.GetChild(i).GetComponent<Tile>();
                return;
            }
        }
        Debug.Log("No se encontró celda actual de personaje: " + this.gameObject.name);
    }

    


    public bool Move(Tile targetTile, CharController targetCharacter = null)
    {
        CheckCharacterState();
        if (this.characterState == CharacterState.Alive)
        {
            if (!MovementManager.sharedInstance.MoveCharacter(this, targetTile, this.currentTile, this.gameObject.GetComponent<Stats>().GetMovementSpeed(), targetCharacter))
            {
                Debug.Log("No se ha podido iniciar rutina de movimiento para: " + this.gameObject.name);
                return false;
            }
            return true;
        }
        return false;


        
    }

    private void CheckCharacterState()
    {
        if (GameManager.sharedInstance.currentMap == null || this.currentTile == null || this.currentGroup == null)
        {
            //GameManager.sharedInstance.SetGame();
            
        }
        if (this.currentTile == null)
        {
            GameManager.sharedInstance.mapTileMatrix.SetObjectAt(GameManager.sharedInstance.mapTileMatrix.GetObjectPosition(GameObjectType.Character, this.gameObject), GameObjectType.Character, this.gameObject);
            this.currentTile = GameManager.sharedInstance.mapTileMatrix.GetTileAt(GameManager.sharedInstance.mapTileMatrix.GetObjectPosition(GameObjectType.Character, this.gameObject));
            Debug.Log("Player position: " + GameManager.sharedInstance.mapTileMatrix.GetObjectPosition(GameObjectType.Character, this.gameObject));
            Debug.Log("Tile en posición del player: " + GameManager.sharedInstance.mapTileMatrix.GetTileAt(GameManager.sharedInstance.mapTileMatrix.GetObjectPosition(GameObjectType.Character, this.gameObject)));
        }
        if (this.currentGroup == null)
        {
            this.gameObject.GetComponentInParent<CharactersGroup>().SetGroup();
        }
    }

    IEnumerator HealthRecoveryPerSecond()
    {
        while (this.characterState == CharacterState.Alive)
        {
            if (GameManager.sharedInstance.gameState == GameState.Normal)
            {
                this.HealthModifier(this.characterStats.CharacterResource(CharacterResourceType.HealthRecoveryPerSecond));
            }
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }

    public void CharacterResources(int amount, CharacterResourceType resource)
    {
        this.characterStats.CharacterResource(resource, true, amount);
    }

    public void HealthModifier(int healthAmount)
    {
        if (this.characterState == CharacterState.Alive)
        {
            this.characterStats.CharacterResource(CharacterResourceType.HealthPoints, true, healthAmount);
            this.healthBar.SetFillAmount(this.characterStats.CharacterResource(CharacterResourceType.MaxHealthPoints), this.characterStats.CharacterResource(CharacterResourceType.HealthPoints));

            if (GameManager.sharedInstance.gameState == GameState.Fighting)
            {
                DamageHUD.sharedInstance.ShowDamage(healthAmount, this.currentTile);
            }
            
            if (this.characterStats.CharacterResource(CharacterResourceType.HealthPoints) <= 0)
            {
                Debug.Log("Muerte");
                SetCharacterState(CharacterState.Dead);
                TurnsManager.sharedInstance.CharacterDeath(this);
            }
        }
        
    }

    private void OnBecameVisible()
    {
        Debug.Log("e");
    }
}
