    using UnityEngine;

/// <summary>
/// Component that you attach to all projectile prefabs. All spawned projectile will fly in the direction
/// they are facing and deal damage when the hit an object.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    protected Rigidbody2D rb;
    protected int piercing;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.linearVelocity = transform.right * stats.speed * weapon.Owner.Stats.speed;
        }

        // Prevent the area from being 0, as it hides the projectile.
        float area = weapon.GetArea();
        if(area <= 0) area = 1;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y), 1
        );

        // Set how much piercing this object has.
        piercing = stats.piercing;

        // Destroy the projectile after its lifespan expires.
        if (stats.lifespan > 0) Destroy(gameObject, stats.lifespan);

        // if the projectile is auto-aiming, automatically find a suitable target.
        if (hasAutoAim) AcquireAutoAimFacing();
    }

    // if the projectile is homing, it will automatically find a suitable target
    // to move towards.
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle; // The angle offset to apply to the projectile's current facing.

        // Find all enemies on the screen.
        EnemyStats[] targets = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);

        // Select a randm enemy (if there is at least 1).
        // Otherwise, pick a random angle.
        if (targets.Length > 0)
        {
            EnemyStats selectecTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectecTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0, 360f);
        }

        // Point to the projectile towards where we are aiming at.
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        // Only drive movement ourselves if this is a kinematic.
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * weapon.Owner.Stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        // Only collide with enemies or breaakable studd.
        if (es)
        {
            // If there is an owner, and the damage source is set to owner,
            // calculate knockback using the owner instead of the projectile.
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            // Deals damage and destroy the projectile.
            es.TakeDamage(GetDamage(), source);

            Weapon.Stats stats = weapon.GetStats();
            piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (p)
        {
            // Deals damage and destroy the projectile.
            p.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }

        // Destroy the projectile if it has no more piercing left.
        if (piercing <= 0) Destroy(gameObject);
    }
}
