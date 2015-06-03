using UnityEngine;
using System.Collections;

public class CustomBanner : MonoBehaviour {

    public float Width;
    public float Height;

    private Mesh m_Mesh;
    private string m_ImgLink;
    private string m_StoreUrl;

    void OnDestroy()
    {
        StopAllCoroutines();
        Object.Destroy(m_Mesh);
    }

    IEnumerator DownloadImage()
    {
        WWW www = new WWW(m_ImgLink);
        yield return www;
        renderer.material.SetTexture("_MainTex", www.texture);
        yield return null;
    }

    public void SetImageLink(string str, string url)
    {
        m_ImgLink = str;
        m_StoreUrl = url;
    }

    void OnMouseUpAsButton()
    {
        Application.OpenURL(m_StoreUrl);
    }

	// Use this for initialization
	void Start () {
        Width = Camera.main.orthographicSize * Camera.main.aspect * 2.0f;
        Height = Width / 6.0f;
        transform.position = new Vector3(0.0f, -5.0f + Height / 2.0f, -9.5f);

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
        StartCoroutine(DownloadImage());

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.size = new Vector2(Width, Height);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
