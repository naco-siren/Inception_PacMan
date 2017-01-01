using UnityEngine;
using System.Collections;

public class ObjectTransparent : MonoBehaviour {

    private Renderer rend;
    private Shader orgMtShader;
    private Color orgMtColor;
    private bool isTrans;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        orgMtShader = rend.material.shader;
        orgMtColor = rend.material.color;
        isTrans = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (!isTrans)
            resetTransparency();
	}

    void FixedUpdate()
    {
        isTrans = false;
    }

    public void setTransparency(float t)
    {
        rend.material.shader = Shader.Find("Transparent/Diffuse");
        Color tempColor = rend.material.color;
        tempColor.a = t;
        rend.material.color = tempColor;
        isTrans = true;
    }

    public void resetTransparency()
    {
        rend.material.shader = orgMtShader;
        rend.material.color = orgMtColor;
    }
}
