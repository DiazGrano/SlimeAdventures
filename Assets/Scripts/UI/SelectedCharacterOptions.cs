using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacterOptions : MonoBehaviour
{
    public static SelectedCharacterOptions sharedInstance;
    private FightsManager fManager;
    public CharController selectedCharacter;
    [SerializeField]
    private bool showOptions = false;
    public Canvas optionsCanvas;
    public Animator anim;
    [SerializeField]
    private Camera cam;
    private Player player;


    public bool characterOptionsOpen()
    {
        return showOptions;
    }

    private void Awake()
    {
        sharedInstance = this;
    }

    void Start()
    {
        this.fManager = FightsManager.sharedInstance;
        if (this.cam == null)
        {
            this.cam = CameraController.sharedInstance.cam;
        }
        if (this.GetComponent<Canvas>())
        {
            this.optionsCanvas = this.GetComponent<Canvas>();
            this.showOptions = false;
            this.optionsCanvas.enabled = this.showOptions;
        }
        if (this.anim == null)
        {
            if (this.GetComponent<Animator>())
            {
                this.anim = this.GetComponent<Animator>();
            }
            else
            {
                Debug.Log("No se ha encontrado el animator de " + this.gameObject.name);
            }
        }

        if (this.player == null)
        {
            this.player = GameManager.sharedInstance.currentPlayer.GetComponent<Player>();
        }
    }

    public void LaunchFight()
    {

        this.player.TargetCharacterClicked(this.selectedCharacter);
        //this.fManager.LaunchFight(this.selectedCharacter);
        this.ShowSelectedCharacterOptions();
    }
    public void Cancel()
    {
        this.ShowSelectedCharacterOptions();
    }

    public void ForceClose()
    {
        this.anim.SetTrigger("ForceIdle");
        this.showOptions = false;
    }

    public void ShowSelectedCharacterOptions(CharController selectedCharacter = null)
    {
        if (selectedCharacter)
        {
            if (this.showOptions)
            {
                ForceClose();
            }
            Vector2 position = this.cam.WorldToScreenPoint(selectedCharacter.gameObject.transform.position);
            this.showOptions = true;
            this.anim.SetTrigger("Open");

            this.transform.transform.position = position;
        }
        else
        {
            this.showOptions = false;
            this.anim.SetTrigger("Close");
        }
        
        this.selectedCharacter = selectedCharacter;
        //this.optionsCanvas.enabled = this.showOptions;
    }
}
