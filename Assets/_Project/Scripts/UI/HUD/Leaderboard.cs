using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace BobIO
{
    public class Leaderboard : NetworkBehaviour
    {
        [SerializeField] private LeaderboardMember _memberPrefab;

        [SerializeField] private RectTransform _membersLayoutGroupTransform;
        [SerializeField] private LayoutGroup _membersLayoutGroup;

        private List<LeaderboardMember> _membersList;

        private void Start()
        {
            Init();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedServerRpc;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectServerRpc;
            }
        }

        private void Init()
        {
            _membersList = new List<LeaderboardMember>();
            ClearAllMembers();

            //NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedServerRpc;
            //NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectServerRpc;

            if (IsClient)
            {
                
            }
            else
            {
                //NetworkManager.Singleton.OnServerStarted += OnServerStartedServerRpc;
            }

            StartCoroutine(DoSortingWithDelay());
        }

        private void ClearAllMembers()
        {
            _membersList.Clear();

            foreach (Transform child in _membersLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void Update()
        {
            
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnClientConnectedServerRpc(ulong clientId)
        {
            if (!IsServer || !IsHost)
                return;

            NetworkObjectReference[] allPlayers = new NetworkObjectReference[NetworkManager.ConnectedClientsList.Count];

            for (int i = 0; i < allPlayers.Length; i++)
            {
                allPlayers[i] = NetworkManager.ConnectedClientsList[i].PlayerObject;
            }

            UpdateMembersListClientRpc(allPlayers);

            Debug.Log($"ClientId: {clientId} connected");

            //Player newPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();

            //NetworkObjectReference playerReference = new NetworkObjectReference(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject);
            //AddNewMemberClientRpc(playerReference);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnClientDisconnectServerRpc(ulong clientId)
        {
            if (!IsServer || !IsHost)
                return;
            //if (clientId != NetworkManager.Singleton.LocalClientId)
            //    return;

            Debug.Log($"ClientId: {clientId} disconnected");
            DeleteMemberClientRpc(clientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnServerStartedServerRpc()
        {
            if (!IsServer || !IsHost)
                return;

            Debug.Log($"OnServerStarted AddNewPlayer");
            Player newPlayer = NetworkManager.LocalClient.PlayerObject.GetComponent<Player>();

            //AddNewMemberClientRpc(new NetworkObjectReference(newPlayer.NetworkObject));
        }

        [ClientRpc]
        private void AddNewMemberClientRpc(NetworkObjectReference playerReference)
        {
            playerReference.TryGet(out var playerNetworkObject);
            Player newPlayer = playerNetworkObject.GetComponent<Player>();
            //Player newPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();

            LeaderboardMember newMember = Instantiate(_memberPrefab, _membersLayoutGroup.transform);

            Debug.Log($"{newMember} - {newPlayer}");

            newMember.SetPlayer(newPlayer);
            _membersList.Add(newMember);
        }

        [ClientRpc]
        private void DeleteMemberClientRpc(ulong clientId)
        {
            LeaderboardMember member = _membersList.Find(x => x.Player.OwnerClientId == clientId);
            _membersList.Remove(member);
            Destroy(member.gameObject);
        }

        [ClientRpc]
        private void UpdateMembersListClientRpc(NetworkObjectReference[] allPlayers)
        {
            ClearAllMembers();

            foreach (NetworkObjectReference playerReference in allPlayers)
            {
                playerReference.TryGet(out var playerNetworkObject);
                Player newPlayer = playerNetworkObject.GetComponent<Player>();

                LeaderboardMember newMember = Instantiate(_memberPrefab, _membersLayoutGroup.transform);

                Debug.Log($"{newMember} - {newPlayer}");

                newMember.SetPlayer(newPlayer);
                _membersList.Add(newMember);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_membersLayoutGroupTransform);
            Canvas.ForceUpdateCanvases();
            _membersLayoutGroup.SetLayoutVertical();
            //_membersLayoutGroupTransform.position = new Vector2(0, 0);
            _membersLayoutGroupTransform.anchoredPosition = new Vector2(0, 0);

        }

        private IEnumerator DoSortingWithDelay() // to reduce amount of sortings and improve optimizaiton
        {
            while (true)
            {
                SortMembersByScores();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void SortMembersByScores()
        {
            // First, sort members
            _membersList = _membersList.OrderByDescending(member => member.Player.Score).ToList();

            // Then update it's real positions on leaderboard
            for (int i = 0; i < _membersList.Count; i++)
            {
                _membersList[i].transform.SetSiblingIndex(i);
                _membersList[i].MemberNumber = i;
            }

            if (_membersList.Count != 0)
            {
                string allMembers = "";

                foreach (var member in _membersList)
                {
                    allMembers += $"{member.MemberNumberText}\t| {member.Player.Nickname}\t| {member.Player.Score}\n";
                }

                Debug.Log($"Members count: {_membersList.Count}\n" +
                          $"Leader: {_membersList[0].Player.Nickname}\n" +
                          $"=======================\n" +
                          $"{allMembers}");
            }
        }
    }
}