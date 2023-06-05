using System;
using System.Collections.Generic;
using UnityEngine;

namespace BobIO
{
    public class GameManager : MonoBehaviour
    {
        public Action<Player> NewPlayerConnected;
        //public Action GameStarts;
        //public Action GameEnds;

        public List<Player> PlayersList = new List<Player>();

        private void Awake()
        {
            Init();
        }

        private void Init()
        {

            NewPlayerConnected += OnNewPlayerConnected;
        }

        public void StartGame()
        {

        }

        private void OnNewPlayerConnected(Player newPlayer)
        {
            PlayersList.Add(newPlayer);
        }
    }
}