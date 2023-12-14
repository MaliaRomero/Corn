using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEditor;
using TMPro.Examples;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;

    //Info
    public int curHP;
    public int maxHP;

    public SkinnedMeshRenderer mr1;
    public SkinnedMeshRenderer mr2;
    public SkinnedMeshRenderer mr3;

    public Transform cam;
    public CharacterController controller;

    float turnSmoothTime = .1f;
    float turnSmoothVelocity;
    Animator anim;


    //Moving
    Vector2 movement;
    public float walkSpeed;
    float trueSpeed;

    //Jumping 
    public float jumpHeight;
    public float gravity;
    public bool isGrounded;
    public bool isMoving;

    //actions
    public bool isDead;

    public Player photonPlayer;
    public Rigidbody rig;
    Vector3 velocity;


    //game stuff
    public int cornPickups;
    public int maxPickups = 10;

    public PlayerWeapon weapon;

    public float startTime;
    public float timeTaken;
    public float curSurvivalTime;
    //public Slider cornSlider;

    public GameObject playerFlashlight;
    public GameObject playerCobGlock9000;
    public AudioClip footsteps;
    public AudioClip jump;
    public AudioClip breathing;
    public AudioClip ouch;
    public AudioClip yippee;
    public AudioClip GobblinTime;
    public AudioClip boom;
    public AudioClip pop;
    public AudioClip gobblerdeathline;

    //public GameObject PP;

    //public bool Phase2Started;

    public PostProcessVolume postProcessVolume;

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;


        if (!photonView.IsMine)
        {
            Camera playerCamera = GetComponentInChildren<Camera>();
            if(playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false);
            }
            rig.isKinematic = true;
        }
        else
        {
            GameUI.instance.InitializePlayerUI(this);
        }
    }


    void Start()
    {

        trueSpeed = walkSpeed;
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (postProcessVolume == null)
        {
            Debug.LogWarning("Post-Process Volume is not assigned.");
            return;
        }

        // Disable post-processing effects at the start (optional)
        postProcessVolume.enabled = false;

    }


    void Update()
    {

        if (!photonView.IsMine || isDead)
        {
            return;
        }
        AudioSource audio = GetComponent<AudioSource>();


        isGrounded = Physics.CheckSphere(transform.position, .5f, 1);
        anim.SetBool("isGrounded", isGrounded);

        if (Input.GetMouseButtonDown(0))
        {
            audio.clip = pop;
            audio.Play();
            weapon.TryShoot();
        }
        if(GameManager.instance.Phase2Started == true)
        {
            curSurvivalTime += Time.deltaTime;
            GameUI.instance.UpdateTimer(curSurvivalTime, startTime);
        }


        if (isGrounded && velocity.y < 0)
        {

            velocity.y = -1;
            

        }

        if (movement.magnitude > 0.0001f)
        {
            isMoving = true;

            anim.SetBool("isRunning", isMoving);
            //footsteps



        }
        else
        {
            isMoving = false;
            anim.SetBool("isRunning", isMoving);
            audio.loop = false;
            audio.clip = footsteps;
            audio.Stop();
        }

        anim.transform.localPosition = Vector3.zero;
        //anim.transform.localEulerAngles = Vector3.zero;


        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized;



        if (direction.magnitude >= 0.1f)
        {
            isMoving = true;            
            anim.SetBool("isRunning", isMoving);
            audio.loop = false;
            audio.clip = footsteps;
            audio.Play();
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDirection.normalized * trueSpeed * Time.deltaTime);

        }

        //Jumping
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt((jumpHeight * 10) * -2f * gravity);
            //jump

        }

        if(velocity.y > -20)
        {
            velocity.y += (gravity * 10) * Time.deltaTime;
        }

        
        controller.Move(velocity * Time.deltaTime);
        if (isMoving == true)
        {
            isMoving = true;

        }
        ///


    }
    [PunRPC]
    public void TakeDamage(int damage)
    {
        if(isDead)
        {
            return;
        }
        if (curHP <= 0) {
            StartCoroutine(Death());
            IEnumerator Death()
            {
                anim.SetTrigger("Death");
                AudioSource audio = GetComponent<AudioSource>();
                audio.clip = gobblerdeathline;
                audio.Play();
                GameUI.instance.DeadScreen();
                yield return new WaitForSeconds(0.05f);
                photonView.RPC("Die", RpcTarget.All);
            }
        }

            StartCoroutine(DamageFlash());
            IEnumerator DamageFlash()
            {
                Color defaultColor = mr1.material.color;
                Color delfaultcolor2 = mr2.material.color;
                Color delfaultcolor3 = mr3.material.color;
                mr1.material.color = Color.red;
                mr2.material.color = Color.red;
                mr3.material.color = Color.red;
            //hurt
                AudioSource audio = GetComponent<AudioSource>();
                audio.clip = ouch;
                audio.Play();
            yield return new WaitForSeconds(0.05f);
                mr1.material.color = Color.white;
                mr2.material.color = Color.white;
                mr3.material.color = Color.white;
            }
        curHP -= damage;
        Debug.Log("ow");
        GameUI.instance.UpdatePlayerHealth(curHP);
        //damage overlay
        GameUI.instance.UpdateDamageOverlay(curHP);


    }

    [PunRPC] 
    public void Die ()
    {
        curHP = 0;
        isDead = true;

        GameManager.instance.alivePlayers--;

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.CheckPhase2WinCondition();
        }


        if(photonView.IsMine)
        {
            //anim.SetTrigger("Death");

            Vector3 spawnPos = GameManager.instance.respawnPoint.transform.position;


           mr1.gameObject.SetActive(false);
            mr2.gameObject.SetActive(false);
            mr3.gameObject.SetActive(false);
            playerFlashlight.SetActive(false);
            playerCobGlock9000.SetActive(false);
            postProcessVolume.enabled = false;

            StartCoroutine(Respawn(spawnPos, GameManager.instance.respawnTime));

        }


    }
    IEnumerator Respawn(Vector3 spawnPos, float timeToSpawn)
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(timeToSpawn);
        transform.position = spawnPos;

        transform.rotation = Quaternion.Euler(90, 0, 0);
        GameUI.instance.DeadScreen();
        rig.isKinematic = true;
    }

    [PunRPC]
    public void GiveCorn()
    {
        Debug.Log("Get corn");
        cornPickups++;
        GameUI.instance.UpdateSlider(cornPickups);
        //yippee
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = yippee;
        audio.Play();

        if (cornPickups == 10)
        {
            Debug.Log("playercontroller phase 2 works");
            //itsgobblin time
            audio.clip = GobblinTime;
            audio.Play();
            audio.clip = boom;
            audio.Play();
            playerFlashlight.SetActive(true);
            playerCobGlock9000.SetActive(true);

            startTime = 0;
 
            /////
            ///
            if (photonView.IsMine)
            {
                postProcessVolume.enabled = true;
                photonView.RPC("SyncPostProcessing", RpcTarget.Others, true);
            }


            //
            GameUI.instance.UpdateTimer(curSurvivalTime, startTime);
               //how curSurvivalTime

            Debug.Log("going to phase 2 call ");
            //photonView.RPC("GotoPhase2", RpcTarget.All);
            GameManager.instance.GotoPhase2();



        }
    }

    [PunRPC]
    private void SyncPostProcessing(bool isActive)
    {
        if (!photonView.IsMine)
        {
            postProcessVolume.enabled = isActive;
        }
    }



    [PunRPC]
    public void GiveHealth(int amountToHeal)
    {
        curHP = Mathf.Clamp(curHP + amountToHeal, 0, maxHP);
        GameUI.instance.UpdatePlayerHealth(curHP);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curSurvivalTime);
        }
        else if (stream.IsReading)
        {
            curSurvivalTime = (float)stream.ReceiveNext();
        }
    }

    //enemy attack

    //void OnCollisionEnter(Collision collision)
    //{
    // if (!photonView.IsMine)
    //    return;

    //if (collision.gameObject.CompareTag("Enemy"))
    //{
    //   isDead = true;
    //  anim.SetTrigger("die");
    //}
    //}
    /*
     * if(PhotonNetwork.IsMasterClient)
        {
            if(curHatTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);

            }
        }

    */
}
