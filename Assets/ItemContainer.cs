using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemContainer : MonoBehaviour {

    private int m_ItemNum;
    public int ItemNum
    {
        get
        {
            return m_ItemNum;
        }
    }

    public GameObject PrefabListItem;
    public GameObject EmptyNotification;
    private LinkedList<GameObject> m_ItemList;

    private float m_UpperBound;
    private float m_LowerBound;
    private float m_Offset;
    private float m_GListLength;

    // Dragging
    private Vector3 m_LastMousePos;
    private float m_YOffset;
    private bool m_IsBegan;

    private float GetUpper()
    {
        return transform.position.y;
    }

    private float GetLower()
    {
        return transform.position.y - m_GListLength;
    }

    public void ResetPosition()
    {
        transform.position = new Vector2(0.0f, m_UpperBound);
    }

    void Awake()
    {
        m_ItemList = new LinkedList<GameObject>();
        m_ItemNum = 0;
        m_Offset = ((Camera.main.orthographicSize * 2.0f - 1.3f) / 10.0f) / 2.0f;
        m_UpperBound = 5.0f - 0.1f - m_Offset * 2.0f;
        m_LowerBound = -5.0f + 0.1f + m_Offset * 2.0f;
        m_GListLength = m_UpperBound - m_LowerBound;
        transform.position = new Vector2(0.0f, m_UpperBound);
        m_IsBegan = false;
    }

    IEnumerator RepositionUpper()
    {
        TestController.Instance.IsRepositioning = true;
        Debug.Log("Reposition Upper");
        float startTime = Time.time;
        while (Mathf.Abs(GetUpper() - m_UpperBound) > 0.001f)
        {
            Vector2 tmp = Vector2.Lerp(transform.position, 
                new Vector2(0.0f, m_UpperBound), 
                Time.time - startTime);
            transform.position = tmp;
            yield return null;
        }
        Debug.Log("Done Repositon Upper");
        TestController.Instance.IsRepositioning = false;
    }

    IEnumerator RepositionLower()
    {
        TestController.Instance.IsRepositioning = true;
        Debug.Log("Reposition Lower");
        float startTime = Time.time;
        Debug.Log("Low = " + m_LowerBound);
        while (Mathf.Abs(GetLower() - m_LowerBound) > 0.001f)
        {
            Vector2 tmp = Vector2.Lerp(transform.position, 
                new Vector2(0.0f, m_LowerBound + m_GListLength), 
                Time.time - startTime);
            transform.position = tmp;
            yield return null;
        }
        Debug.Log("Done Repositon Lower");
        TestController.Instance.IsRepositioning = false;
    }

    void OnMouseBeginDrag()
    {
        // Debug.Log("Begin Drag");
        TestController.Instance.IsRepositioning = false;
        StopAllCoroutines();
        rigidbody2D.velocity = Vector2.zero;
        m_YOffset = - Camera.main.ScreenToWorldPoint(Input.mousePosition).y + transform.position.y;
    }

    void OnMouseDrag()
    {
        // Debug.Log("Dragging");
        Vector2 position = transform.position;
        position.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y + m_YOffset;
        transform.position = position;
    }

    void OnMouseEndDrag()
    {
        // Debug.Log("End Drag");
        Vector2 force = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - 
            Camera.main.ScreenToWorldPoint(m_LastMousePos))*10.0f;
        force.x = 0.0f;
        // error: quan tinh lam transform.position.y < m_UpperBound
        if (GetUpper() < m_UpperBound)
            StartCoroutine(RepositionUpper());
        else if (GetLower() > m_LowerBound)
            StartCoroutine(RepositionLower());
        else
            rigidbody2D.AddForce(force, ForceMode2D.Impulse);
    }

    public void AddItem(string avatarLink, string name, float score)
    {
        EmptyNotification.renderer.enabled = false;
        int no = m_ItemList.Count;
        GameObject obj = Instantiate(PrefabListItem) as GameObject;
        obj.transform.parent = transform;
        obj.GetComponent<ListItem>().SetInfo(no + 1, avatarLink, name, score);
        obj.transform.localPosition = new Vector3(0.0f, - m_Offset - 0.1f - (m_Offset * 2.0f + 0.1f) * no, -1.0f);
        m_ItemList.AddLast(obj);
        if (no > 7)
            m_GListLength += (m_Offset * 2.0f + 0.1f);
    }

	// Use this for initialization
	void Start () {
    }

    void FixedUpdate()
    {
        if ((GetUpper() < m_UpperBound || GetLower() > m_LowerBound)
            && rigidbody2D.velocity != Vector2.zero)
        {
            rigidbody2D.velocity = Vector2.zero;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos.y >= m_LowerBound && mousePos.y <= m_UpperBound)
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
