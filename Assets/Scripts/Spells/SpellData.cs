using UnityEngine;

[CreateAssetMenu(fileName ="New spell", menuName ="Spell")]
public class SpellData : ScriptableObject
{
    public int spellId;
    public Sprite spellIcon;
    public GameObject spellAnimation;
    public GameObject spellTileEffectAnimation;
    public string spellName;
    public string spellDescription;
    public SpellType spellType;
    public int spellEffectValue;
    public int spellCost;
    public int spellMinRange;
    public int spellMaxRange;
    public int spellAOEMin;
    public int spellAOEMax;
}
