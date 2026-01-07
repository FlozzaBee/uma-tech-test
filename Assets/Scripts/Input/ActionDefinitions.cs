using UnityEngine;

namespace Input
{
    public class ActionDefinitions
    {
        public readonly InputAction Interact;
        public readonly InputAction<Vector2> Look;

        public ActionDefinitions()
        {
            Interact = new();
            Look = new();
        }
    }
}
