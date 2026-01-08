using UnityEngine;

[CreateAssetMenu(fileName = "New Cube Highlight Color Data", menuName = "Uma Tech Test/New Cube Highlight Color Data")]
public class HighlightColorData : ScriptableObject
{
    public float SelectedColorPhaseSpeed = 1.5f;
    public Color SelectedColorA = Color.cornflowerBlue;
    public Color SelectedColorB = Color.white;

    public Color HoveredColor = Color.greenYellow;
    public Color InvalidColor = Color.red;
}
