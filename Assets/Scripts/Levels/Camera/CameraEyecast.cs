using UnityEngine;
using System.Collections;

public class CameraEyecast : MonoBehaviour {

    public Transform target;

    private RaycastHit[] hits;
    private float dist;
    private Vector3 dir;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        dist = Vector3.Distance(transform.position, target.position);
        dir = target.position - transform.position;
        hits = Physics.RaycastAll(transform.position, dir, dist);
        RaycastHit hit;
        ObjectTransparent temp;
        for (int i = 0; i < hits.Length; i++)
        {
            hit = hits[i];
            temp = hit.transform.GetComponent<ObjectTransparent>();
            if (temp != null)
            {
                temp.setTransparency(0.3f);
            }
        }
        hits = Physics.RaycastAll(target.position, -dir, dist);
        for (int i = 0; i < hits.Length; i++)
        {
            hit = hits[i];
            temp = hit.transform.GetComponent<ObjectTransparent>();
            if (temp != null)
            {
                temp.setTransparency(0.3f);
            }
        }
	}
}
