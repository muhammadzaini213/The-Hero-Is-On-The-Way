using UnityEngine;

public class InjuredKnightAttack : MonoBehaviour
{
    private Animator _animator;
    public bool isAttacking { get; private set; }
    private InjuredKnightHealth health;

    [Header("Hit Detection Settings")]
    [SerializeField] private Transform attackPoint;    // Drag & Drop objek kosong di depan player ke sini
    [SerializeField] private float attackRange = 0.6f; // Jari-jari lingkaran serangan
    [SerializeField] private LayerMask enemyLayers;    // Pilih layer "Enemy" di Inspector
    [SerializeField] private int attackDamage = 10;    // Jumlah damage
    [SerializeField] private AudioClip attacksfx1; // Suara serangan musuh
    [SerializeField] private AudioClip attacksfx2; // Suara serangan musuh

    void Awake()
    {
        _animator = GetComponent<Animator>();
        health = GetComponent<InjuredKnightHealth>();
        isAttacking = false;
    }

    void Update()
    {
        // Pengecekan: Tidak boleh menyerang kalau sedang attack, sedang kena hit, atau mati
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking && !health.isDeath && !health.isHitAnimation)
        {
            Attack();
        }
    }

    private void Attack()
    {
        // Karena int 0,3 berarti 0, 1, 2. 
        // 0 -> attack1, sisanya (1, 2) -> attack2.
        int random = Random.Range(0, 3);
        isAttacking = true;

        if (random == 0)
        {
            _animator.SetTrigger("attack1");
            SfxPlayer.Instance.PlayPlayerSfx(attacksfx1);
        }
        else
        {
            _animator.SetTrigger("attack2");
            SfxPlayer.Instance.PlayPlayerSfx(attacksfx2);
        }
    }

    // DIPANGGIL VIA ANIMATION EVENT (di tengah animasi saat pedang mengayun)
    public void OnAttackHit()
    {
        if (attackPoint == null) return;

        // Membuat lingkaran imaginer untuk mendeteksi Collider musuh
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Pastikan musuh punya script Health (atau script apapun yang kamu gunakan untuk nyawa musuh)
            if (enemy.TryGetComponent(out Health targetHealth))
            {
                targetHealth.TakeDamage(attackDamage);
            }
        }
    }

    // DIPANGGIL VIA ANIMATION EVENT (di akhir animasi)
    public void OnAttackEnds()
    {
        isAttacking = false;
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }

    // Membantu kamu melihat area serangan di jendela Scene (Garis Merah)
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}