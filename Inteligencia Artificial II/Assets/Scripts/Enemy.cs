using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 50;
    public float health = 50;
    public Image healthBar;
    public float speed = 2f;
    public float damage = 10;             // Da�o que inflige el enemigo

    //public EnemyState state;

    private void Start()
    {
        InitializeStats();

        health = maxHealth;

        UpdateHealthBar();
    }

    // M�todo para inicializar las estad�sticas
    public void InitializeStats()
    {
        maxHealth = RandomizeStat(maxHealth);
        damage = RandomizeStat(damage);
        speed = RandomizeStat(speed);
    }

    // M�todo que ajusta una estad�stica dentro de un rango de +/- 10%
    private float RandomizeStat(float baseValue)
    {
        float randomFactor = Random.Range(0.9f, 1.1f); // Genera un valor entre 90% y 110% del valor base
        return baseValue * randomFactor;
    }

    protected virtual void Update()
    {

    }

    // M�todo para mover el enemigo hacia adelante en l�nea recta
    protected void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // M�todo para detectar torretas usando raycast

    public void TakeDamage(int damage)
    {
        if (health > 0)
            health -= damage;

        if (health <= 0)
            Die();

        UpdateHealthBar();
    }

    void Die()
    {
        if (gameObject != null)
            Destroy(gameObject);
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = health / maxHealth;
    }
}



