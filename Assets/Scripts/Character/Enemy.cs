using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public CharactersGroup currentGroup;
    public CharController characterController;

    private void Awake()
    {
        this.characterController = this.GetComponent<CharController>();
    }
    /*
    private void OnMouseOver()
    {
        // Terminar código
        
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Enemigo seleccionado");

            if (GameManager.sharedInstance.gameState == GameState.Normal)
            {
                if (Player.sharedInstance.MovePlayer(this.GetComponent<CharacterController>().currentTile))
                {
                    FightsManager.sharedInstance.LaunchFight(this.characterController);
                }
                else
                {
                    Debug.Log("No se pudo iniciar pelea, enemigo fuera de alcance.");
                }
            }
            else
            {
                Debug.Log("Acción imposible, pelea en curso");
            }
            
        }
    }*/

    public void GetCurrentTile()
    {/*
        GameObject auxTilesContainer = GameManager.sharedInstance.currentMap.tilesContainer;


        for (int i = 0; i < auxTilesContainer.transform.childCount; i++)
        {
            if (new Vector2(auxTilesContainer.transform.GetChild(i).transform.localPosition.x, auxTilesContainer.transform.GetChild(i).transform.localPosition.y) == new Vector2(this.transform.localPosition.x, this.transform.localPosition.y))
            {
                this.currentTile = auxTilesContainer.transform.GetChild(i).GetComponent<Tile>();
                Debug.Log("Celda actual de enemigo obtenida");
                return;
            }
        }
        Debug.Log("No se encontró celda actual");
        */
    }
    /*
    public void MoveEnemy(Tile target)
    {
        GameManager.sharedInstance.MoveCharacter(this.gameObject, target, this.currentTile, this.gameObject.GetComponent<Stats>().MovementSpeed());
    }*/

}
