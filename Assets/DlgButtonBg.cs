using UnityEngine;
using System.Collections;

public class DlgButtonBg : MonoBehaviour {

    public GameObject BtnBg;
    private Color m_NormColor;
    private Color m_DownColor;

    public delegate void EventClicked();
    public EventClicked EvtClicked;

    void OnMouseDown()
    {
        BtnBg.renderer.material.SetColor("_Color", m_DownColor);
    }

    void OnMouseUp()
    {
        BtnBg.renderer.material.SetColor("_Color", m_NormColor);
    }

    void OnMouseUpAsButton()
    {
        if (EvtClicked != null)
            EvtClicked();
    }

	// Use this for initialization
	void Start () {
        m_NormColor = BtnBg.GetComponent<DlgBg>().FillColor;
        m_DownColor = BtnBg.GetComponent<DlgBg>().DownColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
