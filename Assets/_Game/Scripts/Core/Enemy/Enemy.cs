using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public Animator    Animator     { get; private set; }
    public EnemyState  CurrentState { get; private set; }

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        if (Animator == null)
            Debug.LogWarning($"[Enemy] Tidak ada Animator di {name}");
    }

    protected virtual void Start()
    {
        CurrentState = InitialState();
        CurrentState.Enter();
    }

    protected virtual void Update()      => CurrentState?.Update();
    protected virtual void FixedUpdate() => CurrentState?.FixedUpdate();

    public void ChangeState(EnemyState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    protected abstract EnemyState InitialState();
}