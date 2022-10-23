using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class gameStart : MonoBehaviour
{
    public string levelToLoad;

    public Vector2 spawnPoint;

    public GameObject player;
    public GameObject mainCamera;
    public GameObject cameraHelper;
    public GameObject sceneManager;

    public GameObject debug;

    public string fallback;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //should grab serialized info on spawnpoint and level
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnPoint == null)
        {
            spawnPoint = Vector2.zero;
        }
    }

    public void loadLevel()
    {
        SceneManager.LoadScene(levelToLoad, LoadSceneMode.Single);
        Instantiate(player, spawnPoint, Quaternion.identity);
        Instantiate(cameraHelper, spawnPoint, Quaternion.identity);
        Instantiate(sceneManager, spawnPoint, Quaternion.identity);

        if (Debug.isDebugBuild)
        {
            Instantiate(debug, spawnPoint, Quaternion.identity);
        }

        StartCoroutine(waitForSceneLoad());
    }

    IEnumerator waitForSceneLoad()
    {
        while (SceneManager.GetActiveScene().name != levelToLoad)
        {
            yield return null;
        }

        // Do anything after proper scene has been loaded
        if (SceneManager.GetActiveScene().name == levelToLoad)
        {
            //Instantiate(mainCamera, spawnPoint, Quaternion.identity);
            player.transform.position = spawnPoint;
        }
        
    }

}
//test spawnpoints and levels
