using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // Make game manager public static so can access this from other scripts
    public static GameManager gm;

    // Levels to move to on victory and lost
    public string levelAfterVictory;
    public string levelAfterGameOver;

    // Public variables
    public int score = 0;
    public int startLives = 3;
    public float levelTime = 60.0f;
    public int highScore = 0;

    // UI elements
    public Text UIScore;
    public Text UIHighScore;
    public Text UITime;
    public Text UIGameOver;
    public Text UIGamePaused;
    public GameObject[] UIExtraLives;

    // Audio control
    public AudioSource targetSource;
    public AudioSource deathSource;

    // Background musics
    public AudioClip backgroundMusic;
    public AudioClip countdownMusic;

    // Sound effects
    public AudioClip openingSound;
    public AudioClip pacmanDieSound;

    // References
    private GameObject _player;
    private Vector3 _spawnLocation;
    private Scene _scene;
    [SerializeField]
    private int curLives;
    private int curScore;
    private float timeLeft;
    private bool gamePaused = false;
    [SerializeField]
    private bool inPowerPelletStatus;

    //

    // Use this for initialization
    void Awake () {
        // Play the OP
        //audioSource = GetComponent<AudioSource>();
        //audioSource.PlayOneShot(audioSource.clip, 1f);

        // get a reference to the GameManager component for use by other scripts
        if (gm == null)
            gm = this.gameObject.GetComponent<GameManager>();

        // init scoreboard to 0
        //mainScoreDisplay.text = "Score: 0";
        Time.timeScale = 1f;

        // Use AudioSource on main camera
        if (targetSource == null)
            targetSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        // If none, create one
        if (targetSource == null)
            targetSource = GameObject.FindGameObjectWithTag("MainCamera").AddComponent<AudioSource>();

        setupDefaults();

        
    }

    void Start () {

        StartCoroutine(startLevelSound());

        inPowerPelletStatus = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!gamePaused)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0) timeLeft = 0;
                UITime.text = "Time Left: " + timeLeft.ToString("0.00");
            }
            else
            {
                endLevel();
            }
            if (Input.GetButtonUp("Fire2"))
            {
                Debug.Log("in");
                gamePaused = true;
                UIGamePaused.gameObject.SetActive(true);
                _player.GetComponent<QM_CharController>().freezeMotion();
                Time.timeScale = 0f;
            }
        }
        else
        {
            if (Input.GetButtonUp("Fire2"))
            {
                gamePaused = false;
                UIGamePaused.gameObject.SetActive(false);
                _player.GetComponent<QM_CharController>().unfreezeMotion();
                Time.timeScale = 1f;
            }
        }
    }

    void setupDefaults()
    {
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
            Debug.LogError("Player not found in Game Manager.");

        // Get current scene
        _scene = SceneManager.GetActiveScene();
        // Initial player location
        _spawnLocation = _player.transform.position;
        // If levels not specified, default to current level
        if (levelAfterVictory == "")
        {
            Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
            levelAfterVictory = _scene.name;
        }
        if (levelAfterGameOver == "")
        {
            Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
            levelAfterGameOver = _scene.name;
        }
        timeLeft = levelTime;

        refreshPlayerState();

        refreshGUI();
    }

    void refreshPlayerState()
    {
        PlayerPrefManager.ResetPlayerState(startLives, false);
        curLives = PlayerPrefManager.GetLives();
        score = PlayerPrefManager.GetScore();
        //Debug.Log(PlayerPrefManager.GetScore());
        highScore = PlayerPrefManager.GetHighscore();
    }

    void refreshGUI()
    {
        UIScore.text = "Score: " + score.ToString();
        UIHighScore.text = "Highscore: " + highScore.ToString();
        //timeLeft = levelTime;
        UITime.text = "Time Left: " + timeLeft.ToString("0.00");

        // turn on the appropriate number of life indicators in the UI based on the number of lives left
        for (int i = 0; i < UIExtraLives.Length; i++)
        {
            if (i < (curLives - 1))
            { // show one less than the number of lives since you only typically show lifes after the current life in UI
                UIExtraLives[i].SetActive(true);
            }
            else
            {
                UIExtraLives[i].SetActive(false);
            }
        }
    }

    public void resetLevel()
    {
        curLives--;
        refreshGUI();
        if (curLives > 0)
        {
            deathSource.PlayOneShot(pacmanDieSound, 0.8f);
            _player.GetComponent<QM_CharController>().respawn();
        }
        else
        {
            endLevel();
        }
    }

    public void endLevel()
    {
        UIGameOver.gameObject.SetActive(true);
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        for (int i = 0; i < ghosts.Length; i++)
        {
            Debug.Log(ghosts[i].name);
            ghosts[i].GetComponent<GhostAI>().freezeMotion();
            Debug.Log(i);
        }
        _player.GetComponent<QM_CharController>().freezeMotion();
        targetSource.Stop();
        targetSource.loop = false;
        targetSource.PlayOneShot(pacmanDieSound, 0.8f);
        PlayerPrefManager.SetHighscore(highScore);
        //SceneManager.LoadScene(levelAfterGameOver);
        StartCoroutine(loadNextLevel(3.0f, levelAfterGameOver));
    }

    public void pacdotEaten(int scoreAmount)
    {

        // increase the score by the scoreAmount and update the text UI
        score += scoreAmount;
        UIScore.text = "Score: " + score.ToString();
        if (score > highScore)
        {
            highScore = score;
            UIHighScore.text = "Highscore: " + highScore.ToString();
        }
    }
    public void powerPelletEaten(int scoreAmount)
    {
        // increase the score by the scoreAmount and update the text UI
        score += scoreAmount;
        UIScore.text = "Score: " + score.ToString();
        if (score > highScore)
        {
            highScore = score;
            UIHighScore.text = "Highscore: " + highScore.ToString();
        }


        // PowerPellet status
        if(inPowerPelletStatus == true)
        {
            StopCoroutine(enterPowerPelletStatus());
        }
        inPowerPelletStatus = true;
        StartCoroutine(enterPowerPelletStatus());
        
    }

    public bool isGamePaused()
    {
        return gamePaused;
    }

    public int getPanelIndex()
    {
        return _player.GetComponent<QM_CharController>().getPanelIndex();
    }

    public IEnumerator startLevelSound()
    {
        targetSource.loop = false;
        targetSource.PlayOneShot(openingSound);
        yield return new WaitForSeconds(4.4f);
        targetSource.PlayOneShot(backgroundMusic, 0.8f);
        targetSource.loop = true;
    }

    IEnumerator loadNextLevel(float time, string level)
    {
        yield return new WaitForSeconds(time);
        Time.timeScale = 1f;
        SceneManager.LoadScene(level);
    }

    public IEnumerator enterPowerPelletStatus()
    {
        yield return new WaitForSeconds(12f);
        inPowerPelletStatus = false;
    }

    public bool isInPowerPelletStatus()
    {
        return inPowerPelletStatus;
    }
}
