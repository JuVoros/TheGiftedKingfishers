
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class gunSystem : MonoBehaviour
{
    [Header("----- Gun Stats ------")]
    [Range(0, 10)][SerializeField] int damage;
    [Range(10, 20)][SerializeField] int magazineSize;
    [Range(1, 10)][SerializeField] int bulletsPerTap;

    [Range(0, 1)][SerializeField] float timeBetweenShooting;
    [Range(0, 1)][SerializeField] float timeBetweenShots;
    [Range(1, 100)][SerializeField] float shootDistance; //make same as shootDistance in playerController
    [Range(0, 10)][SerializeField] float spread;
    [Range(0, 5)][SerializeField] float reloadTime;

    public bool allowButtonHold;
    int bulletsLeft;
    int bulletsShot;
    playerController player;
    bool shooting;
    bool readyToShoot;
    bool reloading;

    //Reference
    public Transform attackPoint;
    public RaycastHit rayHit;

    //Graphics
    public GameObject bulletGraphic; 
    //muzzle flash?
    public TextMeshProUGUI text;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();

        //set Text        text.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetButton("Shoot");
        else shooting = Input.GetButton("Shoot");

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //calculate Direction with Spread
        Vector3 direction = Camera.main.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out rayHit, shootDistance))
        {
            Debug.Log(rayHit.collider.name);

            IDamage damageable = rayHit.collider.GetComponent<IDamage>();
            if (rayHit.transform != transform && damageable != null)
            {
                damageable.takeDamage(damage);
            }
        }

        //Graphics
        Instantiate(bulletGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        //muzzle flash at attack point

        bulletsLeft--;
        bulletsShot--;
        gameManager.instance.playerManaBar.fillAmount = (float)bulletsLeft / magazineSize;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0) 
        Invoke(nameof(Shoot), timeBetweenShots);
    }


    private void ResetShot()
    {
        
        readyToShoot=true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke(nameof(reloadFinished), reloadTime);
    }

    private void reloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
    
    


}
