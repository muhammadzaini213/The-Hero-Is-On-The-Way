using UnityEngine;

// Animation Events yang dibutuhkan di clip:
// Attack clip : OnAttackHit (tengah ayunan), OnAttackEnds (frame terakhir)
// Hurt clip   : OnHurtEnds (frame terakhir)
// Death clip  : OnDeathEnds (frame terakhir)

public class DemonSoldier : Enemy
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float chaseRange = 6f;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask playerLayers;

    [Header("SFX")]
    [SerializeField] private AudioClip walkSfx;
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip hurtSfx;
    [SerializeField] private AudioClip deathSfx;

    [HideInInspector] public Transform target;
    [HideInInspector] public bool attackEnded;
    [HideInInspector] public bool hurtEnded;
    [HideInInspector] public bool deathEnded;

    private EnemyHealth _health;

    protected override void Awake()
    {
        base.Awake();
        _health = GetComponent<EnemyHealth>();

        if (_health == null) { Debug.LogWarning($"[DemonSoldier] Tidak ada EnemyHealth di {name}"); return; }
        _health.OnHealthChanged += OnHealthChanged;
        _health.OnDeath += OnDeath;
    }

    protected override void Start()
    {
        base.Start();
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void OnDestroy()
    {
        if (_health == null) return;
        _health.OnHealthChanged -= OnHealthChanged;
        _health.OnDeath -= OnDeath;
    }

    protected override EnemyState InitialState() => new EnemyIdleState(this);

    public void FaceTarget()
    {
        if (target == null) return;
        Vector3 scale = transform.localScale;
        scale.x = target.position.x < transform.position.x ? -1f : 1f;
        transform.localScale = scale;
    }

    // ---- SFX ----
    public void PlayWalkSfx() => SfxPlayer.Instance.PlayEnemySfx(walkSfx, loop: true);
    public void PlayAttackSfx() => SfxPlayer.Instance.PlayEnemySfx(attackSfx);
    public void PlayHurtSfx() => SfxPlayer.Instance.PlayEnemySfx(hurtSfx);
    public void PlayDeathSfx() => SfxPlayer.Instance.PlayEnemySfx(deathSfx);

    // ---- Animation Events ----
    public void OnAttackHit()
    {
        if (attackPoint == null) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);
        foreach (Collider2D hit in hits)
            if (hit.TryGetComponent(out Health h))
                h.TakeDamage(attackDamage);
    }

    public void OnAttackEnds() => attackEnded = true;
    public void OnHurtEnds() => hurtEnded = true;
    public void OnDeathEnds() => deathEnded = true;

    // ---- Health Events ----
    private void OnHealthChanged(int current, int max)
    {
        if (_health.IsDead) return;
        if (CurrentState is EnemyHurtState || CurrentState is EnemyDeathState) return;
        ChangeState(new EnemyHurtState(this));
    }

    private void OnDeath() => ChangeState(new EnemyDeathState(this));

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}

// -----------------------------------------------------------------------
// Idle
// -----------------------------------------------------------------------
public class EnemyIdleState : EnemyState
{
    private DemonSoldier _d;
    public EnemyIdleState(DemonSoldier e) : base(e) { _d = e; }

    public override void Enter() => Play("Idle");
    public override void Exit() { }

    public override void Update()
    {
        if (_d.target == null) return;
        _d.FaceTarget();
        float dist = Vector2.Distance(_d.transform.position, _d.target.position);
        if (dist <= _d.chaseRange)
            _d.ChangeState(new EnemyRunState(_d));
    }
}

// -----------------------------------------------------------------------
// Run
// -----------------------------------------------------------------------
public class EnemyRunState : EnemyState
{
    private DemonSoldier _d;
    public EnemyRunState(DemonSoldier e) : base(e) { _d = e; }

    public override void Enter()
    {
        Play("Run");
        _d.PlayWalkSfx();
    }

    public override void Exit() { }

    public override void Update()
    {
        if (_d.target == null) return;
        _d.FaceTarget();
        float dist = Vector2.Distance(_d.transform.position, _d.target.position);

        if (dist <= _d.attackRange) { _d.ChangeState(new EnemyAttackState(_d)); return; }
        if (dist > _d.chaseRange) { _d.ChangeState(new EnemyIdleState(_d)); return; }
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
public class EnemyAttackState : EnemyState
{
    private DemonSoldier _d;
    public EnemyAttackState(DemonSoldier e) : base(e) { _d = e; }

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
            ? (EnemyState)new EnemyAttackState(_d)
            : new EnemyRunState(_d));
    }

    public override void Exit() { }
}

// -----------------------------------------------------------------------
// Hurt
// -----------------------------------------------------------------------
public class EnemyHurtState : EnemyState
{
    private DemonSoldier _d;
    public EnemyHurtState(DemonSoldier e) : base(e) { _d = e; }

    public override void Enter()
    {
        _d.hurtEnded = false;
        Play("Hurt");
        _d.PlayHurtSfx();
    }

    public override void Exit() { }

    public override void Update()
    {
        if (!_d.hurtEnded) return;

        if (_d.target != null)
        {
            float dist = Vector2.Distance(_d.transform.position, _d.target.position);
            _d.ChangeState(dist <= _d.chaseRange
                ? (EnemyState)new EnemyRunState(_d)
                : new EnemyIdleState(_d));
        }
        else
        {
            _d.ChangeState(new EnemyIdleState(_d));
        }
    }
}

// -----------------------------------------------------------------------
// Death
// -----------------------------------------------------------------------
public class EnemyDeathState : EnemyState
{
    private DemonSoldier _d;
    public EnemyDeathState(DemonSoldier e) : base(e) { _d = e; }

    public override void Enter()
    {
        _d.deathEnded = false;
        Play("Death");
        _d.PlayDeathSfx();
    }

    public override void Exit() { }

    public override void Update()
    {
        if (!_d.deathEnded) return;
        Object.Destroy(_d.gameObject);
    }
}