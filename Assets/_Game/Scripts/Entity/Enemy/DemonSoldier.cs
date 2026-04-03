using UnityEngine;

// Animator: taruh 5 clip (Idle, Run, Attack, Hit, Death) tanpa panah apapun.
// Semua transisi dikontrol penuh dari kode.

public class DemonSoldier : Enemy
{
    [Header("Stats")]
    public float moveSpeed   = 3f;
    public float attackRange = 1.5f;
    public float chaseRange  = 6f;
    public int   maxHp       = 3;

    [HideInInspector] public int       hp;
    [HideInInspector] public Transform target;
    [HideInInspector] public bool      isDead;

    protected override void Awake()
    {
        base.Awake();
        hp = maxHp;
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override EnemyState InitialState() => new EnemyIdleState(this);

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        hp -= amount;
        if (hp <= 0)
            ChangeState(new EnemyDeathState(this));
        else
            ChangeState(new EnemyHitState(this));
    }
}

// -----------------------------------------------------------------------
// Idle — diam, scan player
// -----------------------------------------------------------------------
public class EnemyIdleState : EnemyState
{
    private DemonSoldier _d;
    public EnemyIdleState(DemonSoldier e) : base(e) { _d = e; }

    public override void Enter()  => Play("Idle");
    public override void Exit()   { }

    public override void Update()
    {
        if (_d.target == null) return;
        float dist = Vector2.Distance(_d.transform.position, _d.target.position);
        if (dist <= _d.chaseRange)
            _d.ChangeState(new EnemyRunState(_d));
    }
}

// -----------------------------------------------------------------------
// Run — kejar player
// -----------------------------------------------------------------------
public class EnemyRunState : EnemyState
{
    private DemonSoldier _d;
    public EnemyRunState(DemonSoldier e) : base(e) { _d = e; }

    public override void Enter() => Play("Run");
    public override void Exit()  { }

    public override void Update()
    {
        if (_d.target == null) return;
        float dist = Vector2.Distance(_d.transform.position, _d.target.position);

        if (dist <= _d.attackRange) { _d.ChangeState(new EnemyAttackState(_d)); return; }
        if (dist > _d.chaseRange)   { _d.ChangeState(new EnemyIdleState(_d));   return; }
    }

    public override void FixedUpdate()
    {
        if (_d.target == null) return;
        Vector2 dir = (_d.target.position - _d.transform.position).normalized;
        _d.transform.Translate(dir * _d.moveSpeed * Time.fixedDeltaTime);
    }
}

// -----------------------------------------------------------------------
// Attack — serang, tunggu animasi selesai, lalu cek lagi
// -----------------------------------------------------------------------
public class EnemyAttackState : EnemyState
{
    private DemonSoldier _d;
    private bool         _attacked;

    public EnemyAttackState(DemonSoldier e) : base(e) { _d = e; }

    public override void Enter()
    {
        _attacked = false;
        Play("Attack");
    }

    public override void Update()
    {
        // Tunggu animasi attack selesai baru transisi
        if (!IsAnimationDone()) return;

        if (!_attacked)
        {
            // lakukan damage di sini atau via AnimationEvent
            _attacked = true;
        }

        float dist = Vector2.Distance(_d.transform.position, _d.target.position);
        if (dist <= _d.attackRange)
            _d.ChangeState(new EnemyAttackState(_d)); // loop attack
        else
            _d.ChangeState(new EnemyRunState(_d));
    }

    public override void Exit() { }
}

// -----------------------------------------------------------------------
// Hit — kena damage, tunggu animasi, lanjut
// -----------------------------------------------------------------------
public class EnemyHitState : EnemyState
{
    private DemonSoldier _d;
    public EnemyHitState(DemonSoldier e) : base(e) { _d = e; }

    public override void Enter() => Play("Hit");
    public override void Exit()  { }

    public override void Update()
    {
        if (!IsAnimationDone()) return;

        // Setelah kena hit, lanjut chase atau idle
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
// Death — mati, tunggu animasi, destroy
// -----------------------------------------------------------------------
public class EnemyDeathState : EnemyState
{
    private DemonSoldier _d;
    public EnemyDeathState(DemonSoldier e) : base(e) { _d = e; }

    public override void Enter()
    {
        _d.isDead = true;
        Play("Death");
    }

    public override void Update()
    {
        if (!IsAnimationDone()) return;
        Object.Destroy(_d.gameObject);
    }

    public override void Exit() { }
}