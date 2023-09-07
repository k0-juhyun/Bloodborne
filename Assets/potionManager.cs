using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace bloodborne
{
    public class potionManager : MonoBehaviour
    {
        PlayerStats playerStats;
        public Text potionAmountText;

        private void Start()
        {
            playerStats = FindObjectOfType<PlayerStats>();
        }

        void Update()
        {
            potionAmountText.text = playerStats.potionAmount.ToString();
        }
    }
}