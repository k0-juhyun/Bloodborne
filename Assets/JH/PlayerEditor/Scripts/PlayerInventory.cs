using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;

        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start()
        {
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon,false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon,true);
        }

        //public void ChangeRightWeapon()
        //{
        //    currentRightWeaponIndex = currentRightWeaponIndex + 1;

        //    if (currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
        //    {
        //        currentRightWeaponIndex = -1;
        //        rightWeapon = unarmedWeapon;
        //        weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
        //    }
        //    else if (weaponsInRightHandSlots[currentRightWeaponIndex] != null)
        //    {
        //        rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
        //        weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
        //    }
        //    else
        //    {
        //        currentRightWeaponIndex = currentRightWeaponIndex + 1;
        //    }
        //}
    }
}