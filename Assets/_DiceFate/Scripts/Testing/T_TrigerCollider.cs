using UnityEngine;

public class T_TrigerCollider : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");

    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Stay");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("TriggerExit");
    }

}
