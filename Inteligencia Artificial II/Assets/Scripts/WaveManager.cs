using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WaveManager : MonoBehaviour
{
    public List<Wave> waves;  // Lista de oleadas
    public Transform[] spawnPoints;  // Puntos de aparición de enemigos
    public float timeBetweenWaves = 5f;  // Tiempo entre oleadas
    private int currentWaveIndex = 0;
    private int spawnMethodIndex = 0;  // Índice del método de spawn actual
    public float cooldownToStart = 10f;

    // Referencias a métodos de SpawnWave
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
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(cooldownToStart);

        StartNextWave();
    }

    public void StartNextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];

            // Selección del método de spawn basado en el índice
            switch (spawnMethods[spawnMethodIndex])
            {
                case SpawnType.RandomProgressive:
                    StartCoroutine(SpawnWaveGeneric(currentWave, SelectRandomEnemies)); // Spawn aleatorio
                    break;

                case SpawnType.ByGroup:
                    StartCoroutine(SpawnWaveGeneric(currentWave, SelectEnemiesByGroup)); // Spawn por grupo
                    break;

                case SpawnType.Partial:
                    StartCoroutine(SpawnWaveGeneric(currentWave, enemies => SelectPartialEnemies(enemies, 5))); // Spawn por lotes
                    break;

                case SpawnType.ControlledRandom:
                    StartCoroutine(SpawnWaveGeneric(currentWave, SelectControlledRandomEnemies)); // Spawn controlado
                    break;
            }

            // Cambiar al siguiente método de spawn para la próxima oleada
            spawnMethodIndex = (spawnMethodIndex + 1) % spawnMethods.Length;
            currentWaveIndex++;  // Pasar a la siguiente oleada
        }
        else
        {
            Debug.Log("Todas las oleadas completadas. Reiniciando...");
            currentWaveIndex = 0;  // Reiniciar el índice de las oleadas
            StartNextWave();  // Llamar de nuevo para reiniciar las oleadas
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
    }

    //IEnumerable<GameObject> SpawnEnemies(GameObject enemyPrefab)
    //{
    //    while (true)
    //    {
    //        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
    //        yield return Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation); ;
    //    }
    //}

    IEnumerator SpawnWaveGeneric(Wave wave, Func<IEnumerable<EnemyGroup>, IEnumerable<GameObject>> enemySelector)
    {
        var selectedEnemies = enemySelector(wave.enemyGroups); // Selecciona los enemigos

        // Convertir a lista para evitar múltiples enumeraciones
        var enemiesList = selectedEnemies.ToList();
        int totalEnemies = enemiesList.Count;

        for (int i = 0; i < totalEnemies; i++)
        {
            SpawnEnemy(enemiesList[i]); // Spawnear el enemigo
            
            yield return new WaitForSeconds(wave.spawnInterval); // Esperar el intervalo de spawn
        }

        yield return new WaitForSeconds(timeBetweenWaves); // Esperar entre oleadas
        StartNextWave(); // Iniciar la siguiente oleada
    }

    IEnumerable<GameObject> SelectRandomEnemies(IEnumerable<EnemyGroup> enemyGroups)
    {
        return enemyGroups
            .Where(group => group.count > 0)
            .SelectMany(group => Enumerable.Repeat(group.enemyPrefab, group.count)) 
            .OrderBy(x => UnityEngine.Random.value); // Aleatorizar
    }

    IEnumerable<GameObject> SelectEnemiesByGroup(IEnumerable<EnemyGroup> enemyGroups)
    {
        return enemyGroups
            .OrderByDescending(group => group.count)
            .SelectMany(group => Enumerable.Repeat(group.enemyPrefab, group.count)); // En orden de mayor cantidad
    }

    IEnumerable<GameObject> SelectPartialEnemies(IEnumerable<EnemyGroup> enemyGroups, int batchSize = 5)
    {
        var enemiesToSpawn = enemyGroups
            .Where(group => group.count > 0)                                            //Grupo 1: Where
            .SelectMany(group => Enumerable.Repeat(group.enemyPrefab, group.count))     //Grupo 2: SelectMany
            .OrderByDescending(enemy => enemy.GetComponent<Enemy>().maxHealth)          //Grupo 2: OrderByDescending
            .ToList();                                                                  //Grupo 3: ToList

        for (int i = 0; i < enemiesToSpawn.Count; i += batchSize)
        {
            foreach (var enemy in enemiesToSpawn.Skip(i).Take(batchSize))
            {
                yield return enemy; // Devolver lotes
            }
        }
    }

    IEnumerable<GameObject> SelectControlledRandomEnemies(IEnumerable<EnemyGroup> enemyGroups)
    {
        var validEnemyGroups = enemyGroups.Where(group => group.count > 0).ToList();

        var fastEnemyGroup = validEnemyGroups.FirstOrDefault(group => group.enemyPrefab.GetComponent<Enemy>().speed > .5f);

        if (fastEnemyGroup != null)
        {
            yield return fastEnemyGroup.enemyPrefab; // Enemigo rápido primero
        }

        foreach (var enemy in validEnemyGroups.SelectMany(group => Enumerable.Repeat(group.enemyPrefab, group.count)).OrderBy(x => UnityEngine.Random.value))
        {
            yield return enemy; // Luego, el resto aleatorizado
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

