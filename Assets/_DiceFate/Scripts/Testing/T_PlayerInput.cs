using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using DiceFate.Units;

public class T_PlayerInput : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private CinemachineCamera CinemachineCamera;
    [SerializeField] private LayerMask selectableLayers;
    [SerializeField] private LayerMask floorLayers;


    private ISelectable selectableUnit;




    // Update is called once per frame
    void Update()
    {
        HandelLeftClick();
        HandelRightClick();
        HandelRightClick2();


    }
    private void HandelRightClick()
    {
        if (selectableUnit == null || selectableUnit is not IMoveable moveable) { return; }
        // IMoveable moveable = selectableUnit as IMoveable;
        //IMoveable moveable = (IMoveable)selectableUnit;

        Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Mouse.current.rightButton.wasReleasedThisFrame 
            && Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, floorLayers))  //LayerMask.GetMask("Default"))
        {           
             //перемещаем юнита
           moveable.MoveTo(hit.point);
        }

    }


    private void HandelRightClick2()
    {
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
           Debug.Log("Right Clicked at position: " + Mouse.current.position.ReadValue());
        }
    }

    private void HandelLeftClick()
    {
        if (camera == null) { return; }

        Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            //Если до этого был выделен какой-то юнит, с него снимается выделениеъ
            if (selectableUnit != null)
            {
                selectableUnit.Deselect();
                selectableUnit = null;
            }

            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers)
            && hit.collider.TryGetComponent(out ISelectable selectable))
            {               
                selectable.Select();             
                selectableUnit = selectable;
            }          
        }   
    }
}
