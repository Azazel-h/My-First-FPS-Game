using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Yasuhiro.FPSGame {
    public class WeaponManager : MonoBehaviour
    {

        #region Variables

        public Gun[] loadout;
        public Transform weaponParent;
        private GameObject currentWeapon; 

        #endregion

        #region Monobehavior Callbacks

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
        }

        #endregion

        #region Private Methods
        
        void Equip(int p_ind) {
            if (currentWeapon != null) Destroy(currentWeapon);
            GameObject _newEquipment = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            _newEquipment.transform.localPosition = Vector3.zero;
            _newEquipment.transform.localEulerAngles = Vector3.zero;
            currentWeapon = _newEquipment;
        }

        #endregion
    }
}
