using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StaminaBar : MonoBehaviour
{
    [SerializeField] public Slider slider;
    private TMP_Text text;

    public void UpdateStaminaBar(int Stamina, int maxStamina)
    {
        text = GetComponentInChildren<TMP_Text>();
        slider.value = (float)Stamina / (float)maxStamina;
        text.text = Stamina + "/" + maxStamina;
    }
}
