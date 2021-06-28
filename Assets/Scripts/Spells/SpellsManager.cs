using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsManager : MonoBehaviour
{
    public static SpellsManager sharedInstance;

    public List<SpellData> spellsList = new List<SpellData>();

    public SpellUI selectedSpell;

    private void Awake()
    {
        sharedInstance = this;
    }

    public void SpellSelected(SpellUI spell)
    {
        if (this.selectedSpell != null)
        {
            this.selectedSpell.SetButton();

            this.selectedSpell = null;
        }

        if (spell != null)
        {
            this.selectedSpell = spell;
        }
        else
        {
            this.selectedSpell = null;
        }

    }

    public bool Fighting()
    {
        return GameManager.sharedInstance.gameState == GameState.Fighting;
    }

    public CharController GetCharacterOnTile(Tile tile)
    {
        foreach (CharController character in FightsManager.sharedInstance.charactersFighting)
        {
            if (character.currentTile == tile)
            {
                return character;
            }
        }
        return null;
    }

    private SpellData GetSpell(int id)
    {
        foreach (SpellData spell in spellsList)
        {
            if (spell.spellId == id)
            {
                return spell;
            }
        }
        return null;
    }

    private void Cast(int spellID, Tile tile, CharController caster, CharController targetCharacter = null)
    {
        if (Fighting())
        {
            if (TurnsManager.sharedInstance.IsCharacterTurn(caster))
            {
                SpellData spell = GetSpell(spellID);

                if (spell != null)
                {

                    CharController target;
                    Tile targetTile;

                    if (tile != null)
                    {
                        targetTile = tile;
                    }
                    else if (targetCharacter != null)
                    {
                        target = targetCharacter;
                        targetTile = target.currentTile;
                    }
                    else
                    {
                        Debug.Log("No se ha seleccionado un objetivo");
                        return;
                    }

                    if (spell.spellCost <= caster.characterStats.CharacterResource(CharacterResourceType.ManaPoints))
                    {
                        caster.characterStats.CharacterResource(CharacterResourceType.ManaPoints, true, -spell.spellCost);

                        StartCoroutine(SpellAnimation(spell, targetTile));


                        Debug.Log("Hechizo lanzado");
                    }
                    else
                    {
                        Debug.Log("No hay suficiente mana");
                    }

                    


                }
                else
                {
                    Debug.Log("Hechizo inválido \n ID de hechizo: " + spellID);
                }
            }
            else
            {
                Debug.Log("No es el turno actual del personaje");
            }

        }
        else
        {
            Debug.Log("Estado de juego diferente a peleando");
        }

    }

    IEnumerator SpellAnimation(SpellData spell, Tile tile)
    {
        List<Tile> aoe = FightsManager.sharedInstance.tilesInAOE;
        GameObject auxSpell = Instantiate(spell.spellAnimation);
        auxSpell.transform.localPosition = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.y - 0.2f);

        yield return new WaitForSeconds(auxSpell.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);


        Destroy(auxSpell);

        foreach (Tile auxTile in aoe)
        {
            if (spell.spellTileEffectAnimation != null)
            {
                GameObject auxTileEffect = Instantiate(spell.spellTileEffectAnimation);
                auxTileEffect.transform.localPosition = new Vector3(auxTile.transform.position.x, auxTile.transform.position.y, auxTile.transform.position.y - 0.2f);
            }
            CharController affectedCharacter = GetCharacterOnTile(auxTile);
            if (affectedCharacter != null)
            {
                affectedCharacter.HealthModifier(spell.spellEffectValue);
            }
        }
        yield break;
    }

    public void CastSpell(Tile tile, CharController caster = null, CharController target = null)
    {
        if (caster == null)
        {
            caster = GameManager.sharedInstance.currentPlayer.GetComponent<CharController>();
        }
        if (this.selectedSpell != null)
        {
            this.Cast(this.selectedSpell.spellID, tile, caster, target);
            this.SpellSelected(null);
            //this.selectedSpell.SetButton();
        }
    }




}
