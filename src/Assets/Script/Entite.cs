using UnityEngine;
using UnityEngine.UI;

public class Entite : MonoBehaviour
{
    public const int MAXHP = 100;
    public int HP;
    public int HP_avant;
    public int Vies_utilisees;
    public const int MAXSTAMINA = 100;
    public int Stamina;
    public int Stamina_avant;
    public Animator animator;
    public int degats;
    public int defense;
    public int heal;

    [SerializeField] public HealthBar healthBar;
    [SerializeField] public StaminaBar staminaBar;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
        staminaBar = GetComponentInChildren<StaminaBar>();

        Vies_utilisees = 0;
        HP_avant = HP;
        Stamina_avant = Stamina;
        setMaxHealth();
        setMaxStamina();

    }

    protected virtual void Update()
    {
        //Si les HP du personnage a diminiué, on lance l'animator de dégat
        if (HP < HP_avant)
        {
            animator.SetTrigger("Degat");
            HP_avant = HP;
        }

    }


    public void utiliserVie()
    {
        Vies_utilisees += 1;
        setMaxHealth();
        setMaxStamina();
        //Debug.Log("nb de vies du personnage utilisées : " + Vies_utilisees);
    }

    protected void setMaxHealth()
    {
        HP = MAXHP;
    }

    protected void setMaxStamina()
    {
        Stamina = MAXSTAMINA;
    }

}