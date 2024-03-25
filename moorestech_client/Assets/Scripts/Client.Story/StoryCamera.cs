using DG.Tweening;
using UnityEngine;

namespace Client.Story
{
    public interface IStoryCamera
    {
        public void TweenCamera(Vector3 fromPos,Vector3 fromRot,Vector3 toPos,Vector3 toRot,float duration,Ease easing);
        
        public void SetEnabled(bool enabled);
    }
    
    public class StoryCamera : MonoBehaviour, IStoryCamera
    {
        [SerializeField] private Camera camera;    
        
        public void TweenCamera(Vector3 fromPos, Vector3 fromRot, Vector3 toPos, Vector3 toRot, float duration, Ease easing)
        {
            camera.transform.position = fromPos;
            camera.transform.eulerAngles = fromRot;
            
            camera.transform.DOMove(toPos, duration).SetEase(easing);
            camera.transform.DORotate(toRot, duration).SetEase(easing);
        }
        
        public void SetEnabled(bool enabled)
        {
            camera.enabled = enabled;
        }
    }
}