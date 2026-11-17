using UnityEngine;

namespace SniperOnline.Gameplay
{
    /// <summary>
    /// Local trajectory preview for gameplay prototyping.
    /// Competitive hit validation must be authoritative on the game server.
    /// </summary>
    public static class SniperBallistics
    {
        public static Vector3 GetPosition(
            Vector3 origin,
            Vector3 initialVelocity,
            float elapsedSeconds,
            Vector3 windAcceleration)
        {
            float t = Mathf.Max(0f, elapsedSeconds);
            Vector3 acceleration = Physics.gravity + windAcceleration;
            return origin + initialVelocity * t + 0.5f * acceleration * t * t;
        }

        public static float EstimateFlightTime(float horizontalDistance, float horizontalSpeed)
        {
            if (horizontalSpeed <= 0.01f) return 0f;
            return Mathf.Max(0f, horizontalDistance) / horizontalSpeed;
        }
    }
}
