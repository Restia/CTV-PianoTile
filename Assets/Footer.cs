using UnityEngine;
using System.Collections;

public class Footer : MonoBehaviour {

    public GameObject RankTxt;
    public GameObject Avatar;
    private string m_AvatarLink;

    public void SetRank(int rank)
    {
        float Height = (Camera.main.orthographicSize * 2.0f + 1.3f) / 10.0f;
        transform.position = new Vector3(0.0f, -(Camera.main.orthographicSize - Height / 2.0f), -3.0f);
        if (rank < 0)
        {
            RankTxt.GetComponent<TextMesh>().text = "No highscore";
        }
        else
            RankTxt.GetComponent<TextMesh>().text = "Your rank: #" + (rank + 1).ToString();
    }

    IEnumerator DownloadAvatar()
    {
        WWW www = new WWW(m_AvatarLink);
        Debug.Log(m_AvatarLink);
        yield return www;
        Avatar.GetComponent<Avatar>().SetTexture(www.texture);
        yield return null;
    }

    public void SetAvatarLink(string str)
    {
        m_AvatarLink = str;
        StartCoroutine(DownloadAvatar());
    }

	// Use this for initialization
	void Start () {
        float height = (Camera.main.orthographicSize * 2.0f + 1.3f) / 10.0f;
        float width = (Camera.main.orthographicSize * Camera.main.aspect);
        Avatar.transform.localPosition = new Vector3(-width + height / 2.0f, 0.0f, -1.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
