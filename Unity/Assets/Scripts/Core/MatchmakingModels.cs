using System;

namespace SniperOnline.Core
{
    public enum MatchmakingState
    {
        Idle, Searching, MatchFound, Connecting, InMatch, Failed, Cancelled
    }

    [Serializable]
    public sealed class MatchmakingRequest
    {
        public string PlayerId;
        public int Rating;
        public string Region;
        public string GameMode = "SniperDuel1v1";
    }

    [Serializable]
    public sealed class MatchTicket
    {
        public string MatchId;
        public string RoomCode;
        public string Endpoint;
        public string Region;
        public string PlayerOneId;
        public string PlayerTwoId;
    }

    // Contract only: connect this to a real multiplayer backend.
    // The server must validate hits, damage, and match results.
    public interface IMatchmakingService
    {
        event Action<MatchmakingState, string> StateChanged;
        void Search(MatchmakingRequest request);
        void CancelSearch();
    }
}
