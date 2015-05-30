using UnityEngine;
using System.Collections;

public class MainMenuItem : MonoBehaviour {

    public int X;
    public int Y;

    public GameObject Bg;
    public delegate void Clicked();
    public Clicked EvtClicked;

    private Color m_OldBgColor;

    void OnMouseDown()
    {
        m_OldBgColor = Bg.renderer.material.GetColor("_Color");
        Color newColor = m_OldBgColor;
        newColor.a *= 1.5f;
        Bg.renderer.material.SetColor("_Color", newColor);
    }

    void OnMouseUpAsButton()
    {
        if (EvtClicked != null)
            EvtClicked();
    }

    void OnMouseUp()
    {
        Bg.renderer.material.SetColor("_Color", m_OldBgColor);
    }

    void Awake()
    {
        float Width = (Camera.main.orthographicSize * 2.0f * Camera.main.aspect - 0.5f) / 2.0f;
        float Height = (Camera.main.orthographicSize * 2.0f - 0.7f) / 4.0f;

        Vector3 position = new Vector3();
        position.x = (Width / 2.0f + 0.05f) * (X);
        position.y = (Height / 2.0f + 0.05f) * (Y);
        transform.position = position;
        BoxCollider2D box = collider2D as BoxCollider2D;
        box.size = new Vector2(Width, Height);
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
