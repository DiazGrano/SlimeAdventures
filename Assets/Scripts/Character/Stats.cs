using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField]
    private int characterLevel;

    [SerializeField]
    private int maxHealthPoints;
    [SerializeField]
    private int currentHealthPoints;
    [SerializeField]
    private int maxAbsoluteHealthRecoveryPerSecond = 100;
    [SerializeField]
    private int healthRecoveryPerSecond;

    [SerializeField]
    private int maxAbsoluteManaPoints = 16;
    [SerializeField]
    private int currentManaPoints;
    [SerializeField]
    private int maxManaPoints;

    [SerializeField]
    private int maxAbsoluteMovementPoints = 8;
    [SerializeField]
    private int maxMovementPoints;
    [SerializeField]
    private int currentMovementPoints;


    [Tooltip("Cada punto de fuerza aumenta en 2% el daño de ataques físicos, 1 punto de vida y un punto de iniciativa")]
    public int strengthPoints;
    [Tooltip("Cada punto de inteligencia aumenta en 2% el daño de hechizos, en 3% los efectos curativos y un punto de iniciativa")]
    public int inteligencePoints;
    [Tooltip("Cada punto de agilidad aumenta un 0.5% de probabilidad de golpe crítico, 0.1 puntos de velocidad de movimiento y 2 puntos de iniciativa.")]
    public int agilityPoints;
    [Tooltip("Cada punto de armadura reduce el daño físico recibido en un 1.5%, hasta un 60%")]
    public int armorPoints;
    [Tooltip("Cada punto de resistencia mágica reduce el daño mágico recibido en un 1.5%, hasta un 60%")]
    public int magicResistancePoints;
    [Tooltip("Un golpe crítico aumenta en un 100% el daño (ya sea físico o mágico), la probabilidad va de 0% a 100%")]
    public float criticalHitChancePoints;
    [Tooltip("Cada punto de iniciativa aumenta en 5 la iniciativa total")]
    public int initiativePoints;
    public float baseSpeed = 5f;



    public int GetCharacterLevel()
    {
        return this.characterLevel;
    }
    public void CharacterLevelUp()
    {
        this.characterLevel++; 
    }

    public float GetCriticalHitChance()
    {
        return Mathf.Clamp(this.criticalHitChancePoints + (this.agilityPoints * 0.5f), 0f, 100f);
    }

    public int GetInitiative()
    {
        return (Mathf.Clamp(this.strengthPoints, 0, int.MaxValue) + Mathf.Clamp(this.inteligencePoints, 0, int.MaxValue) + (Mathf.Clamp(this.agilityPoints, 0, int.MaxValue) * 2) + this.initiativePoints);
    }

    public float GetMovementSpeed()
    {
        float aux = Mathf.Clamp((baseSpeed + (Mathf.Clamp(this.agilityPoints, 0f, int.MaxValue) * 0.1f)), 0f, 10f);
        return aux;
    }


    public int CharacterResource(CharacterResourceType resourceType, bool set = false, int amount = 0)
    {
        switch (resourceType)
        {
            case CharacterResourceType.MaxManaPoints:
                if (set)
                {
                    if (this.maxManaPoints + amount <= this.maxAbsoluteManaPoints && this.maxManaPoints + amount >= 0)
                    {
                        this.maxManaPoints += amount;
                    }
                    else if (this.maxManaPoints + amount > this.maxAbsoluteManaPoints)
                    {
                        this.maxManaPoints = this.maxAbsoluteManaPoints;
                    }
                    else if (this.maxManaPoints + amount < 0)
                    {
                        this.maxManaPoints = 0;
                    }
                }
                return this.maxManaPoints;

            case CharacterResourceType.ManaPoints:
                if (set)
                {
                    if (this.currentManaPoints + amount <= this.maxManaPoints && this.currentManaPoints + amount >= 0)
                    {
                        this.currentManaPoints += amount;
                    }
                    else if (this.currentManaPoints + amount > this.maxManaPoints)
                    {
                        this.currentManaPoints = this.maxManaPoints;
                    }
                    else if (this.currentManaPoints + amount < 0)
                    {
                        this.currentManaPoints = 0;
                    }
                }
                return this.currentManaPoints;


            case CharacterResourceType.MaxMovementPoints:
                if (set)
                {
                    if (this.maxMovementPoints + amount <= this.maxAbsoluteMovementPoints && this.maxMovementPoints + amount >= 0)
                    {
                        this.maxMovementPoints += amount;
                    }
                    else if (this.maxMovementPoints + amount > this.maxAbsoluteMovementPoints)
                    {
                        this.maxMovementPoints = this.maxAbsoluteMovementPoints;
                    }
                    else if (this.maxMovementPoints + amount < 0)
                    {
                        this.maxMovementPoints = 0;
                    }
                }
                return this.maxMovementPoints;

            case CharacterResourceType.MovementPoints:
                if (set)
                {
                    if (this.currentMovementPoints + amount <= this.maxMovementPoints && this.currentMovementPoints + amount >= 0)
                    {
                        this.currentMovementPoints += amount;
                    }
                    else if (this.currentMovementPoints + amount > this.maxMovementPoints)
                    {
                        this.currentMovementPoints = this.maxMovementPoints;
                    }
                    else if (this.currentMovementPoints + amount < 0)
                    {
                        this.currentMovementPoints = 0;
                    }
                }
                return this.currentMovementPoints;

            case CharacterResourceType.MaxHealthPoints:
                if (set)
                {
                    if (this.maxHealthPoints + amount >= 0)
                    {
                        this.maxHealthPoints += amount;
                    }
                    else if (this.maxMovementPoints + amount < 0)
                    {
                        this.maxMovementPoints = 0;
                    }
                }
                return this.maxHealthPoints;

            case CharacterResourceType.HealthPoints:
                if (set)
                {
                    if (this.currentHealthPoints + amount <= this.maxHealthPoints && this.currentHealthPoints + amount >= 0)
                    {
                        this.currentHealthPoints += amount;
                    }
                    else if (this.currentMovementPoints + amount > this.maxHealthPoints)
                    {
                        this.currentHealthPoints = this.maxHealthPoints;
                    }
                    else if (this.currentHealthPoints + amount < 0)
                    {
                        this.currentHealthPoints = 0;
                    }
                }
                return this.currentHealthPoints;

            case CharacterResourceType.HealthRecoveryPerSecond:
                if (set)
                {
                    if (this.healthRecoveryPerSecond + amount <= this.maxAbsoluteHealthRecoveryPerSecond && this.healthRecoveryPerSecond + amount >= 0)
                    {
                        this.healthRecoveryPerSecond += amount;
                    }
                    else if (this.healthRecoveryPerSecond + amount > this.maxAbsoluteHealthRecoveryPerSecond)
                    {
                        this.healthRecoveryPerSecond = this.maxAbsoluteHealthRecoveryPerSecond;
                    }
                    else if (this.healthRecoveryPerSecond + amount < 0)
                    {
                        this.healthRecoveryPerSecond = 0;
                    }
                }
                return this.healthRecoveryPerSecond;
        }

        return 0;
    }


}
