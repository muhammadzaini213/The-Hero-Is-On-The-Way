using UnityEngine;

// Animator clip: Idle, Run, Attack — tanpa panah
// Animation Events:
// Attack clip : OnAttackHit (tengah), OnAttackEnds (akhir)

public class DemonKing : Enemy
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float chaseRange = 12f;

    [Header("Teleport")]
    public float teleportCooldown = 6f;
    public float teleportTriggerRange = 8f;
    public float teleportOffsetRange = 1.5f;
    [HideInInspector] public float teleportTimer;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private LayerMask playerLayers;

    [Header("SFX")]
    [SerializeField] private AudioClip walkSfx;
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip teleportSfx;

    [HideInInspector] public Transform target;
    [HideInInspector] public bool attackEnded;

    protected override void Awake() => base.Awake();

    protected override void Start()
    {
        base.Start();
        teleportTimer = teleportCooldown;
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override EnemyState InitialState() => new DemonKingIdleState(this);

    public void FaceTarget()
    {
        if (target == null) return;
        Vector3 scale = transform.localScale;
        scale.x = target.position.x < transform.position.x ? -1f : 1f;
        transform.localScale = scale;
    }

    public bool ShouldTeleport(float dist)
    {
        return teleportTimer <= 0f && dist > teleportTriggerRange;
    }

    public void TickTeleportCooldown()
    {
        if (teleportTimer > 0f)
            teleportTimer -= Time.deltaTime;
    }

    // Teleport langsung — tidak ada animasi, pure posisi pindah + sfx
    public void ExecuteTeleport()
    {
        if (target == null) return;
        float side = Random.value > 0.5f ? 1f : -1f;
        transform.position = target.position + new Vector3(side * teleportOffsetRange, 0f, 0f);
        FaceTarget();
        teleportTimer = teleportCooldown;
        PlayTeleportSfx();
    }

    // ---- SFX ----
    public void PlayWalkSfx() => SfxPlayer.Instance.PlayEnemySfx(walkSfx);
    public void PlayAttackSfx() => SfxPlayer.Instance.PlayEnemySfx(attackSfx);
    public void PlayTeleportSfx() => SfxPlayer.Instance.PlayEnemySfx(teleportSfx);

    // ---- Animation Events ----
    public void OnAttackHit()
    {
        if (attackPoint == null) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayers);
        foreach (Collider2D hit in hits)
            if (hit.TryGetComponent(out Health h))
                h.TakeDamage(attackDamage);
    }

    public void OnAttackEnds() => attackEnded = true;

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, teleportTriggerRange);
    }
}

// -----------------------------------------------------------------------
// Idle
// -----------------------------------------------------------------------
public class DemonKingIdleState : EnemyState
{
    private DemonKing _d;
    public DemonKingIdleState(DemonKing e) : base(e) { _d = e; }

    public override void Enter() => Play("Idle");
    public override void Exit() { }

    public override void Update()
    {
        if (_d.target == null) return;
        _d.TickTeleportCooldown();
        _d.FaceTarget();

        float dist = Vector2.Distance(_d.transform.position, _d.target.position);

        if (_d.ShouldTeleport(dist)) { _d.ExecuteTeleport(); return; }
        if (dist <= _d.chaseRange) { _d.ChangeState(new DemonKingRunState(_d)); return; }
    }
}

// -----------------------------------------------------------------------
// Run
// -----------------------------------------------------------------------
public class DemonKingRunState : EnemyState
{
    private DemonKing _d;
    public DemonKingRunState(DemonKing e) : base(e) { _d = e; }

    public override void Enter()
    {
        Play("Run");
        _d.PlayWalkSfx();
    }

    public override void Exit() { }

    public override void Update()
    {
        if (_d.target == null) return;
        _d.TickTeleportCooldown();
        _d.FaceTarget();

        float dist = Vector2.Distance(_d.transform.position, _d.target.position);

        if (_d.ShouldTeleport(dist)) { _d.ExecuteTeleport(); return; }
        if (dist <= _d.attackRange) { _d.ChangeState(new DemonKingAttackState(_d)); return; }
        if (dist > _d.chaseRange) { _d.ChangeState(new DemonKingIdleState(_d)); return; }
    }

    public override void FixedUpdate()
    {
        if (_d.target == null) return;
        Vector2 dir = (_d.target.position - _d.transform.position).normalized;
        _d.transform.Translate(dir * _d.moveSpeed * Time.fixedDeltaTime);
    }
}

// -----------------------------------------------------------------------
// Attack
// -----------------------------------------------------------------------
public class DemonKingAttackState : EnemyState
{
    private DemonKing _d;
    public DemonKingAttackState(DemonKing e) : base(e) { _d = e; }

    public override void Enter()
    {
        _d.attackEnded = false;
        _d.FaceTarget();
        Play("Attack");
        _d.PlayAttackSfx();
    }

    public override void Update()
    {
        if (!_d.attackEnded) return;

        float dist = Vector2.Distance(_d.transform.position, _d.target.position);
        _d.ChangeState(dist <= _d.attackRange
            ? (EnemyState)new DemonKingAttackState(_d)
            : new DemonKingRunState(_d));
    }

    public override void Exit() { }
}