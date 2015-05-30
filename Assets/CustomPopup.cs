using UnityEngine;
using System.Collections;

public class CustomPopup : MonoBehaviour {

    public float Width;
    public float Height;

    private Mesh m_Mesh;
    private string m_ImgLink;

    void OnDestroy()
    {
        Object.Destroy(m_Mesh);
    }

    IEnumerator DownloadImage()
    {
        WWW www = new WWW(m_ImgLink);
        Debug.Log("popup link: " + m_ImgLink);
        yield return www;
        renderer.material.SetTexture("_MainTex", www.texture);
        yield return null;
    }

    public void SetImageLink(string str)
    {
        m_ImgLink = str;
    }

    // Use this for initialization
    void Start()
    {
        Height = Camera.main.orthographicSize * 2.0f;
        Width = Height / 1.33333f;
        transform.position = new Vector3(0.0f, 0.0f, -9.6f);

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
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
