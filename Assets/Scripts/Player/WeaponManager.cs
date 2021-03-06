﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
        private bool isReloading;

        #endregion

        #region Monobehavior Callbacks


        private void Start() {
                foreach(Gun a in loadout) {
                    a.Initialize();
                }
                Equip(0);
        }

        void Update()
        {
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1)) {
                photonView.RPC("Equip", RpcTarget.All, 0);
            }

            if (currentWeapon != null) {
                if (photonView.IsMine) {
                    Aim(Input.GetMouseButton(1));
                    if (Input.GetMouseButtonDown(0) && currentCoolDown <= 0) {
                        if (!isReloading) {
                            if (loadout[currentIndex].FireBullet()) photonView.RPC("Shoot", RpcTarget.All);
                            else if (loadout[currentIndex].GetStash() > 0) StartCoroutine(Reload(loadout[currentIndex].reloadTime));
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.R) && loadout[currentIndex].GetStash() > 0) {
                        StartCoroutine(Reload(loadout[currentIndex].reloadTime));
                    }

                    currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
                    if (currentCoolDown > 0) currentCoolDown -= Time.deltaTime;
                }
            }
        }

        #endregion

        #region Private Methods

        IEnumerator Reload(float p_wait) {
            isReloading = true;
            currentWeapon.SetActive(false);

            yield return new WaitForSeconds(p_wait);
            loadout[currentIndex].ReLoad();

            isReloading = false;
            currentWeapon.SetActive(true);
        }


        [PunRPC]
        void Equip(int p_ind) {
            if (currentWeapon != null) {
                if (isReloading) StopCoroutine("Reload");
                Destroy(currentWeapon);
            }
            currentIndex = p_ind;
            GameObject _newEquipment = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            _newEquipment.transform.localPosition = Vector3.zero;
            _newEquipment.transform.localEulerAngles = Vector3.zero;
            _newEquipment.GetComponent<Sway>().isMine = photonView.IsMine;
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
        [PunRPC]
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

                if (photonView.IsMine) {
                    if (_hit.collider.gameObject.layer == 11) {
                        _hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage);
                    }
                }
            }

            currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
            currentCoolDown = loadout[currentIndex].firerate;
        }

        [PunRPC]
        private void TakeDamage(int p_damage) {
            GetComponent<Player>().TakeDamage(p_damage);
        }

        #endregion

        #region Public Methods

        public void UpdateAmmoBar(Text p_uiAmmoBar) {
            p_uiAmmoBar.text = loadout[currentIndex].GetClip().ToString("00") + "/ " + loadout[currentIndex].GetStash().ToString("00");
        }

        #endregion
    }
}
