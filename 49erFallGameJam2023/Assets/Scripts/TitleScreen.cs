using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private BulletRicochet br;

    private bool showMenu = true;

    public Canvas menuCanvas;

    private Player player;

    private TurretFire tFire;

    public bool shouldAutoSwitch = true;

    public TextMeshProUGUI tMesh;

    public List<GameObject> robotPrefabs = new List<GameObject>();

    public List<Transform> spawnPoints = new List<Transform>();

    public bool souldSpawnRobots = true;

    public TextMeshProUGUI winText;

    private void Start()
    {
        br = FindObjectOfType<BulletRicochet>();
        menuCanvas = GetComponent<Canvas>();
        player = FindObjectOfType<Player>();
        tFire = FindObjectOfType<TurretFire>();
        if (player != null)
        {
            player.canControl = !showMenu;
        }
        if (tMesh != null && br != null)
        {//set the Ricochet mesh number.
            tMesh.text = "Ricochet: " + br.durability.ToString();
        }

        //on the title screen we will add these to our special robots list in GameManager.
        if (GameManager.instance.robotPrefabs.Count == 0 && this.robotPrefabs != null)
        {
            //GameManager.instance.robotPrefabs = this.robotPrefabs;
            foreach (GameObject robot in robotPrefabs)
            {
                GameManager.instance.robotPrefabs.Add(new Robot(true, robot));
            }

        }


        if (spawnPoints.Count > 0 && GameManager.instance.robotPrefabs.Count > 0)
        {
            GameManager.instance.robots.Clear();
            GameManager.instance.robots = new List<Slowbody>(spawnPoints.Count);
            //spawn the robots at the spawn points
            int i = 0;
            if (souldSpawnRobots == true)
                foreach (Robot r in GameManager.instance.robotPrefabs)
                {
/*                    if (r.alive)
                        GameManager.instance.robots.Insert(i, Instantiate(r.prefab, spawnPoints[i].position, spawnPoints[i].rotation).GetComponent<Slowbody>());
                    i++;*/

                    if (r.alive)
                    {
                        GameManager.instance.robots.Add(Instantiate(r.prefab, spawnPoints[i].position, spawnPoints[i].rotation).GetComponent<Slowbody>());
                        i++;
                    }

                }
        }
    }


    private void Update()
    {
        //THIS IS WHY YOU HAVE TO DESTROY THE BULLET BEFORE YOU CAN CALCULATE IF PLAYER LOST
        if (shouldAutoSwitch && br == null && !SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(GameManager.instance.scenes[GameManager.instance.scenes.Count - 1])))
        {
            nextLevel();
        }
        else if (shouldAutoSwitch && br == null && GameManager.instance.robots.Count > 0)
        {
            Debug.Log("LAST SCENE");
            GameManager.instance.finalRobotCount = GameManager.instance.robots.Count;
            GameManager.instance.setScene("WinScene");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showMenu = !showMenu;
            menuCanvas.enabled = showMenu;
            if (!tFire.playerControlled && player != null)
            {
                //if the player's control state isn't controlled
                //by the turret then we should set it to be
                //inverse of if the menu is shown. Menu on = player off.
                player.canControl = !showMenu;
            }
        }
        else
        {
            if (!tFire.playerControlled && player != null)
            {
                //if the player's control state isn't controlled
                //by the turret then we should set it to be
                //inverse of if the menu is shown. Menu on = player off.
                player.canControl = !showMenu;
            }
        }

        if (tMesh != null && br != null)
        {//set the Ricochet mesh number.
            tMesh.text = "Ricochet: " + br.durability.ToString();
        }

        /*        //if we are a normal level check if all robots are dead.
                if (GameManager.instance.robots.Count == 0 && shouldAutoSwitch)
                {
                    Debug.Log("GAME OVER");
                    setScene("GameOver_RobotsDied");
                }*/

        if (GameManager.instance.robotPrefabs.Count > 0)
        {
            if (GameManager.instance.robotPrefabs.All(r => r.alive == false) && SceneManager.GetActiveScene().name != "GameOver_RobotsDied")
            {
                Debug.Log("GAME OVER");
                setScene("GameOver_RobotsDied");
            }
        }

        Debug.Log(GameManager.instance.robots);

        if (winText != null)
        {
            winText.text = "You saved " + GameManager.instance.finalRobotCount.ToString() + " Robots!";
        }
    }

    public void setScene(string scene)
    {
        GameManager.instance.setScene(scene);
    }

    public void nextLevel()
    {
        GameManager.instance.nextScene();
        //showMenu = !showMenu;
        //menuCanvas.enabled = showMenu;
    }

    public void startLevel()
    {
        showMenu = !showMenu;
        menuCanvas.enabled = showMenu;
    }

    public void exitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
