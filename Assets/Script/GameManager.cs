using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //Instance sebagai global access
    public static GameManager instance;
    public int playerScore;
    public Text scoreText;
    public Text timeText;
    public GameObject comboBanner;
    public Text comboText;
    public string namatag="";
    public bool checkTile=false;
    public bool doneHapus = false;
    public bool decreaseAktif = false;
    public bool moveActive = true;
    public int comboMultiply = 1;
    private int time;
    private float deltaTime;

    // singleton
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        resetGame();

    }

    void resetGame()
    {
        time = 60;
        deltaTime = 0;
        timeText.text = time.ToString();
    }

    private void Update()
    {
        if(time>=1)
        deltaTime += Time.deltaTime;
        if (deltaTime >= 1f && time >=1)
        {
            time -= 1;
            timeText.text = time.ToString();
            if (time == 0)
            {
                Time.timeScale = 0;
                moveActive = false;
                GameObject.Find("Canvas").transform.Find("Restart").gameObject.SetActive(true);
            }
            deltaTime = 0;
        }
    }

    void ActivateComboBanner(bool active)
    {
        comboText.text = comboMultiply+" X";
        comboBanner.gameObject.SetActive(active);
    }

    public void ShowComboBanner()
    {
        StartCoroutine(ShowCombo());
    }

    IEnumerator ShowCombo()
    {
        ActivateComboBanner(true);
        yield return new WaitForSeconds(2f);
        ActivateComboBanner(false);
    }

    public void restartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    //Update score dan ui
    public void GetScore(int point)
    {
        playerScore += point*comboMultiply;
        scoreText.text = playerScore.ToString();
    }
}