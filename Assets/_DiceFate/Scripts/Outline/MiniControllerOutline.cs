using DiceFate.Units;
using UnityEngine;

public class MiniControllerOutline : MonoBehaviour, IHover
{
    [field: SerializeField] public bool IsHover { get; private set; }   // IHover
    
    [SerializeField] private ObjectOutline Outline; //обводка

    private void Start()
    {
        Outline?.DisableOutline();
    }
    public void OnEnterHover()
    {
        Outline?.EnableOutline(); 
        IsHover = true;
    }

    public void OnExitHover()
    {
        Outline?.DisableOutline();
        IsHover = false;
    }


}
