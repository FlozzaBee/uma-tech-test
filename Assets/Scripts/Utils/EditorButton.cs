using UnityEngine;

[System.Serializable]
public class EditorButton
{
    [System.NonSerialized]
    public string method;
    [System.NonSerialized]
    public bool runtimeOnly;

    public EditorButton(string method, bool runtimeOnly = false)
    {
        this.method = method;
        this.runtimeOnly = runtimeOnly;
    }
}
