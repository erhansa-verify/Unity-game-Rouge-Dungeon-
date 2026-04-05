using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData.Stats Stats{
        get{return actualStats;}
        set{
            actualStats = value;
        }
    }

    float health;

    #region Current Stats Properties

    public float CurrentHealth
    {
        get { return health; }

        // If we try and set the current health, the UI interface
        // on the pause screen will also be updated.
        set
        {
            // Check if the value has changed
            if (health != value)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }

    #endregion

    [Header("Visual")]
    public ParticleSystem damageEffect; // if damage is dealt
    public ParticleSystem blockedEffect; // if armor completly block damage

    // Expirience and Level
    [Header("Experience/Level")]
    public int expirience = 0;
    public int level = 1;
    public int expirienceCap;

    // Class for defining a level range and the corresponding expirience cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int expirienceCapIncrease;
    }

    //I-Frames
    [Header("I-Frames")]
    public float invicibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;

    PlayerCollector collector;
    PlayerInventory inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMP_Text levelText;


    void Awake()
    {
        characterData = CharacterSelector.GetData();

        if(CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();

        // Assign the variables
        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;

    }

    void Start()
    {
        // Spawn starting weapon
        inventory.Add(characterData.StartingWeapon);

        // Initialize the expirience cap as the first expirience cap increase
        expirienceCap = levelRanges[0].expirienceCapIncrease;

        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
        collector.SetRadius(actualStats.magnet);
    }

    public void IncreaseExpirience(int amount)
    {
        expirience += amount;

        LevelUpChecker();
        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        if (expirience >= expirienceCap)
        {
            level++;
            expirience -= expirienceCap;

            int expirienceCapIncrease = 0;
            // Update expirience cap based on the new level
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    expirienceCap = range.expirienceCapIncrease;
                    break;
                }
            }
            expirienceCap += expirienceCapIncrease;

            UpdateLevelText();

            GameManager.instance.StartLevelUp();

            if(expirience >= expirienceCap)
                LevelUpChecker();
        }
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float)expirience / expirienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = "LV " + level.ToString();
    }

    public void TakeDamage(float dmg)
    {

        // If the player is not currently invincible, reduce health and start invincibility
        if (!isInvincible)
        {
            // Take armor into account before dealing the damage
            dmg -= actualStats.armor;

            if (dmg > 0)
            {
                // Deal the damage
                CurrentHealth -= dmg;

                // Play damage effect if assigned
                if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

                // Check death
                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
            else
            {
                // Damage blocked
                if (blockedEffect)Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity),5f);
            }

            invincibilityTimer = invicibilityDuration;
            isInvincible = true;
        }
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignedLevelReachedUI(level);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        // Only heal the player if they are not at max health
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            // Ensure current health does not exceed max health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;
            
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
            UpdateHealthBar();
        }
    }
}
