using TMPro;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
    public TMP_Text textField;

    public AnimationName currentAnimation;
    public bool loop;
    public float duration;

    #region Animations properties

    // Here for Unity Animator support
    public float wavingOffsetX;
    public float wavingOffsetY;
    public float wavingSpeed;
    public float wavingDelay;

    public float scalingSpeed;
    public float scalingDelay;

    #endregion

    private ITextAnimation _animation;
    private TA_Waving _tA_Waving;
    private TA_Scaling _tA_Scaling;

    private float _timeElapsed;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        textField = GetComponent<TMP_Text>();
        if (textField == null)
        {
            enabled = false;
            return;
        }

        _animation = SelectAnimation(currentAnimation);
    }

    void Update()
    {
        StartAnimation();
    }

    private void StartAnimation()
    {
        if (_animation != null)
        {
            if (loop)
            {
                _animation.Play();
            }
            else
            {
                _timeElapsed = Time.time;
                if (_timeElapsed < duration)
                {
                    _animation.Play();
                }
            }
        }
    }

    public void SetCurrent(AnimationName animation)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            _animation = SelectAnimation(currentAnimation);
        }
    }

    private ITextAnimation SelectAnimation(AnimationName current)
    {
        switch (current)
        {
            case AnimationName.Waving:
                if (_tA_Waving == null)
                    _tA_Waving = new TA_Waving(this);
                return _tA_Waving;

            case AnimationName.Scaling:
                if (_tA_Scaling == null)
                    _tA_Scaling = new TA_Scaling(this);
                return _tA_Scaling;

            default:
                return null;
        }
    }
}
