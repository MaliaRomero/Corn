using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;


public class GameUI : MonoBehaviourPun
{
    //public PlayerUIContainer[] playerContainers;
    public Slider cornSlider;
    public Slider Health;
    //public Slider Ammo;
    public TextMeshProUGUI playerInfoText;

    public GameObject ammoText;
    private PlayerController player;
    public GameObject Phase2Text;
    ///Damage overlays 
    public GameObject BarelyDO;
    public GameObject HalfHP;
    public GameObject AlmostDead;

    public TextMeshProUGUI curTimeText;

    public GameObject Phase2LightBall;
    public GameObject MainLight;

    public GameObject Phase2PlayerCanvas;
    public GameObject Phase1PlayerCanvas;


    private Image[] ammoImages;


    //public static bool Phase2Started;

    public static GameUI instance;

    void Awake()
    {
        instance = this;
    }




    public void InitializePlayerUI(PlayerController localPlayer)
    {

       player = localPlayer;
       cornSlider.maxValue = 10;




        //UpdatePlayerUI();


    }
 
    [PunRPC]
    public void UpdateTimer(float curSurvivalTime, float startTime)
    {
        //curTimeText.text = (Time.time ).ToString("F2");
        curTimeText.text = (curSurvivalTime - startTime ).ToString("F2");
    }

    [PunRPC]
    public void UpdateSlider(float curValue)
    {
        cornSlider.value = curValue;
        if(curValue == 10)
        {
            Debug.Log("10");

            Phase1PlayerCanvas.SetActive(false);
            Phase2PlayerCanvas.SetActive(true);

            Phase2LightBall.SetActive(true);
            MainLight.SetActive(false);
            //MainLight.transform.position(-6, 152, 47);
            //photonView.RPC("GotoPhase2", RpcTarget.All);
            GameManager.instance.Phase2Started = true;

            //UpdateTimer();
        }
        /*
        if(cornSlider.value == 10)
        {
            Debug.Log("Go to Phase 2");

            GameManager.instance.GotoPhase2();
        }
        */

}


public void UpdatePlayerAmmo(int curAmmo)
    {
        if (curAmmo == 0)
        {
            ammoText.SetActive(true);
        }
        else
        {
            ammoText.SetActive(false);
        }
    }

    //Health overlay 
    public void UpdatePlayerHealth(int curHP)
    {
        Health.value = curHP;
        Debug.Log("Damaghe overlayin update health");
        if (curHP <= 20)
        {
            //almost dead
            AlmostDead.SetActive(true);

        }
        else if (curHP <= 50)
        {
            AlmostDead.SetActive(false);
            HalfHP.SetActive(true);
        }
        else if (curHP <= 80)
        {
            HalfHP.SetActive(true);
            BarelyDO.SetActive(true);

            //barely
        }

    }
    public void UpdateDamageOverlay(int curHP)
    {
        Debug.Log("Damaghe overlay");
        if (curHP <= 20)
        {
            //almost dead
            AlmostDead.SetActive(true);
            
        }
        else if (curHP <= 50)
        {
            AlmostDead.SetActive(false);
            HalfHP.SetActive(true);
        }
        else if (curHP <= 80)
        {
            HalfHP.SetActive(true);
            BarelyDO.SetActive(true);

            //barely
        }


    }
    public void DeadScreen()
    {
        Phase2PlayerCanvas.SetActive(false);
        //DeadCanvas.SetActive(true):

    }
    [PunRPC]
    public void Phase2UIText ()
    {
        Phase2Text.gameObject.SetActive(true);

    }
    [PunRPC]
    public void ClosePhase2UIText()
    {
        Phase2Text.gameObject.SetActive(false);
    }

}

/*
[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;
}
*/


