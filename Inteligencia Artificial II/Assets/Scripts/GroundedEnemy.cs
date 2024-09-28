using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemy : Enemy
{
    [SerializeField] private Transform target;
    public float fireRate = 1f;         // Tiempo entre ataques
    private float nextFireTime = 0f;    // Tiempo para el próximo ataque
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
        // Dibuja una línea en la escena de Unity para visualizar el raycast
        Debug.DrawRay(transform.position, transform.forward * attackRange, Color.red);

        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange, turretLayer))
        {
            // Si detecta una torreta dentro del rango
            target = hit.transform;
            Debug.Log("Torreta detectada: " + target.name);
        }
    }

    // Método para atacar la torreta cuando se detecta
    void AttackTarget()
    {
        if (Time.time >= nextFireTime)
        {
            // Ataca si ya pasó el tiempo del fire rate
            nextFireTime = Time.time + fireRate;
            Debug.Log("Atacando a la torreta: " + target.name);

            // Aquí iría la lógica de infligir daño a la torreta
            target.GetComponent<Turret>().TakeDamage(damage);
        }
    }

    void OnDrawGizmos()
    {
        // Dibuja la esfera solo cuando el objeto está seleccionado
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
