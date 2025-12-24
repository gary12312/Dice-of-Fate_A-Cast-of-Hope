
namespace DiceFate.Units
{
    public interface ISelectableForVisibleUi
    {
        bool IsSelectedForVisibleUi { get; }
        void SelectForUi();
        void DeselectForUi();
    }
}

