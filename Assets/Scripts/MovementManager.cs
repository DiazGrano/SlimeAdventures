using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCoroutine
{
    public CharController character;
    public Coroutine movementCoroutine;
}

public class MovementManager : MonoBehaviour
{
    public static MovementManager sharedInstance;

    private void Awake()
    {
        sharedInstance = this;
    }

    private List<MovementCoroutine> coroutineCharactersMovementList = new List<MovementCoroutine>();

    public bool MoveCharacter(CharController character, Tile targetTile, Tile currentTile, float characterSpeed, CharController targetCharacter = null)
    {
        if (coroutineCharactersMovementList.Count > 0)
        {
            foreach (MovementCoroutine movementCoroutine in coroutineCharactersMovementList.ToArray())
            {
                if (movementCoroutine.character == character)
                {
                    if (movementCoroutine.movementCoroutine != null)
                    {
                        StopCoroutine(movementCoroutine.movementCoroutine);
                    }
                    coroutineCharactersMovementList.Remove(movementCoroutine);
                }
            }
        }
        MovementCoroutine auxMovementCoroutine = new MovementCoroutine();
        auxMovementCoroutine.character = character;
        auxMovementCoroutine.movementCoroutine = StartCoroutine(Move(PathFindingA.sharedInstance.FindPath(targetTile, currentTile, targetCharacter), character, characterSpeed));
        coroutineCharactersMovementList.Add(auxMovementCoroutine);
        return true;
    }

    IEnumerator Move(List<Tile> path, CharController character, float characterSpeed)
    {

        if (path != null)
        {
            character.SetCharacterAnimation(CharacterAnimation.Walking);

            foreach (Tile tile in path.ToArray())
            {
                Vector2 auxPosition = new Vector2(tile.transform.localPosition.x, tile.transform.localPosition.y);

                GameManager.sharedInstance.mapTileMatrix.RemoveObjectAt((Vector2Int)character.currentTile.coordinates, GameObjectType.Character);
                character.currentTile = tile;
                GameManager.sharedInstance.mapTileMatrix.SetObjectAt((Vector2Int)tile.coordinates, GameObjectType.Character, character.gameObject);
                //terminar
                //character.matrixPiece.currentTile = tile;
                /*
                 * 
                 * 
                 * 
                 */


                while (Vector2.Distance(character.gameObject.transform.localPosition, auxPosition) >= 0.15f)
                {

                    Vector2 direction = auxPosition - new Vector2(character.gameObject.transform.localPosition.x, character.gameObject.transform.localPosition.y);
                    character.gameObject.transform.Translate(direction.normalized * characterSpeed * Time.deltaTime);

                    character.gameObject.transform.localPosition = new Vector3(character.gameObject.transform.localPosition.x, character.gameObject.transform.localPosition.y, character.gameObject.transform.localPosition.y);



                    yield return new WaitForFixedUpdate();
                }
                character.gameObject.transform.localPosition = new Vector3(auxPosition.x, auxPosition.y, auxPosition.y - 0.1f);

                continue;
            }

            

            foreach (MovementCoroutine movementCoroutine in coroutineCharactersMovementList.ToArray())
            {
                if (movementCoroutine.character == character)
                {
                    coroutineCharactersMovementList.Remove(movementCoroutine);
                }
            }

            character.SetCharacterAnimation(CharacterAnimation.Idle);


            if (character.characterType == CharacterType.Player)
            {
                if (character.GetComponent<Player>())
                {
                    character.GetComponent<Player>().MovementCoroutineFinished();
                }
            }
        }
        else
        {
            Debug.Log("Corrutina de movimiento no ha podido ser iniciada");
        }
        
        yield break;
    }
}
