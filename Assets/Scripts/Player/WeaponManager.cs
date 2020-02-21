using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Yasuhiro.FPSGame {
    public class WeaponManager : MonoBehaviour
    {

        #region Variables

        public Gun[] loadout;
        public Transform weaponParent;
        public GameObject currentWeapon; 
        private int currentIndex;

        #endregion

        #region Monobehavior Callbacks

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
            if (currentWeapon != null) Aim(Input.GetMouseButton(1));
        }

        #endregion

        #region Private Methods

        void Equip(int p_ind) {
            if (currentWeapon != null) Destroy(currentWeapon);
            currentIndex = p_ind;
            GameObject _newEquipment = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            _newEquipment.transform.localPosition = Vector3.zero;
            _newEquipment.transform.localEulerAngles = Vector3.zero;
            currentWeapon = _newEquipment;
        }

        void Aim(bool p_isAiming) {
            Transform _anchor = currentWeapon.transform.Find("Anchor");
            Transform _stateADS = currentWeapon.transform.Find("States/ADS");
            Transform _stateHip = currentWeapon.transform.Find("States/Hip");

            if (p_isAiming) {
                _anchor.position = Vector3.Lerp(_anchor.position, _stateADS.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            } else {
                _anchor.position = Vector3.Lerp(_anchor.position, _stateHip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            }
        }

        #endregion
    }
}
