using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnsManager : MonoBehaviour
{
    public static TurnsManager sharedInstance;

    public GameObject highLight;

    public float maxTurnDuration = 10f;

    public List<CharacterHolder> enemiesGroup = new List<CharacterHolder>();
    public List<CharacterHolder> playersGroup = new List<CharacterHolder>();

    List<CharController> charactersTurns = new List<CharController>();

    public UIFightButton UIFightButton;

    [Header("Characters turns HUD")]
    public Canvas background;
    public GameObject container;
    public GameObject characterHolder;



    [Header("Current character turn")]
    public CharController currentCharacterTurn;
    //public float currentCharacterTurnTime;
    public int nextCharacter = 0;
    private Coroutine auxTurnCoroutine;

    GameObject auxHighLight;
    CharacterHolder holder;

    public ResourceCostOptions resourceCostOptions;

    private void Awake()
    {
        sharedInstance = this;
        this.background.enabled = false;
    }

    private void Start()
    {
        resourceCostOptions = GameManager.sharedInstance.uiManager.resourceCostOptions;
        Debug.Log(resourceCostOptions);
    }

    public void StartingTurn()
    {
        if (GameManager.sharedInstance.gameState == GameState.Fighting)
        {
            currentCharacterTurn = charactersTurns[0];
            SetTurn();
        }
    }

    public void EndTurn()
    {
        if (GameManager.sharedInstance.gameState == GameState.Fighting)
        {


            if (currentCharacterTurn.tag == "Player")
            {
                //ResourceCostOptions.sharedInstance.ShowSpellCastOptions( CharacterResourceType.ManaPoints, 0, null, false);
                resourceCostOptions.ShowSpellCastOptions(CharacterResourceType.ManaPoints, 0, null, false);
                SpellsManager.sharedInstance.SpellSelected(null);
                PathFindingA.sharedInstance.ShowPath(false);
            }
            if (currentCharacterTurn)
            {
                currentCharacterTurn.isCharacterTurn = false;
                currentCharacterTurn.characterStats.CharacterResource(CharacterResourceType.MovementPoints, true, 100);
                currentCharacterTurn.characterStats.CharacterResource(CharacterResourceType.ManaPoints, true, 100);
            }



            //FightersTurnsHUDManager.sharedInstance.NextFighterTurn();
            NextCharacterTurn();

            if (holder != null && currentCharacterTurn.characterState == CharacterState.Alive)
            {
                holder.fill.fillAmount = 0f;
                holder.SetColor(true);

            }


            while (currentCharacterTurn.GetComponent<CharController>().characterState != CharacterState.Alive)
            {
                EndTurn();
            }

            // Terminar código para mostrar a personaje muerto



            SetTurn();
        }
    }


    public void CharacterDeath(CharController deadCharacter)
    {
        if (currentCharacterTurn == deadCharacter)
        {
            EndTurn();
        }

        switch (deadCharacter.characterType)
        {
            case CharacterType.Player:
                foreach (CharacterHolder charHolder in playersGroup)
                {
                    if (charHolder.character == deadCharacter)
                    {
                        charHolder.SetColor(false);
                    }
                }

                foreach (CharacterHolder character in playersGroup)
                {
                    if (character.character.characterState == CharacterState.Alive)
                    {
                        return;
                    }
                }

                FightsManager.sharedInstance.EndFight(CharacterType.Enemy);

                break;
            case CharacterType.Enemy:
                foreach (CharacterHolder charHolder in enemiesGroup)
                {
                    if (charHolder.character == deadCharacter)
                    {
                        charHolder.SetColor(false);
                    }
                }

                foreach (CharacterHolder character in enemiesGroup)
                {
                    if (character.character.characterState == CharacterState.Alive)
                    {
                        return;
                    }
                }

                FightsManager.sharedInstance.EndFight(CharacterType.Player);

                break;
            case CharacterType.NPC:
                break;
        }

        foreach (CharacterHolder charHolder in playersGroup)
        {
            charHolder.character.SetCharacterState(CharacterState.Alive);
            charHolder.character.characterStats.CharacterResource(CharacterResourceType.ManaPoints, true, 100);
            charHolder.character.characterStats.CharacterResource(CharacterResourceType.MovementPoints, true, 100);
            //charHolder.character.characterStats.CharacterResource(CharacterResourceType.HealthPoints, true, int.MaxValue);
        }
        foreach (CharacterHolder charHolder in enemiesGroup)
        {
            charHolder.character.SetCharacterState(CharacterState.Alive);
            charHolder.character.characterStats.CharacterResource(CharacterResourceType.ManaPoints, true, 100);
            charHolder.character.characterStats.CharacterResource(CharacterResourceType.MovementPoints, true, 100);
            charHolder.character.characterStats.CharacterResource(CharacterResourceType.HealthPoints, true, int.MaxValue);
            
        }

    }

    public void SetTurn()
    {


        if (GameManager.sharedInstance.gameState == GameState.Fighting)
        {

            //currentCharacterTurnTime = maxTurnDuration;

            currentCharacterTurn.GetComponent<CharController>().isCharacterTurn = true;

            if (auxHighLight)
            {
                Destroy(auxHighLight);
            }

            auxHighLight = Instantiate(highLight, currentCharacterTurn.GetComponent<CharController>().currentTile.gameObject.transform);

            if (auxTurnCoroutine != null)
            {
                StopCoroutine(auxTurnCoroutine);
                auxTurnCoroutine = null;
            }

            auxTurnCoroutine = StartCoroutine(TurnTimer());


            if (currentCharacterTurn.GetComponent<CharController>().characterType == CharacterType.Player)
            {
                UIFightButton.SetButtonFightState(ButtonFightState.CurrentTurn);
            }
            else
            {
                UIFightButton.SetButtonFightState(ButtonFightState.Disabled);
            }

            //UIFightButton.SetButtonFightState(ButtonFightState.CurrentTurn);


        }

    }


    IEnumerator TurnTimer()
    {
        holder = this.container.transform.GetChild(0).GetComponent<CharacterHolder>();
        holder.fill.fillAmount = 0f;
        float currentCharacterTurnTime = 0f;
        while (currentCharacterTurnTime < this.maxTurnDuration)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            currentCharacterTurnTime += Time.deltaTime;
            holder.fill.fillAmount = currentCharacterTurnTime / this.maxTurnDuration;
            //remainingTurnTimeText.text = currentCharacterTurnTime.ToString();
        }
        holder.fill.fillAmount = 0f;
        EndTurn();
        yield break;
    }

    public bool IsCharacterTurn(CharController character)
    {
        return currentCharacterTurn == character;
    }

    private void SetFightersTurnsContainer(List<CharController> characters)
    {
        enemiesGroup.Clear();
        playersGroup.Clear();

        for (int i = 0; i < this.container.transform.childCount; i++)
        {
            Destroy(this.container.transform.GetChild(i).gameObject);
        }

        foreach (CharController fighter in characters)
        {
            CharacterHolder characterHolder = Instantiate(this.characterHolder, this.container.transform).GetComponent<CharacterHolder>();
            //auxFighter.GetComponent<Image>().sprite = fighter.GetComponent<SplashArt>().sprite;
            characterHolder.characterSprite.sprite = fighter.gameObject.GetComponent<SplashArt>().sRenderer.sprite;
            characterHolder.characterSprite.color = fighter.gameObject.GetComponent<SplashArt>().sRenderer.color;
            characterHolder.character = fighter;
            switch (fighter.characterType)
            {
                case CharacterType.Player:
                    playersGroup.Add(characterHolder);
                    break;
                case CharacterType.Enemy:
                    enemiesGroup.Add(characterHolder);
                    break;
                case CharacterType.NPC:
                    break;
            }
        }
    }

    public void NextCharacterTurn()
    {
        if (nextCharacter >= charactersTurns.Count - 1)
        {
            nextCharacter = 0;
        }
        else
        {
            nextCharacter++;
        }
        container.transform.GetChild(0).gameObject.transform.SetAsLastSibling();
        currentCharacterTurn = charactersTurns[nextCharacter];

    }

    public void ShowHUD(bool show = true)
    {
        this.background.enabled = show;
    }


    public void /*List<GameObject>*/ CalculateFightersTurns(CharController target, CharController player)
    {
        nextCharacter = 0;
        charactersTurns = new List<CharController>();

        List<CharController> enemies = new List<CharController>();
        List<CharController> players = new List<CharController>();

        foreach (CharController auxEnemy in target.currentGroup.group.ToArray())
        {
            enemies.Add(auxEnemy);
        }
        foreach (CharController auxPlayer in player.currentGroup.group.ToArray())
        {
            players.Add(auxPlayer);
        }

        enemies = SortTurnsList(enemies);
        players = SortTurnsList(players);

        bool alternate;

        if (players[0].GetComponent<Stats>().GetInitiative() >= enemies[0].GetComponent<Stats>().GetInitiative())
        {
            alternate = false;
        }
        else
        {
            alternate = true;
        }

        int auxEnemyCounter = 0;
        int auxPlayerCounter = 0;

        int auxCounter = (players.Count + enemies.Count);

        for (int i = 0; i < auxCounter; i++)
        {
            if (alternate)
            {
                if (auxEnemyCounter < enemies.Count)
                {
                    charactersTurns.Add(enemies[auxEnemyCounter]);
                    auxEnemyCounter++;
                }
                else
                {
                    auxCounter++;
                }
                alternate = !alternate;
            }
            else
            {
                if (auxPlayerCounter < players.Count)
                {
                    charactersTurns.Add(players[auxPlayerCounter]);
                    auxPlayerCounter++;
                }
                else
                {
                    auxCounter++;
                }
                alternate = !alternate;
            }
        }

        SetFightersTurnsContainer(charactersTurns);
        //return charactersTurns;


    }

    private List<CharController> SortTurnsList(List<CharController> list)
    {
        int n = list.Count;
        bool cambio = true;

        while (cambio)
        {
            cambio = false;
            for (int i = 0; i < n; i++)
            {
                if (i < list.Count - 1)
                {
                    if (list[i].characterStats.GetInitiative() < list[i + 1].characterStats.GetInitiative())
                    {
                        cambio = true;
                        CharController auxObject = list[i + 1];
                        list[i + 1] = list[i];
                        list[i] = auxObject;
                    }
                }
            }
        }

        return list;
    }
}
