using System;

namespace Input
{
    public interface IAction
    {
    }

    public class InputAction : IAction
    {
        private Action _action;

        public void Invoke()
        {
            _action?.Invoke();
        }

        public void Subscribe(Action action) => _action += action;
        public void Unsubscribe(Action action) => _action -= action;
    }

    public class InputAction<T> : IAction
    {
        private Action<T> _action;
        
        public void Invoke(T arg)
        {
            _action?.Invoke(arg);
        }
        
        public void Subscribe(Action<T> action) => _action += action;
        public void Unsubscribe(Action<T> action) => _action -= action;
    }
}