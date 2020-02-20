using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Yasuhiro.FPSGame {
    public class WeaponManager : MonoBehaviour
    {
        public Gun[] loadout;
        public Transform weaponParent;
        private GameObject currentWeapon;        void Start()
        {
            
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
        }

        void Equip(int p_ind) {
            if (currentWeapon != null) Destroy(currentWeapon);
            GameObject _newEquipment = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            _newEquipment.transform.localPosition = Vector3.zero;
            _newEquipment.transform.localEulerAngles = Vector3.zero;
            currentWeapon = _newEquipment;
        }
    }
}
