using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Import namespace này

public class HeroKnight : MonoBehaviour
{
    public GameManagement gameManager;
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] int maxHealth = 6;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask enemyLayer;
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
    private FireBoss fireBoss;
    public int currentHealth;
    private bool isDead = false;
    private List<EnemyBase> enemiesInRange = new List<EnemyBase>();
    private List<FireBoss> bossesInRange = new List<FireBoss>();
    public GameManagement gameManger;
    public ThanhMau thanhmau;

    // Use this for initialization
    private bool isBlocking = false;
    private int currentDamage;
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
        fireBoss = FindObjectOfType<FireBoss>();
        currentHealth = maxHealth;
        currentDamage = attackDamage;
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

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
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

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            Die();  // Thay đổi từ Invoke thành trực tiếp gọi Die()
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        else if (Input.GetKeyDown("j") && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;

            // Trigger TakeHit on enemySkeleton
            /*if (enemySkeleton != null)
            {
                enemySkeleton.TakeHit();
            }*/
        }

        // Block
        else if (Input.GetKeyDown("k") && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            isBlocking = true; // Bật trạng thái block
        }
        else if (Input.GetMouseButtonUp(1) || Input.GetKeyUp("k"))
        {
            m_animator.SetBool("IdleBlock", false);
            isBlocking = false; // Tắt trạng thái block
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
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

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.

    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            if (isBlocking)
            {
                Debug.Log("Blocked the damage!");
                return; // Chặn damage
            }

            currentHealth -= damage;
            if (thanhmau != null)
            {
                thanhmau.capNhatThanhMau(currentHealth, maxHealth);
            }

            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }
            else
            {
                m_animator.SetTrigger("Hurt");
            }
        }
    }

    public void Die()
    {
        isDead = true;
        m_animator.SetTrigger("Death");
        gameManager.gameOver();

        // Bắt đầu Coroutine để chuyển đến scene GameOver sau 0.7 giây
        StartCoroutine(LoadGameOverScene());
    }

    private IEnumerator LoadGameOverScene()
    {
        // Đợi 0.7 giây trước khi chuyển đến scene GameOver
        yield return new WaitForSeconds(0.7f);

        // Tải scene GameOver
        SceneManager.LoadScene("GameOver");
    }

    public bool IsDead()
    {
        return isDead;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
        else if (other.CompareTag("Boss"))
        {
            FireBoss boss = other.GetComponent<FireBoss>();
            if (boss != null && !bossesInRange.Contains(boss))
            {
                bossesInRange.Add(boss);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
        else if (other.CompareTag("Boss"))
        {
            FireBoss boss = other.GetComponent<FireBoss>();
            if (boss != null && bossesInRange.Contains(boss))
            {
                bossesInRange.Remove(boss);
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
                enemy.TakeDamage(currentDamage);
            }
        }
        foreach (var fireBoss in bossesInRange)
        {
            fireBoss.TakeDamage(currentDamage);
        }
    }
    public void IncreaseDamage(int amount)
    {
        currentDamage += amount;
        Debug.Log("HeroKnight's damage increased to: " + currentDamage);
        // Thực hiện các hành động khác khi tăng damage, nếu cần
    }
    public void Heal(int amount)
    {
        if (!isDead)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth; // Đảm bảo không vượt quá máu tối đa
            }
            Debug.Log("Healed! Current Health: " + currentHealth);

            if (thanhmau != null)
            {
                thanhmau.capNhatThanhMau(currentHealth, maxHealth);
            }
        }
    }
    public bool CanHeal()
    {
        return currentHealth < maxHealth;
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
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}
    