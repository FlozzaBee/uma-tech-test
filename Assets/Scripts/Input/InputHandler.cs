using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace Input
{
    public class InputHandler : InputSystem_Actions.IPlayerActions
    {
        private InputSystem_Actions _inputSystemActions;
    
        public void Init()
        {
            _inputSystemActions = new InputSystem_Actions();
            _inputSystemActions.Player.SetCallbacks(this);
            _inputSystemActions.Player.Enable();
        }

        public void OnMoveHorizontal(CallbackContext context)
        {
            Systems.InputSystem.SetHorizontalMovement(context.ReadValue<Vector2>().normalized);
        }

        public void OnMoveVertical(CallbackContext context)
        {
            Systems.InputSystem.SetVerticalMovement(context.ReadValue<float>());
        }

        public void OnLook(CallbackContext context)
        {
            Systems.InputSystem.SetLookDelta(context.ReadValue<Vector2>());
        }

        public void OnInteract(CallbackContext context)
        {
            if (context.performed)
            {
                Systems.InputSystem.ActionDefinitions.Interact.Invoke();
            }
        }

        public void OnRotateX(CallbackContext context)
        {
            if (context.performed)
            {
                Systems.InputSystem.ActionDefinitions.RotateX.Invoke();
            }
        }

        public void OnRotateY(CallbackContext context)
        {
            if (context.performed)
            {
                Systems.InputSystem.ActionDefinitions.RotateY.Invoke();
            }
        }

        public void OnRotateZ(CallbackContext context)
        {
            if (context.performed)
            {
                Systems.InputSystem.ActionDefinitions.RotateZ.Invoke();
            }
        }
    }
}
