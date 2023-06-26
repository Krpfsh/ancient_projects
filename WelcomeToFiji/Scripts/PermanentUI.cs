using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PermanentUI : MonoBehaviour
{
    //Player stats
    public int coins = 0;
    public int health = 3;
    public TextMeshProUGUI coinCount;
    public Text healthAmount;




    public static PermanentUI perm;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        //Singeton
        if (!perm)
        {
            perm = this;
            perm.healthAmount.text = perm.health.ToString();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetCoins()
    {
        coins = 0;
        coinCount.text = coins.ToString();
    }
    public void ResetHealth()
    {
        PermanentUI.perm.health = 3;
        PermanentUI.perm.healthAmount.text = PermanentUI.perm.health.ToString();
    }
}
