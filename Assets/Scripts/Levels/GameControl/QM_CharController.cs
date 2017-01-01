using UnityEngine;
using System.Collections;

public class QM_CharController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 6; // move speed
    //private float turnSpeed = 90; // turning speed (degrees/second)
    private float lerpSpeed = 10; // smoothing speed
    private float gravity = 10; // gravity acceleration
    private bool isGrounded;
    private float deltaGround = 0.2f; // character is grounded up to this distance
    private float jumpSpeed = 10; // vertical jump initial speed
    private float jumpRange = 8; // range to detect target wall
    private Vector3 surfaceNormal; // current surface normal
    [SerializeField]
    private Vector3 myNormal; // character normal
    private float distGround; // distance from character position to ground
    private bool jumping = false; // flag &quot;I'm jumping to wall&quot;
    private bool canMove = true;
    private float vertSpeed = 0; // vertical jump current speed

    private Transform myTransform;
    [SerializeField]
    private Vector3 myForward;
    [SerializeField]
    private Vector3 myRight;
    [SerializeField]
    private int panelIndex;
    private Vector3 myOrgNormal;
    private Vector3 myOrgForward;
    private Vector3 myOrgPos;
    private Quaternion myOrgRot;
    private Vector3 mMove;
    private float h, v, forwardAmount;
    public BoxCollider boxCollider; // drag BoxCollider ref in editor

    private void Start()
    {
        myForward = Vector3.left;
        myRight = Vector3.forward;
        myNormal = transform.up; // normal starts as character up direction
        myOrgNormal = myNormal;
        myOrgForward = myForward;
        myOrgPos = transform.position;
        myOrgRot = transform.rotation;
        myTransform = transform;
        GetComponent<Rigidbody>().freezeRotation = true; // disable physics rotation
                                                         // distance from transform.position to ground
        distGround = boxCollider.size.y - boxCollider.center.y;

        
    }

    private void FixedUpdate()
    {
        // apply constant weight force according to character normal:
        GetComponent<Rigidbody>().AddForce(-gravity * GetComponent<Rigidbody>().mass * myNormal);

        if (Vector3.Distance(myNormal, Vector3.forward) < 0.1f)
        {
            panelIndex = 1;
        }
        else if (Vector3.Distance(myNormal, Vector3.up) < 0.1f)
        {
            panelIndex = 0;
        }
        else
            panelIndex = 2;
    }

    private void Update()
    {
        if (!canMove)
            return;
        // jump code - jump to wall or simple jump
        if (!jumping)
        {
            RaycastHit hit;

            if (Input.GetButtonDown("Jump"))
            { // jump pressed:
                bool wallAhead = Physics.Raycast(myTransform.position, myTransform.forward, out hit, jumpRange);
                if (wallAhead && hit.transform.tag.Equals("Jumpable"))
                { // wall ahead?
                    Debug.Log("Jumping to <" + hit.transform.tag + "> " + hit.transform.name);
                    JumpToWall(hit.point, hit.normal); // yes: jump to the wall
                }
                //else if (Physics.Raycast(myTransform.position, -myTransform.forward, out hit, jumpRange))
                //{
                //    JumpToWall(hit.point, hit.normal); // yes: jump to the wall
                //}
                else if (isGrounded)
                { // no: if grounded, jump up
                    GetComponent<Rigidbody>().velocity += jumpSpeed * myNormal;
                }
            }

            // movement code - turn left/right with Horizontal axis:
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            mMove = v * myForward + h * myRight;
            if (h != 0 || v != 0)
            {
                myTransform.rotation = Quaternion.LookRotation(mMove.normalized, myTransform.up);
            }

            //myTransform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0);

            // update surface normal and isGrounded:
            if (Physics.Raycast(myTransform.position, -myNormal, out hit))
            { // use it to update myNormal and isGrounded
                if (hit.transform.tag.Equals("Jumpable"))
                {
                    isGrounded = hit.distance <= distGround + deltaGround;
                    surfaceNormal = hit.normal;
                }
            }
            else
            {
                isGrounded = false;
                // assume usual ground normal to avoid "falling forever"
                surfaceNormal = Vector3.up;
            }
            myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed * Time.deltaTime);
            // find forward direction with new myNormal:
            Vector3 curForward = Vector3.Cross(myTransform.right, myNormal);
            // align character to the new myNormal while keeping the forward direction:
            Quaternion targetRot = Quaternion.LookRotation(curForward, myNormal);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRot, lerpSpeed * Time.deltaTime);
            // move the character forth/back with Vertical axis:
            //myTransform.Translate(0, 0, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
            myTransform.Translate(mMove * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void JumpToWall(Vector3 point, Vector3 normal)
    {
        // jump to wall
        jumping = true; // signal it's jumping to wall
        GetComponent<Rigidbody>().isKinematic = true; // disable physics while jumping
        Vector3 orgPos = myTransform.position;
        Quaternion orgRot = myTransform.rotation;
        Vector3 dstPos = point + normal * (distGround + 0.5f); // will jump to 0.5 above wall
        Vector3 curForward = Vector3.Cross(myTransform.right, normal);
        Quaternion dstRot = Quaternion.LookRotation(curForward, normal);

        StartCoroutine(jumpTime(orgPos, orgRot, dstPos, dstRot, normal));
        //jumptime
    }

    private IEnumerator jumpTime(Vector3 orgPos, Quaternion orgRot, Vector3 dstPos, Quaternion dstRot, Vector3 normal)
    {
        for (float t = 0.0f; t < 1.0f;)
        {
            t += Time.deltaTime;
            myTransform.position = Vector3.Lerp(orgPos, dstPos, t);
            myTransform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
            yield return null; // return here next frame
        }
        if (myNormal != myOrgNormal && normal != myOrgNormal)
        {
            myNormal = normal;
            myForward = Quaternion.FromToRotation(myOrgNormal, myNormal) * myOrgForward;
        }
        else
        {
            myForward = Quaternion.FromToRotation(myNormal, normal) * myForward;
            myNormal = normal; // update myNormal
                               //myForward = dstRot * Quaternion.Inverse(orgRot) * myForward;
        }
        myRight = -Vector3.Cross(myForward, myNormal);

        GetComponent<Rigidbody>().isKinematic = false; // enable physics
        jumping = false; // jumping to wall finished

    }

    public void respawn(Vector3 pos, Quaternion rot)
    {
        myTransform.position = pos;
        myTransform.rotation = rot;
    }

    public void respawn()
    {
        myTransform.position = myOrgPos;
        myTransform.rotation = myOrgRot;
        myNormal = myOrgNormal;
        myForward = myOrgForward;
        myRight = -Vector3.Cross(myForward, myNormal);
    }

    public void freezeMotion()
    {
        canMove = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void unfreezeMotion()
    {
        canMove = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public int getPanelIndex()
    {
        return panelIndex;
    }
}
