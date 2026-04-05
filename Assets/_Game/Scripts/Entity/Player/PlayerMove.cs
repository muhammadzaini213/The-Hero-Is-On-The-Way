using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator _animator;

    private float horizontalInput;
    private bool jumpRequest;

    private bool isGrounded;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 18f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Combat")]
    [SerializeField] private float attackCooldown = 0.5f;
    private float lastAttackTime;
    private bool isAttacking;

    [SerializeField] private float counterWindow = 0.25f;
    private bool isCountering;

    [Header("SFX")]
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip counterSfx;
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip hurtSfx;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequest = true;
        }

        // Attack
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }

        // Counter (FIXED: no longer tied to attack)
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCounter();
        }

        // Animator
        bool moving = Mathf.Abs(horizontalInput) > 0.01f;
        _animator.SetBool("isRunning", moving);
        _animator.SetBool("isJumping", !isGrounded);

        FlipSprite();
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();

        if (jumpRequest)
        {
            Jump();
        }
    }

    // ================= MOVEMENT =================

    private void Move()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        if (jumpSfx != null)
            SfxPlayer.Instance.PlayPlayerSfx(jumpSfx);

        jumpRequest = false;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private void FlipSprite()
    {
        if (horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // ================= COMBAT =================

    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        _animator.SetTrigger("attack");

        if (attackSfx != null)
            SfxPlayer.Instance.PlayPlayerSfx(attackSfx);

        Invoke(nameof(EndAttack), 0.2f);
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    private void StartCounter()
    {
        if (isCountering) return;

        isCountering = true;
        _animator.SetBool("isDefend", true);

        if (counterSfx != null)
            SfxPlayer.Instance.PlayPlayerSfx(counterSfx);

        Invoke(nameof(EndCounter), counterWindow);
    }

    private void EndCounter()
    {
        isCountering = false;
        _animator.SetBool("isDefend", false);
    }

    public bool IsCountering() => isCountering;

    // ================= DAMAGE =================

    public void PlayHurtSfx()
    {
        if (hurtSfx != null)
            SfxPlayer.Instance.PlayPlayerSfx(hurtSfx);

        _animator.SetTrigger("hurt");
    }

    // ================= DEBUG =================

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}