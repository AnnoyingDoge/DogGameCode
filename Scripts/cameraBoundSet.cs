using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraBoundSet : MonoBehaviour
{
    //This is a quick script which sets the collider attached to the camera to be the same size as the camera. Useful as it ensures any camera bounds always work.
    BoxCollider2D coll;
    private void Awake()
    {
        coll = gameObject.GetComponent<BoxCollider2D>();
        coll.size = new Vector2(Camera.main.orthographicSize * 2 * Screen.width / Screen.height, Camera.main.orthographicSize);
    }
    void Update()
    {
        coll.size = new Vector2(Camera.main.orthographicSize * 2 * Screen.width / Screen.height, Camera.main.orthographicSize);
    }
}
