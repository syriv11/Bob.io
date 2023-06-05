using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace BobIO
{
    public class LeaderboardMember : MonoBehaviour
    {
        [SerializeField] private TMP_Text _memberNumberText;
        [SerializeField] private TMP_Text _memberNameText;
        [SerializeField] private TMP_Text _memberScoresText;

        public Player Player { get; private set; }

        public int MemberNumber { get; set; }
        public string MemberNumberText { get { return $"#{MemberNumber + 1}"; } }

        public void SetPlayer(Player player)
        {
            Player = player;
            //UpdateMemberData();
        }

        private void Update()
        {
            UpdateMemberData();
        }

        private void UpdateMemberData()
        {
            _memberScoresText.SetText(Player.Score.ToString());
            _memberNameText.SetText(Player.Nickname);
            _memberNumberText.SetText(MemberNumberText);
        }
    }
}