using UnityEngine;

/// <summary>
/// Component can be attached to all Weapon Prefabs. The weapon prefab works together with the WeaponData
/// ScriptableObjects to manage and run the bahviour of all weapons in the game.
/// </summary>
public abstract class Weapon : Item
{
    [System.Serializable]
    public class Stats : LevelData
    {
        [Header("Visuals")]
        public Projectile projectilePrefab; // if attached, a projectile will spawn every time the weapon cools down.
        public Aura auraPrefab; // if attached. an aura will spawn around the player when the weapon is equipped.
        public ParticleSystem hitEffect, procEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; // if 0, the weapon lasts forever.
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;

        // Allow to use the + operator to add two Stats together.
        // Very important for later when we want to increase out weapon stats.
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.procEffect = s2.procEffect == null ? s1.procEffect : s2.procEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            return result;
        }

        // Get damage dealt
        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }
    
    protected Stats currentStats;
    protected float currentCooldown;
    protected PlayerMovement movement; // Refrence to all player's movement.

    // For dynamically created weapons, call initialize to set everything up.

    public virtual void Initialize(WeaponData data)
    {
        base.Initialize(data);
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        ActivateCooldown();
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f) // Once the cooldown becomes 0, attack.
        {
            Attack(currentStats.number + owner.Stats.amount);
        }
    }

    // Levels up the weapon by 1, and calcualtes the corresponding stats.

    public override bool DoLevelUp()
    {
        base.DoLevelUp();
        // Prevent level up if we are already at max level.
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        // Otherwise, add stats of the next level to the weapon.
        currentStats += (Stats)data.GetLevelData(++currentLevel);
        return true;
    }

    // Lets us check whether this weapon can attack at this current moment.
    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    // Peforms an attach with this weapon.
    // Return true if the attack was successful.
    // This doesn;t do anything. Must be implemented by child classes.
    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            currentCooldown += currentStats.cooldown;
            return true;
        }
        return false;
    }

    // Gets the amount of damage that the weapon does.
    // Factoring in the weapon stats (includng damage variance).
    // and the character's Might stat.
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.Stats.might;
    }

    // Get area, including modifications from the player's stats.
    public virtual float GetArea()
    {
        return currentStats.area + owner.Stats.area;
    }
    // For retrieving the weapon's stats.

    public virtual Stats GetStats()
    {
        return currentStats;
    }

    // Refreshes the cooldown of the weapon.
    // if <strict> is true, refreshes only when currentCooldown = 0.
    public virtual bool ActivateCooldown(bool strict = false)
    {
        // When strict is enabled and the cooldown is not yet finished,
        // do not refresh the cooldown.
        if (strict && currentCooldown > 0) return false;

        // Calculate the actual cooldown, factoring in the player's cooldown stat
        float actualCooldown = currentStats.cooldown * Owner.Stats.cooldown;

        // Limit the cooldown so it never exceeds the actual cooldown value
        // if this function is called multiple times
        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }

}
