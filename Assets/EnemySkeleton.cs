using UnityEngine;

public class EnemySkeleton : MonoBehaviour
{
    public float walkSpeed = 3f;
    public Transform heroKnight;
    public float attackDistance = 1f;
    public Animator animator;

    private Rigidbody2D rb;
    private bool isFollowing = false;
    private bool isAttacking = false;
    private bool isWalking = false;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>(); // Lấy Circle Collider2D của Skeleton
    }

    private void Update()
    {
        // Nếu đang follow và không attack
        if (isFollowing && !isAttacking)
        {
            MoveTowardsHeroKnight();
            isWalking = true; // Đang di chuyển khi đang theo sau
        }
        else
        {
            rb.velocity = Vector2.zero;
            isWalking = false; // Ngừng di chuyển khi không follow hoặc đang attack
        }

        animator.SetBool("isWalking", isWalking); // Cập nhật animation di chuyển

        // Nếu đang attack và khoảng cách với HeroKnight lớn hơn attackDistance
        if (isAttacking && Vector2.Distance(transform.position, heroKnight.position) > attackDistance)
        {
            EndAttack(); // Kết thúc attack
        }
    }

    private void MoveTowardsHeroKnight()
    {
        if (heroKnight != null)
        {
            // Kiểm tra khoảng cách giữa Skeleton và HeroKnight
            float distance = Vector2.Distance(circleCollider.bounds.center, heroKnight.position);

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
    public void TakeHit()
    {
        animator.SetTrigger("takeHit");
    }

}
