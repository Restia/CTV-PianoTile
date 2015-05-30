using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayController : MonoBehaviour {

    // Singleton
    public static PlayController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    private int m_Score;
    public GameObject ScoreNum;
    public GameObject ScoreNumShadow;

    public AudioClip[] BackgroundMusic;

    public GameObject PrefabPianoRow;
    public GameObject PrefabShiftScene;

    private GameObject m_ShiftScene;

    public LinkedList<GameObject> m_Rows;
    public float GameSpeed;
    public float RollBackSpeed;
    public float DeadEnd;

    public bool IsPaused;
    public bool FirstTouch;
    public bool IsGameOver = false;

    private float m_Step;

    public int LastId;
    public bool SuccessThisTouch = false;
    private int m_LastRowId;

    private bool m_DisableFunction = true;

    private WaitForSeconds speedWait = new WaitForSeconds(1.6f);

    IEnumerator RollBackRoutine()
    {
        Debug.Log("Rolling back");
        float rollBackDist = 0.0f;
        while (rollBackDist < m_Step)
        {
            rollBackDist += RollBackSpeed;
            foreach (GameObject obj in m_Rows)
            {
                Vector3 currPos = obj.transform.position;
                if (rollBackDist > m_Step)
                    currPos.y += (RollBackSpeed - (rollBackDist - m_Step));
                else currPos.y += RollBackSpeed;
                obj.transform.position = currPos;
            }
            yield return null;
        }
        EndGame();
    }

    public void RollBack()
    {
        IsGameOver = true;
        IsPaused = true;
        audio.Stop();
        StartCoroutine(RollBackRoutine());
    }

    IEnumerator PoolRowRoutine()
    {
        yield return new WaitForEndOfFrame();
        GameObject firstObj = m_Rows.First.Value;
        Vector3 position = m_Rows.Last.Value.transform.position;

        position.y += m_Step;
        firstObj.transform.position = position;
        firstObj.GetComponent<PianoRow>().Id = m_LastRowId + 1;
        m_LastRowId++;
        m_Rows.RemoveFirst();
        m_Rows.AddLast(firstObj);
    }

    public void GenNewRow()
    {
        StartCoroutine(PoolRowRoutine());
    }

    public static float fast_sqrt( float val)  
    {
        if (val <= 0.00001f)
            return 0.0f;
        int i = 0;
        float x;
        x = val;
        i = (1<<29) + (i >> 1) - (1<<22); 
 
        x = x + val/x;
        x = 0.25f*x + val/x;

        return x;
    }  

    IEnumerator IncreaseSpeedRoutine()
    {
        float maxSpeed = 1.5f * GameSpeed;
        float minSpeed = GameSpeed;
        float diff = maxSpeed - minSpeed;
        float startTime = Time.time;
        while (GameSpeed < maxSpeed)
        {
            float t = (Time.time - startTime) / 20.0f;
            if (t >= 1.0f)
                break;
            GameSpeed = diff * (fast_sqrt(1.0f - (t - 1.0f) * (t - 1.0f))) + minSpeed;
            Debug.Log("GameSpeed= " + GameSpeed);
            yield return speedWait;
        }
        GameSpeed = maxSpeed;
        Debug.Log("Max Speed reached!");
        yield return null;
    }

    IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
        Application.LoadLevelAdditive("FailedScene");
    }

    public void EndGame()
    {
        IsGameOver = true;
        if (PlayerPrefs.GetInt("Settings_Sound") == 1)
        {
            audio.Stop();
            CancelInvoke("PlayOtherSong");
        }
        PlayerPrefs.SetInt("Score", m_Score);
        StopAllCoroutines();
        StartCoroutine(EndGameRoutine());
    }

    public void IncScore()
    {
        m_Score++;
        ScoreNum.GetComponent<TextMesh>().text = m_Score.ToString();
        ScoreNumShadow.GetComponent<TextMesh>().text = m_Score.ToString();
    }

    private void ShiftSceneCallback()
    {
        m_DisableFunction = false;
    }

	// Use this for initialization
	void Start () {
        m_Rows = new LinkedList<GameObject>();
        Vector2 t1 = Camera.main.ScreenToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
        Vector2 t3 = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width - 5.0f) / 4.0f + 2.0f,
            (Screen.height - 5.0f) / 4.0f + 2.0f, 0.0f));
        float step = t3.y - t1.y;
        m_Step = step;
        DeadEnd = t1.y - 0.5f * step;
        for (int i = 0; i < 6; i++)
        {
            GameObject obj = Instantiate(PrefabPianoRow) as GameObject;
            Vector3 position = new Vector3();
            position.x = 0.0f;
            position.y = t1.y + (i + 0.5f) * step;
            position.z = 0.0f;
            obj.transform.position = position;
            obj.GetComponent<PianoRow>().Id = i;
            m_Rows.AddLast(obj);
        }
        m_Rows.First.Value.GetComponent<PianoRow>().IsMostFirstRow = true;
        IsPaused = true;
        FirstTouch = true;
        LastId = 0;
        m_LastRowId = 5;
        m_Score = 0;
        ScoreNum.GetComponent<TextMesh>().text = m_Score.ToString();
        ScoreNumShadow.GetComponent<TextMesh>().text = m_Score.ToString();

        if (PlayerPrefs.GetInt("Settings_Sound") == 1)
        {
            audio.clip = BackgroundMusic[Random.Range(0, 2)];
            Invoke("PlayOtherSong", audio.clip.length);
        }

        m_ShiftScene = Instantiate(PrefabShiftScene) as GameObject;
        m_ShiftScene.GetComponent<ShiftScene>().ShiftInWhenReady = true;
        m_ShiftScene.GetComponent<ShiftScene>().ShiftInCallback += ShiftSceneCallback;
	}

    private void PlayOtherSong()
    {
        audio.Stop();
        audio.clip = BackgroundMusic[Random.Range(0, 1)];
        audio.Play();
    }

    public void TryStartGame()
    {
        if (FirstTouch)
        {
            FirstTouch = false;
            IsPaused = false;
            if (PlayerPrefs.GetInt("Settings_Sound") == 1)
                audio.Play();
            StartCoroutine(IncreaseSpeedRoutine());
        }
    }

	// Update is called once per frame
	void Update () {
        if (!m_DisableFunction)
        {
            if (Input.GetMouseButtonDown(0) && !IsGameOver)
            {
                for (LinkedListNode<GameObject> i = m_Rows.First; i != m_Rows.Last; i = i.Next)
                {
                    i.Value.SendMessage("PlayerTapped", Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
            if (SuccessThisTouch)
            {
                LastId++;
                SuccessThisTouch = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_DisableFunction = true;
                StopAllCoroutines();
                m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
                Application.LoadLevelAdditive("MenuScene");
            }
        }
	}
}
