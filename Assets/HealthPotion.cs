using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healAmount = 2; // Lượng máu hồi phục
    private Transform targetEnemy; // Vị trí của quái vật

    // Thiết lập vị trí của quái vật
    public void SetTargetEnemy(Transform enemyTransform)
    {
        targetEnemy = enemyTransform;
    }

    private void Start()
    {
        if (targetEnemy != null)
        {
            // Di chuyển đến vị trí của quái vật
            transform.position = targetEnemy.position;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Heroknight không
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tìm script quản lý máu của Heroknight
            HeroKnight heroknightHealth = collision.GetComponent<HeroKnight>();

            if (heroknightHealth != null && heroknightHealth.CanHeal())
            {
                // Gọi hàm hồi máu
                heroknightHealth.Heal(healAmount);
                // Hủy đối tượng bình máu sau khi hồi máu
                Destroy(gameObject);
            }
        }
    }
}
