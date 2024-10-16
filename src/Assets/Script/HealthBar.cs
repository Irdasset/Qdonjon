using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthBar : MonoBehaviour
{
    [SerializeField] public Slider slider;
    private TMP_Text text;
    //[SerializeField] public Entite entite;

    public void UpdateHealthBar(int HP, int maxHP)
    {
        text = GetComponentInChildren<TMP_Text>();
        slider.value = (float)HP / (float)maxHP;
        text.text = HP + "/" + maxHP;
    }

    public void UpdateStaminaBar(int Stamina, int maxStamina)
    {
        text = GetComponentInChildren<TMP_Text>();
        slider.value = (float)Stamina / (float)maxStamina;
        text.text = Stamina + "/" + maxStamina;
    }

    // Mettez à jour la barre de vie en fonction des points de vie du personnage
    void Update()
    {
        
    }
}
