using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private Renderer _highlight;
    [SerializeField] private HighlightColorData _highlightColor;
    
    private HighlightType _currentHighlightType;
    
    public enum HighlightType
    {
        None,
        Selected,
        Hovered,
        Invalid,
    }
    
    private Polycube _parentPolycube;
    
    public void Init(Polycube polycube, Color color)
    {
        _parentPolycube = polycube;
        GetComponent<Renderer>().material.color = color;
    }
    
    private void Update()
    {
        //Subtle pulsating effect on selected block to help make it clearer
        if (_currentHighlightType == HighlightType.Selected)
        {
            float t = Mathf.PingPong(Time.time / _highlightColor.SelectedColorPhaseSpeed, 1);
            SetColor(Color.Lerp(_highlightColor.SelectedColorA, _highlightColor.SelectedColorB, t));
        }
    }

    public void SetHighlight(HighlightType highlightType)
    {
        _highlight.gameObject.SetActive(highlightType != HighlightType.None);
        _currentHighlightType = highlightType;

        switch (highlightType)
        {
            case HighlightType.Hovered:
                SetColor(_highlightColor.HoveredColor);
                break;
            case HighlightType.Invalid: 
                SetColor(_highlightColor.InvalidColor);
                break;
        }
    }

    private void SetColor(Color color)
    {
        _highlight.material.color = color;
    }

    public Polycube GetPolycube()
    {
        return _parentPolycube;
    }
}
