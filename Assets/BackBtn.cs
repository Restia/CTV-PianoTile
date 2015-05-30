using UnityEngine;
using System.Collections;

public class BackBtn : MonoBehaviour {

    public GameObject BtnBg;
    private Color m_NormColor;
    private Color m_DownColor;

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
        TestController.Instance.BackToMainMenu();
    }

	// Use this for initialization
	void Start () {
        float Height = (Camera.main.orthographicSize * 2.0f + 1.3f) / 10.0f;
        transform.localPosition = new Vector3((Camera.main.orthographicSize * Camera.main.aspect - Height / 2.0f),
            0.0f,
            -1.0f);

        BoxCollider2D box = collider2D as BoxCollider2D;
        box.size = new Vector2(Height, Height);
        m_NormColor = BtnBg.GetComponent<BackBtnBg>().FillColor;
        m_DownColor = BtnBg.GetComponent<BackBtnBg>().DownColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
