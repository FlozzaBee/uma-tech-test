using UnityEngine;

public class Cube : MonoBehaviour
{
    private Polycube _parentPolycube;
    
    public void Init(Polycube polycube, Color color)
    {
        _parentPolycube = polycube;
        GetComponent<Renderer>().material.color = color;
    }

    public Polycube GetPolycube()
    {
        return _parentPolycube;
    }
}
