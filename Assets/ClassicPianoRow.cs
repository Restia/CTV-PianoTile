using UnityEngine;
using System.Collections;

public class ClassicPianoRow : MonoBehaviour {

    public GameObject PrefabPianoTile;

    private GameObject[] m_Tiles = new GameObject[4];
    private PianoTile2[] m_CachedTiles = new PianoTile2[4];

    private float m_Height;
    private float m_Step;
    private float m_Left;

    // hitbox
    private Vector2 m_TopLeft;
    private Vector2 m_BottomRight;

    public int Id;
    public bool IsMostFirstRow = false;

    private int m_BlackId;
    private bool m_GotPressed = false;

    IEnumerator DelayThenCallEndGame()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerPrefs.SetInt("Win", 1);
        ClassicController.Instance.EndGame();
    }

    private void PlayerTapped(Vector3 touchPosition)
    {
        if (m_GotPressed)
            return;

        Vector2 currPos = transform.position;
        Vector2 absTopLeft = currPos + m_TopLeft;
        Vector2 absBottomRight = currPos + m_BottomRight;
        if (touchPosition.x > absTopLeft.x && touchPosition.x < absBottomRight.x
            && touchPosition.y < absTopLeft.y && touchPosition.y > absBottomRight.y
            && Id == ClassicController.Instance.LastId + 1 && m_GotPressed == false)
        {
            ClassicController.Instance.SuccessThisTouch = true;
            m_Tiles[m_BlackId].GetComponent<Animator>().SetBool("Pressed", true);
            ClassicController.Instance.StartGame();
            ClassicController.Instance.ShiftRow(gameObject);
            m_GotPressed = true;
            if (ClassicController.Instance.LastId + 1 == ClassicController.Instance.GetMaxRowNumber())
            {
                StartCoroutine(DelayThenCallEndGame());
            }
            return;
        }

        if (touchPosition.y > currPos.y - m_Height / 2.0f
            && touchPosition.y < currPos.y + m_Height / 2.0f
            && Id == ClassicController.Instance.LastId + 1)
        {
            float val = touchPosition.x - m_Left;
            int id = (int)(val / m_Step);
            // ClassicController.Instance.LastId++;
            ClassicController.Instance.SuccessThisTouch = true;
            if (id == m_BlackId)
            {
                ClassicController.Instance.StartGame();
                m_Tiles[id].GetComponent<Animator>().SetBool("Pressed", true);
                ClassicController.Instance.ShiftRow(gameObject);
                m_GotPressed = true;
                if (ClassicController.Instance.LastId == ClassicController.Instance.GetMaxRowNumber())
                {
                    StartCoroutine(DelayThenCallEndGame());
                }
            }
            else
            {
                ClassicController.Instance.IsPaused = true;
                m_Tiles[id].GetComponent<Animator>().SetBool("Wrong", true);
                // End game invoke
                PlayerPrefs.SetInt("Win", 0);
                ClassicController.Instance.IsGameOver = true;
                ClassicController.Instance.EndGame();
            }
        }
    }

    public int GetBlackId()
    {
        return m_BlackId;
    }

    public void SetBlack()
    {
        IsMostFirstRow = false;
        m_GotPressed = false;
        for (int i = 0; i < 4; i++)
        {
            m_CachedTiles[i].BackgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            m_CachedTiles[i].Refresh();
        }
        m_BlackId = Random.Range(0, 4);
        m_CachedTiles[m_BlackId].BackgroundColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        m_CachedTiles[m_BlackId].Refresh();

        // calculate hitbox
        m_TopLeft = new Vector2(-m_Step * 2.3f + m_Step * m_BlackId, m_Height / 2.0f * 1.3f);
        m_BottomRight = new Vector2(-m_Step * 1.7f + m_Step * (m_BlackId + 1), -m_Height / 2.0f * 1.3f);
    }

    public void SetYellowAll()
    {
        m_GotPressed = true;
        for (int i = 0; i < 4; i++)
        {
            m_Tiles[i].GetComponent<PianoTile2>().BackgroundColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            m_Tiles[i].GetComponent<PianoTile2>().Refresh();
        }
    }

    // Use this for initialization
    void Start()
    {
        Vector2 t1 = Camera.main.ScreenToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
        Vector2 t2 = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width - 5.0f) / 4.0f + 1.0f,
            (Screen.height - 5.0f) / 4.0f + 1.0f, 0.0f));
        Vector2 t3 = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width - 5.0f) / 4.0f + 2.0f,
            (Screen.height - 5.0f) / 4.0f + 2.0f, 0.0f));
        Vector2 t = (t1 + t2) / 2.0f;
        float step = t3.x - t1.x;

        float width = t2.x - t1.x;
        float height = t2.y - t1.y;

        m_Height = height;
        m_Step = step;
        m_Left = t1.x;

        for (int i = 0; i < 4; i++)
        {
            Debug.Log("Performance impace! Instantiate!");
            m_Tiles[i] = Instantiate(PrefabPianoTile) as GameObject;
            m_CachedTiles[i] = m_Tiles[i].GetComponent<PianoTile2>();
            m_CachedTiles[i].Width = width;
            m_CachedTiles[i].Height = height;
            m_Tiles[i].transform.parent = transform;
            m_Tiles[i].transform.localPosition = new Vector2(t.x + i * step, 0.0f);
        }
        if (!IsMostFirstRow)
            SetBlack();
        else SetYellowAll();
    }

    void LateUpdate()
    {
        if (!ClassicController.Instance.IsPaused)
        {
            Vector3 currPos = transform.position;
            if (currPos.y <= ClassicController.Instance.DeadEnd)
            {
                SetBlack();
                ClassicController.Instance.GenNewRow();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
