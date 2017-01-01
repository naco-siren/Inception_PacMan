using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {


    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonUp("Submit"))
        {
            SceneManager.LoadScene("Level1_UIUC", LoadSceneMode.Single);
        }
    }
    
}
