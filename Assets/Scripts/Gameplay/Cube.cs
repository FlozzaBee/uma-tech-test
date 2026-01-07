using UnityEngine;

public class Cube : MonoBehaviour
{
    private Polycube _parentPolycube;
    
    public void Init(Polycube polycube)
    {
        _parentPolycube = polycube;
    }

    public Polycube GetPolycube()
    {
        return _parentPolycube;
    }
}
