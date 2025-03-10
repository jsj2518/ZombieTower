using UnityEngine;
[CreateAssetMenu(fileName = "ZombieStatMelee", menuName = "GameData/Create ZombieStatMelee")]

public class ZombieMeleeStatSO : ScriptableObject
{
    [Header("Movement")]
    public float runSpeed;
    public float runAcceleration;
    public float pushbackSpeed;
    public float climbSpeed;
    public float rayCastFrontDistance;
    public float rayCastBackDistance;
    public float rayCastUpDistance;

    [Header("Stat")]
    public int hp;
}
