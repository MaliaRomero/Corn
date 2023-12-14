using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviourPun
{
    [Header("Stats")]
    //public int damage;
    public int curAmmo;
    public int maxAmmo;
    public float bulletSpeed;
    public float shootRate;
    Animator anim;
    private float lastShootTime;
    public bool isShooting;

    public float onscreenDelay = 5f;

    //public Rigidbody rig;

    //public Camera PlayermainCamera;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;
    public Camera unityCamera;
    private PlayerController player;

    public AudioClip pop;

    void Awake()
    {
        // get required components
        player = GetComponent<PlayerController>();
        anim = GetComponentInChildren<Animator>();
    }
    [PunRPC]
    public void TryShoot()
    {
        // can we shoot?
        if (curAmmo <= 0 || Time.time - lastShootTime < shootRate)
        {
            return;
        }
        //set animations
        StartCoroutine(shootBullet());
        IEnumerator shootBullet()
        {
            //setbool Stunned animation
            Debug.Log("Shoot animation start");
            anim.SetTrigger("Shooting");
            //player.photonView.RPC("SpawnBullet", RpcTarget.All);
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = pop;
            audio.Play();
            player.photonView.RPC("SpawnBullet", RpcTarget.All);
            //player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.transform.position, Camera.main.transform.forward);
            yield return new WaitForSeconds(10f);
            //setBool Stunned off
            anim.ResetTrigger("Shooting");
            Debug.Log("Bullet dead");

        }
        curAmmo--;
        lastShootTime = Time.time;

        // update the ammo UI
        GameUI.instance.UpdatePlayerAmmo(curAmmo);

        //Debug.Log("PEW");
        //anim.SetBool("Shooting",true);


        //yield return new WaitForSeconds(2f);




        //
        //Debug.Log("End animation");
        //anim.SetBool("Shooting", false);
        // spawn the bullet



    }
    /*
    [PunRPC]
    void SpawnBullet(Vector3 pos, Vector3 dir)
    {
        Debug.Log("SHOOT");
        // spawn and orientate it
        GameObject Kernal = Instantiate(bulletPrefab, pos, Quaternion.identity);
        Kernal.transform.forward = dir;

        // get bullet script
        StunBullet bulletScript = Kernal.GetComponent<StunBullet>();

        // initialize it and set the velocity
        bulletScript.rig.velocity = dir * bulletSpeed;
    }
    */
    [PunRPC]
    void SpawnBullet()
    {
        Debug.Log("Spawn Bullet");
        CinemachineFreeLook freeLookCamera = FindFirstObjectByType<CinemachineFreeLook>();
        if (freeLookCamera == null)
        {
            Debug.LogError("Cinemachine FreeLook camera not found!");
            return;
        }

        // Get the Virtual Camera from the FreeLook Cinemachine camera
        CinemachineVirtualCamera virtualCamera = freeLookCamera.GetRig(0);

        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera not found!");
            return;
        }

        // Access the internal Unity Camera from the Cinemachine VirtualCamera

        if (unityCamera == null)
        {
            Debug.LogError("Unity Camera not found in the Virtual Camera!");
            return;
        }

        // Convert screen position to a ray using the Cinemachine VirtualCamera's Unity Camera
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = unityCamera.ScreenPointToRay(screenCenter);

        RaycastHit hit;
        Vector3 spawnPos;

        // Cast a ray from the Cinemachine VirtualCamera to determine the spawn position
        if (Physics.Raycast(ray, out hit))
        {
            spawnPos = hit.point;
        }
        else
        {
            spawnPos = ray.GetPoint(10); // Set a default spawn distance if the raycast doesn't hit anything
        }

        // Debug draw to visualize the ray in the Scene view
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

        // Spawn and orient the bullet
        GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        // Set the bullet's direction (you may need to adjust this based on your requirements)
        bulletObj.transform.forward = unityCamera.transform.forward;

        // Get bullet script
        StunBullet bulletScript = bulletObj.GetComponent<StunBullet>();

        // Initialize it and set the velocity
        bulletScript.rig.velocity = unityCamera.transform.forward * bulletSpeed;


        // if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        if ( hit.collider.gameObject.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = hit.collider.GetComponent<EnemyBehavior>();
            enemy.isStunned = true;
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient);

            //Destroy(this.gameObject, onscreenDelay);
        }
    }


    
 

    [PunRPC]
    public void GiveAmmo()
    {
        curAmmo = Mathf.Clamp(curAmmo + 1, 0, maxAmmo);

        // update the ammo text
        GameUI.instance.UpdatePlayerAmmo(curAmmo);
    }

}
