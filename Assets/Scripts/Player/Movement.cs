using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yasuhiro.FPSGame {
    public class Movement : MonoBehaviour
    {
        // Start is called before the first frame update
        private Rigidbody player_rig; 
        public float moveSpeed;
        void Start()
        {
            Camera.main.enabled = false;
            player_rig = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            float _hMove = Input.GetAxisRaw("Horizontal");
            float _vMove = Input.GetAxisRaw("Vertical");
            Vector3 _direction = new Vector3(_hMove, 0, _vMove);
            _direction.Normalize();
            player_rig.velocity = transform.TransformDirection(_direction) * moveSpeed * Time.fixedDeltaTime;
        }
    }
}