using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneLoader : MonoBehaviour
{
    public string currentScene;
    public GameObject[] roomList;
    public GameObject[] oldRoomList;
    public Scene activeScene;

    public List<Scene> keepLoaded;
    public List<Scene> allScenes;

    private Collision coll;

    public Vector3 spawnPoint;
    public GameObject playerObject;
    public GameObject deathParticles;

    // Start is called before the first frame update
    void Start()
    {
        //roomList = new GameObject[8];
        currentScene = SceneManager.GetActiveScene().name;
        roomList = GameObject.FindGameObjectsWithTag("roomChange");
        print(roomList);
        activeScene = SceneManager.GetActiveScene();

        playerObject = GameObject.FindGameObjectWithTag("player");
        coll = playerObject.GetComponent<Collision>();

        foreach(GameObject obj in roomList)
        {
            if(obj.scene == SceneManager.GetActiveScene())
            {
                loadArea(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentScene = activeScene.name;
        if(activeScene != SceneManager.GetActiveScene())
        {
            activeScene = SceneManager.GetActiveScene();
            transitionScene();
        }
        if(coll.dead)
        {
            coll.dead = false;
            StartCoroutine(playerDeath());
        }
    }


    private void loadArea(GameObject obj)
    {
        roomChange room;
        string sceneName;
        Scene scene;

        room = obj.GetComponent<roomChange>();
        sceneName = room.sceneToActivate;
        scene = SceneManager.GetSceneByName(sceneName);

        if(!scene.isLoaded)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    public void transitionScene()
    {
        if (coll.transitionObject != null)
        {
            spawnPoint = coll.transitionObject.transform.position;
        }
        keepLoaded.Clear();
        oldRoomList = roomList;
        roomList = GameObject.FindGameObjectsWithTag("roomChange");

        roomChange room;
        string sceneName;
        Scene scene;

        foreach (GameObject obj in roomList)
        {
            if (obj.scene == SceneManager.GetActiveScene())
            {
                room = obj.GetComponent<roomChange>();
                sceneName = room.sceneToActivate;
                scene = SceneManager.GetSceneByName(sceneName);

                loadArea(obj);

                keepLoaded.Add(scene);
            }
        }
        unloadAreas();
    }

    //public IEnumerator transitionScene()
    //{
    //    print(roomList.Length);
    //    print("transition");
    //    if (coll.transitionObject != null)
    //    {
    //        spawnPoint = coll.transitionObject.transform.position;
    //    }
    //    keepLoaded.Clear();
    //    oldRoomList = roomList;

    //    roomChange room;
    //    string sceneName;
    //    Scene scene;

    //    while (roomList.Length == 0)
    //    {
    //        roomList = GameObject.FindGameObjectsWithTag("roomChange");
    //        print("gaming");
    //        yield return null;
    //    }

    //    foreach (GameObject obj in roomList)
    //    {
    //        if (obj.scene == SceneManager.GetActiveScene())
    //        {
    //            room = obj.GetComponent<roomChange>();
    //            sceneName = room.sceneToActivate;
    //            scene = SceneManager.GetSceneByName(sceneName);

    //            loadArea(obj);

    //            keepLoaded.Add(scene);
    //        }
    //    }
    //    unloadAreas();
    //}
    private void unloadAreas()
    {
        allScenes = SceneManager.GetAllScenes().ToList();

        foreach(Scene scene in allScenes)
        {
            if(!keepLoaded.Contains(scene) && scene.name != "PlayerScene" && scene != SceneManager.GetActiveScene())
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    private IEnumerator playerDeath()
    {
        playerObject.SetActive(false);
        Instantiate(deathParticles, playerObject.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1.0f);

        ResetRoom();
        yield return null;
    }

    public void ResetRoom()
    {
        print("die");
        //add to death counter
        //make sure spawnpoint is set elsewhere, save to file probably so on load it works.
        playerObject.GetComponent<newMove>().EndDash(true);
        playerObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerObject.transform.position = spawnPoint;
        playerObject.SetActive(true);
    }

    

    //private IEnumerator loadScene(GameObject obj)
    //{
    //    roomChange room;
    //    string sceneName;
    //    Scene scene;
    //    bool waiting;

    //    room = obj.GetComponent<roomChange>();
    //    sceneName = room.sceneToActivate;
    //    scene = SceneManager.GetSceneByName(sceneName);
    //    yield return null;

    //    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Scene3");
    //    asyncOperation.allowSceneActivation = false;
    //    waiting = true;

    //    while (waiting)
    //    {
    //        yield return null;
    //    }

    //    asyncOperation.allowSceneActivation = true;
    //}
}
