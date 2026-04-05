using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;

    Vector2 knockbackVelocity;
    float knockbackDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (knockbackDuration > 0)
        {
            // Apply knockback movement
            transform.position += (Vector3)(knockbackVelocity * Time.deltaTime);
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            // Normal movement towards player
            //Vector2 direction = (player.position - transform.position).normalized;
            //transform.position += (Vector3)(direction * enemy.currentMoveSpeed * Time.deltaTime);
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemy.currentMoveSpeed * Time.deltaTime);
        }
    }

    public void Knockbak(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0) return; // Prevents overlapping knockbacks

        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
