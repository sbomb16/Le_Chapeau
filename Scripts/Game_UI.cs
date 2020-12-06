using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Game_UI : MonoBehaviour
{

    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;

    public static Game_UI instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerUI();
    }

    void InitializePlayerUI()
    {
        for(int x = 0; x < playerContainers.Length; x++)
        {
            PlayerUIContainer container = playerContainers[x];

            if(x < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[x].NickName;
                container.hatTimeSlider.maxValue = Game_Manager.instance.timeToWin;
            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerUI();
    }

    void UpdatePlayerUI()
    {
        for(int x = 0; x < Game_Manager.instance.players.Length; x++)
        {
            if(Game_Manager.instance.players[x] != null)
            {
                playerContainers[x].hatTimeSlider.value = Game_Manager.instance.players[x].curHatTime;
            }
        }
    }

    public void SetWinText(string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + " wins!";
    }

}

[System.Serializable]
public class PlayerUIContainer 
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;

}
