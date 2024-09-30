using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WaveManager : MonoBehaviour
{
    public List<Wave> waves; 
    public Transform[] spawnPoints; 

    public float timeBetweenWaves = 5f;  

    public float cooldownToStart = 10f;

    private int currentWaveIndex = 0;

    //Variable que guarda el generador que se va a usar. Recibe una coleccion de grupos de enemigos y devuelve los enemigos a spawnear uno por uno
    private Func<IEnumerable<EnemyGroup>, IEnumerable<GameObject>> currentEnemySelector;    

    private enum SpawnType
    {
        RandomProgressive,
        ByGroup,
        Partial,
        ControlledRandom
    }

    [SerializeField]
    private SpawnType[] spawnMethods = new SpawnType[]
    {
        SpawnType.RandomProgressive,
        SpawnType.ByGroup,
        SpawnType.Partial,
        SpawnType.ControlledRandom
    };


    void Start()
    {
        StartCoroutine(StartWaveDelay());
    }

    private IEnumerator StartWaveDelay()
    {
        yield return new WaitForSeconds(cooldownToStart);
        SetNextEnemySelector(); // Asigna el primer selector
        StartNextWave();
    }

    // Inicia la próxima oleada
    public void StartNextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];
            StartCoroutine(SpawnWave(currentWave, currentEnemySelector));

            currentWaveIndex++;
        }
        else
        {
            ResetWaves(); // Reinicia las oleadas cuando se completan todas
        }
    }

    // Asigna el proximo generador
    private void SetNextEnemySelector()
    {
        int methodIndex = currentWaveIndex % spawnMethods.Length;

        switch (spawnMethods[methodIndex])
        {
            case SpawnType.RandomProgressive:
                currentEnemySelector = SelectRandomEnemies;
                break;

            case SpawnType.ByGroup:
                currentEnemySelector = SelectEnemiesByGroupCount;
                break;

            case SpawnType.Partial:
                currentEnemySelector = enemies => SelectPartialEnemiesByHealth(enemies, 5);
                break;

            case SpawnType.ControlledRandom:
                currentEnemySelector = SelectEnemiesBySpeed;
                break;

            default:
                throw new InvalidOperationException("Método de spawn desconocido.");
        }
    }

    private void ResetWaves()
    {
        currentWaveIndex = 0;

        SetNextEnemySelector(); 

        StartNextWave();
    }

    // Spawnea los enemigos utilizando el método de spawn actual
    private IEnumerator SpawnWave(Wave wave, Func<IEnumerable<EnemyGroup>, IEnumerable<GameObject>> enemySelector) 
    {
        var enemiesToSpawn = enemySelector(wave.enemyGroups); // Selecciona los enemigos

        foreach (var enemyPrefab in enemiesToSpawn)
        {
            SpawnEnemyAtRandomPoint(enemyPrefab);

            yield return new WaitForSeconds(wave.spawnInterval); // Espera entre spawns de enemigos
        }

        yield return new WaitForSeconds(timeBetweenWaves); // Espera entre oleadas

        SetNextEnemySelector(); // Cambia el selector antes de la siguiente oleada

        StartNextWave(); // Inicia la siguiente oleada
    }

    private void SpawnEnemyAtRandomPoint(GameObject enemyPrefab)
    {
        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);

        Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
    }

    //Zheng Emanuel [Generator]
    IEnumerable<GameObject> SelectRandomEnemies(IEnumerable<EnemyGroup> enemyGroups)
    {
        // Obtener una lista de enemigos a generar
        var enemies = enemyGroups
            .Where(group => group.count > 0)                                            //Grupo 1: Where
            .SelectMany(group => Enumerable.Repeat(group.enemyPrefab, group.count))     //Grupo 2: SelectMany           //Repite y guarda el mismo enemigo en base a la cantidad que tiene el grupo
            .OrderBy(x => UnityEngine.Random.value)  // Mezcla los ordenes              //Grupo 2: OrderBy              //Randomiza el orden de los enemigos a spawnear
            .ToList();                                                                  //Grupo 3: ToList

        // Usar yield para devolver uno por uno
        foreach (var enemy in enemies)
        {
            yield return enemy;  // Generar un enemigo a la vez
        }
    }


    IEnumerable<GameObject> SelectEnemiesByGroupCount(IEnumerable<EnemyGroup> enemyGroups)
    {
        foreach (var group in enemyGroups.OrderByDescending(g => g.count))              //Ordena de mayor a menor en base a la cantidad de enemigos en los grupos
        {
            for (int i = 0; i < group.count; i++)           
            {
                yield return group.enemyPrefab;
            }
        }
    }

    IEnumerable<GameObject> SelectPartialEnemiesByHealth(IEnumerable<EnemyGroup> enemyGroups, int batchSize = 5)
    {
        var enemiesToSpawn = enemyGroups
            .Where(group => group.count > 0)                                            //Grupo 1: Where
            .SelectMany(group => Enumerable.Repeat(group.enemyPrefab, group.count))     //Grupo 2: SelectMany
            .OrderByDescending(enemy => enemy.GetComponent<Enemy>().maxHealth)          //Grupo 2: OrderByDescending    //Ordena por salud maxima de mayor a menor
            .ToList();                                                                  //Grupo 3: ToList

        for (int i = 0; i < enemiesToSpawn.Count; i += batchSize)
        {
            foreach (var enemy in enemiesToSpawn.Skip(i).Take(batchSize))   //Toma enemigos de 5 en 5
            {
                yield return enemy; //Devolver lotes
            }
        }
    }

    //Valenzuela Matias [Generator]
    IEnumerable<GameObject> SelectEnemiesBySpeed(IEnumerable<EnemyGroup> enemyGroups)
    {
        // Ordenar los grupos válidos por la velocidad del enemigo (de menor a mayor)
        var orderedEnemyGroups = enemyGroups
            .Where(group => group.count > 0)                                                //Grupo 1: Where
            .OrderBy(group => group.enemyPrefab.GetComponent<Enemy>().speed)                //Grupo 2: OrderBy
            .ToList();                                                                      //Grupo 3: ToList

        // Itera sobre los grupos ordenados y spawnea todos sus enemigos

        foreach (var group in orderedEnemyGroups)       //Recorre cada grupo de la coleccion de grupos de enemigos
        {
            for (int i = 0; i < group.count; i++)       //Recorre sobre el grupo por la cantidad de enemigos que tiene
            {
                yield return group.enemyPrefab;  // Y devuelve un enemigo a la vez
            }
        }
    }


    [System.Serializable]
    public class Wave
    {
        public List<EnemyGroup> enemyGroups;  // Lista de grupos de enemigos en la oleada
        public float spawnInterval;           // Intervalo de spawn entre enemigos
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab;   // Prefab del enemigo
        public int count;                // Cantidad de este tipo de enemigo
    }
}

