using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public ObjectPoolManager poolManager;
    public List<GameObject> zombiesMelee;

    [SerializeField] private float spawnStartTime = 0;
    [SerializeField] private float spawnStartInterval = 1;
    [SerializeField] private float SpawnAccelerationTime = 5.5f;
    [SerializeField] private float SpawnAccelerationRatio = 2f;

    protected override void Init()
    {
        GameObject poolManagerPrefab = Resources.Load<GameObject>($"{Constants.ResourcePath_Manager}PoolManager");
        poolManager = Instantiate(poolManagerPrefab).GetComponent<ObjectPoolManager>();
        poolManager.name = "Object Pool";
        poolManager.CreatePool($"{Constants.ResourcePath_Monster}{Constants.ZombieMelee0001_1}", 100);
        poolManager.CreatePool($"{Constants.ResourcePath_Projectile}{Constants.ShotgunPallet}", 20);
        poolManager.CreatePool($"{Constants.ResourcePath_UI}{Constants.DamageIndicator}", 30);

        zombiesMelee = new List<GameObject>();
        zombiesMelee.Capacity = 100;
    }

    private void Start()
    {
        InvokeRepeating(nameof(SpawnZombie), spawnStartTime, spawnStartInterval);
        Invoke(nameof(SpawnAcceleration), SpawnAccelerationTime);
    }



    private void SpawnZombie()
    {
        GameObject zombieMelee = poolManager.GetObject(Constants.ZombieMelee0001_1);
        zombiesMelee.Add(zombieMelee);
        ZombieMeleeMovement zombieMeleeMovement = zombieMelee.GetComponent<ZombieMeleeMovement>();
        zombieMeleeMovement.Initialize((Enums.Lane)Random.Range((int)Enums.Lane.Lane1, (int)Enums.Lane.Lane3 + 1));
        ZombieMeleeState zombieMeleeState = zombieMelee.GetComponent<ZombieMeleeState>();
        zombieMeleeState.Initialize();
        zombieMeleeState.OnDead += ZombieMeleeDeadEvent;
    }

    private void ZombieMeleeDeadEvent(GameObject zombieMelee)
    {
        zombiesMelee.Remove(zombieMelee);
        ZombieMeleeState zombieMeleeState = zombieMelee.GetComponent<ZombieMeleeState>();
        zombieMeleeState.OnDead -= ZombieMeleeDeadEvent;
    }

    private void SpawnAcceleration()
    {
        CancelInvoke(nameof(SpawnZombie));
        InvokeRepeating(nameof(SpawnZombie), 0f, spawnStartInterval / SpawnAccelerationRatio);
    }
}
