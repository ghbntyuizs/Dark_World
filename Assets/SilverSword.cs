using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilverSword : MonoBehaviour
{
    public int dameAmount = 1;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Lấy component HeroKnight từ GameObject Player
            HeroKnight heroKnight = other.GetComponent<HeroKnight>();

            // Kiểm tra nếu đã có HeroKnight và thực hiện tăng damage
            if (heroKnight != null)
            {
                heroKnight.IncreaseDamage(dameAmount); // Tăng damage của HeroKnight lên 1
                Destroy(gameObject); // Hủy GameObject SilverSword sau khi nhặt được
            }
        }
    }
}
