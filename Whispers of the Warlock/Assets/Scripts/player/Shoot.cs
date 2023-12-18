using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public static IEnumerator shoot()
    {
        playerController player = gameManager.instance.playerScript;
        if (player.manaCur > 0)
        {
            player.isShooting = true;
            player.manaCur--;

            player.updatePlayerUI();
            player.audi.PlayOneShot(player.staffList[player.staffSelected].shootSound, player.staffList[player.staffSelected].shootSoundVol);

            string weaponName = player.staffList[player.staffSelected].weaponName;

            GameObject attackPoint = AttackPointManager.instance.GetAttackPoint(weaponName);

            if (attackPoint != null)
            {
                Vector3 spawnPosition = attackPoint.transform.position;
                Vector3 spawnDirection = Camera.main.transform.forward;

                Instantiate(player.staffList[player.staffSelected].bulletPrefab, spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(player.shootRate);
            player.isShooting = false;
        }
    }
}
