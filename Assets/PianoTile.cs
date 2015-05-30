using UnityEngine;
using System.Collections;

public class PianoTile : MonoBehaviour {

    public float Width;
    public float Height;

    public Color BackgroundColor;
    public Color ForegroundColor;

    private Mesh m_Mesh;

    private float m_Sign;
    private bool m_Toggle;

    private void BeginIdle()
    {
        GetComponent<MeshRenderer>().material.SetColor("_BgColor", BackgroundColor);
        GetComponent<MeshRenderer>().material.SetColor("_Color", ForegroundColor);
        GetComponent<MeshRenderer>().material.SetFloat("_Fill", 0.0f);
        GetComponent<Animator>().SetBool("Pressed", false);
        GetComponent<Animator>().SetBool("Wrong", false);
    }

    public void SetBgColor(Color bgColor)
    {
        BackgroundColor = bgColor;
    }

    public void ResetTile()
    {
        BeginIdle();
    }

    public void OnDisable()
    {
        GetComponent<Animator>().Play("PianoTile_Idle", -1, 0.0f);
    }

	// Use this for initialization
	void Start () {
        Vector3[] newVertices = new Vector3[] { new Vector3(-Width/2, -Height/2, 0.0f),
                                      new Vector3(-Width/2, Height/2, 0.0f),
                                      new Vector3(Width/2, -Height/2, 0.0f),
                                      new Vector3(Width/2, Height/2, 0.0f)};
        Vector2[] newUV = new Vector2[] { new Vector2(0.0f, 0.0f),
                                new Vector2(0.0f, 1.0f),
                                new Vector2(1.0f, 0.0f),
                                new Vector2(1.0f, 1.0f)};
        int[] newTriangles = new int[] { 0, 1, 2, 2, 1, 3 };
        m_Mesh = new Mesh();
        m_Mesh.vertices = newVertices;
        m_Mesh.uv = newUV;
        m_Mesh.triangles = newTriangles;

        GetComponent<MeshFilter>().mesh = m_Mesh;
        GetComponent<MeshRenderer>().material.SetColor("_BgColor", BackgroundColor);
        GetComponent<MeshRenderer>().material.SetColor("_Color", ForegroundColor);
        m_Sign = 1.0f;
        m_Toggle = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Toggle)
        {
            Vector3 pos = transform.position;
            pos.y += 0.15f * m_Sign;
            if (pos.y > 5.0f)
            {
                pos.y = 5.0f;
                m_Sign = -1.0f;
            }
            if (pos.y < -5.0f)
            {
                pos.y = -5.0f;
                m_Sign = 1.0f;
            }

            transform.position = pos;
        }

        if (Input.GetMouseButtonDown(0))
            m_Toggle = !m_Toggle;
	}
}
