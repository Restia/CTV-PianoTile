using UnityEngine;
using System.Collections;

public class ListItem : MonoBehaviour {

    private int m_No;
    private string m_AvatarLink;
    private string m_Name;
    private float m_Score;

    public GameObject ListItemBg;
    public GameObject No;
    public GameObject NoBg;
    public GameObject Name;
    public GameObject AvatarPic;
    public GameObject Score;

    public Color TopBgColor;
    public Color RunningUpColor;

    public void SetInfo(int no, string avatarLink, string name, float score)
    {
        m_No = no;
        m_AvatarLink = avatarLink;
        m_Name = name;
        m_Score = score;
        if (no <= 3)
            NoBg.renderer.material.SetColor("_Color", TopBgColor);
        else NoBg.renderer.material.SetColor("_Color", RunningUpColor);
    }

    IEnumerator DownloadAvatar()
    {
        WWW www = new WWW(m_AvatarLink);
        Debug.Log(m_AvatarLink);
        yield return www;
        AvatarPic.GetComponent<Avatar>().SetTexture(www.texture);
        yield return null;
    }

    void Awake()
    {
        float bgHeight = (Camera.main.orthographicSize * 2.0f - 1.3f) / 10.0f;
        float bgWidth = (Camera.main.orthographicSize * 2.0f * Camera.main.aspect - 0.4f);

        ListItemBg.GetComponent<ListItemBg>().Width = bgWidth;
        ListItemBg.GetComponent<ListItemBg>().Height = bgHeight;
        
        No.transform.localPosition = new Vector3(- bgWidth / 2.0f + 0.3f, 0.025f, -1.0f);
        NoBg.transform.localPosition = new Vector3(-bgWidth / 2.0f + 0.3f, 0.0f, -0.9f);
        Name.transform.localPosition = new Vector3(-bgWidth / 2.0f + 1.75f, 0.0f, -1.0f);
        Score.transform.localPosition = new Vector3(bgWidth / 2.0f - 0.2f, 0.0f, -1.0f);
        AvatarPic.transform.localPosition = new Vector3(-bgWidth / 2.0f + 1.15f * bgHeight, 0.0f, -1.0f);
    }

	// Use this for initialization
	void Start () {
        No.GetComponent<TextMesh>().text = m_No.ToString();
        Name.GetComponent<TextMesh>().text = m_Name;
        Score.GetComponent<TextMesh>().text = m_Score.ToString();

        StartCoroutine(DownloadAvatar());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
