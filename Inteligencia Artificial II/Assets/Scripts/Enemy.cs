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
    public float damage = 10;             // Daño que inflige el enemigo

    //public EnemyState state;

    private void Start()
    {
        InitializeStats();

        health = maxHealth;

        UpdateHealthBar();
    }

    // Método para inicializar las estadísticas
    public void InitializeStats()
    {
        maxHealth = RandomizeStat(maxHealth);
        damage = RandomizeStat(damage);
        speed = RandomizeStat(speed);
    }

    // Método que ajusta una estadística dentro de un rango de +/- 10%
    private float RandomizeStat(float baseValue)
    {
        float randomFactor = Random.Range(0.9f, 1.1f); // Genera un valor entre 90% y 110% del valor base
        return baseValue * randomFactor;
    }

    protected virtual void Update()
    {

    }

    // Método para mover el enemigo hacia adelante en línea recta
    protected void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // Método para detectar torretas usando raycast

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



