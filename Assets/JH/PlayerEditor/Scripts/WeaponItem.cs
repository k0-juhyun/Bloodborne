using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Idle Animation")]
        public string right_Hand_Idle;
        public string left_Hand_Idle;

        [Header("One Hand Light Attack Animation")]
        public string oneHandLightAttack1;
        public string oneHandLightAttack2;

        [Header("One Hand Heavy Attack Animation")]
        public string oneHandHeavyAttack1;
        public string oneHandHeavyAttack2;

        [Header("Stamina Costs")]
        public int baseStamina;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;
    }
}
