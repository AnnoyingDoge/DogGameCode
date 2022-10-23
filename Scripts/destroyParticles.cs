using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyParticles : MonoBehaviour
{
    //This script is kind of... bad. It helps orient the dash particles, then destroys them after 0.1s.
    public float particleLife;

    private dogAnimator animationScript;
    // Start is called before the first frame update
    void Start()
    {
        animationScript = GameObject.FindGameObjectWithTag("player").GetComponent<dogAnimator>();
        if(animationScript.facingRight)
        {
            var shape = gameObject.GetComponent<ParticleSystem>().shape;
            shape.rotation = new Vector3(0, 270, 0);
        }
        particleLife = 0.5f;
        Destroy(gameObject, particleLife);
        StartCoroutine(transparencyChange());
    }

    IEnumerator transparencyChange()
    {
        yield return new WaitForSeconds(0.1f);
        var emission = gameObject.GetComponent<ParticleSystem>().emission;
        emission.enabled = false;
    }
}
