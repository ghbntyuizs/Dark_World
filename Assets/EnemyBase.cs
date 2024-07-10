using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float walkSpeed;
    public Transform heroKnight;
    public float attackDistance;
    public int attackDamage;
    public int health;
    public float attackCooldown;
    private float nextAttackTime;
    public Animator animator;
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider; // Thêm khai báo biến boxCollider
    private bool isFollowing = false;
    private bool isAttacking = false;
    private bool isDead = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>(); // Khởi tạo boxCollider
    }

    protected virtual void Update()
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (heroKnight != null && !heroKnight.GetComponent<HeroKnight>().IsDead())
        {
            float distanceX = Mathf.Abs(transform.position.x - heroKnight.position.x);

            if (isFollowing && !isAttacking)
            {
                if (distanceX <= attackDistance)
                {
                    if (Time.time >= nextAttackTime)
                    {
                        StartAttack();
                        nextAttackTime = Time.time + attackCooldown;
                    }
                }
                else
                {
                    MoveTowardsHeroKnight();
                    SetWalkingAnimation(true);
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
                SetWalkingAnimation(false);
            }

            if (isAttacking && distanceX > attackDistance)
            {
                EndAttack();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("takeHit");
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        animator.SetTrigger("die");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    public bool IsDead()
    {
        return isDead;
    }

    protected virtual void MoveTowardsHeroKnight()
    {
        if (heroKnight != null)
        {
            Vector2 direction = (heroKnight.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * walkSpeed, rb.velocity.y);

            if (direction.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    protected virtual void StartAttack()
    {
        if (isDead) return;
        rb.velocity = Vector2.zero; // Đặt vận tốc về 0 để kẻ thù đứng yên khi tấn công
        animator.SetTrigger("attack");
        isAttacking = true;
    }

    protected virtual void EndAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            animator.SetTrigger("endAttack");
        }
    }

    protected abstract void SetWalkingAnimation(bool isWalking);

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Player"))
        {
            heroKnight = other.transform;
            isFollowing = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isFollowing = false;
            isAttacking = false;
            rb.velocity = Vector2.zero;
            SetWalkingAnimation(false);
        }
    }

    public void AttackPlayer()
    {
        if (isDead) return;
        if (heroKnight != null && !heroKnight.GetComponent<HeroKnight>().IsDead() &&
           Mathf.Abs(transform.position.x - heroKnight.position.x) <= attackDistance)
        {
            HeroKnight hero = heroKnight.GetComponent<HeroKnight>();
            if (hero != null)
            {
                hero.TakeDamage(attackDamage);
            }
        }
    }

    public void OnAttackHit()
    {
        AttackPlayer();
    }
}
