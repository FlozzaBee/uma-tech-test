using UnityEngine;

namespace Input
{
    public class ActionDefinitions
    {
        public readonly InputAction Interact = new();
        public readonly InputAction<Vector2> Look = new();
    }
}
