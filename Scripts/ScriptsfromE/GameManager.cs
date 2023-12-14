using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{


    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerController[] players;
    public Transform[] spawnPoints;
    public int alivePlayers;
    private int playersInGame;
    public static GameManager instance;

    //public PlayerController cornPickups;
    //public PlayerController maxPickups;
    public float timeTaken;

    //
    public GameObject Gobbler;

    public bool Phase2Started;

    //public TextMeshProUGUI curTimeText;
    //public GameObject Phase2PlayerCanvas;
    //public GameObject Phase1PlayerCanvas;
    //public GameObject playerFlashlight;
    //public GameObject playerCobGlock9000;

    public float postGameTime;

    public float respawnTime;
    public GameObject respawnPoint;

    public static Material DarkSky;
    public AudioClip EZDub;
    public AudioClip Sound2;
    public GameObject Sound2AS;

    void Awake()
    {
        instance = this;

    }

    void Start()
    {

        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
    [PunRPC]
    void Update()
    {
        CheckPhase2WinCondition();
        //CheckPhase1WinCondition();
        //if (Phase2Started == true)
        //{
        //    GameUI.instance.UpdateTimer();
       // }
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
            photonView.RPC("SpawnPlayer", RpcTarget.All);
    }

    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }


    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }
    //checking max pickups in gameui

 


    public void CheckPhase2WinCondition()
    {
        //change to 0!!!!!
        if (alivePlayers == 0)
        {
            Debug.Log("All players dead");
            //end phase 2 bool to false to stop timer
            Phase2Started = false;
            StartCoroutine(EndGame());
            IEnumerator EndGame()
            {
                AudioSource audio = GetComponent<AudioSource>();
                audio.clip = EZDub;
                audio.Play();
                yield return new WaitForSeconds(3f);
                Debug.Log("Endgame");
                //photonView.RPC("EndGame2", RpcTarget.All, players.First(x => !x.isDead).id, players.First(x=> !x.isDead).startTime);
                photonView.RPC("EndGame2", RpcTarget.All, players.First(x => !x.isDead).startTime);

            }
        }
    }

    [PunRPC]
    public void GotoPhase2()
    {
        StartCoroutine(TextFlash());
        IEnumerator TextFlash()
        {
            GameUI.instance.Phase2UIText();
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = Sound2;
            audio.Play();
            yield return new WaitForSeconds(5f);
            GameUI.instance.ClosePhase2UIText();

        }
        Debug.Log("In function go to phase 2");
        Gobbler.SetActive(true);

        Sound2AS.SetActive(true);
        // Phase 2 begins :)
        Phase2Started = true;



    }

    [PunRPC]
    public void EndGame2(float startTime)
    {
        //GameUI.instance.SetWinText(GetPlayer(winningPlayer).photonPlayer.NickName);
        float curSurvivalTime = Time.time - startTime;
        Debug.Log("Leaderboard set");
        //SetLeaderboard(startTime);
        Leaderboard.instance.SetLeaderboardEntry(-Mathf.RoundToInt(curSurvivalTime * 1000.0f));
        //Phase2Started = false;
        //PhotonNetwork.Disconnect();
        Invoke("GoBackToMenu", postGameTime);


    }

    public void SetLeaderboard(float startTime)
    {

        timeTaken = Time.time - startTime;
        Leaderboard.instance.SetLeaderboardEntry(-Mathf.RoundToInt(timeTaken * 1000.0f));
    }

    void GoBackToMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(2);

    }
}
