using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float lifespan = 0.5f;
    protected PlayerStats target; // If the pickup has a target, then fly towards the target.
    protected float speed; // The speed at which the pickup travels.
    Vector2 intialPosition;
    float intialOffset; 

    // To represent the bobbing animation of the object.
    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency;
        public Vector2 direction;
    }
    public BobbingAnimation bobbingAnimation = new BobbingAnimation{
        frequency = 2f, direction = new Vector2(0, 0.3f)
    };

    [Header("Bonuses")]
    public int experience;
    public int health;

    public virtual void Start()
    {
        intialPosition = transform.position;
        intialOffset = Random.Range(0, bobbingAnimation.frequency);
    }

    protected virtual void Update()
    {
        if (target)
        {
            // Move it towards the player and check the distance between.
            Vector2 distance = target.transform.position - transform.position;

            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            else
                Destroy(gameObject);
        }
        else
        {
            // handle the animation of the object.
            transform.position = intialPosition + bobbingAnimation.direction * Mathf.Sin((Time.time + intialOffset) * bobbingAnimation.frequency);
        }
    }

    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0)
    {
        if(!this.target)
        {
            this.target = target;
            this.speed = speed;
            if(lifespan > 0) this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if(!target) return;
        if(experience != 0) target.IncreaseExpirience(experience);
        if(health != 0) target.RestoreHealth(health);
    }
}
