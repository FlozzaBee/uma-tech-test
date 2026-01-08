using UnityEngine;

namespace Input
{
    public class ActionDefinitions
    {
        public readonly InputAction Interact = new();
        public readonly InputAction<Vector2> Look = new();
        public readonly InputAction RotateX = new();
        public readonly InputAction RotateY = new();
        public readonly InputAction RotateZ = new();
    }
}
