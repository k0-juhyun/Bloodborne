using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;

        public GameObject currentWeaponModel;

        public void UnloadWeapon()
        {
            if (currentWeaponModel != null) 
            {
                currentWeaponModel.SetActive(false);
            }
        }

        public void UnloadWeaponAndDestroy()
        {
            if(currentWeaponModel != null) 
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeaponModel(WeaponItem weaponItem)
        {
            UnloadWeaponAndDestroy();

            if(weaponItem == null) 
            {
                UnloadWeapon();
                return;
            }

            GameObject model = Instantiate(weaponItem.modelPrefab) as GameObject;
            if(model != null ) 
            {
                if(parentOverride != null) 
                {
                    model.transform.parent = parentOverride;
                }
                else
                {
                    model.transform.parent = transform;
                }

                //model.transform.localScale = Vector3.zero;
                model.transform.position = model.transform.parent.position;
                model.transform.localRotation = Quaternion.identity;
            }

            currentWeaponModel = model;
        }
    }
}
