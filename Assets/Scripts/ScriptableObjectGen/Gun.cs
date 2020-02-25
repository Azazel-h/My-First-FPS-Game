using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Yasuhiro.FPSGame {
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ScriptableObject
    {
        public string gunName;
        public float firerate;
        public int damage;
        public int ammo;
        public int clipsize;
        public float reloadTime;
        public float bloom;
        public float kickback;
        public float recoil;
        public float aimSpeed;
        public GameObject prefab;

        private int stash;
        private int clip;

        public void Initialize() {
            stash = ammo;
            clip = clipsize;
        }

        public bool FireBullet() {
            if (clip > 0) {
                clip -= 1;
                return true;
            }
            return false;
        }

        public void ReLoad() {
            stash += clip;
            clip = Mathf.Min(clipsize, stash);
            stash -= clip;
        }

        public int GetStash() {
            return stash;
        }

        public int GetClip() {
            return clip;
        }
    }
}