using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public const float DEFAULT_MOVESPEED = 2f;
    [HideInInspector]
    public float moveH, moveV;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMovedVector;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;

    // Refrences
    Rigidbody2D rb;
    PlayerStats player;

    void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);
    }
    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();
        FindAnyObjectByType<PlayerAnimation>().SetDirection(moveDir);
    }
    void InputManagement()
    {
        if(GameManager.instance.isGameOver)
        {
            return;
        }

        moveH = Input.GetAxisRaw("Horizontal");
        moveV = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveH, moveV).normalized;

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);
        }
        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);
        }
        if(moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }
    void Move()
    {
        if(GameManager.instance.isGameOver)
        {
            return;
        }
        rb.linearVelocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
    }
}