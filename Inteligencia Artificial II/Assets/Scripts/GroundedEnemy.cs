using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemy : Enemy
{
    [SerializeField] private Transform target;
    public float fireRate = 1f;         // Tiempo entre ataques
    private float nextFireTime = 0f;    // Tiempo para el pr�ximo ataque
    public float attackRange = 5f;      // Rango para detectar torretas
    public LayerMask turretLayer;       // Capa para detectar solo torretas

    protected override void Update()
    {
        if (target == null)
        {
            DetectFrontalTurret();
            MoveForward();
        }
        else
        {
            AttackTarget();
        }
    }

    void DetectFrontalTurret()
    {
        RaycastHit hit;
        // Dibuja una l�nea en la escena de Unity para visualizar el raycast
        Debug.DrawRay(transform.position, transform.forward * attackRange, Color.red);

        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange, turretLayer))
        {
            // Si detecta una torreta dentro del rango
            target = hit.transform;
            Debug.Log("Torreta detectada: " + target.name);
        }
    }

    // M�todo para atacar la torreta cuando se detecta
    void AttackTarget()
    {
        if (Time.time >= nextFireTime)
        {
            // Ataca si ya pas� el tiempo del fire rate
            nextFireTime = Time.time + fireRate;
            Debug.Log("Atacando a la torreta: " + target.name);

            // Aqu� ir�a la l�gica de infligir da�o a la torreta
            target.GetComponent<Turret>().TakeDamage(damage);
        }
    }

    void OnDrawGizmos()
    {
        // Dibuja la esfera solo cuando el objeto est� seleccionado
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
