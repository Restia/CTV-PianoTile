﻿using UnityEngine;
using System.Collections;

public class MenuItemBg : MonoBehaviour {

    public float Width;
    public float Height;

    public Color FillColor;

    private Mesh m_Mesh;

    void OnDestroy()
    {
        Object.Destroy(m_Mesh);
    }

	// Use this for initialization
    void Awake()
    {
        Width = (Camera.main.orthographicSize * 2.0f * Camera.main.aspect - 0.5f) / 2.0f;
        Height = (Camera.main.orthographicSize * 2.0f - 0.7f) / 4.0f;

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
        GetComponent<MeshRenderer>().material.SetColor("_Color", FillColor);
	}

    void Start()
    {
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
