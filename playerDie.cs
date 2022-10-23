using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDie : MonoBehaviour
{
    private sceneLoader sceneScript;
    public GameObject sceneManager;

    private Collision coll;
    public GameObject playerObject;

    public GameObject deathParticles;
    // Start is called before the first frame update
    void Start()
    {
        //sceneScript = sceneManager.GetComponent<sceneLoader>();
        //coll = playerObject.GetComponent<Collision>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
