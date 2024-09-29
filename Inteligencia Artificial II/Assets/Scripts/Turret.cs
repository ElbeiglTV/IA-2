using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    public AttackType attackTargetType;  // Se selecciona desde el inspector
    public DetectionType detectionType;  // Se selecciona desde el inspector
    private Action currentAttack;  // Action para guardar el método de ataque
    private Action currentDetection;  // Action para guardar el método de deteccion
    private Action extraDamage;  // Action para guardar el método de ataque extra

    public int maxHealth = 50;
    public float currentHealth = 50;
    public Image healthBar;
    public int damage = 10;
    public float range = 5f;
    public float fireRate = 1f;

    [SerializeField] private float fireCountdown = 0f;
    private Cell currentCell;

    [SerializeField] private List<Enemy> enemiesInRange = new List<Enemy>();

    public enum DetectionType
    {
        LinearRange,
        AreaRange
    }

    public enum AttackType
    {
        EnemiesStatsBonus,
        TwoTankEnemies,
        DamagedEnemies,
        HealthyEnemies,
        PiercingAttack,
        MostDangerousEnemy,
        HightestStatsEnemy,
        LowestStatsEnemy,
        ClosestEnemy,
        MostDamagedEnemy,
    }

    public enum ExtraDamageType
    {
        MaxHealth,
        CurrentHealth,
        Speed,
        AttackDamage
    }

    private void Start()
    {
        SetAttack();

        currentHealth = maxHealth;

        fireCountdown = 1f / fireRate;

        UpdateHealthBar();
    }

    void SetAttack()
    {
        if (detectionType == DetectionType.LinearRange)
        {
            currentDetection += DetectFrontalEnemies;

            if (attackTargetType == AttackType.PiercingAttack)
                currentAttack = MultiPiercingAttack;
            else if (attackTargetType == AttackType.LowestStatsEnemy)
                currentAttack += AttackEnemyWithLowestStats;
            else if (attackTargetType == AttackType.ClosestEnemy)
                currentAttack += AttackClosestEnemy;
            else if (attackTargetType == AttackType.HightestStatsEnemy)
                currentAttack += AttackEnemyWithHighestStats;
        }
        else if (detectionType == DetectionType.AreaRange)
        {
            currentDetection += DetectEnemiesInArea;

            if (attackTargetType == AttackType.TwoTankEnemies)
                currentAttack += DoubleAttackTankEnemies;
            else if (attackTargetType == AttackType.DamagedEnemies)
                currentAttack += MultiAttackExecution;
            else if (attackTargetType == AttackType.HealthyEnemies)
                currentAttack += MultiAttackHealthyEnemies;
            else if (attackTargetType == AttackType.EnemiesStatsBonus)
                currentAttack += TripleAttackWithStatBonus;
            else if (attackTargetType == AttackType.MostDangerousEnemy)
                currentAttack += AttackMostDangerousEnemy;
            else if (attackTargetType == AttackType.MostDamagedEnemy)
                currentAttack += AttackWeakestEnemy;
        }
    }

    void Update()
    {
        if (fireCountdown > 0f)
        {
            fireCountdown -= Time.deltaTime;
        }
        else
        {
            EnemyDetection();
            Debug.Log("Start Detection");

            fireCountdown = 1f / fireRate;
        }

        Debug.DrawRay(transform.position, transform.right * range, Color.red);
    }

    void EnemyDetection()
    {
        currentDetection?.Invoke(); ;

        if (enemiesInRange.Any())
        {
            Debug.Log("Enemy Detected: Start Attack");

            currentAttack?.Invoke();
        }
    }

    private void DetectFrontalEnemies()
    {
        enemiesInRange = Physics.RaycastAll(transform.position, transform.right, range)
            .OrderBy(hit => hit.distance)                       // Grupo 2:OrderBy             //Ordena por distancia
            .Select(hit => hit.collider.GetComponent<Enemy>())  // Grupo 1: Select             //Consigue el script de Enemy
            .Where(e => e != null && e.health > 0)              // Grupo 1: Where              //Filtra enemigos vivos
            .ToList();                                          // Grupo 3: ToList             //Lo vuelve una lista
    }

    private void DetectEnemiesInArea()
    {
        // Obtener enemigos en rango y filtrar solo los vivos
        enemiesInRange = Physics.OverlapSphere(transform.position, range)
            .Select(c => c.GetComponent<Enemy>())                    // Grupo 1: Select               //Consigue al enemigo
            .Where(e => e != null && e.health > 0)                   // Grupo 1: Where                //Filtra enemigos vivos
            .ToList();                                               // Grupo 3: ToList               //Lo vuelve una lista
    }

    /// <summary>
    /// ////////////////////////////////
    /// </summary>
    /// <returns></returns>

    //private Enemy GetMostDamagedEnemy() //Aggregate y Tupla
    //{
    //    return enemiesInRange
    //        .Select(e => Tuple.Create(e, e.maxHealth - e.health))
    //        .Aggregate((mostDamaged, next) => next.Item2 > mostDamaged.Item2 ? next : mostDamaged).Item1;
    //}

    //private Enemy GetMostDangerousEnemy()
    //{
    //    return enemiesInRange
    //        .Select(e => Tuple.Create(e, e.damage, e.speed, e.maxHealth)) // Tupla con enemigo, daño, velocidad y salud máxima
    //        .Aggregate((mostDangerous, next) =>
    //        {
    //            // Primero comparamos el daño
    //            if (next.Item2 > mostDangerous.Item2) // Compara daño
    //                return next;
    //            // Si el daño es igual, comparamos la velocidad
    //            if (next.Item3 > mostDangerous.Item3) // Compara velocidad
    //                return next;
    //            // Si daño y velocidad son iguales, comparamos la salud máxima
    //            return next.Item4 > mostDangerous.Item4 ? next : mostDangerous; // Compara salud máxima
    //        }).Item1; // Devuelve el enemigo más peligroso
    //}

    private float MaxHealthExtraDamage()
    {
        // Acumulador de daño extra
        return enemiesInRange
            .Select(e => Tuple.Create(e, e.maxHealth * 0.1f))
            .Aggregate(0f, (acc, tuple) => acc + tuple.Item2);
    }

    private float CurrentHealthExtraDamage()
    {
        // Acumulador de daño extra
        return enemiesInRange
            .Select(e => Tuple.Create(e, e.health * 0.1f))
            .Aggregate(0f, (acc, tuple) => acc + tuple.Item2);
    }

    private float SpeedExtraDamage()
    {
        // Acumulador de daño extra
        return enemiesInRange
            .Select(e => Tuple.Create(e, e.speed * 0.1f))
            .Aggregate(0f, (acc, tuple) => acc + tuple.Item2);
    }

    private float EnemyAttackExtraDamage()
    {
        // Acumulador de daño extra
        return enemiesInRange
            .Select(e => Tuple.Create(e, e.damage * 0.1f))
            .Aggregate(0f, (acc, tuple) => acc + tuple.Item2);
    }

    //GROUNDED AND FLYING TARGETS
    void AttackEnemyWithLowestStats() //Aggregate & Tuple
    {
        var weakestEnemy = enemiesInRange
            .Select(e => Tuple.Create(e, e.maxHealth + e.damage + e.speed))             // Crea una tupla con la suma de estadísticas
            .Aggregate((weakest, next) => next.Item2 < weakest.Item2 ? next : weakest); // Encontrar el enemigo con menos estadísticas

        if (weakestEnemy.Item1 != null)
        {
            weakestEnemy.Item1.TakeDamage(damage);
            Debug.Log("Atacando al enemigo con menos estadísticas: " + weakestEnemy.Item1.name);
        }
    }

    void AttackClosestEnemy() //Aggregate & Tuple
    {
        var closestEnemy = enemiesInRange
            .Select(e => Tuple.Create(e, Vector3.Distance(transform.position, e.transform.position))) //Crea una tupla con las distancias de los enemigos
            .Aggregate((closest, next) => next.Item2 < closest.Item2 ? next : closest).Item1;         //Devuelve el mas cercano

        if (closestEnemy != null)
        {
            closestEnemy.TakeDamage(damage);
            Debug.Log("Atacando al enemigo más cercano: " + closestEnemy.name);
        }
    }

    void TripleAttackWithStatBonus() //Tupla
    {
        var enemiesWithDamage = enemiesInRange
            // Crear tupla con estadísticas
            .Take(3)
            .Select(enemy => Tuple.Create(enemy, enemy.maxHealth, enemy.damage, enemy.speed))

            .Select(tuple => Tuple.Create(
                tuple.Item1,                                                                  //Enemigo 
                tuple.Item2 + tuple.Item3 + tuple.Item4,                                      //Suma de stats
                Mathf.CeilToInt(damage + (0.05f * (tuple.Item2 + tuple.Item3 + tuple.Item4))) //daño base + 5% de la suma de stats del enemigo
                ))
            .OrderByDescending(tuple => tuple.Item3)                                          //Ataca al enemigo con mas stats primero
            .ToList();

        foreach (var enemyTuple in enemiesWithDamage)
        {
            var enemy = enemyTuple.Item1;
            var totalDamage = enemyTuple.Item3;

            // Aplicar el daño calculado
            enemy.TakeDamage(totalDamage);
            Debug.Log($"Atacando al enemigo: {enemy.name} con daño total: {totalDamage} (incluye un 10% extra basado en sus estadísticas)");
        }
    }

    void MultiAttackExecution()
    {
        StartCoroutine(MultiAttackExecutionDamaged());
    }

    IEnumerator MultiAttackExecutionDamaged()
    {
        var woundedEnemies = enemiesInRange
            .Where(e => e.health <= e.maxHealth / 2)   // Seleccionar enemigos con menos de la mitad de la vida
            .OrderBy(e => e.health)                    // Ordenarlos por salud
            .ToList();                                 // Convertir a lista

        if (woundedEnemies.Any())
        {
            // Aplicar daño en lotes para evitar sobrecargar un solo frame
            for (int i = 0; i < woundedEnemies.Count; i++)
            {
                woundedEnemies[i].TakeDamage(damage);

                yield return null;
            }

            // Time slicing antes de la comprobación del segundo ataque
            yield return null;

            // Si todos los enemigos tienen menos de un cuarto de vida, aplicar otro ataque
            if (woundedEnemies.All(e => e.health <= e.maxHealth / 4))
            {
                Debug.Log("Execute wounded enemies");

                // Aplicar el segundo daño en lotes
                for (int i = 0; i < woundedEnemies.Count; i++)
                {
                    woundedEnemies[i].TakeDamage(damage);

                    yield return null;
                }
            }
        }
    }


    void DoubleAttackTankEnemies()
    {
        // Verificar si hay al menos 2 enemigos en rango

        if (enemiesInRange.Count < 2)
        {
            Debug.Log("No hay suficientes enemigos para realizar el ataque.");
            return; // Salir si no hay al menos dos enemigos
        }

        // Consigo los dos enemigos con más salud máxima 
        var twoStrongestEnemies = enemiesInRange
            .OrderByDescending(e => e.maxHealth)    // Grupo 2: OrderByDescending
            .Take(2)                                // Grupo 1: Take
            .ToList();                              // Grupo 3: ToList

        int totalDamage = damage;

        if (twoStrongestEnemies.All(e => e.health == e.maxHealth)) //Si los dos estan a vida maxima, reciben el doble de daño
        {
            totalDamage *= 2;
        }

        foreach (var enemy in twoStrongestEnemies)
        {
            enemy.TakeDamage(totalDamage);
        }
    }

    void MultiAttackHealthyEnemies()
    {
        var enemiesToAttack = enemiesInRange
            .OrderByDescending(e => e.health)                    // Grupo 2: OrderByDescending
            .TakeWhile(e => e.health > e.maxHealth / 2)          // Grupo 1: TakeWhile
            .ToArray();                                          // Grupo 3: ToArray

        if (enemiesToAttack.Any())                               // Grupo 3: Any
        {
            foreach (var enemy in enemiesToAttack)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Atacando al enemigo: " + enemy.name);
            }
        }
    }

    //GROUNDED TARGETS
    void AttackMostDangerousEnemy() //Tipo anonimo
    {
        if (enemiesInRange.OfType<GroundedEnemy>().Any())
        {
            var strongestEnemy = enemiesInRange
            .OfType<GroundedEnemy>()                                                                            //Grupo 3: OfType
            .Select(e => new                     //Crea un tipo anonimo que almacena info de los enemigos       //Grupo 1: Select
            {
                Enemy = e,
                Distance = Vector3.Distance(transform.position, e.transform.position),
                Speed = e.speed,
                Damage = e.damage
            })
            .OrderBy(e => e.Distance)            // Enemigos más cercanos primero                               //Grupo 2: OrderBy
            .ThenByDescending(e => e.Speed)      // Prioriza a los enemigos más rápidos                         //Grupo 2: ThenByDescending
            .ThenByDescending(e => e.Damage)     // Prioriza a los que hacen más daño                           //Grupo 2: ThenByDescending
            .FirstOrDefault()?.Enemy;            // Devuelve el enemigo más peligroso según los criterios       //Grupo 1: FirstOrDefault

            if (strongestEnemy != null)
            {
                strongestEnemy.TakeDamage(damage);
            }
        }
    }

    void AttackEnemyWithHighestStats() //Tipo Anonimo
    {
        if (enemiesInRange.OfType<GroundedEnemy>().Any())
        {
            var strongestEnemy = enemiesInRange
                .OfType<GroundedEnemy>()                                                                           //Grupo 3: OfType
                .Select(e => new { Enemy = e, TotalStats = e.maxHealth + e.damage + e.speed })                     //Grupo 1: Select
                .OrderByDescending(stats => stats.TotalStats) // Ordena por la suma total de estadísticas          //Grupo 2: OrderByDescending
                .FirstOrDefault().Enemy;                     // Toma el primero (el que tiene mas estadísticas)    //Grupo 1: FirstOrDefault

            if (strongestEnemy != null)
            {
                strongestEnemy.TakeDamage(damage);
                Debug.Log("Atacando al enemigo con mas estadísticas: " + strongestEnemy.name);
            }
        }
    }

    void AttackWeakestEnemy() //Tipo anonimo
    {
        if (enemiesInRange.OfType<GroundedEnemy>().Any())
        {
            var mostDamagedEnemy = enemiesInRange
            .OfType<GroundedEnemy>()                                                                                  //Grupo 3: OfType
            .Select(e => new                        //Crea un tipo anonimo que almacena info de los enemigos          //Grupo 1: Select
            {
                Enemy = e,
                MaxHealth = e.health,
                DamageTaken = e.maxHealth - e.health
            })
            .OrderBy(e => e.MaxHealth)              // Primero ordena por la menor vida actual                        //Grupo 2: OrderBy
            .ThenByDescending(e => e.DamageTaken)   // Luego por el mayor daño recibido                               //Grupo 2: ThenByDescending
            .FirstOrDefault()?.Enemy;               // Devuelve el enemigo con la menor vida y mayor daño recibido    //Grupo 1: FirstOrDefault

            if (mostDamagedEnemy != null)
            {
                mostDamagedEnemy.TakeDamage(damage);
                Debug.Log("Atacando al enemigo más dañado: " + mostDamagedEnemy.name);
            }
        }
    }

    void MultiPiercingAttack()
    {
        if (enemiesInRange.OfType<GroundedEnemy>().Any())
            StartCoroutine(PiercingAttackWithDelay());
    }

    IEnumerator PiercingAttackWithDelay() //Aggregate & Tuple
    {
        float currentDamage = damage;

        //Usamos Aggregate para crear una lista de tuplas con el enemigo y el daño actual
        var damageDistribution = enemiesInRange
            .OfType<GroundedEnemy>()
            .Aggregate(new List<Tuple<GroundedEnemy, float>>(), (acc, enemy) =>
            {
                if (enemy != null)
                {
                    acc.Add(Tuple.Create(enemy, currentDamage)); //Guarda cada enemigo con su respectivo daño a recibir
                    currentDamage *= 0.7f; // Por cada enemigo atravesado, hay una Reducción del daño infligido
                }
                return acc;
            });

        // Recorremos la lista de tuplas y aplicamos el daño
        foreach (var (enemy, damageToApply) in damageDistribution)
        {
            enemy.TakeDamage(Mathf.CeilToInt(damageToApply));

            Debug.Log("Atacando al enemigo con daño de penetración: " + enemy.name + " con daño: " + damageToApply);
            
            yield return new WaitForSeconds(0.25f); //Delay entre cada enemigo atravesado
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            DestroyTurret();
        }

        UpdateHealthBar();
    }

    public void SetCell(Cell cell)
    {
        currentCell = cell;
    }

    public void DestroyTurret()
    {
        if (currentCell != null)
        {
            currentCell.RemoveTurret();  // Libera la celda cuando la torreta sea destruida
        }

        Destroy(gameObject);
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}

