using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderSetting : MonoBehaviour
{
    [SerializeField] private GlobalContext globalContext;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueTMP;

    void Start()
    {
        int gameTime = globalContext.PData().gameTime;
        slider.value = gameTime;
        SetValue(gameTime);
    }

    void OnSliderValueChanged(float val)
    {
        SetValue(val);
    }

    void SetValue(float value)
    {
        valueTMP.text = value.ToString();
        globalContext.PData().gameTime = (int)value;
        globalContext.accountDataComponent.Save();
    }

    void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnDisable()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }
}
