using UnityEngine;
using System.Collections;

public class Navigator : MonoBehaviour {

    public GameObject btNew;
    public GameObject btHighscore;
    public GameObject btFriends;
    public GameObject btClassic;
    public GameObject NavButtons;

    private int m_Position;
    public int Position
    {
        get
        {
            return m_Position;
        }
    }

    // Detect swipe
    private float m_BgHeight;
    private bool m_IsBegan;
    private Vector3 m_LastMousePos;
    private float m_OrgX;

    private void btNew_Clicked()
    {
        if (TestController.Instance.IsSliding
            || TestController.Instance.IsRepositioning)
            return;

        if (m_Position != 0)
        {
//             Debug.Log("Slide left to right");
            TestController.Instance.SlideRight(0);
            btNew.GetComponent<TextMesh>().color = Color.cyan;
            btHighscore.GetComponent<TextMesh>().color = Color.white;
            btFriends.GetComponent<TextMesh>().color = Color.white;
            btClassic.GetComponent<TextMesh>().color = Color.white;
            m_Position = 0;
        }
    }

    private void btHighscore_Clicked()
    {
//         Debug.Log("Slide= " + TestController.Instance.IsSliding);
//         Debug.Log("Positioning= " + TestController.Instance.IsRepositioning);
        if (TestController.Instance.IsSliding
            || TestController.Instance.IsRepositioning)
            return;
        if (m_Position == 0)
        {
            // Debug.Log("Slide right to left");
            TestController.Instance.SlideLeft(1);
            btNew.GetComponent<TextMesh>().color = Color.white;
            btHighscore.GetComponent<TextMesh>().color = Color.cyan;
            btFriends.GetComponent<TextMesh>().color = Color.white;
            btClassic.GetComponent<TextMesh>().color = Color.white;
            m_Position = 1;
        }
        else if (m_Position == 2 || m_Position == 3)
        {
            // Debug.Log("Slide left to right");
            TestController.Instance.SlideRight(1);
            btNew.GetComponent<TextMesh>().color = Color.white;
            btHighscore.GetComponent<TextMesh>().color = Color.cyan;
            btFriends.GetComponent<TextMesh>().color = Color.white;
            btClassic.GetComponent<TextMesh>().color = Color.white;
            m_Position = 1;
        }
    }

    private void btFriends_Clicked()
    {
        if (TestController.Instance.IsSliding
            || TestController.Instance.IsRepositioning)
            return;
        if (m_Position == 0 || m_Position == 1)
        {
            // Debug.Log("Slide right to left");
            TestController.Instance.SlideLeft(2);
            btNew.GetComponent<TextMesh>().color = Color.white;
            btHighscore.GetComponent<TextMesh>().color = Color.white;
            btFriends.GetComponent<TextMesh>().color = Color.cyan;
            btClassic.GetComponent<TextMesh>().color = Color.white;
            m_Position = 2;
        }
        else if (m_Position == 3)
        {
            // Debug.Log("Slide left to right");
            TestController.Instance.SlideRight(2);
            btNew.GetComponent<TextMesh>().color = Color.white;
            btHighscore.GetComponent<TextMesh>().color = Color.white;
            btFriends.GetComponent<TextMesh>().color = Color.cyan;
            btClassic.GetComponent<TextMesh>().color = Color.white;
            m_Position = 2;
        }
    }

    private void btClassic_Clicked()
    {
        if (TestController.Instance.IsSliding
            || TestController.Instance.IsRepositioning)
            return;

        if (m_Position != 3)
        {
            TestController.Instance.SlideLeft(3);
            btNew.GetComponent<TextMesh>().color = Color.white;
            btHighscore.GetComponent<TextMesh>().color = Color.white;
            btFriends.GetComponent<TextMesh>().color = Color.white;
            btClassic.GetComponent<TextMesh>().color = Color.cyan;
            m_Position = 3;
        }
    }

    void OnMouseBeginDrag()
    {
        m_OrgX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
    }

    void OnMouseDrag()
    {
    }

    // TODO (void): prevent click when done swiping
    void OnMouseEndDrag()
    {
        float newX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        if (newX > m_OrgX)
        {
            NavButtons.SendMessage("SwipeRight");
        }
        else if (newX < m_OrgX)
        {
            NavButtons.SendMessage("SwipeLeft");
        }
        else Debug.Log("No swipe");
    }

	// Use this for initialization
	void Start () {
        m_Position = 0;
        m_BgHeight = (Camera.main.orthographicSize * 2.0f - 1.3f) / 10.0f;
        btNew.GetComponent<TextMesh>().color = Color.cyan;
        btNew.GetComponent<NavButton>().EvtClicked += btNew_Clicked;
        btHighscore.GetComponent<NavButton>().EvtClicked += btHighscore_Clicked;
        btFriends.GetComponent<NavButton>().EvtClicked += btFriends_Clicked;
        btClassic.GetComponent<NavButton>().EvtClicked += btClassic_Clicked;

        m_IsBegan = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos.y >= 5.0f - m_BgHeight && mousePos.y <= 5.0f)
            {
                m_IsBegan = true;
                gameObject.SendMessage("OnMouseBeginDrag");
                m_LastMousePos = Input.mousePosition;
            }
        }

        if (Input.GetMouseButton(0) && m_IsBegan)
        {
            if (Input.mousePosition != m_LastMousePos)
            {
                m_LastMousePos = Input.mousePosition;
                gameObject.SendMessage("OnMouseDrag");
            }
        }

        if (Input.GetMouseButtonUp(0) && m_IsBegan)
        {
            m_IsBegan = false;
            gameObject.SendMessage("OnMouseEndDrag");
        }
	}
}
