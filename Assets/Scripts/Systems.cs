using Input;
using UnityEngine;

public static class Systems
{
    public static InputSystem InputSystem { get; private set; }
    public static SaveResumeSystem SaveResumeSystem { get; private set; }
    public static GameGridSystem GameGridSystem { get; private set; }

    private static bool _initialized = false;
    
    public static void Init(GameGridManager gameGridManager)
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        InputSystem = new InputSystem();
        GameGridSystem = new GameGridSystem(gameGridManager);
        SaveResumeSystem = new SaveResumeSystem();
    }

    public static void Reset()
    {
        _initialized = false;
    }
}
