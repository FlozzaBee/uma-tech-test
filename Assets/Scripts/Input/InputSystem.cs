using UnityEngine;

namespace Input
{
    public class InputSystem
    {
        private InputHandler _inputHandler = new();
        
        public Vector2 HorizontalMovement { get; private set; }
        public float VerticalMovement { get; private set; }
        public Vector2 LookDelta { get; private set; }

        public InputSystem()
        {
            _inputHandler.Init();
        }

        public void SetHorizontalMovement(Vector2 movement)
        {
            HorizontalMovement = movement;
        }

        public void SetVerticalMovement(float movement)
        {
            VerticalMovement = movement;
        }

        public void SetLookDelta(Vector2 delta)
        {
            LookDelta = delta;
        }
    }
}
