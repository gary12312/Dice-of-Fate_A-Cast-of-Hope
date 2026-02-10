using UnityEngine;
using Unity.Cinemachine;

public class ButtunCinemachineShackeTast : MonoBehaviour
{
   
    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void TestShack()
    {
        CameraShakeManadger.instance.CameraShake(impulseSource);


    }
}
