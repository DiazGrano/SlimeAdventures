using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonFightState
{
    StartFight,
    CurrentTurn,
    Disabled
}


public class UIFightButton : MonoBehaviour
{
    public ButtonFightState state;
    private Text buttonText;
    private Button button;


    private void Start()
    {
        this.button = this.GetComponent<Button>();
        this.buttonText = this.GetComponentInChildren<Text>();
        SetButtonFightState(ButtonFightState.Disabled);
    }

    public void ButtonPressed()
    {
        switch (state)
        {
            case ButtonFightState.StartFight:
                FightsManager.sharedInstance.StartFight();
                break;
            case ButtonFightState.CurrentTurn:
                SetButtonFightState(ButtonFightState.Disabled);
                TurnsManager.sharedInstance.EndTurn();
                break;
        }
    }
  

    public void SetButtonFightState(ButtonFightState state)
    {
        switch (state)
        {
            case ButtonFightState.StartFight:
                this.button.enabled = true;
                this.buttonText.text = "Iniciar pelea";
                break;
            case ButtonFightState.CurrentTurn:
                this.button.enabled = true;
                this.buttonText.text = "Terminar turno";
                break;
            case ButtonFightState.Disabled:
                this.buttonText.text = "";
                this.button.enabled = false;
                break;
        }
        this.state = state;
    }

}
