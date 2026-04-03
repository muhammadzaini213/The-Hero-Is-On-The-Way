using UnityEngine;

public abstract class EnemyState
{
    protected Enemy    enemy;
    protected Animator animator;

    public EnemyState(Enemy enemy)
    {
        this.enemy    = enemy;
        this.animator = enemy.Animator;
    }

    public abstract void Enter();
    public abstract void Update();
    public virtual  void FixedUpdate() { }
    public abstract void Exit();

    // Langsung play clip — tidak butuh panah di Animator
    protected void Play(string stateName, int layer = 0)
    {
        if (animator == null) return;
        if (animator.HasState(layer, Animator.StringToHash(stateName)))
            animator.Play(stateName, layer, 0f);
        else
            Debug.LogWarning($"[EnemyState] '{stateName}' tidak ada di Animator {enemy.name}");
    }

    // Cek apakah animasi sekarang sudah selesai
    protected bool IsAnimationDone(int layer = 0)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);
        return info.normalizedTime >= 1f && !animator.IsInTransition(layer);
    }
}