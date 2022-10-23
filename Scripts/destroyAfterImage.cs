using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyAfterImage : MonoBehaviour
{
    //All this does is destroy the "afterimage" that the player has while dashing. It's kind of hack-y, but whatever.
    private SpriteRenderer sr;
    [SerializeField]
    private float fadeAmount = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        //fade particle
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - fadeAmount);
        //destroy when faded
        if(sr.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
