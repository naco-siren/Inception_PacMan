using UnityEngine;
using System.Collections;
using System.Linq;

public class GhostAI : MonoBehaviour {
    
    // which panel the ghost is standing on
    public int panelIndex; // 0: bottom, 1: left, 2: right

    // reference to the player
    private GameObject player;

    // move configuration
    private bool canMove = true;
    private float moveSpeed;
    private float minMoveSpace = 4;
    private float maxDetectDistance = 50;

    // const value
    const float chaseSpeed = 5;
    const float hideSpeed = 7;
    const int scoreAmount = 500;
    

	// Use this for initialization
	void Start () {
        moveSpeed = chaseSpeed;
        player = GameObject.FindGameObjectWithTag("Player");
	}

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.collider.transform.tag;
        if(tag != null)
        {
            // If catch the player
            if (tag.Equals("Player"))
            {
                if(GameManager.gm.isInPowerPelletStatus() == true)
                {
                    GameManager.gm.pacdotEaten(scoreAmount);
                }
                else
                {
                    GameManager.gm.resetLevel();
                }
                
            }
        }
            
    }
    

    // Update is called once per frame
    void Update () {
        if (!canMove)
            return;

        switch (panelIndex)
        {
            case 0:
                if(GameManager.gm.getPanelIndex() == 0)
                {
                    if(GameManager.gm.isInPowerPelletStatus() == true)
                    {
                        moveSpeed = hideSpeed;
                        HideOnPanelBottom();
                    }
                    else
                    {
                        moveSpeed = chaseSpeed;
                        ChaseOnPanelBottom();
                    }
                }
                    
                break;
            case 1:
                if (GameManager.gm.getPanelIndex() == 1)
                {
                    if (GameManager.gm.isInPowerPelletStatus() == true)
                    {
                        moveSpeed = hideSpeed;
                        HideOnPanelLeft();
                    }
                    else
                    {
                        moveSpeed = chaseSpeed;
                        ChaseOnPanelLeft();
                    }
                }
                break;
            case 2:
                if (GameManager.gm.getPanelIndex() == 2)
                {
                    if (GameManager.gm.isInPowerPelletStatus() == true)
                    {
                        moveSpeed = hideSpeed;
                        HideOnPanelRight();
                    }
                    else
                    {
                        moveSpeed = chaseSpeed;
                        ChaseOnPanelRight();
                    }
                }
                break;
        }

    }

 


    void ChaseOnPanelBottom()
    {
        Vector3 srcPosition = transform.position + new Vector3(0, 2, 0);  // y = 0.2
        Vector3 targetPosition = player.transform.position;  // y = 1.6
        //Debug.Log(srcPosition + " -> " + targetPosition);

        // Angrily stare at the player
        transform.LookAt(targetPosition);

        // Cast ray towards four directions
        RaycastHit[][] hits = new RaycastHit[4][];
        
        hits[0] = Physics.RaycastAll(srcPosition, Vector3.left, maxDetectDistance); // Look into north
        hits[1] = Physics.RaycastAll(srcPosition, Vector3.forward, maxDetectDistance); // Look into east
        hits[2] = Physics.RaycastAll(srcPosition, Vector3.right, maxDetectDistance); // Look into south
        hits[3] = Physics.RaycastAll(srcPosition, Vector3.back, maxDetectDistance); // Look into west

        //Debug.DrawRay(srcPosition, Vector3.left * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.forward * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.right * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.back * maxDetectDistance);

        // Calculate each direction's angle with the <source -> target>
        Vector3 idealDirection = targetPosition - srcPosition;
        float[] angles = new float[4];
        angles[0] = Vector3.Angle(idealDirection, Vector3.left);
        angles[1] = Vector3.Angle(idealDirection, Vector3.forward);
        angles[2] = Vector3.Angle(idealDirection, Vector3.right);
        angles[3] = Vector3.Angle(idealDirection, Vector3.back);
        
        
        for (int i = 0; i < 4; i++)
        {
            float minObstacleDist = 6;
            bool allyAhead = false;
            bool playerAhead = false;
            foreach (RaycastHit hit in hits[i])
            {
                string hitTag = hit.transform.tag;

                if (hitTag == null)
                    continue;

                if (hitTag.Equals("Player")) {
                    playerAhead = true;
                }
                else if (hitTag.Equals("Obstacle"))
                {
                    if (hit.distance < minObstacleDist)
                        minObstacleDist = hit.distance;
                }
                else if (hitTag.Equals("Ghost"))
                {
                    if (hit.distance < maxDetectDistance)
                        allyAhead = true;
                }
                
            }

            // 
            if(playerAhead == true)
            {
                Debug.Log("[" + i + "] player ahead");
                angles[i] = 0;
            }
            else if(allyAhead == true || minObstacleDist < minMoveSpace)
            {
                Debug.Log("[" + i + "] ally ahead");
                angles[i] = 180;
            }   
        }
        
        
        // Find the direction with minimum angle with the <source -> target>
        var angleList = angles.ToList();
        float minAngle = angleList.Min();
        int minAngleIndex = angleList.IndexOf(minAngle);
        //angleList.Remove(minAngle);

        Debug.Log(minAngleIndex + ":: " + angles[0] + ", " + angles[1] + ", " + angles[2] + ", " + angles[3]);
        
        // Move towards the best way
        switch (minAngleIndex)
        {
            case 0:
                transform.Translate(Vector3.left * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 1:
                transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 2:
                transform.Translate(Vector3.right * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 3:
                transform.Translate(Vector3.back * Time.deltaTime * moveSpeed, Space.World);
                break;
        }   
    }

    void ChaseOnPanelLeft()
    {
        Vector3 srcPosition = transform.position + new Vector3(0, 0, 2);  // z = 0.2
        Vector3 targetPosition = player.transform.position;
        //Debug.Log(srcPosition + " -> " + targetPosition);

        // Angrily stare at the player
        transform.LookAt(targetPosition, Vector3.forward);

        // Cast ray towards four directions
        RaycastHit[][] hits = new RaycastHit[4][];

        hits[0] = Physics.RaycastAll(srcPosition, Vector3.left, maxDetectDistance); // Look into north
        hits[1] = Physics.RaycastAll(srcPosition, Vector3.down, maxDetectDistance); // Look into east
        hits[2] = Physics.RaycastAll(srcPosition, Vector3.right, maxDetectDistance); // Look into south
        hits[3] = Physics.RaycastAll(srcPosition, Vector3.up, maxDetectDistance); // Look into west


        //Debug.DrawRay(srcPosition, Vector3.left * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.down * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.right * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.up * maxDetectDistance);

        // Calculate each direction's angle with the <source -> target>
        Vector3 idealDirection = targetPosition - srcPosition;
        float[] angles = new float[4];

        angles[0] = Vector3.Angle(idealDirection, Vector3.left);
        angles[1] = Vector3.Angle(idealDirection, Vector3.down);
        angles[2] = Vector3.Angle(idealDirection, Vector3.right);
        angles[3] = Vector3.Angle(idealDirection, Vector3.up);

        for (int i = 0; i < 4; i++)
        {
            bool allyAhead = false;
            bool playerAhead = false;
            float minObstacleDist = 6;
            foreach (RaycastHit hit in hits[i])
            {
                string hitTag = hit.transform.tag;
                if (hitTag == null)
                    continue;

                if (hitTag.Equals("Player"))
                {
                    playerAhead = true;
                    break;
                }
                else if (hitTag.Equals("Obstacle"))
                {
                    if (hit.distance < minObstacleDist)
                        minObstacleDist = hit.distance;
                }
                else if (hitTag.Equals("Ghost"))
                {
                    if (hit.distance < maxDetectDistance)
                        allyAhead = true;
                }

            }
            if (playerAhead == true)
            {
                angles[i] = 0;
            }
            else if (allyAhead == true || minObstacleDist < minMoveSpace)
            {
                angles[i] = 180;
            }
        }


        // Find the direction with minimum angle with the <source -> target>
        var angleList = angles.ToList();
        float minAngle = angleList.Min();
        int minAngleIndex = angleList.IndexOf(minAngle);
        //angleList.Remove(minAngle);

        //Debug.Log(minAngleIndex + ":: " + angles[0] + ", " + angles[1] + ", " + angles[2] + ", " + angles[3]);

        // Move towards the best way
        switch (minAngleIndex)
        {
            case 0:
                transform.Translate(Vector3.left * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 1:
                transform.Translate(Vector3.down * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 2:
                transform.Translate(Vector3.right * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 3:
                transform.Translate(Vector3.up * Time.deltaTime * moveSpeed, Space.World);
                break;
        }
    }

    void ChaseOnPanelRight()
    {
        Vector3 srcPosition = transform.position + new Vector3(2, 0, 0);  // z = 0.2
        Vector3 targetPosition = player.transform.position;
        //Debug.Log(srcPosition + " -> " + targetPosition);

        // Angrily stare at the player
        transform.LookAt(targetPosition, Vector3.right);

        // Cast ray towards four directions
        RaycastHit[][] hits = new RaycastHit[4][];

        hits[0] = Physics.RaycastAll(srcPosition, Vector3.up, maxDetectDistance); // Look into north
        hits[1] = Physics.RaycastAll(srcPosition, Vector3.forward, maxDetectDistance); // Look into east
        hits[2] = Physics.RaycastAll(srcPosition, Vector3.down, maxDetectDistance); // Look into south
        hits[3] = Physics.RaycastAll(srcPosition, Vector3.back, maxDetectDistance); // Look into west

        //Debug.DrawRay(srcPosition, Vector3.up * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.forward * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.down * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.back * maxDetectDistance);

        // Calculate each direction's angle with the <source -> target>
        Vector3 idealDirection = targetPosition - srcPosition;
        float[] angles = new float[4];

        angles[0] = Vector3.Angle(idealDirection, Vector3.up);
        angles[1] = Vector3.Angle(idealDirection, Vector3.forward);
        angles[2] = Vector3.Angle(idealDirection, Vector3.down);
        angles[3] = Vector3.Angle(idealDirection, Vector3.back);

        for (int i = 0; i < 4; i++)
        {
            float minObstacleDist = 6;
            bool allyAhead = false;
            bool playerAhead = false;
            foreach (RaycastHit hit in hits[i])
            {
                string hitTag = hit.transform.tag;
                if (hitTag == null)
                    continue;

                if (hitTag.Equals("Player"))
                {
                    playerAhead = true;
                    break;
                }
                else if (hitTag.Equals("Obstacle"))
                {
                    if (hit.distance < minObstacleDist)
                        minObstacleDist = hit.distance;
                }
                else if (hitTag.Equals("Ghost"))
                {
                    if (hit.distance < maxDetectDistance)
                        allyAhead = true;
                }

            }

            if (playerAhead == true)
            {
                angles[i] = 0;
            }
            else if (allyAhead == true || minObstacleDist < minMoveSpace)
            {
                angles[i] = 180;
            }
        }


        // Find the direction with minimum angle with the <source -> target>
        var angleList = angles.ToList();
        float minAngle = angleList.Min();
        int minAngleIndex = angleList.IndexOf(minAngle);
        //angleList.Remove(minAngle);

        //Debug.Log(minAngleIndex + ":: " + angles[0] + ", " + angles[1] + ", " + angles[2] + ", " + angles[3]);

        // Move towards the best way
        switch (minAngleIndex)
        {
            case 0:
                transform.Translate(Vector3.up * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 1:
                transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 2:
                transform.Translate(Vector3.down * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 3:
                transform.Translate(Vector3.back * Time.deltaTime * moveSpeed, Space.World);
                break;
        }
    }


    void HideOnPanelBottom()
    {
        Vector3 srcPosition = transform.position + new Vector3(0, 2, 0);  // y = 0.2
        Vector3 targetPosition = player.transform.position;  // y = 1.6
        Vector3 idealDirection = targetPosition - srcPosition;
        //Debug.Log(srcPosition + " -> " + targetPosition);

        // Angrily stare at the player
        transform.LookAt(srcPosition - idealDirection);

        // Cast ray towards four directions
        RaycastHit[][] hits = new RaycastHit[4][];

        hits[0] = Physics.RaycastAll(srcPosition, Vector3.left, maxDetectDistance); // Look into north
        hits[1] = Physics.RaycastAll(srcPosition, Vector3.forward, maxDetectDistance); // Look into east
        hits[2] = Physics.RaycastAll(srcPosition, Vector3.right, maxDetectDistance); // Look into south
        hits[3] = Physics.RaycastAll(srcPosition, Vector3.back, maxDetectDistance); // Look into west

        //Debug.DrawRay(srcPosition, Vector3.left * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.forward * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.right * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.back * maxDetectDistance);

        // Calculate each direction's angle with the <source -> target>
        float[] angles = new float[4];
        angles[0] = Vector3.Angle(idealDirection, Vector3.left);
        angles[1] = Vector3.Angle(idealDirection, Vector3.forward);
        angles[2] = Vector3.Angle(idealDirection, Vector3.right);
        angles[3] = Vector3.Angle(idealDirection, Vector3.back);


        for (int i = 0; i < 4; i++)
        {
            float minObstacleDist = 6;
            bool allyAhead = false;
            foreach (RaycastHit hit in hits[i])
            {
                string hitTag = hit.transform.tag;

                if (hitTag == null)
                    continue;

                if (hitTag.Equals("Obstacle"))
                {
                    if (hit.distance < minObstacleDist)
                        minObstacleDist = hit.distance;
                }
                else if (hitTag.Equals("Ghost"))
                {
                    if (hit.distance < maxDetectDistance)
                        allyAhead = true;
                }

            }
            if (allyAhead == true || minObstacleDist < minMoveSpace)
            {
                angles[i] = 0;
            }
        }


        // Find the direction with minimum angle with the <source -> target>
        var angleList = angles.ToList();
        float maxAngle = angleList.Max();
        int maxAngleIndex = angleList.IndexOf(maxAngle);
        //angleList.Remove(minAngle);

        //Debug.Log(minAngleIndex + ":: " + angles[0] + ", " + angles[1] + ", " + angles[2] + ", " + angles[3]);

        // Move towards the best way
        switch (maxAngleIndex)
        {
            case 0:
                transform.Translate(Vector3.left * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 1:
                transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 2:
                transform.Translate(Vector3.right * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 3:
                transform.Translate(Vector3.back * Time.deltaTime * moveSpeed, Space.World);
                break;
        }
    }

    void HideOnPanelLeft()
    {
        Vector3 srcPosition = transform.position + new Vector3(0, 0, 2);  // z = 0.2
        Vector3 targetPosition = player.transform.position;
        Vector3 idealDirection = targetPosition - srcPosition;
        //Debug.Log(srcPosition + " -> " + targetPosition);

        // Angrily stare at the player
        transform.LookAt(srcPosition - idealDirection, Vector3.forward);

        // Cast ray towards four directions
        RaycastHit[][] hits = new RaycastHit[4][];

        hits[0] = Physics.RaycastAll(srcPosition, Vector3.left, maxDetectDistance); // Look into north
        hits[1] = Physics.RaycastAll(srcPosition, Vector3.down, maxDetectDistance); // Look into east
        hits[2] = Physics.RaycastAll(srcPosition, Vector3.right, maxDetectDistance); // Look into south
        hits[3] = Physics.RaycastAll(srcPosition, Vector3.up, maxDetectDistance); // Look into west


        //Debug.DrawRay(srcPosition, Vector3.left * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.down * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.right * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.up * maxDetectDistance);

        // Calculate each direction's angle with the <source -> target>
        float[] angles = new float[4];
        angles[0] = Vector3.Angle(idealDirection, Vector3.left);
        angles[1] = Vector3.Angle(idealDirection, Vector3.down);
        angles[2] = Vector3.Angle(idealDirection, Vector3.right);
        angles[3] = Vector3.Angle(idealDirection, Vector3.up);

        for (int i = 0; i < 4; i++)
        {
            bool allyAhead = false;
            float minObstacleDist = 6;
            foreach (RaycastHit hit in hits[i])
            {
                string hitTag = hit.transform.tag;
                if (hitTag == null)
                    continue;

                if (hitTag.Equals("Obstacle"))
                {
                    if (hit.distance < minObstacleDist)
                        minObstacleDist = hit.distance;
                }
                else if (hitTag.Equals("Ghost"))
                {
                    if (hit.distance < maxDetectDistance)
                        allyAhead = true;
                }

            }
            if (allyAhead == true || minObstacleDist < minMoveSpace)
            {
                angles[i] = 0;
            }
        }


        // Find the direction with minimum angle with the <source -> target>
        var angleList = angles.ToList();
        float maxAngle = angleList.Max();
        int maxAngleIndex = angleList.IndexOf(maxAngle);
        //angleList.Remove(minAngle);

        //Debug.Log(minAngleIndex + ":: " + angles[0] + ", " + angles[1] + ", " + angles[2] + ", " + angles[3]);

        // Move towards the best way
        switch (maxAngleIndex)
        {
            case 0:
                transform.Translate(Vector3.left * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 1:
                transform.Translate(Vector3.down * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 2:
                transform.Translate(Vector3.right * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 3:
                transform.Translate(Vector3.up * Time.deltaTime * moveSpeed, Space.World);
                break;
        }
    }

    void HideOnPanelRight()
    {
        Vector3 srcPosition = transform.position + new Vector3(2, 0, 0);  // z = 0.2
        Vector3 targetPosition = player.transform.position;
        Vector3 idealDirection = targetPosition - srcPosition;
        //Debug.Log(srcPosition + " -> " + targetPosition);

        // Angrily stare at the player
        transform.LookAt(srcPosition - idealDirection, Vector3.right);

        // Cast ray towards four directions
        RaycastHit[][] hits = new RaycastHit[4][];

        hits[0] = Physics.RaycastAll(srcPosition, Vector3.up, maxDetectDistance); // Look into north
        hits[1] = Physics.RaycastAll(srcPosition, Vector3.forward, maxDetectDistance); // Look into east
        hits[2] = Physics.RaycastAll(srcPosition, Vector3.down, maxDetectDistance); // Look into south
        hits[3] = Physics.RaycastAll(srcPosition, Vector3.back, maxDetectDistance); // Look into west

        //Debug.DrawRay(srcPosition, Vector3.up * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.forward * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.down * maxDetectDistance);
        //Debug.DrawRay(srcPosition, Vector3.back * maxDetectDistance);

        // Calculate each direction's angle with the <source -> target>
        float[] angles = new float[4];
        angles[0] = Vector3.Angle(idealDirection, Vector3.up);
        angles[1] = Vector3.Angle(idealDirection, Vector3.forward);
        angles[2] = Vector3.Angle(idealDirection, Vector3.down);
        angles[3] = Vector3.Angle(idealDirection, Vector3.back);

        for (int i = 0; i < 4; i++)
        {
            bool allyAhead = false;
            float minObstacleDist = 6;
            foreach (RaycastHit hit in hits[i])
            {
                string hitTag = hit.transform.tag;
                if (hitTag == null)
                    continue;

                if (hitTag.Equals("Obstacle"))
                {
                    if (hit.distance < minObstacleDist)
                        minObstacleDist = hit.distance;
                }
                else if (hitTag.Equals("Ghost"))
                {
                    if (hit.distance < maxDetectDistance)
                        allyAhead = true;
                }

            }
            if (allyAhead == true || minObstacleDist < minMoveSpace)
            {
                angles[i] = 0;
            }
        }


        // Find the direction with minimum angle with the <source -> target>
        var angleList = angles.ToList();
        float maxAngle = angleList.Max();
        int maxAngleIndex = angleList.IndexOf(maxAngle);
        //angleList.Remove(minAngle);

        //Debug.Log(minAngleIndex + ":: " + angles[0] + ", " + angles[1] + ", " + angles[2] + ", " + angles[3]);

        // Move towards the best way
        switch (maxAngleIndex)
        {
            case 0:
                transform.Translate(Vector3.up * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 1:
                transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 2:
                transform.Translate(Vector3.down * Time.deltaTime * moveSpeed, Space.World);
                break;
            case 3:
                transform.Translate(Vector3.back * Time.deltaTime * moveSpeed, Space.World);
                break;
        }
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

    //void UpdateUsingUnityNavigation()
    //{
    //    NavMeshAgent agent = GetComponent<NavMeshAgent>();
    //    agent.speed = moveSpeed;
    //    agent.destination = player.transform.position;
        
    //}

    
}




