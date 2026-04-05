using System;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator enemy;

    public string[] idleDirections = { "Idle N", "Idle NW", "Idle W", "Idle SW", "Idle S", "Idle SE", "Idle E", "Idle NE" };
    public string[] runDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };

    int lastDirection;
    private Vector2 lastPosition;
    private Rigidbody2D rb;

    [SerializeField] private bool useRigidbodyVelocity = true;
    [SerializeField] private float movementThreshold = 0.01f; // ambang untuk dianggap bergerak

    // debug
    [SerializeField] private bool debugDirection = false;
    [SerializeField] private Color debugRayColor = Color.green;
    
    private void Awake()
    {
        enemy = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position;
    }

    // Use LateUpdate so movement scripts/physics have already moved the object this frame.
    private void LateUpdate()
    {
        Vector2 move = Vector2.zero;

        if (useRigidbodyVelocity && rb != null && rb.linearVelocity.sqrMagnitude > movementThreshold * movementThreshold)
        {
            move = rb.linearVelocity;
        }
        else
        {
            Vector2 currentPos = (Vector2)transform.position;
            float dt = Mathf.Max(Time.deltaTime, 0.0001f);
            move = (currentPos - lastPosition) / dt;
            lastPosition = currentPos;
        }

        if (debugDirection)
        {
            Debug.Log($"{name} move={move} mag={move.magnitude:F3}");
            Debug.DrawRay(transform.position, (Vector3)move.normalized, debugRayColor, 0.5f);
        }

        SetDirection(move);
    }

    public void SetDirection(Vector2 _direction)
    {
        string[] directionArray;

        if (_direction.magnitude < movementThreshold)
        {
            directionArray = idleDirections;
        }
        else
        {
            directionArray = runDirections;
            lastDirection = DirectionToIndex(_direction);
        }

        if (enemy != null && lastDirection >= 0 && lastDirection < directionArray.Length)
        {
            if (debugDirection)
            {
                Debug.Log($"{name} -> index={lastDirection} anim={directionArray[lastDirection]}");
            }
            enemy.Play(directionArray[lastDirection]);
        }
    }

    private int DirectionToIndex(Vector2 _direction)
    {
        if (_direction.sqrMagnitude <= 0.000001f) return lastDirection;

        Vector2 norDir = _direction.normalized;
        float a = Mathf.Atan2(norDir.y, norDir.x) * Mathf.Rad2Deg; // (-180,180]
        if (a < 0f) a += 360f; // -> [0,360)

        // angle relative to Up (0 = up), CCW
        float angleFromUp = a - 90f;
        if (angleFromUp < 0f) angleFromUp += 360f;

        float step = 360f / 8f; // 45°
        float halfStep = step * 0.5f;

        float snapped = Mathf.Floor((angleFromUp + halfStep) / step) * step;
        int index = (int)(snapped / step) % 8;

        // index mapping: 0 = N, 1 = NW, 2 = W, 3 = SW, 4 = S, 5 = SE, 6 = E, 7 = NE
        return index;
    }
}