using UnityEngine;

public enum BombAnimStates
{
    comes,
    boom
}

public class BombAnimation : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayAnimStep(BombAnimStates state)
    {
        _animator.SetTrigger(state.ToString());
    }
    public void SetAlertAnim(bool isAlert)
    {
        _animator.SetBool("isAlert", isAlert);
    }

}
