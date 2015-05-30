using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClassicController : MonoBehaviour {

    // Singleton
    public static ClassicController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject PrefabPianoRow;
    public GameObject PrefabShiftScene;

    public GameObject TimeScore;
    public GameObject TimeScoreShadow;

    private GameObject m_ShiftScene;

    public LinkedList<GameObject> m_Rows;
    public float GameSpeed;
    public float DeadEnd;

    public bool IsPaused;
    private float m_StartTime;
    private float m_MeanTime;

    private float m_Step;
    private IEnumerator m_LastShiftRoutine;

    public int LastId;
    public bool SuccessThisTouch = false;
    public bool IsGameOver = false;

    private int m_RowNum;
    private int m_MaxRowNum;

    IEnumerator PoolRowRoutine()
    {
        yield return new WaitForEndOfFrame();
        GameObject firstObj = m_Rows.First.Value;
        Vector3 position = m_Rows.Last.Value.transform.position;

        position.y += m_Step;
        firstObj.transform.position = position;
        firstObj.GetComponent<ClassicPianoRow>().Id = m_Rows.Last.Value.GetComponent<ClassicPianoRow>().Id + 1;
        m_Rows.RemoveFirst();
        m_Rows.AddLast(firstObj);
        m_RowNum++;
    }

    public void GenNewRow()
    {
        if (m_RowNum == m_MaxRowNum)
        {
            m_Rows.First.Value.SetActive(false);
            m_Rows.RemoveFirst();
            return;
        }
        StartCoroutine(PoolRowRoutine());
    }

    IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
        PlayerPrefs.SetFloat("ClassicScore", m_MeanTime);
        Application.LoadLevelAdditive("ClassicEndScene");
    }

    public void EndGame()
    {
        StopAllCoroutines();
        StartCoroutine(EndGameRoutine());
    }

    IEnumerator ShiftRowRoutine(GameObject obj)
    {
        Vector3 dest = new Vector3(0.0f, DeadEnd + m_Step, 0.0f);
        Vector3 source = obj.transform.position;
        float startTime = Time.time;
        while (obj.transform.position != dest)
        {
            Vector3 newPos = Vector3.Lerp(source, dest, (Time.time - startTime) / 0.2f);
            Vector3 tslVct = newPos - obj.transform.position;
            foreach (GameObject row in m_Rows)
                row.transform.position += tslVct;
            yield return null;
        }
    }

    public void ShiftRow(GameObject org)
    {
        if (m_LastShiftRoutine != null)
            StopCoroutine(m_LastShiftRoutine);
        m_LastShiftRoutine = ShiftRowRoutine(org);
        StartCoroutine(m_LastShiftRoutine);
    }

    public int GetMaxRowNumber()
    {
        return m_MaxRowNum;
    }

    IEnumerator CountingRoutine()
    {
        while (true)
        {
            m_MeanTime = Time.time - m_StartTime;
            string str = string.Format("{0:0.0000}\"", m_MeanTime);
            TimeScore.GetComponent<TextMesh>().text = str;
            TimeScoreShadow.GetComponent<TextMesh>().text = str;
            yield return null;
        }
    }

    public void StartGame()
    {
        if (IsPaused)
        {
            IsPaused = false;
            m_StartTime = Time.time;
            StartCoroutine(CountingRoutine());
        }
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
            Debug.Log("Performance impace! Instantiate!");
            GameObject obj = Instantiate(PrefabPianoRow) as GameObject;
            Vector3 position = new Vector3();
            position.x = 0.0f;
            position.y = t1.y + (i + 0.5f) * step;
            position.z = 0.0f;
            obj.transform.position = position;
            obj.GetComponent<ClassicPianoRow>().Id = i;
            m_Rows.AddLast(obj);
        }
        m_Rows.First.Value.GetComponent<ClassicPianoRow>().IsMostFirstRow = true;
        IsPaused = true;
        LastId = 0;
        m_RowNum = 5;
        m_MaxRowNum = 50;

        Debug.Log("Performance impace! Instantiate!");
        m_ShiftScene = Instantiate(PrefabShiftScene) as GameObject;
        m_ShiftScene.GetComponent<ShiftScene>().ShiftInWhenReady = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && !IsGameOver)
        {
            foreach (GameObject obj in m_Rows)
            {
                obj.SendMessage("PlayerTapped", Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        if (SuccessThisTouch)
        {
            LastId++;
            SuccessThisTouch = false;
        }
	}
}
