using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player sharedInstance;

    public CharController characterController;

    public List<Tile> path;

    public CharController targetCharacter;

    public GameObject highLight;
    private GameObject auxHighLight;





    private void Awake()
    {
        sharedInstance = this;
        this.characterController = this.GetComponent<CharController>();
    }

    private void Start()
    {
       // GameManager.sharedInstance.SetGame();
    }


    public bool MovePlayer(Tile targetTile, GameObject enemyTarget = null, int cost = 0)
    {
        switch (GameManager.sharedInstance.gameState)
        {
            case GameState.Normal:
                
                if (this.characterController.Move(targetTile))
                {
                    if (auxHighLight)
                    {
                        Destroy(auxHighLight);
                    }
                    auxHighLight = Instantiate(highLight, targetTile.gameObject.transform);
                    if (enemyTarget && enemyTarget.GetComponent<Enemy>())
                    {
                        FightsManager.sharedInstance.LaunchFight(enemyTarget.GetComponent<Enemy>().characterController);
                    }
                }

                break;

            case GameState.SettingFight:
                if (targetTile.spriteRenderer.color == Color.blue)
                {
                    this.transform.localPosition = new Vector3(targetTile.gameObject.transform.localPosition.x, targetTile.gameObject.transform.localPosition.y, targetTile.gameObject.transform.localPosition.y - 0.1f);
                    this.characterController.currentTile = targetTile;
                }
                else
                {
                    Debug.Log("Nel padre");
                    return false;
                }
                break;

            case GameState.Fighting:

                this.characterController.Move(targetTile);
                this.characterController.characterStats.CharacterResource(CharacterResourceType.MovementPoints, true, -cost);



                break;

            case GameState.Pause:
                break;
        }
        
        return true;
        
    }



    public void TargetCharacterClicked(CharController targetCharacter)
    {
        switch (GameManager.sharedInstance.gameState)
        {
            case GameState.Normal:
                if (targetCharacter)
                {
                    if (this.characterController.Move(targetCharacter.currentTile, targetCharacter))
                    {
                        if (auxHighLight)
                        {
                            Destroy(auxHighLight);
                        }
                        auxHighLight = Instantiate(highLight, targetCharacter.currentTile.gameObject.transform);
                        this.targetCharacter = targetCharacter;
                    }
                    else
                    {
                        Debug.Log("Objetivo inalcanzable");
                    }
                }
                break;

            case GameState.SettingFight:
                break;

            case GameState.Fighting:
                break;

            case GameState.Pause:
                break;
        }
        
    }

    public void MovementCoroutineFinished()
    {
        if (auxHighLight)
        {
            Destroy(auxHighLight);
        }
        switch (GameManager.sharedInstance.gameState)
        {
            case GameState.Normal:
                if (this.targetCharacter)
                {
                    switch (targetCharacter.characterType)
                    {
                        case CharacterType.Enemy:
                            FightsManager.sharedInstance.LaunchFight(targetCharacter);
                            this.targetCharacter = null;
                            break;
                        case CharacterType.NPC:
                            break;
                    }
                }
                break;

            case GameState.SettingFight:
                break;

            case GameState.Fighting:
                break;

            case GameState.Pause:
                break;
        }
        
    }

}
