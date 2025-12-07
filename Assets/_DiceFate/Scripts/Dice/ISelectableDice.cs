
namespace DiceFate.Dice
{
    public interface ISelectableDice
    {
        bool IsShakeSelectDice { get; }

        void SelectDice();
        void DeselectDice();

        void ShakeSelectDice();
        void DropSelectDice();

    }
}
