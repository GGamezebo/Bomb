using UnityEngine;

public enum AnimStates
{
    ready,
    comes,
    alert,
    boom
}

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator _bombAnimator;
    [SerializeField] private Animator _backgroundAnimator;

    private Animator[] _animators;

    private void Start()
    {
        _animators = new[] { _bombAnimator, _backgroundAnimator};
    }

    public void PlayAnimStep(AnimStates state)
    {
        foreach (var animator in _animators) { 
            animator.SetTrigger(state.ToString()); 
        };
    }
}