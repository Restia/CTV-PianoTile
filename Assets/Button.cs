using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

    public GameObject Text;
    public GameObject ButtonBg;

    public delegate void Clicked();
    public Clicked EvtClicked;
    private Color m_OldBgColor;

    public void SetText(string text)
    {
        Text.GetComponent<TextMesh>().text = text;
    }

    void OnMouseDown()
    {
        m_OldBgColor = ButtonBg.renderer.material.GetColor("_Color");
        Color newColor = m_OldBgColor;
        newColor.a *= 2.0f;
        ButtonBg.renderer.material.SetColor("_Color", newColor);
    }

    void OnMouseUpAsButton()
    {
        if (EvtClicked != null)
            EvtClicked();
    }

    void OnMouseUp()
    {
        ButtonBg.renderer.material.SetColor("_Color", m_OldBgColor);
    }

    void Awake()
    {
        float btHeight = (Camera.main.orthographicSize) / 2.0f;
        float btWidth = (Camera.main.orthographicSize * 2.0f * Camera.main.aspect - 0.6f) / 3.0f;

        ButtonBg.GetComponent<ButtonBg>().Width = btWidth;
        ButtonBg.GetComponent<ButtonBg>().Height = btHeight;
        BoxCollider2D box = collider2D as BoxCollider2D;
        box.size = new Vector2(btWidth, btHeight);
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
