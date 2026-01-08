using UnityEngine;

namespace Utils
{
    public static class MathUtils
    {
        // Exponential Decay functions as an alternative to lerp smoothing
        // https://www.youtube.com/watch?v=LSNQuFEDOyQ 
        public static float ExpDecay(float a, float b, float decay, float dt)
        {
            return b + (a - b) * Mathf.Exp(-decay * dt);
        }

        public static Vector3 ExpDecay(Vector3 a, Vector3 b, float decay, float dt)
        {
            return b + (a - b) * Mathf.Exp(-decay * dt);
        }

        public static Quaternion ExpDecay(Quaternion a, Quaternion b, float decay, float dt)
        {
            float t = 1f - Mathf.Exp(-decay * dt);
            return Quaternion.Slerp(a, b, t);
        }

        public static float ExpDecayRotation(float a, float b, float decay, float dt)
        {
            float t = 1f - Mathf.Exp(-decay * dt);
            return Mathf.LerpAngle(a, b, t);
        }
    }
}
