using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    public class DamagePlayer : MonoBehaviour
    {
        private int damage = 10;

        private void OnTriggerEnter(Collider other)
        {
            PlayerStats playerstats = other.GetComponent<PlayerStats>();

            if(playerstats != null) 
            {
                playerstats.TakeDamage(damage);
            }
        }
    }
}