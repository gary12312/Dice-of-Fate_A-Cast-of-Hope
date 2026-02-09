using UnityEngine;
using DG.Tweening;


    public class MoveCameraTarget : MonoBehaviour
    {

        [SerializeField] private GameObject cameraTarget;
        [SerializeField] private GameObject targetOne;
        [SerializeField] private GameObject targetSecond;
        [SerializeField] private Vector3 offset;
        public AnimationCurve moveToOne = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(1f, 0f));
        public AnimationCurve moveToNarget = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(1f, 0f));

        private Sequence _animationSequence;
        private Vector3 _initialRotation;


        public void CameraTargetAnimationToOneTarget()
        {
            cameraTarget.transform.DOMove(targetOne.transform.position, 0.5f).SetEase(moveToOne).Play();
        }

        public void DOMoveTargetCameraToTarget(Transform targetObject)
        {
            cameraTarget.transform.DOMove((targetObject.position - offset), 0.5f).SetEase(moveToNarget).Play();
        }




    }

