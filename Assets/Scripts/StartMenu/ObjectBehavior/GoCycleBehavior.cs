using UnityEngine;
using System.Collections;

public class GoCycleBehavior : MonoBehaviour {

    [SerializeField]
    private float moveSpeed = 3;

    private float width = 5;
    private float length = 9;
    private int direction; // 0: down, 1: left, 2: up; 3: right
    
    void Start () {
        direction = 0;
    }
	
	void Update () {
        if(direction == 0)
        {
            if(transform.position.z < width)
            {   
                // Continue to move down
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            } else
            {
                
                // Turn left
                transform.Rotate(Vector3.up, 90);
                direction = 1;
            }
        } else if (direction == 1)
        {
            if (transform.position.x < length)
            {
                // Continue to move left
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            else
            {
                // Turn left
                transform.Rotate(Vector3.up, 90);
                direction = 2;
            }
        }
        else if (direction == 2)
        {
            if (transform.position.z > -width)
            {
                // Continue to move left
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            else
            {
                // Turn left
                transform.Rotate(Vector3.up, 90);
                direction = 3;
            }
        }
        else if (direction == 3)
        {
            if (transform.position.x > -length)
            {
                // Continue to move left
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            else
            {
                // Turn left
                transform.Rotate(Vector3.up, 90);
                direction = 0;
            }
        }

        //transform.position.x;
	}
}
