using UnityEngine;

public class SaveResumeDebug : MonoBehaviour
{
    public EditorButton saveState = new EditorButton(nameof(SaveState));
    public EditorButton loadState = new EditorButton(nameof(LoadState));
    public EditorButton resetState = new EditorButton(nameof(ResetState));

    public void SaveState()
    {
        Systems.SaveResumeSystem.SaveState();
    }

    public void LoadState()
    {
        Systems.SaveResumeSystem.TryLoadState();
    }
    
    public void ResetState()
    {
        Systems.SaveResumeSystem.ResetState();
    }
}
