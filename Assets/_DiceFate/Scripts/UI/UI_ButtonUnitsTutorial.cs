using UnityEngine;
using DiceFate.Events;
using DiceFate.EventBus;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DiceFate.Units;


namespace DiceFate.UI
{
    public class UI_ButtonUnitsTutorial : MonoBehaviour
    {
        [SerializeField] private Button buttonUnit;

        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
        }

        private void Start()
        {
            buttonUnit.onClick.AddListener(SelectUnit);
        }

      

        private void HandelUnitSelected(UnitSelectedEvent evt)
        {           
            if (buttonUnit == null)
            {
                Debug.LogWarning($" Для {this.name} UI_ButtonUnits: Button reference is not set!");
                return;
            }

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(buttonUnit.gameObject);
            }
        }

        private void SelectUnit()
        {            
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject == null)
            {
                Debug.LogWarning("Не найден объект с тегом 'Player' на сцене");
                return;
            }

            if (playerObject.TryGetComponent(out ISelectable selectable))
            {
               
                selectable.Select();                
               // Bus<OnUpdateUIAvatarEvent>.Raise(new OnUpdateUIAvatarEvent(1));
            }   
        }
    }
}