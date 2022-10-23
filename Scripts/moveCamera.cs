using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{






    //unnecessary script lol











    public float x;
    public float y;
    // Start is called before the first frame update
    void Start()
    {
        x = 0;
        y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(x, y, transform.position.z); 
    }
}
