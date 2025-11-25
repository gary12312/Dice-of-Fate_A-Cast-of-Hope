using UnityEngine;
using UnityEngine.AI;
using DiceFate.Units;
using UnityEngine.Rendering.Universal;

public class T_Worker : MonoBehaviour, ISelectable, IMoveable
{
   //  [SerializeField] private Transform target;
    [SerializeField] private DecalProjector decalProjector;

    private NavMeshAgent agent;



    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Èםעונפויס ISelectable
    public void Deselect()
    {
        if (decalProjector != null)
        {
            decalProjector.gameObject.SetActive(false);
        }
    }

    public void Select()
    {
       if (decalProjector != null)
        {
            decalProjector.gameObject.SetActive(true); 
        }
    }

    public void MoveTo(Vector3 position)
    {
       // agent.destination = position;
      agent.SetDestination(position);
    }
}
 