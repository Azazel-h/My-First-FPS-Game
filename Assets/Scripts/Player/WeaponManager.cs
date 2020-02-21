using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace Yasuhiro.FPSGame {
    public class WeaponManager : MonoBehaviourPunCallbacks
    {

        #region Variables

        public Gun[] loadout;
        public Transform weaponParent;
        private GameObject currentWeapon; 
        public GameObject bulletHolePrefab;
        public LayerMask canBeShoot;

        private float currentCoolDown;
        private int currentIndex;

        #endregion

        #region Monobehavior Callbacks

        void Update()
        {
            if (!photonView.IsMine) {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
            if (currentWeapon != null) {
                Aim(Input.GetMouseButton(1));
                if (Input.GetMouseButtonDown(0) && currentCoolDown <= 0) {
                    Shoot();
                }

                currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
                if (currentCoolDown > 0) currentCoolDown -= Time.deltaTime;
            }
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

        void Shoot() {
            Transform _spawn = transform.Find("Cameras/PlayerCamera");

            Vector3 _bloom = _spawn.position + _spawn.forward * 100f;
            _bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * _spawn.up;
            _bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * _spawn.right;
            _bloom -= _spawn.position;
            _bloom.Normalize();

            RaycastHit _hit = new RaycastHit();

            if (Physics.Raycast(_spawn.position, _bloom, out _hit, 1000f, canBeShoot)) {
                GameObject _newHole = Instantiate(bulletHolePrefab, _hit.point + _hit.normal * 0.001f, Quaternion.identity) as GameObject;
                _newHole.transform.LookAt(_hit.point + _hit.normal);
                Destroy(_newHole, 5f);
            }

            currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
            currentCoolDown = loadout[currentIndex].firerate;
        }

        #endregion
    }
}
