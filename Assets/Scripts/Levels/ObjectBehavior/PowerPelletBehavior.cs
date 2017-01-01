using UnityEngine;
using System.Collections;

public class PowerPelletBehavior : MonoBehaviour {

    [SerializeField]
    private float rotationSpeed = 0.6f;
    private int scoreAmount = 100;
    private bool isDone;

    // Sound effect
    private AudioSource audioSource;
    public AudioClip eatSound;

    // Use this for initialization
    void Start ()
    {
        isDone = false;
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.Rotate(rotationSpeed * Vector3.forward * Time.deltaTime,  rotationSpeed);
	}

    
    void OnTriggerEnter(Collider other)
    {
        if (isDone)
            return;

        if (other.transform.tag == "Player")
        {
            // if game manager exists, make adjustments based on target properties
            if (GameManager.gm)
            {
                GameManager.gm.powerPelletEaten(scoreAmount);
            }

            // play sound effect
            audioSource.PlayOneShot(eatSound, 1.0f);

            // destroy self
            transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            transform.GetChild(1).GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(finish());
        }
    }

    IEnumerator finish()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

}
