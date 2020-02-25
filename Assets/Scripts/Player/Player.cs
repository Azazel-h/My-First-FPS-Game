using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using Photon.Pun;

namespace Yasuhiro.FPSGame {
    public class Player : MonoBehaviourPunCallbacks
    {
        #region Variables

        private Rigidbody player_rig; 
        public Camera playerCamera;
        public GameObject cameraParent;
        private Manager manager;
        private WeaponManager weapon;
        public LayerMask ground;
        public Transform groundDetector;
        public Transform weaponParent;
        private Vector3 weaponParentOrigin;
        private Vector3 targetWeaponBobPosition;
        private Transform ui_healthBar;
        private Text ui_ammo;
        private Vector3 slideDirection;
        private Vector3 originCamPos;
        private Vector3 weaponParentCurrentPosition;

        public float maxHealth;
        private float currentHealth;
        private float movementCounter;
        private float idleCounter;
        private float baseFOV;
        public float moveSpeed;
        public float jumpForce;
        public float sprintModifier;
        public float sprintFOVModifier = 1.25f;
        public float slideModifier;
        public float slideTime;
        private bool sliding;
        private float _slideTime;

        #endregion

        #region Monobehavior Callbacks

        private void Start()
        {
            cameraParent.SetActive(photonView.IsMine);
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            weapon = GetComponent<WeaponManager>();

            currentHealth = maxHealth;

            if (!photonView.IsMine) {
                gameObject.layer = 11;
            }

            baseFOV = playerCamera.fieldOfView;
            originCamPos = playerCamera.transform.localPosition;
            if (Camera.main) Camera.main.enabled = false;
            player_rig = GetComponent<Rigidbody>();
            weaponParentOrigin = weaponParent.localPosition;
            weaponParentCurrentPosition = weaponParentOrigin;
            if (photonView.IsMine) {
                ui_healthBar = GameObject.Find("HUD/Health/Bar").transform;
                ui_ammo = GameObject.Find("HUD/Ammo/Text").GetComponent<Text>();
                UpdateHealthBar();
                weapon.UpdateAmmoBar(ui_ammo);
            }

        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine) {
                return;
            }

            float _hMove = Input.GetAxisRaw("Horizontal");
            float _vMove = Input.GetAxisRaw("Vertical");
            bool _sprint = Input.GetKey(KeyCode.LeftShift);
            bool _jump = Input.GetKey(KeyCode.Space);
            bool _slide = Input.GetKey(KeyCode.LeftControl);

            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
            bool isJumping = _jump && isGrounded;
            bool isSprinting = _sprint && _vMove > 0 && isGrounded;
            bool isSliding = isSprinting && _slide && !sliding;

            if (isJumping) {
                player_rig.AddForce(Vector3.up * jumpForce);
            }

            if (Input.GetKeyDown(KeyCode.U)) {
                TakeDamage(10);
            }

            Vector3 _direction = Vector3.zero;
            float _adjSpeed = moveSpeed;

            if (!sliding) {
                _direction = new Vector3(_hMove, 0, _vMove);
                _direction.Normalize();
                _direction = transform.TransformDirection(_direction);
                if (isSprinting) _adjSpeed *= sprintModifier;

            } else {
               _direction = slideDirection;
               _adjSpeed *= slideModifier;
               _slideTime -= Time.fixedDeltaTime;
               if (_slideTime <= 0) {
                   sliding = false;
                   weaponParentCurrentPosition = weaponParentOrigin;
               }
            }

            Vector3 _targetVolocity = _direction * _adjSpeed * Time.fixedDeltaTime;
            _targetVolocity.y = player_rig.velocity.y;
            player_rig.velocity = _targetVolocity;

            if (isSliding) {
                sliding = true;
                slideDirection = _direction;
                _slideTime = slideTime;
                weaponParentCurrentPosition += Vector3.down * Time.fixedDeltaTime;
            }

            if (sliding) {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, baseFOV * (sprintFOVModifier + 0.3f), Time.fixedDeltaTime * 8f);
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originCamPos + Vector3.down * 0.2f, Time.fixedDeltaTime * 6f);
            } else {
                    if (isSprinting) {
                        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, baseFOV * sprintFOVModifier, Time.fixedDeltaTime * 8f);
                    } else {
                        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, baseFOV, Time.fixedDeltaTime * 8f);
                    }
                    playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originCamPos, Time.fixedDeltaTime * 6f);
            }

            if (_hMove == 0 && _vMove == 0) {
                HeadBob(idleCounter, 0.015f, 0.015f);
                idleCounter += Time.deltaTime;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
            } else if (isSprinting) {
                HeadBob(movementCounter, 0.15f, 0.05f);
                movementCounter += Time.deltaTime * 7f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f);
            } else {
                HeadBob(movementCounter, 0.035f, 0.035f);
                movementCounter += Time.deltaTime * 3f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
            }
            UpdateHealthBar();
            weapon.UpdateAmmoBar(ui_ammo);
        }
        #endregion
    
        #region  Private Methods

            private void HeadBob(float p_z, float p_xIntensity, float p_yIntensity) {
                targetWeaponBobPosition = weaponParentCurrentPosition + new Vector3(Mathf.Cos(p_z) * p_xIntensity, Mathf.Sin(p_z * 2) * p_yIntensity, 0);
            }

            private void UpdateHealthBar() {
                float _healthRatio = (float)currentHealth / (float)maxHealth;
                ui_healthBar.localScale = Vector3.Lerp(ui_healthBar.localScale , new Vector3 (_healthRatio, 1, 1), Time.deltaTime * 9f);
            }

        #endregion

        #region Public Methods

        public void TakeDamage(int p_damage) {
            if (photonView.IsMine) {
                currentHealth -= p_damage;
                Debug.Log(currentHealth);
                UpdateHealthBar();

                if (currentHealth <= 0) {
                    manager.Spawn();
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }

        #endregion
    }
}