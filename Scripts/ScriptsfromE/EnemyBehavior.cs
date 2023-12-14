using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviourPun
{
    public float chaseRange = 20f;
    public float attackRange = 2f;
    public float attackRate = 2f;
    public int damage = 10;
    public float moveSpeed = 20f;

    public bool isStunned = false;

    private NavMeshAgent agent;
    private PlayerController targetPlayer;
    private float lastAttackTime;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        lastAttackTime = -attackRate; // To allow immediate attack upon finding a target
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;
        if (!PhotonNetwork.IsMasterClient)
            return;

        // FindNearestPlayer();


        if (targetPlayer != null && !isStunned)
        {
            float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (dist < attackRange && Time.time - lastAttackTime >= attackRate)
            {
                Attack();
            }
            else if (dist > attackRange)
            {
                agent.SetDestination(targetPlayer.transform.position);
                SetAnimation("walk");
            }
            else
            {
                agent.velocity = Vector3.zero;
                SetAnimation("idle");
            }
        }
        if (isStunned == true)
        {
            Debug.Log("Gobbler Stunned");
            TakeDamage();
        }

        FindNearestPlayer();
    }

    void FindNearestPlayer()
    {
        float nearestDist = float.MaxValue;
        targetPlayer = null;

        foreach (PlayerController player in GameManager.instance.players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist < chaseRange && dist < nearestDist)
            {
                nearestDist = dist;
                targetPlayer = player;
                //added
                //SetAnimation("walk");
            }
        }
    }
    void Attack()
    {
            lastAttackTime = Time.time;
            Debug.Log("Player detected - ATTACK!");
            SetAnimation("attack");
            targetPlayer.photonView.RPC("TakeDamage", targetPlayer.photonPlayer, damage);

    }

    [PunRPC]
    public void TakeDamage()
    {
        //Debug.Log("Gobbler Stunned");
        //TimesStunned++;
        photonView.RPC("StunnedDamage", RpcTarget.All);
    }

    [PunRPC]
    void StunnedDamage()
    {
        StartCoroutine(StunFlash());
        IEnumerator StunFlash()
        {
            Debug.Log("In enum");
            SetAnimation("stunned");
            yield return new WaitForSeconds(3.0f);
            isStunned = false;
            //FindNearestPlayer();
            //SetAnimation("walk");
        }
    }

    [PunRPC]
    void SetAnimation(string animationName)
    {
        // Set animation trigger based on the provided animation name
        anim.ResetTrigger("walk");
        anim.ResetTrigger("attack");
        anim.ResetTrigger("stunned");
        anim.ResetTrigger("idle");

        switch (animationName)
        {
            case "walk":
                anim.SetTrigger("walk");
                break;
            case "attack":
                anim.SetTrigger("attack");
                break;
            case "stunned":
                anim.SetTrigger("stunned");
                break;
            case "idle":
                anim.SetTrigger("idle");
                break;
                // Add more cases for other animations (if needed)
        }
    }
}
