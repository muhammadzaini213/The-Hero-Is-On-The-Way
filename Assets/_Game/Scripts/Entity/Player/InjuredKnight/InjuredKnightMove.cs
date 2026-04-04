using UnityEngine;

public class InjuredKnightMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator _animator;

    private float horizontalInput;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 18f;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        bool moving = Mathf.Abs(horizontalInput) > 0.01f;
        _animator.SetBool("isRunning", moving);

        FlipSprite();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    private void FlipSprite()
    {
        if (horizontalInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

}