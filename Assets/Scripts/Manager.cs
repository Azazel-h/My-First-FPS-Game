using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Yasuhiro.FPSGame {
    public class Manager : MonoBehaviour
    {
        public string playerPrefab;
        public Transform[] spawnPoints;

        private void Start() {
            Spawn();
        }

        public void Spawn() {
            Transform _spawn = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
            PhotonNetwork.Instantiate(playerPrefab, _spawn.position, _spawn.rotation);
        }
    }
}