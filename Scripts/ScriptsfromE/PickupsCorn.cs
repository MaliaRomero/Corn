using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public enum PickupType
{
    Corn,
    Ammo,
    Health
}

public class PickupsCorn : MonoBehaviourPun
{
    public PickupType type;
    public int value;

    public AudioClip callingSound;
    private AudioSource audioSource;
    public float repeatInterval;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If AudioSource is not attached, add it
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set the audio clip to play
        audioSource.clip = callingSound;
        PlayAudioWithRepeat();
    }

    [PunRPC]
    void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (other.CompareTag("Player"))
        {

            // get the player
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);
            if (type == PickupType.Corn)
            {
                player.photonView.RPC("GiveCorn", RpcTarget.AllBuffered);
                Debug.Log("Give Corn");
            }
            else if (type == PickupType.Ammo)
            {
                player.photonView.RPC("GiveAmmo", player.photonPlayer, value);
                Debug.Log("Give Ammo");
            }
            else if (type == PickupType.Health)
            {
                player.photonView.RPC("GiveHealth", player.photonPlayer, value);
            }

            photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
        }
    }

    private void PlayAudioWithRepeat()
    {
        StartCoroutine(PlayAudioRepeatCoroutine());
    }

    private IEnumerator PlayAudioRepeatCoroutine()
    {
        while (true)
        {
            // Play the audio clip
            audioSource.Play();

            // Wait for the specified interval before playing again
            yield return new WaitForSeconds(repeatInterval);
        }
    }

    [PunRPC]
    public void DestroyPickup()
    {
        Destroy(gameObject);
    }
}
