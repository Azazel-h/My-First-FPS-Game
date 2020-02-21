using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Yasuhiro.FPSGame {
    public class Movement : MonoBehaviour
    {
        #region Variables

        private Rigidbody player_rig; 
        public Camera playerCamera;
        public LayerMask ground;
        public Transform groundDetector;
        public Transform weaponParent;
        private Vector3 weaponParentOrigin;
        private Vector3 targetWeaponBobPosition;

        private float movementCounter;
        private float idleCounter;
        private float baseFOV;
        public float moveSpeed;
        public float jumpForce;
        public float sprintModifier;
        public float sprintFOVModifier = 1.25f;

        #endregion

        #region Monobehavior Callbacks

        void Start()
        {
            baseFOV = playerCamera.fieldOfView;
            Camera.main.enabled = false;
            player_rig = GetComponent<Rigidbody>();
            weaponParentOrigin = weaponParent.localPosition;
        }

        void FixedUpdate()
        {
            float _hMove = Input.GetAxisRaw("Horizontal");
            float _vMove = Input.GetAxisRaw("Vertical");
            bool _sprint = Input.GetKey(KeyCode.LeftShift);
            bool _jump = Input.GetKey(KeyCode.Space);

            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
            bool isJumping = _jump && isGrounded;
            bool isSprinting = _sprint && _vMove > 0 && isGrounded;

            if (isJumping) {
                player_rig.AddForce(Vector3.up * jumpForce);
            }

            Vector3 _direction = new Vector3(_hMove, 0, _vMove);
            _direction.Normalize();

            float _adjSpeed = moveSpeed;
            if (isSprinting) _adjSpeed *= sprintModifier;
            Vector3 _targetVolocity = transform.TransformDirection(_direction) * _adjSpeed * Time.fixedDeltaTime;
            _targetVolocity.y = player_rig.velocity.y;
            player_rig.velocity = _targetVolocity;

            if (isSprinting) {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, baseFOV * sprintFOVModifier, Time.fixedDeltaTime * 8f);
            } else {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, baseFOV, Time.fixedDeltaTime * 8f);
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
        }
        #endregion
    
        #region  Private Methods

            void HeadBob(float p_z, float p_xIntensity, float p_yIntensity) {
                targetWeaponBobPosition = weaponParentOrigin + new Vector3(Mathf.Cos(p_z) * p_xIntensity, Mathf.Sin(p_z * 2) * p_yIntensity, 0);
            }

        #endregion
    }
}