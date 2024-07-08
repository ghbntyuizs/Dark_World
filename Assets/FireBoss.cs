using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoss : MonoBehaviour
{
    // Start is called before the first frame update
    public float walkSpeed = 3.5f;
    public Transform heroKnight;
    public float attackDistance = 3f;
    public int attackDamage = 2;
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int currentHealth;
    public float attackCooldown = 1.5f;
    private float nextAttackTime;
    public Animator animator;
    private Rigidbody2D rb;
    private bool isFollowing = false;
    private bool isAttacking = false;
    private bool isRun = false;
    private BoxCollider2D boxCollider;
    private bool isDead = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            isRun = false;
            isFollowing = false;
            isAttacking = false;
            animator.SetBool("isRun", isRun);
            return;
        }
        else

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
                    isRun = true;
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
                isRun = false;
            }

            animator.SetBool("isRun", isRun);

            if (isAttacking && distanceX > attackDistance)
            {
                EndAttack();
            }
        }
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("takeHit");
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("die");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 1.5f);
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void MoveTowardsHeroKnight()
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

    private void StartAttack()
    {
        if (isDead) return;
        rb.velocity = Vector2.zero; // Đặt vận tốc về 0 để Skeleton đứng yên khi tấn công
        animator.SetTrigger("attack");
        isAttacking = true;
    }

    public void EndAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            animator.SetTrigger("endAttack");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Player"))
        {
            heroKnight = other.transform;
            isFollowing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isFollowing = false;
            isAttacking = false;
            rb.velocity = Vector2.zero;
            isRun = false;
            animator.SetBool("isRun", isRun);
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    public void OnAttackHit()
    {
        AttackPlayer();
    }
}
