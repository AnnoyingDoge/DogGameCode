using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashIndicator : MonoBehaviour
{
    [SerializeField]
    private GameObject parent;
    [SerializeField]
    private SpriteRenderer sr;
    [SerializeField]
    private SpriteRenderer parentSR;

    [SerializeField]
    private Shader GUIShader;

    [SerializeField]
    private newMove moveScript;

    [SerializeField]
    private Color canDashColor;
    [SerializeField]
    private Color noDashColor;

    // Start is called before the first frame update
    void Start()
    {
        parentSR = GetComponentInParent<SpriteRenderer>();
        moveScript = GetComponentInParent<newMove>();

        sr = gameObject.AddComponent<SpriteRenderer>();

        GUIShader = Shader.Find("GUI/Text Shader");
        sr.material.shader = GUIShader;
    }

    // Update is called once per frame
    void Update()
    {
        sr.sprite = parentSR.sprite;
        if(moveScript.canDash)
        {
            sr.color = canDashColor;
        }
        else
        {
            sr.color = noDashColor;
        }
    }
}
