using UnityEngine;
using System.Collections;

public class PopupCloseBtn : MonoBehaviour {

    public GameObject PopupWindow;

    public float Width;
    public float Height;

    private Mesh m_Mesh;

    void OnDestroy()
    {
        Object.Destroy(m_Mesh);
    }

    void OnMouseDown()
    {

    }

    void OnMouseUp()
    {

    }

    void OnMouseUpAsButton()
    {
        Object.Destroy(PopupWindow);
    }

	// Use this for initialization
    void Start()
    {
        Width = Camera.main.orthographicSize * Camera.main.aspect / 6.0f;
        Height = Width;
        transform.localPosition = new Vector3(Camera.main.orthographicSize * Camera.main.aspect - Width / 2.0f,
            Camera.main.orthographicSize - Height / 2.0f, -0.001f);

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
        BoxCollider2D col = collider2D as BoxCollider2D;
        col.size = new Vector2(Width, Height);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
