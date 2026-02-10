using UnityEngine;
using Unity.Cinemachine;


public class CameraShakeManadger : MonoBehaviour
{
    public static CameraShakeManadger instance;
    [SerializeField] private float globalShekeForce = 1f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CameraShake(CinemachineImpulseSource cinemachineImpulseSource)
    {
        cinemachineImpulseSource.GenerateImpulseWithForce(globalShekeForce);
    }
}
