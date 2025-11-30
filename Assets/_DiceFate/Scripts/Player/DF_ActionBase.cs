using System.Windows.Input;
using UnityEngine;

public abstract class DF_ActionBase : ScriptableObject //, ICommand
{


    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: Range(0, 8)][field: SerializeField] public int Slot { get; private set; } //Слоты в меню 
    [field: SerializeField] public bool RequiresClicToActive { get; private set; } = true; // Клик для активции //Requires - требует
    //public abstract bool CanHandle(CommandContext context);
    //public abstract void Handle(CommandContext context);
}
