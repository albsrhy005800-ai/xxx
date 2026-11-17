using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

namespace SniperOnline.Core
{
    public sealed class SniperOnlineClient : MonoBehaviour
    {
        [SerializeField] private string serverUrl = "ws://127.0.0.1:8080";
        [SerializeField] private bool connectOnStart = false;

        public event Action<string> StatusChanged;
        public event Action<string, int, string> MatchFound;
        public event Action<string, float, float> OpponentAim;
        public event Action<string, Vector3, float, float> OpponentPose;
        public event Action<string> OpponentFired;
        public event Action OpponentLeft;
        public event Action<string> ErrorReceived;

        public string PlayerId { get; private set; }
        public string MatchId { get; private set; }
        public bool IsConnected => socket != null && socket.State == WebSocketState.Open;

        private WebSocket socket;
        private readonly Queue<Action> mainThreadActions = new Queue<Action>();
        private readonly object queueLock = new object();

        [Serializable] private class Outgoing { public string type; }
        [Serializable] private class AimMessage { public string type = "aim"; public float yaw; public float pitch; }
        [Serializable] private class PoseMessage
        {
            public string type = "pose";
            public float x, y, z, yaw, pitch;
        }
        [Serializable] private class Incoming
        {
            public string type, playerId, opponentId, matchId, code, reason, message;
            public int spawnSlot;
            public float x, y, z, yaw, pitch;
            public Rules rules;
        }
        [Serializable] private class Rules { public string mode, map; public int maxRangeMeters; }

        private void Start() { if (connectOnStart) Connect(); }

        public async void Connect()
        {
            if (socket != null && socket.State == WebSocketState.Open) return;
            if (string.IsNullOrWhiteSpace(serverUrl)) { RaiseStatus("Server URL is empty."); return; }
            socket = new WebSocket(serverUrl);
            socket.OnOpen += () => Enqueue(() => RaiseStatus("Connected to game server."));
            socket.OnError += error => Enqueue(() => RaiseError(error));
            socket.OnClose += code => Enqueue(() => RaiseStatus("Disconnected: " + code));
            socket.OnMessage += bytes =>
            {
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                Enqueue(() => HandleMessage(json));
            };
            try { await socket.Connect(); }
            catch (Exception ex) { RaiseError(ex.Message); }
        }

        public async void Disconnect()
        {
            if (socket == null) return;
            try { await socket.Close(); }
            catch (Exception ex) { RaiseError(ex.Message); }
        }
        public void FindMatch() { Send(new Outgoing { type = "find_match" }); RaiseStatus("Searching for an opponent..."); }
        public void CancelQueue() { Send(new Outgoing { type = "cancel_queue" }); }
        public void LeaveMatch() { Send(new Outgoing { type = "leave_match" }); MatchId = null; }
        public void SendAim(float yaw, float pitch)
        {
            Send(new AimMessage { yaw = Mathf.Clamp(yaw, -360f, 360f), pitch = Mathf.Clamp(pitch, -89f, 89f) });
        }
        public void SendPose(Vector3 position, float yaw, float pitch)
        {
            Send(new PoseMessage {
                x = position.x, y = position.y, z = position.z,
                yaw = Mathf.Clamp(yaw, -360f, 360f), pitch = Mathf.Clamp(pitch, -89f, 89f)
            });
        }
        public void SendFire() { Send(new Outgoing { type = "fire" }); }

        private async void Send(Outgoing message)
        {
            if (!IsConnected) { RaiseError("Not connected to the game server."); return; }
            try { await socket.SendText(JsonUtility.ToJson(message)); }
            catch (Exception ex) { RaiseError(ex.Message); }
        }
        private async void Send(AimMessage message)
        {
            if (!IsConnected) return;
            try { await socket.SendText(JsonUtility.ToJson(message)); }
            catch (Exception ex) { RaiseError(ex.Message); }
        }
        private async void Send(PoseMessage message)
        {
            if (!IsConnected || string.IsNullOrEmpty(MatchId)) return;
            try { await socket.SendText(JsonUtility.ToJson(message)); }
            catch (Exception ex) { RaiseError(ex.Message); }
        }

        private void HandleMessage(string json)
        {
            Incoming message;
            try { message = JsonUtility.FromJson<Incoming>(json); }
            catch (Exception ex) { RaiseError("Invalid server message: " + ex.Message); return; }
            if (message == null || string.IsNullOrEmpty(message.type)) return;
            switch (message.type)
            {
                case "welcome":
                    PlayerId = message.playerId;
                    RaiseStatus("Connected. Player ID: " + PlayerId);
                    break;
                case "queue_joined": RaiseStatus("Waiting for another player..."); break;
                case "match_found":
                    MatchId = message.matchId;
                    MatchFound?.Invoke(message.matchId, message.spawnSlot, message.opponentId);
                    RaiseStatus("1v1 match found.");
                    break;
                case "opponent_aim": OpponentAim?.Invoke(message.playerId, message.yaw, message.pitch); break;
                case "opponent_pose":
                    OpponentPose?.Invoke(message.playerId, new Vector3(message.x, message.y, message.z), message.yaw, message.pitch);
                    break;
                case "opponent_fired": OpponentFired?.Invoke(message.playerId); break;
                case "opponent_left":
                    MatchId = null; OpponentLeft?.Invoke(); RaiseStatus("Opponent left the match.");
                    break;
                case "shot_accepted": RaiseStatus("Shot sent to server (hit not validated)."); break;
                case "shot_rejected": RaiseStatus("Shot rejected: " + message.reason); break;
                case "error": RaiseError(message.code ?? message.message ?? "Server error"); break;
                case "server_full": RaiseError("Server is full."); break;
            }
        }

        private void Enqueue(Action action) { lock (queueLock) mainThreadActions.Enqueue(action); }
        private void Update()
        {
            if (socket != null) socket.DispatchMessageQueue();
            lock (queueLock) while (mainThreadActions.Count > 0) mainThreadActions.Dequeue()?.Invoke();
        }
        private void RaiseStatus(string value) => StatusChanged?.Invoke(value);
        private void RaiseError(string value)
        {
            ErrorReceived?.Invoke(value);
            RaiseStatus("Network error: " + value);
            Debug.LogWarning("[SniperOnline] " + value);
        }
        private async void OnApplicationQuit()
        {
            if (socket != null && socket.State == WebSocketState.Open)
                try { await socket.Close(); } catch { }
        }
    }
}
