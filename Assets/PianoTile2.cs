using UnityEngine;
using System.Collections;

public class PianoTile2 : MonoBehaviour {

    public float Width;
    public float Height;

    public Color BackgroundColor;
    public Color ForegroundColor;
    public Color WrongColor;
    public GameObject WrongOverlay;

    private Mesh m_Mesh;
    private MeshRenderer m_MeshRenderer;
    private Animator m_Animator;

    void OnDestroy()
    {
        Object.Destroy(m_Mesh);
    }

    void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_Animator = GetComponent<Animator>();
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
        WrongOverlay.GetComponent<MeshFilter>().mesh = m_Mesh;
        WrongOverlay.GetComponent<MeshRenderer>().material.SetColor("_BgColor", WrongColor);

        GetComponent<MeshRenderer>().material.SetColor("_BgColor", BackgroundColor);
        GetComponent<MeshRenderer>().material.SetColor("_Color", ForegroundColor);
        Refresh();
	}

    public void Refresh()
    {
        // MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        // Animator animator = GetComponent<Animator>();
        m_MeshRenderer.material.SetColor("_BgColor", BackgroundColor);
        m_MeshRenderer.material.SetColor("_Color", ForegroundColor);
        m_MeshRenderer.material.SetFloat("_Fill", 0.0f);
        m_Animator.SetBool("Wrong", false);
        m_Animator.SetBool("Pressed", false);
        if (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName("PianoTile2_Idle"))
        {
            m_Animator.Play("PianoTile2_Idle", 0, 0.0f);
        }
    }
	
	// Update is called once per frame
	void Update () {
	}
}
