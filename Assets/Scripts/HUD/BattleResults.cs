using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleResults : MonoBehaviour
{
    public static BattleResults sharedInstance;

    public Canvas battleResultsCanvas;

    public GameObject winnersContainer;
    public GameObject losersContainer;
    public GameObject characterContainerPrefab;

    public TextMeshProUGUI battleResultText;
    public TextMeshProUGUI battleDurationText;



    private void Awake()
    {
        sharedInstance = this;
    }

    private void Start()
    {
        if (this.battleResultsCanvas.isActiveAndEnabled)
        {
            this.battleResultsCanvas.enabled = false;
        }
    }

    private void ClearContainer(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    public void ShowBattleResults(List<CharacterBattleResults> winners, List<CharacterBattleResults> losers, int battleDuration, bool victory = true)
    {
        ClearContainer(this.winnersContainer.transform);
        ClearContainer(this.losersContainer.transform);

        foreach (CharacterBattleResults winner in winners)
        {
            HUDCharBattleResults auxCharacterResults = Instantiate(this.characterContainerPrefab, winnersContainer.transform).GetComponent<HUDCharBattleResults>();
            auxCharacterResults.characterNameText.text = winner.character.gameObject.name;
            auxCharacterResults.experienceGainedText.text = winner.experiencePointsGained.ToString();
            if (winner.objectsDropped != null)
            {
                // completar código
            }
            
        }

        foreach (CharacterBattleResults loser in losers)
        {
            HUDCharBattleResults auxCharacterResults = Instantiate(this.characterContainerPrefab, losersContainer.transform).GetComponent<HUDCharBattleResults>();
            auxCharacterResults.characterNameText.text = loser.character.gameObject.name;
            auxCharacterResults.experienceGainedText.text = loser.experiencePointsGained.ToString();
        }

        this.battleDurationText.text = FormatTime(battleDuration);

        if (victory)
        {
            this.battleResultText.text = "¡Victoria!";
        }
        else
        {
            this.battleResultText.text = "¡Derrota!";
        }




        RayCasterState(this.battleResultsCanvas.gameObject, true);
        this.battleResultsCanvas.enabled = true;
    }

    private string FormatTime(int seconds)
    {
        int auxMinutes = Mathf.FloorToInt((float)seconds / 60f);
        int auxSeconds = seconds - auxMinutes * 60;
        return string.Format("{0:00}", auxMinutes) + " : " + string.Format("{0:00}", auxSeconds);
    }

    private void RayCasterState(GameObject parent, bool state)
    {

        if (parent.GetComponent<GraphicRaycaster>() != null)
        {
            parent.GetComponent<GraphicRaycaster>().enabled = state;
        }

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            RayCasterState(parent.transform.GetChild(i).gameObject, state);
        }

    }

}
