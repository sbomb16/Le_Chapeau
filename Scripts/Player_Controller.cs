using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player_Controller : MonoBehaviourPunCallbacks, IPunObservable
{

    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;

    [HideInInspector]
    public float curHatTime;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;

    // Update is called once per frame
    void Update()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            if(curHatTime >= Game_Manager.instance.timeToWin && !Game_Manager.instance.hasGameEnded)
            {
                Game_Manager.instance.hasGameEnded = true;
                Game_Manager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }


        if (photonView.IsMine)
        {
            Move();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryJump();
            }

            if (hatObject.activeInHierarchy)
            {
                curHatTime += Time.deltaTime;
            }
        }
        
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        rig.velocity = new Vector3(x, rig.velocity.y, z);
    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 0.7f))
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            Game_Manager.instance.soundEffects[1].Play();
        }
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        Game_Manager.instance.players[id - 1] = this;

        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }

        if(id == 1)
        {
            Game_Manager.instance.GiveHat(id, true);
        }
        PlayerColor(id - 1);
    }

    public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if(Game_Manager.instance.GetPlayer(collision.gameObject).id == Game_Manager.instance.playerWithHat)
            {
                if (Game_Manager.instance.CanGetHat())
                {
                    Game_Manager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Respawn")
        {

            Game_Manager.instance.RespawnPlayer(id);

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHatTime);
        }
        else if (stream.IsReading)
        {
            curHatTime = (float)stream.ReceiveNext();
        }
    }

    public void PlayerColor(int i)
    {
        switch (i)
        {
            case 0:
                gameObject.GetComponent<Renderer>().material.color = Color.magenta;
                break;
            case 1:
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                break;
            case 2:
                gameObject.GetComponent<Renderer>().material.color = Color.black;
                break;
            case 3:
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
                break;
            case 4:
                gameObject.GetComponent<Renderer>().material.color = Color.gray;
                break;
            case 5:
                gameObject.GetComponent<Renderer>().material.color = Color.green;
                break;
            case 6:
                gameObject.GetComponent<Renderer>().material.color = Color.white;
                break;
            case 7:
                gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            default:
                gameObject.GetComponent<Renderer>().material.color = Color.magenta;
                break;
        }
    }

}
