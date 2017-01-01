using UnityEngine;
using System.Collections;

public class PacdotBehavior : MonoBehaviour {

    [SerializeField]
    private float rotationSpeed = 0.6f;
    [SerializeField]
    private int scoreAmount = 5;
    [SerializeField]
    private ParticleSystem Explosion;
    private bool isDone;

    // Sound effect
    private AudioSource audioSource;
    public AudioClip eatSound;

    // Use this for initialization
    void Awake()
    {
        isDone = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(rotationSpeed * Vector3.up * Time.deltaTime, rotationSpeed);
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
                GameManager.gm.pacdotEaten(scoreAmount);
            }

            // play sound effect
            audioSource.PlayOneShot(eatSound, 0.5f);

            if (Explosion != null)
            {
                ParticleSystem.Instantiate(Explosion,transform.position,Quaternion.identity);
            }

            // destroy self
            transform.GetChild(0).GetComponent<Renderer>().enabled = false;
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
