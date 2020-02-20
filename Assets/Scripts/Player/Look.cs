using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Yasuhiro.FPSGame
{
    public class Look : MonoBehaviour
    {
        public Transform player;
        public Transform cams;
        public float xSensivity;
        public float ySensivity;
        private float maxAngle = 90;
        void Start()
        {

        }

        void Update()
        {
            SetY();
        }

        void SetY() {
            float _input = Input.GetAxis("Mouse Y") * ySensivity * Time.deltaTime;
            Quaternion _adj = Quaternion.AngleAxis(_input, -Vector3.right);
            Quaternion _delta = cams.localRotation * _adj;
            cams.localRotation = _delta;
        }
    }
}

