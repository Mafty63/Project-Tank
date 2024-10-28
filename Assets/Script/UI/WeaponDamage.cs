using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    enum Character
    {
        player,
        enemy
    }
    [SerializeField] private Character character;
    [SerializeField] private Collider myCollider;

    private int level;
    private int damageOutput;
    private int accuracy;
    private float knockback;

    private List<Collider> alreadyCollidedWith = new List<Collider>();

    private void OnEnable()
    {
        alreadyCollidedWith.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == myCollider) { return; }

        if (character == Character.enemy && other.CompareTag("Enemy")) return;

        if (alreadyCollidedWith.Contains(other)) { return; }

        alreadyCollidedWith.Add(other);

        {
        }
    }

    public void SetAttack(int level, int damage, int accuracy, float knockback)
    {
        this.level = level;
        this.accuracy = accuracy;
        this.knockback = knockback;
    }


}
