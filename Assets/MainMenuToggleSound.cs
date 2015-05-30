using UnityEngine;
using System.Collections;

public class MainMenuToggleSound : MonoBehaviour {

    public int X;
    public int Y;

    public GameObject Bg;
    public GameObject Status;

    public delegate void Clicked();
    public Clicked EvtClicked;

    private Color m_OnColor;
    private Color m_OffColor;

    private bool m_IsSoundOn;

    void OnMouseDown()
    {
        if (m_IsSoundOn)
        {
            Bg.renderer.material.SetColor("_Color", m_OffColor);
        }
        else
        {
            Bg.renderer.material.SetColor("_Color", m_OnColor);
        }
    }

    void OnMouseUpAsButton()
    {
        m_IsSoundOn = !m_IsSoundOn;
        if (m_IsSoundOn)
        {
            PlayerPrefs.SetInt("Settings_Sound", 1);
            Status.GetComponent<TextMesh>().text = "ON";
            Bg.renderer.material.SetColor("_Color", m_OnColor);
        }
        else
        {
            PlayerPrefs.SetInt("Settings_Sound", 0);
            Status.GetComponent<TextMesh>().text = "OFF";
            Bg.renderer.material.SetColor("_Color", m_OffColor);
        }
        if (EvtClicked != null)
            EvtClicked();
    }

    void OnMouseUp()
    {
        if (m_IsSoundOn)
        {
            Bg.renderer.material.SetColor("_Color", m_OnColor);
        }
        else
        {
            Bg.renderer.material.SetColor("_Color", m_OffColor);
        }
    }

    public bool GetValue()
    {
        return m_IsSoundOn;
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

        m_OffColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        m_OnColor = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        // Debug.Log(PlayerPrefs.GetInt("Settings_Sound"));

        if (PlayerPrefs.GetInt("Settings_Sound") == 1)
        {
            m_IsSoundOn = true;
            Bg.renderer.material.SetColor("_Color", m_OnColor);
            Status.GetComponent<TextMesh>().text = "ON";
        }
        else
        {
            m_IsSoundOn = false;
            Bg.renderer.material.SetColor("_Color", m_OffColor);
            Status.GetComponent<TextMesh>().text = "OFF";
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
