using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraBoundSet : MonoBehaviour
{
    BoxCollider2D coll;
    private void Awake()
    {
        coll = gameObject.GetComponent<BoxCollider2D>();
        coll.size = new Vector2(Camera.main.orthographicSize * 2 * Screen.width / Screen.height, Camera.main.orthographicSize);
    }
    void Update()
    {
        coll = gameObject.GetComponent<BoxCollider2D>();
        coll.size = new Vector2(Camera.main.orthographicSize * 2 * Screen.width / Screen.height, Camera.main.orthographicSize);
    }
}
