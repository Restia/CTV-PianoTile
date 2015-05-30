using UnityEngine;
using System.Collections;

public class NavButtons : MonoBehaviour {

    private bool m_IsLocked = false;

    IEnumerator ShiftLeft()
    {
        m_IsLocked = true;
        float startTime = Time.time;
        Vector3 pos = transform.position;
        Vector3 dest = pos;
        Vector3 tmp = pos;
        dest.x = -1.7f;
        while (Mathf.Abs(tmp.x + 1.7f) > 0.01f)
        {
            tmp = Vector3.Lerp(pos, dest, (Time.time - startTime)/0.2f);
            transform.position = tmp;
            yield return null;
        }

        Debug.Log("Done next board");
        m_IsLocked = false;
    }

    IEnumerator ShiftRight()
    {
        m_IsLocked = true;
        float startTime = Time.time;
        Vector3 pos = transform.position;
        Vector3 dest = pos;
        Vector3 tmp = pos;
        dest.x = 0.0f;
        while (Mathf.Abs(tmp.x) > 0.01f)
        {
            tmp = Vector3.Lerp(pos, dest, (Time.time - startTime) / 0.2f);
            transform.position = tmp;
            yield return null;
        }

        Debug.Log("Done prev board");
        m_IsLocked = false;
    }

    void SwipeRight()
    {
        if (!m_IsLocked)
            StartCoroutine("ShiftRight");
    }

    void SwipeLeft()
    {
        if (!m_IsLocked)
            StartCoroutine("ShiftLeft");
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.K) && !m_IsLocked)
        {
            Debug.Log("Begin");
            StartCoroutine("ShiftRight");
        }
        if (Input.GetKeyDown(KeyCode.H) && !m_IsLocked)
        {
            Debug.Log("Begin");
            StartCoroutine("ShiftLeft");
        }

	}
}
