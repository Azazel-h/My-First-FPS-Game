using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Yasuhiro.FPSGame
{
    public class Sway : MonoBehaviourPunCallbacks
    {
        #region Variables
        
        public float intensity;
        public float smooth;
        private Quaternion originRotation;

        #endregion

        #region Monobehavior Callbacks

        private void Start()
        {
            originRotation = transform.localRotation;
        }

        private void Update()
        {
            if (!photonView.IsMine) {
                return;
            }
            UpdateSway();
        }

        #endregion

        #region Private Methods

        private void UpdateSway() {
            float _xMouse = Input.GetAxis("Mouse X");
            float _yMouse = Input.GetAxis("Mouse Y");
            Quaternion _xAdj = Quaternion.AngleAxis(-intensity * _xMouse, Vector3.up);
            Quaternion _yAdj = Quaternion.AngleAxis(intensity * _yMouse, Vector3.right);
            Quaternion _targetRotation = originRotation * _xAdj * _yAdj;

            transform.localRotation = Quaternion.Lerp(transform.localRotation , _targetRotation, Time.deltaTime * smooth);
        }

        #endregion
    }
}
