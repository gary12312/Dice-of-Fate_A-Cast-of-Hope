
namespace DiceFate.Units
{
    public interface ISelectable
    {
        bool IsSelected { get; }
        void Select();
        void Deselect();

        void JumpBeforeAttack();
        void UnitsSOCurrent(string userName);
       
    }
}

