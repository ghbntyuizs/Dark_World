using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class holeDame : MonoBehaviour
    {
        // Start is called before the first frame update
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("collision detected with: " + collision.gameObject.name);
            if (collision.CompareTag("Player"))
            {
                HeroKnight hero = collision.gameObject.GetComponent<HeroKnight>();
                if (hero != null)
                {
                    hero.Die();
                }
                {

                }
            }
        }
    }
}