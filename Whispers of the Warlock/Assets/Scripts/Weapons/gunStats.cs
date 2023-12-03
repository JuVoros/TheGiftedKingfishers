using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{


    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public float rechargeRate;


    public string weaponName;
    public GameObject model;
    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    [Range(0, 1)] public float shootSoundVol;




}
