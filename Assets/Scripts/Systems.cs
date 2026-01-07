using Input;

public static class Systems
{
    public static InputSystem InputSystem { get; private set; }

    private static bool _initialized = false;
    
    public static void Init()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        InputSystem = new InputSystem();
    }
}
