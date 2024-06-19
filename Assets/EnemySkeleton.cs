using UnityEngine;

public class EnemySkeleton : MonoBehaviour
{
    public float walkSpeed = 3f;
    public Transform heroKnight;
    public float attackDistance = 1f;
    public int attackDamage = 1;
    public int health = 5;
    public float attackCooldown = 1.5f;
    private float nextAttackTime;
    public Animator animator;
    private Rigidbody2D rb;
    private bool isFollowing = false;
    private bool isAttacking = false;
    private bool isWalking = false;
    private BoxCollider2D boxCollider;
    private bool isDead = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>(); // Lấy Circle Collider2D của Skeleton
    }

    private void Update()
    {
        if (isDead) // Kiểm tra xem đã chết chưa
        {
            rb.velocity = Vector2.zero;
            isWalking = false;
            isFollowing = false;
            isAttacking = false;
            animator.SetBool("isWalking", isWalking);
            return; // Thoát khỏi update nếu đã chết
        }

        if (heroKnight != null && !heroKnight.GetComponent<HeroKnight>().IsDead())
        {
            // Nếu đang follow và không attack
            if (isFollowing && !isAttacking)
            {
                MoveTowardsHeroKnight();
                isWalking = true;
            }
            else
            {
                rb.velocity = Vector2.zero;
                isWalking = false;
            }

            animator.SetBool("isWalking", isWalking);

            // Nếu đang attack và khoảng cách với HeroKnight nhỏ hơn hoặc bằng attackDistance
            if (isAttacking && Vector2.Distance(transform.position, heroKnight.position) <= attackDistance)
            {
                // Kiểm tra xem đã đến thời điểm tấn công tiếp theo chưa
                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                    nextAttackTime = Time.time + attackCooldown; // Cập nhật thời gian tấn công tiếp theo
                }
            }
            else if (isAttacking && Vector2.Distance(transform.position, heroKnight.position) > attackDistance)
            {
                EndAttack(); // Kết thúc tấn công nếu khoảng cách với HeroKnight lớn hơn attackDistance
            }
        }
        else
        {
            // Hero đã chết hoặc không còn tồn tại, ngừng mọi hành động của Skeleton
            rb.velocity = Vector2.zero;
            isWalking = false;
            isFollowing = false;
            isAttacking = false;
            animator.SetBool("isWalking", isWalking);
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

    private void Die()
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

    private void MoveTowardsHeroKnight()
    {
        if (heroKnight != null)
        {
            // Kiểm tra khoảng cách giữa Skeleton và HeroKnight
            float distance = Vector2.Distance(transform.position, heroKnight.position);

            if (distance <= attackDistance)
            {
                StartAttack(); // Nếu trong khoảng attackDistance thì attack
            }
            else
            {
                // Di chuyển về phía HeroKnight
                Vector2 direction = (heroKnight.position - transform.position).normalized;
                rb.velocity = new Vector2(direction.x * walkSpeed, rb.velocity.y);

                // Đảo chiều hình dạng nếu cần
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
    }

    private void StartAttack()
    {
        if (isDead) return;
        isAttacking = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("attack");
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetTrigger("endAttack"); // Kích hoạt trigger kết thúc attack
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
            isWalking = false;
            animator.SetBool("isWalking", isWalking); // Cập nhật animation di chuyển
        }
    }
    public void AttackPlayer()
    {
        if (isDead) return;
        if (heroKnight != null && !heroKnight.GetComponent<HeroKnight>().IsDead() &&
           Vector2.Distance(transform.position, heroKnight.position) <= attackDistance)
        {
            HeroKnight hero = heroKnight.GetComponent<HeroKnight>();
            if (hero != null)
            {
                hero.TakeDamage(attackDamage);
            }
        }
    }

    // Gọi phương thức AttackPlayer từ Animation Event trong animation attack của Skeleton
    public void OnAttackHit()
    {
        AttackPlayer();
    }
}
