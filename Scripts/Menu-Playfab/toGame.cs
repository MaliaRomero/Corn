using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class toGame : MonoBehaviour
{
    // Start is called before the first frame update
    public void onGoGame()
    {
        SceneManager.LoadScene(1);
    }

}
