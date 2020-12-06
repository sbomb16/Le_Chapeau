using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class Game_Manager : MonoBehaviourPunCallbacks
{

    [Header("Stats")]
    public bool hasGameEnded = false;
    public float timeToWin;
    public float invincibleDuration;
    private float hatPickupTime;
    public AudioSource[] soundEffects;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public Player_Controller[] players;
    public GameObject respawnPoint;
    public int playerWithHat;
    private int playersInGame;
    

    public static Game_Manager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new Player_Controller[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.All);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);     //Random.Range(0, spawnPoints.Length)

        Player_Controller playerScript = playerObj.GetComponent<Player_Controller>();

        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);       
    }

    public void RespawnPlayer(int playerID)
    {
        //Debug.LogError(playerID);
        players[playerID - 1].gameObject.transform.position = respawnPoint.transform.position;

        if (players[playerID - 1].curHatTime >= 5)
        {
            players[playerID - 1].curHatTime -= 5;
        }
        else
        {
            players[playerID - 1].curHatTime = 0;
        }
        

        soundEffects[0].Play();
    }

    public Player_Controller GetPlayer(int playerID)
    {
        return players.First(x => x.id == playerID);
    }

    public Player_Controller GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }

    [PunRPC]
    public void GiveHat(int playerID, bool initialGive)
    {
        if (!initialGive)
        {
            GetPlayer(playerWithHat).SetHat(false);            
        }
        playerWithHat = playerID;
        GetPlayer(playerID).SetHat(true);
        hatPickupTime = Time.time;

        soundEffects[2].Play();

    }

    public bool CanGetHat()
    {
        if(Time.time > hatPickupTime + invincibleDuration)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [PunRPC]
    void WinGame(int playerID)
    {
        hasGameEnded = true;
        Player_Controller player = GetPlayer(playerID);

        Game_UI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        Network_Manager.instance.ChangeScene("Menu");
    }
}
