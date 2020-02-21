using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Yasuhiro.FPSGame {
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ScriptableObject
    {
        public string gunName;
        public float firerate;
        public float bloom;
        public float kickback;
        public float recoil;
        public float aimSpeed;
        public GameObject prefab;
    }
}