using UnityEngine;

public class InjuredKnightMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator _animator;
    private float horizontalInput;
    private InjuredKnightAttack attack;
    private InjuredKnightHealth health;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 18f;

    void Awake()
    {
        attack = GetComponent<InjuredKnightAttack>();
        health = GetComponent<InjuredKnightHealth>();
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool moving = Mathf.Abs(horizontalInput) > 0.01f;
        
        if (attack.isAttacking || health.isHitAnimation || health.isDeath) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        _animator.SetBool("isRunning", moving);

        FlipSprite();
    }

    void FixedUpdate()
    {
        if (attack.isAttacking || health.isHitAnimation || health.isDeath)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

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