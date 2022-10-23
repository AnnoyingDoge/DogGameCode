using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyDieParticles : MonoBehaviour
{
    public float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroySelf());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator destroySelf()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
