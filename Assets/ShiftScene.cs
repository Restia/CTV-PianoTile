using UnityEngine;
using System.Collections;

public class ShiftScene : MonoBehaviour {

    public float TimeShift;
    private Vector3 m_OutDestination;
    private Vector3 m_InBegin;

    public bool ShiftInWhenReady = false;

    public delegate void DoneShiftCallback();
    public DoneShiftCallback ShiftInCallback;
    public DoneShiftCallback ShiftOutCallback;

    IEnumerator ShiftOutRoutine()
    {
        float startTime = Time.time;
        transform.position = new Vector3(0.0f, 0.0f, 20.0f);
        yield return null;

        while (transform.position != m_OutDestination)
        {
            transform.position = Vector3.Lerp(transform.position, m_OutDestination, Time.time - startTime);
            yield return null;
        }

        // Debug.Log("Destroy now!!!");
        if (ShiftOutCallback != null)
            ShiftOutCallback();
        Object.Destroy(gameObject);
    }

    IEnumerator ShiftInRoutine()
    {
        float startTime = Time.time;
        transform.position = m_InBegin;
        yield return null;
        while (transform.position != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, Vector3.zero, Time.time - startTime);
            yield return null;
        }
        
        GameObject[] allObj = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        // Debug.Log("Free all " + allObj.Length + " objects");
        foreach (GameObject obj in allObj)
        {
            if (obj.transform.parent == transform && obj.tag == "Untagged")
                obj.transform.parent = null;
        }
        if (ShiftInCallback != null)
            ShiftInCallback();
    }

    public void DoShiftOut(string newScene)
    {
        if (PlayerPrefs.GetInt("Settings_Sound") == 1)
            audio.Play();
        GameObject[] allObj = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        // Debug.Log("ShiftOut obj num: " + allObj.Length);
        foreach (GameObject obj in allObj)
        {
            if (obj.transform.parent == null && obj.tag == "Untagged")
                obj.transform.parent = transform;
        }

        StartCoroutine(ShiftOutRoutine());
    }

    public void DoShiftIn(string newScene)
    {
        GameObject[] allObj = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        // Debug.Log("ShiftIn obj num: " + allObj.Length);
        foreach (GameObject obj in allObj)
        {
            if (obj.transform.parent == null && obj.tag == "Untagged")
                obj.transform.parent = transform;
        }
        StartCoroutine(ShiftInRoutine());
    }

    void Awake()
    {
        m_OutDestination = new Vector3(0.0f,
            -Camera.main.orthographicSize * 2.0f, 20.0f);
        m_InBegin = new Vector3(0.0f,
            Camera.main.orthographicSize * 2.0f, 0.0f);
    }

	// Use this for initialization
	void Start () {
        if (ShiftInWhenReady)
            DoShiftIn("");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Application.LoadLevelAdditive("MenuScene");
            DoShiftOut("");
        }
        if (Input.GetKeyDown(KeyCode.G))
            DoShiftIn("");
	}
}
