using System;

namespace SniperOnline.Core
{
    public enum LoginProvider { Guest, Google }

    [Serializable]
    public sealed class PlayerProfile
    {
        public string PlayerId;
        public string DisplayName;
        public LoginProvider Provider;
        public int Level = 1;
        public int Wins;
        public int Losses;
        public int Rating = 1000;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(PlayerId) &&
            !string.IsNullOrWhiteSpace(DisplayName);

        // Prototype only. In production, match results and rating are server-owned.
        public void ApplyMatchResult(bool won)
        {
            if (won) { Wins++; Rating += 20; }
            else { Losses++; Rating = Math.Max(0, Rating - 15); }
        }
    }
}
