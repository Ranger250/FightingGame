using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player Refs")]
    public Color[] PlayerColors;
    public List<PlayerController> Players = new List<PlayerController>();
    public Transform[] SpawnPoints;

    [Header("Prefabs")]
    public GameObject playerContainerPrefab;

    [Header("Audio")]
    public AudioClip[] managerfx;
    public AudioSource Audio;
    //spawn 0

    [Header("Level Vars")]
    public int startTime;
    public float curTime;
    List<PlayerController> winningPlayers;

    [Header("Components")]
    public Transform playerContainerParent;
    public TextMeshProUGUI time;


    public static GameManager instance;


    private void Awake()
    {
        instance = this;
        Audio = GetComponent<AudioSource>();
        startTime = PlayerPrefs.GetInt("roundTimer", 100);
    }

    // Start is called before the first frame update
    void Start()
    {
        winningPlayers = new List<PlayerController>();
        curTime = startTime;
        time.text = curTime.ToString();
    }

    private void FixedUpdate()
    {
        curTime -= Time.deltaTime;
        time.text = ((int)curTime).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (curTime <= 0)
        {
            int highScore = 0;
            int index = 0;
            
            foreach (PlayerController player in Players)
            {
                if (player.Score > highScore)
                {
                    highScore = player.Score;
                    index = Players.IndexOf(player);
                    winningPlayers.Clear();
                    winningPlayers.Add(player);
                } else if (player.Score == highScore && player.Score != 0)
                {
                    winningPlayers.Add(player);
                }
            }
            if (winningPlayers.Count <= 1)
            {
                PlayerPrefs.SetInt("colorIndex", index);
                SceneManager.LoadScene("WinScene");
                print("one");
                print(winningPlayers.Count);
            } else
            {
                print("more");
                foreach (PlayerController player in Players)
                { 
                    if (!winningPlayers.Contains(player))
                    {
                        player.youSuck();
                        
                    }
                    curTime = 30;
                }
            }
            
            
        }
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        // set player color when joined
        player.GetComponentInChildren<SpriteRenderer>().color = PlayerColors[Players.Count];
        // added the player to the players list

        PlayerContainerUI containerUI = Instantiate(playerContainerPrefab, playerContainerParent).GetComponent<PlayerContainerUI>();
        player.GetComponent<PlayerController>().setUiContainer(containerUI);
        containerUI.initialize(PlayerColors[Players.Count]);

        Players.Add(player.GetComponent<PlayerController>());
        // choose spawn point


        Transform SpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        player.transform.position = SpawnPoint.position;
        Audio.PlayOneShot(managerfx[0]);
    }

    /*public void onPlayerDeath(PlayerController player, PlayerController attacker)
    {
        if(attacker != null)
        {
            attacker.AddScore();
        }
        player.Die();
    }*/
}
