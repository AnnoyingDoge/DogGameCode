using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyDieParticles : MonoBehaviour
{
    //Really simple, just calls a coroutine to wait for a certain amount of time (defined in editor prefab) to destroy particle.
    public float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroySelf());
    }

    private IEnumerator destroySelf()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
