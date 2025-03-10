using UnityEngine;
[CreateAssetMenu(fileName = "ProjectileStat", menuName = "GameData/Create ProjectileStat")]

public class ProjectileStatSO : ScriptableObject
{
    [Header("Movement")]
    public float lifetime;
    public float groundDecisionPosYMin;
    public float groundDecisionPosYMax;
    public bool penetration;

    [Header("Attack")]
    public int damage;
}
