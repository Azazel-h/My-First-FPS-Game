using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Yasuhiro.FPSGame
{
    public class Look : MonoBehaviour
    {
        public static bool cursorIsLocked = true;
        public Transform player;
        public Transform cams;
        public float Sensivity;
        private float maxAngle = 90;
        private Quaternion camCentre;
        void Start()
        {
            camCentre = cams.localRotation;
        }

        void Update()
        {
            SetY();
            SetX();
            UpdateCursorLock();
        }

        void SetY() {
            float _input = Input.GetAxis("Mouse Y") * Sensivity * Time.deltaTime;
            Quaternion _adj = Quaternion.AngleAxis(_input, Vector3.left);
            Quaternion _delta = cams.localRotation * _adj;

            if (Quaternion.Angle(camCentre, _delta) < maxAngle) {
                cams.localRotation = _delta;
            }
        }

        void SetX() {
            float _input = Input.GetAxis("Mouse X") * Sensivity * Time.deltaTime;
            Quaternion _adj = Quaternion.AngleAxis(_input, Vector3.up);
            Quaternion _delta = player.localRotation * _adj;

            player.localRotation = _delta;
        }

        void UpdateCursorLock() {
            if (cursorIsLocked) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (Input.GetKeyDown(KeyCode.Escape)) {
                    cursorIsLocked = false;
                }
            } else {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (Input.GetKeyDown(KeyCode.Escape)) {
                    cursorIsLocked = true;
                }
            }
        }
    }
}

