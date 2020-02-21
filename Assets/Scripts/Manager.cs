using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Yasuhiro.FPSGame {
    public class Manager : MonoBehaviour
    {
        public string playerPrefab;
        public Transform spawnPoint;

        private void Start() {
            Spawn();
        }

        private void Spawn() {
            PhotonNetwork.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}