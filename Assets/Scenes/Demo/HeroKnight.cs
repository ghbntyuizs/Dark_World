using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroKnight : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] int maxHealth = 6;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform respawnPoint; // Thêm biến để lưu vị trí respawn
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;
    private EnemySkeleton enemySkeleton;
    public int currentHealth;
    private bool isDead = false;
    private List<EnemySkeleton> enemiesInRange = new List<EnemySkeleton>();
    public GameManagement gameManger;
    public ThanhMau thanhmau;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        enemySkeleton = FindObjectOfType<EnemySkeleton>();
        currentHealth = maxHealth;

        if (thanhmau != null)
        {
            thanhmau.capNhatThanhMau(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogError("Thanh mau chưa được gán trong Inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            m_body2d.velocity = Vector2.zero; // Đảm bảo nhân vật đứng yên
            return;
        }

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        float inputX = Input.GetAxis("Horizontal");

        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            Die();  // Thay đổi từ Invoke thành trực tiếp gọi Die()
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        else if (Input.GetKeyDown("j") && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            if (m_currentAttack > 3)
                m_currentAttack = 1;

            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);

            m_timeSinceAttack = 0.0f;
        }
        else if (Input.GetKeyDown("k") && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;
            if (thanhmau != null)
            {
                thanhmau.capNhatThanhMau(currentHealth, maxHealth);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                m_animator.SetTrigger("Hurt");
            }
        }
    }

    private void Die()
    {
        isDead = true;
        m_animator.SetTrigger("Death");

        // Bắt đầu Coroutine để hồi sinh sau 0.7 giây
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        // Đợi 0.7 giây trước khi hồi sinh
        yield return new WaitForSeconds(0.7f);

        Respawn();
    }

    private void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogWarning("Respawn point is not assigned!");
            return;
        }

        // Khôi phục trạng thái ban đầu
        currentHealth = maxHealth;
        isDead = false;
        transform.position = respawnPoint.position; // Di chuyển nhân vật đến vị trí respawn

        // Khôi phục trạng thái Animator
        m_animator.SetTrigger("Respawn"); // Kích hoạt animation hồi sinh nếu có
        m_body2d.isKinematic = false;
        m_body2d.simulated = true;

        // Khôi phục trạng thái nhân vật
        m_animator.SetBool("Grounded", true);
        m_animator.SetInteger("AnimState", 0);
        m_rolling = false;  // Đặt lại trạng thái rolling
        m_facingDirection = 1;  // Đặt lại hướng nhân vật sang phải

        // Khôi phục hướng của nhân vật
        GetComponent<SpriteRenderer>().flipX = false;
    }

    public bool IsDead()
    {
        return isDead;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemySkeleton enemy = other.GetComponent<EnemySkeleton>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemySkeleton enemy = other.GetComponent<EnemySkeleton>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }

    public void OnAttackHit()
    {
        float maxAllowedYDifference = 1.0f;

        foreach (var enemy in enemiesInRange)
        {
            if (Mathf.Abs(transform.position.y - enemy.transform.position.y) <= maxAllowedYDifference)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}
