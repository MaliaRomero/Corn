using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBullet : MonoBehaviourPun
{
    //public float onscreenDelay = 5f;

    //Destroy(this.gameObject, onscreenDelay);
    public Rigidbody rig;

    void Start()
    {
        Debug.Log("im a bullet");

    }

    [PunRPC]
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet Hit");
        //if (collision.gameObjectCompareTag("Enemy"))
        //{
        //    EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
        //    enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient);
        //}
        Destroy(this.gameObject, 3f);
    }
}
