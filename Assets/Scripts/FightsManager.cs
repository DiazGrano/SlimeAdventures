using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterBattleResults
{
    public CharController character;
    public int experiencePointsGained;
    public List<GameObject> objectsDropped = new List<GameObject>();
}

public class FightsManager : MonoBehaviour
{
    public static FightsManager sharedInstance;

    public int battleDuration = 0;
    private Coroutine battleDurationCoroutine = null;

    private List<Tile> availableTiles = new List<Tile>();

    public UIFightButton UIFightButton;

    public List<CharController> charactersFighting = new List<CharController>();
    public List<CharController> playerGroup = new List<CharController>();
    public List<CharController> enemiesGroup = new List<CharController>();

    private Player player;
    private GameManager gameManager;
    private BattlefieldManager battlefieldManager;

    public List<Tile> tilesInSpellRange
    {
        get;
        set;
    }

    public List<Tile> tilesInAOE
    {
        get;
        set;
    }

    private void Awake()
    {
        sharedInstance = this;
    }

    private void Start()
    {
        this.gameManager = GameManager.sharedInstance;
        this.player = this.gameManager.currentPlayer;
        this.battlefieldManager = BattlefieldManager.sharedInstance;
    }

    public void LaunchFight(CharController target)
    {
        if (target != null)
        {
            
            Tile auxTile = target.currentGroup.group[Random.Range(0, target.currentGroup.group.Count)].currentTile;


            this.battlefieldManager.GenerateBattlefield(auxTile);
            this.availableTiles = this.battlefieldManager.GenerateStartingTiles();

            this.battlefieldManager.HideExternalObjects(target.currentGroup.gameObject, this.player.characterController.currentGroup.gameObject);

            SetCharactersPosition(target.currentGroup.group, this.player.characterController.currentGroup.group, this.availableTiles);



            // Characters fighting
            this.charactersFighting.Clear();
            enemiesGroup.Clear();
            foreach (CharController character in target.currentGroup.group)
            {
                enemiesGroup.Add(character);
                this.charactersFighting.Add(character);
            }

            playerGroup.Clear();
            foreach (CharController character in this.player.characterController.currentGroup.group)
            {
                playerGroup.Add(character);
                this.charactersFighting.Add(character);
            }

            // Moved to trigger when character reach starting tiles
            //this.battlefieldManager.GenerateBarriers();

            //List<GameObject> fighters = TurnsManager.sharedInstance.CalculateFightersTurns(target.GetComponent<CharacterController>(), GameManager.sharedInstance.currentPlayer.GetComponent<CharacterController>());
            TurnsManager.sharedInstance.CalculateFightersTurns(target, this.player.characterController);

            //FightersTurnsHUDManager.sharedInstance.SetFightersTurnsContainer(fighters);

            UIFightButton.SetButtonFightState(ButtonFightState.StartFight);

            this.gameManager.SetGameState(GameState.SettingFight);
        }
    }

        private void SetCharactersPosition(List<CharController> enemyGroup, List<CharController> playerGroup, List<Tile> avaliableTiles)
        {
            List<Tile> unavaliableTiles = new List<Tile>();
            foreach (CharController enemy in enemyGroup.ToArray())
            {
                foreach (Tile tile in availableTiles)
                {
                    if (tile.spriteRenderer.color == Color.red && !unavaliableTiles.Contains(tile))
                    {
                        unavaliableTiles.Add(tile);
                        enemy.GetComponent<CharController>().Move(tile);
                        break;
                    }
                }
            }

            foreach (CharController player in playerGroup.ToArray())
            {
                foreach (Tile tile in availableTiles)
                {
                    if (tile.spriteRenderer.color == Color.blue && !unavaliableTiles.Contains(tile))
                    {
                        unavaliableTiles.Add(tile);
                        Player.sharedInstance.MovePlayer(tile);
                        break;
                    }
                }
            }
        this.battlefieldManager.GenerateBarriers();
    }
    

    public void StartFight()
    {
        if (this.gameManager.IsSettingFight()/*GameManager.sharedInstance.gameState == GameState.SettingFight*/)
        {
            
            foreach (Tile tile in availableTiles)
            {
                tile.SetTileState(TileState.Enabled, Color.white);
            }
            this.gameManager.SetGameState(GameState.Fighting);
            TurnsManager.sharedInstance.StartingTurn();

            if (this.battleDurationCoroutine != null)
            {
                StopCoroutine(this.battleDurationCoroutine);
                this.battleDurationCoroutine = null;
            }
            // Move to trigger when barriers finish spawn
            //this.battlefieldManager.SetAllColors();
            StartCoroutine(BattleDuration());

        }
    }

    public void EndFightUI()
    {
        EndFight();
    }

    public void EndFight(CharacterType winner = CharacterType.Player)
    {
        if (/*GameManager.sharedInstance.gameState == GameState.SettingFight || GameManager.sharedInstance.gameState == GameState.Fighting*/this.gameManager.IsSettingFightOrFighting())
        {
            this.battlefieldManager.EraseBattlefield();
            TurnsManager.sharedInstance.StopAllCoroutines();
            this.gameManager.SetGameState(GameState.Normal);


            if (this.battleDurationCoroutine != null)
            {
                StopCoroutine(this.battleDurationCoroutine);
                this.battleDurationCoroutine = null;
            }


            List<CharacterBattleResults> winners = new List<CharacterBattleResults>();
            List<CharacterBattleResults> losers = new List<CharacterBattleResults>();

            int totalWinnersLevel = 0;
            int totalLosersLevel = 0;
            switch (winner)
            {
                case CharacterType.Player:

                    foreach (CharController character in enemiesGroup)
                    {
                        totalLosersLevel += character.characterStats.GetCharacterLevel();

                        CharacterBattleResults auxResult = new CharacterBattleResults();

                        auxResult.character = character;
                        auxResult.experiencePointsGained = 0;
                        auxResult.objectsDropped = null;

                        losers.Add(auxResult);
                    }

                    foreach (CharController character in playerGroup)
                    {
                        totalWinnersLevel += character.characterStats.GetCharacterLevel();

                        CharacterBattleResults auxResult = new CharacterBattleResults();

                        auxResult.character = character;

                        winners.Add(auxResult);
                    }

                    foreach (CharacterBattleResults character in winners)
                    {
                        // Experiencia ganada en combate

                        // El promedio de nivel del equipo perdedor, multiplicado por 10, más la multiplicación de 10 por el nivel promedio del equipo perdedor menos el nivel promedio del equipo ganador
                        character.experiencePointsGained = (totalLosersLevel / enemiesGroup.Count)  * 10 + (10 * ((totalLosersLevel / enemiesGroup.Count) - (totalWinnersLevel / playerGroup.Count)));
                        character.objectsDropped = null;
                    }


                    break;
                case CharacterType.Enemy:
                    foreach (CharController character in enemiesGroup)
                    {
                        totalLosersLevel += character.characterStats.GetCharacterLevel();

                        CharacterBattleResults auxResult = new CharacterBattleResults();

                        auxResult.character = character;
                        auxResult.experiencePointsGained = 0;
                        auxResult.objectsDropped = null;

                        losers.Add(auxResult);
                    }

                    foreach (CharController character in playerGroup)
                    {
                        totalWinnersLevel += character.characterStats.GetCharacterLevel();

                        CharacterBattleResults auxResult = new CharacterBattleResults();

                        auxResult.character = character;
                        auxResult.experiencePointsGained = 0;
                        auxResult.objectsDropped = null;

                        winners.Add(auxResult);
                    }



                    break;
            }

            BattleResults.sharedInstance.ShowBattleResults(winners, losers, this.battleDuration);
        }
    }


    IEnumerator BattleDuration()
    {
        this.battleDuration = 0;

        while (true)
        {
            yield return new WaitForSeconds(1);
            this.battleDuration++;
        }
    }

}
