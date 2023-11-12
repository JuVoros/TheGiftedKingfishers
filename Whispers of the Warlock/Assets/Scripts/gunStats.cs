using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{


    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public int ammoCur;
    public int ammoMax;



    public GameObject model;
    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    [Range(0, 1)] public float shootSoundVol;




}
