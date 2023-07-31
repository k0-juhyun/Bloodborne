using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace bloodborne
{
    public class UiInterAction : MonoBehaviour
    {
        PlayerLocomotion playerLocomotion;
        public RawImage potionImageMask;

        private void Start()
        {
            playerLocomotion = FindObjectOfType<PlayerLocomotion>();
        }
        void Update()
        {
            if (playerLocomotion.canDrinkPotion == false)
            {
                potionImageMask.gameObject.SetActive(true);
            }
            else
                potionImageMask.gameObject.SetActive(false);
        }
    }
}