using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterController : MonoBehaviour
{
    public float WalkSpeed;
    public float TurnSpeed;
    protected Animator animator;

    protected Character character;
    protected Weapon weapon;

    public Slider LifeBar;

    void Start()
    {

        this.character = GamesCodeDataSource.Instance.CharacterDAO.GetCharacter(1);
        this.weapon = GamesCodeDataSource.Instance.WeaponDAO.GetWeapon(this.character.WeaponId);
        this.LifeBar.value = this.character.Health;
        this.WalkSpeed = this.character.Agility;

        this.animator = GetComponent<Animator>();
    }

    void Update()
    {
        var movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if(movement.x != 0 || movement.z != 0)
        {
            transform.Translate(movement * WalkSpeed * Time.deltaTime);
            this.animator.SetBool("speed", true);
        }
        else
        {
            this.animator.SetBool("speed", false);
        }

        var mouseX = Input.GetAxis("Mouse X");
        if(mouseX != 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + mouseX * TurnSpeed * Time.deltaTime, transform.eulerAngles.z);
        }

        var mouseClick0 = Input.GetMouseButtonDown(0);
        if (mouseClick0)
        {
            this.animator.SetBool("attack", true);
        }
        else
        {
            this.animator.SetBool("attack", false);
        }
    }

    public void TakeDamage(int damage)
    {
        //se ultrapassar a defesa recebe o dano
        var diff = damage - this.character.Defense;
        if(diff > 0)
        {
            this.character.Health -= diff;
            this.LifeBar.value = this.character.Health;
            GamesCodeDataSource.Instance.CharacterDAO.UpdateCharacter(this.character);
        }
    }
}
