using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;

    void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    public void setMaxStamina(float stamina)
    {
        slider.maxValue = stamina;
    }

    public void setStamina(float stamina)
    {
        slider.value = stamina;
    }
}
