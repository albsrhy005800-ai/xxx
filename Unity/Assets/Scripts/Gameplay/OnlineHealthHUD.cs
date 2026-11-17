using UnityEngine;
using UnityEngine.UI;
using SniperOnline.Core;

namespace SniperOnline.Gameplay
{
    /// <summary>Optional uGUI health, hit feedback, and match result display.</summary>
    public sealed class OnlineHealthHUD : MonoBehaviour
    {
        [SerializeField] private SniperOnlineClient client;
        [Header("Health bars (optional)")]
        [SerializeField] private Slider localHealthBar;
        [SerializeField] private Text localHealthText;
        [SerializeField] private Slider opponentHealthBar;
        [SerializeField] private Text opponentHealthText;
        [Header("Feedback (optional)")]
        [SerializeField] private Text hitFeedbackText;
        [SerializeField] private Text matchResultText;
        [SerializeField] private GameObject matchResultPanel;

        private void OnEnable()
        {
            if (client == null) return;
            client.HealthChanged += OnLocalHealthChanged;
            client.OpponentHealthChanged += OnOpponentHealthChanged;
            client.HitConfirmed += OnHitConfirmed;
            client.MatchEnded += OnMatchEnded;
            client.MatchFound += OnMatchFound;
            client.OpponentLeft += OnOpponentLeft;
        }
        private void OnDisable()
        {
            if (client == null) return;
            client.HealthChanged -= OnLocalHealthChanged;
            client.OpponentHealthChanged -= OnOpponentHealthChanged;
            client.HitConfirmed -= OnHitConfirmed;
            client.MatchEnded -= OnMatchEnded;
            client.MatchFound -= OnMatchFound;
            client.OpponentLeft -= OnOpponentLeft;
        }
        private void Start()
        {
            SetHealth(localHealthBar, localHealthText, 100, 100);
            SetHealth(opponentHealthBar, opponentHealthText, 100, 100);
            if (matchResultPanel != null) matchResultPanel.SetActive(false);
            SetText(hitFeedbackText, string.Empty);
        }
        private void OnMatchFound(string matchId, int spawnSlot, string opponentId)
        {
            SetHealth(localHealthBar, localHealthText, 100, 100);
            SetHealth(opponentHealthBar, opponentHealthText, 100, 100);
            if (matchResultPanel != null) matchResultPanel.SetActive(false);
            SetText(hitFeedbackText, string.Empty);
        }
        private void OnLocalHealthChanged(int current, int maximum, string reason) => SetHealth(localHealthBar, localHealthText, current, maximum);
        private void OnOpponentHealthChanged(int current, int maximum) => SetHealth(opponentHealthBar, opponentHealthText, current, maximum);
        private void OnHitConfirmed(int damage, string distance) => SetText(hitFeedbackText, "HIT  -" + damage + " HP  (" + distance + ")");
        private void OnMatchEnded(string winnerId, string loserId, string reason)
        {
            bool won = client != null && winnerId == client.PlayerId;
            SetText(matchResultText, (won ? "VICTORY" : "DEFEAT") + (string.IsNullOrEmpty(reason) ? "" : "\n" + reason));
            if (matchResultPanel != null) matchResultPanel.SetActive(true);
        }
        private void OnOpponentLeft()
        {
            SetText(matchResultText, "OPPONENT LEFT");
            if (matchResultPanel != null) matchResultPanel.SetActive(true);
        }
        private static void SetHealth(Slider bar, Text label, int current, int maximum)
        {
            current = Mathf.Clamp(current, 0, Mathf.Max(1, maximum));
            if (bar != null) { bar.minValue = 0; bar.maxValue = Mathf.Max(1, maximum); bar.value = current; }
            SetText(label, current + " / " + maximum);
        }
        private static void SetText(Text target, string value) { if (target != null) target.text = value; }
    }
}
