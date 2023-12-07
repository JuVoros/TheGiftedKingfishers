using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPointManager : MonoBehaviour
{

    [System.Serializable]
    public class AttackPoint
    {
        public string weaponName;
        public GameObject pointTransform;
    }

    [SerializeField] private List<AttackPoint> attackPoints = new List<AttackPoint>();

    public static AttackPointManager instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject GetAttackPoint(string weaponName)
    {
        AttackPoint point = attackPoints.Find(ap => ap.weaponName == weaponName);
        return point != null ? point.pointTransform : null;
    }

    public void AddAttackPoint(AttackPoint attackPoint)
    {
        attackPoints.Add(attackPoint);
    }
}