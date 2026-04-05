using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator _animator;

    private float horizontalInput;
    private bool jumpRequest;
    private bool isGrounded;

    // Enum tutorial dengan tambahan step Heal
    private enum TutorialStep { Movement, Jump, Attack, Counter, Heal, Completed }
    private TutorialStep currentStep = TutorialStep.Movement;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 18f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Combat")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform attackPoint;    
    [SerializeField] private float attackRange = 0.6f;  
    [SerializeField] private LayerMask enemyLayers;    

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

    void Start()
    {
        UpdateTutorialUI();
    }

    public bool IsCountering() => isCountering;

    void Update()
    {
        // Input Logic
        if (isAttacking) horizontalInput = 0;
        else horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) jumpRequest = true;

        if (Input.GetKeyDown(KeyCode.J) && Time.time >= lastAttackTime + attackCooldown && isGrounded)
            Attack();

        if (Input.GetKeyDown(KeyCode.K)) StartCounter();

        // Animator Updates
        bool moving = Mathf.Abs(horizontalInput) > 0.01f;
        _animator.SetBool("isRunning", moving);
        _animator.SetBool("isJumping", !isGrounded);

        FlipSprite();

        // Tutorial Logic Check
        if (currentStep != TutorialStep.Completed)
        {
            CheckTutorialProgress();
        }
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
        if (jumpRequest) Jump();
    }

    // ================= TUTORIAL LOGIC =================

    private void UpdateTutorialUI()
    {
        if (TutorialText.Instance == null) return;

        switch (currentStep)
        {
            case TutorialStep.Movement:
                TutorialText.Instance.ShowTutorial("Use A/D to MOVE");
                break;
            case TutorialStep.Jump:
                TutorialText.Instance.ShowTutorial("Great! Press SPACE to JUMP");
                break;
            case TutorialStep.Attack:
                TutorialText.Instance.ShowTutorial("Nice! Press J to ATTACK");
                break;
            case TutorialStep.Counter:
                TutorialText.Instance.ShowTutorial("Defend yourself! Press K to COUNTER");
                break;
            case TutorialStep.Heal:
                TutorialText.Instance.ShowTutorial("Injured? Press F to get Hero support (The hero would arrive more late...)");
                break;
            case TutorialStep.Completed:
                TutorialText.Instance.ShowTutorial("Tutorial Complete!");
                Invoke(nameof(HideTutorialNow), 5f);
                break;
        }
    }

    private void CheckTutorialProgress()
    {
        if (currentStep == TutorialStep.Movement && Mathf.Abs(horizontalInput) > 0.1f)
        {
            currentStep = TutorialStep.Jump;
            UpdateTutorialUI();
        }
        else if (currentStep == TutorialStep.Jump && jumpRequest)
        {
            currentStep = TutorialStep.Attack;
            UpdateTutorialUI();
        }
        else if (currentStep == TutorialStep.Attack && isAttacking)
        {
            currentStep = TutorialStep.Counter;
            UpdateTutorialUI();
        }
        else if (currentStep == TutorialStep.Counter && isCountering)
        {
            currentStep = TutorialStep.Heal;
            UpdateTutorialUI();
        }
        else if (currentStep == TutorialStep.Heal && Input.GetKeyDown(KeyCode.F))
        {
            currentStep = TutorialStep.Completed;
            UpdateTutorialUI();
        }
    }

    private void HideTutorialNow()
    {
        if (TutorialText.Instance != null)
            TutorialText.Instance.HideTutorial();
    }

    // ================= MOVEMENT & COMBAT =================

    private void Move() => rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        if (jumpSfx != null) SfxPlayer.Instance.PlayPlayerSfx(jumpSfx);
        jumpRequest = false;
    }

    private void CheckGround() => isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

    private void FlipSprite()
    {
        if (horizontalInput > 0) transform.eulerAngles = Vector3.zero;
        else if (horizontalInput < 0) transform.eulerAngles = new Vector3(0, 180, 0);
    }

    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        _animator.SetTrigger("attack");
        if (attackSfx != null) SfxPlayer.Instance.PlayPlayerSfx(attackSfx);
    }

    public void OnAttackHit()
    {
        if (attackPoint == null) return;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out EnemyHealth target)) target.TakeDamage(attackDamage);
        }
    }

    public void OnAttackEnds() => isAttacking = false;

    private void StartCounter()
    {
        if (isCountering) return;
        isCountering = true;
        _animator.SetBool("isDefend", true);
        if (counterSfx != null) SfxPlayer.Instance.PlayPlayerSfx(counterSfx);
        Invoke(nameof(EndCounter), counterWindow);
    }

    private void EndCounter()
    {
        isCountering = false;
        _animator.SetBool("isDefend", false);
    }

    public void PlayHurtSfx()
    {
        if (hurtSfx != null) SfxPlayer.Instance.PlayPlayerSfx(hurtSfx);
        _animator.SetTrigger("hurt");
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(groundCheck.position, checkRadius); }
        if (attackPoint != null) { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
    }
}