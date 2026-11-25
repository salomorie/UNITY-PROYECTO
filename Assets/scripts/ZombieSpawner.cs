using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    public static ZombieSpawner Instance;

    [Header("Zombie")]
    public GameObject[] zombiePrefabs;

    [Header("Spawn")]
    public Transform spawnPoint;

    [Header("Timing")]
    public float respawnDelay = 3f;

    private GameObject currentZombie;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnZombie();
    }

    public void ZombieDied()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        SpawnZombie();
    }

    void SpawnZombie()
    {
    if(zombiePrefabs.Length == 0)
    {
        Debug.LogError("No hay zombies asignados");
        return;
    }

    int randomIndex =
        Random.Range(0, zombiePrefabs.Length);

    GameObject selectedZombie =
        zombiePrefabs[randomIndex];

    currentZombie = Instantiate(
        selectedZombie,
        spawnPoint.position,
        spawnPoint.rotation
    );

    currentZombie.name =
        selectedZombie.name;
    }
}