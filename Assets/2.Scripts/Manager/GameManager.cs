using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ObjectPoolManager poolManager;
    private List<GameObject> zombiesMelee;

    [SerializeField] private float spawnStartTime = 0;
    [SerializeField] private float spawnStartInterval = 1;

    private void Awake()
    {
        GameObject poolManagerPrefab = Resources.Load<GameObject>($"{Constants.ResourcePath_Manager}PoolManager");
        poolManager = Instantiate(poolManagerPrefab).GetComponent<ObjectPoolManager>();
        poolManager.name = "Object Pool";
        poolManager.CreatePool($"{Constants.ResourcePath_Monster}{Constants.ZombieMelee0001_1}", 100);

        zombiesMelee = new List<GameObject>();
        zombiesMelee.Capacity = 100;
    }

    private void Start()
    {
        InvokeRepeating(nameof(SpawnZombie), spawnStartTime, spawnStartInterval);
    }



    private void SpawnZombie()
    {
        GameObject zombieMelee = poolManager.GetObject(Constants.ZombieMelee0001_1);
        zombiesMelee.Add(zombieMelee);
        ZombieMeleeMovement zombieMeleeMovement = zombieMelee.GetComponent<ZombieMeleeMovement>();
        zombieMeleeMovement.Initialize((Enums.Lane)Random.Range((int)Enums.Lane.Lane1, (int)Enums.Lane.Lane3 + 1));
    }
}
