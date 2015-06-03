using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class ClassicESController : MonoBehaviour {

    public GameObject btShare;
    public GameObject btAgain;
    public GameObject btExit;
    public GameObject PrefabShiftScene;
    private GameObject m_ShiftScene;
    public GameObject Score;
    public GameObject ScoreShadow;
    public GameObject BestScore;
    public GameObject BestScoreShadow;
    public GameObject EndImage;

    public GameObject BtnLeaderBoard;

    private bool m_NoHighScore = false;
    private bool m_DisableFunction = true;

    private void Exit_Clicked()
    {
        if (!m_DisableFunction)
        {
            m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
            Application.LoadLevelAdditive("MenuScene");
            m_DisableFunction = true;
        }
    }

    private void Again_Clicked()
    {
        if (!m_DisableFunction)
        {
            m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
            Application.LoadLevelAdditive("ClassicScene");
            m_DisableFunction = true;
        }
    }

    private void LoginCallback(FBResult result)
    {
        Debug.Log("done login.");
        FB.Feed(
            toId: "",
            link: "",
            linkName: "WhiteTiles",
            linkCaption: "Don't tab the white tiles",
            linkDescription: "I completed Classic Mode in " + string.Format("{0:0.0000}", PlayerPrefs.GetFloat("ClassicScore")) + " seconds.",
            picture: "",
            mediaSource: "",
            actionName: "",
            actionLink: "",
            reference: ""
        );
    }

    private void InitCallback()
    {
        Debug.Log("done init");
        if (!FB.IsLoggedIn)
        {
            Debug.Log("Call login");
            FB.Login("email,user_friends,public_profile", LoginCallback);
        }
        else
        {
            LoginCallback(null);
        }
    }

    private void Share_Clicked()
    {
        if (!FB.IsInitialized)
            FB.Init(InitCallback);
        else InitCallback();
    }

    private void LB_Clicked()
    {
        if (!m_DisableFunction)
        {
            m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
            Application.LoadLevelAdditive("LeaderBoard1");
        }
    }

    private void CreateHighscoreFile(string path)
    {
        XmlTextWriter writer = new XmlTextWriter(path + "/highscore.xml", System.Text.Encoding.UTF8);
        writer.WriteStartDocument(true);
        writer.Formatting = Formatting.Indented;
        writer.Indentation = 2;
        writer.WriteStartElement("Highscore");
        writer.WriteStartElement("Classic");
        writer.WriteAttributeString("posted", "false");
        writer.WriteString("-1");
        writer.WriteEndElement();
        writer.WriteStartElement("Arcade");
        writer.WriteAttributeString("posted", "false");
        writer.WriteString("0");
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Close();
        Debug.Log("Done creating new highscore.xml");
    }

    void Awake()
    {
        // float btHeight = (Camera.main.orthographicSize) / 2.0f;
        float btWidth = (Camera.main.orthographicSize * 2.0f * Camera.main.aspect - 0.6f) / 3.0f;

        btShare.transform.position = new Vector2(-(btWidth + 0.1f), -3.0f);
        btAgain.transform.position = new Vector2(0.0f, -3.0f);
        btExit.transform.position = new Vector2(btWidth + 0.1f, -3.0f);
        BtnLeaderBoard.transform.position = new Vector3(Camera.main.orthographicSize * Camera.main.aspect - 0.5f,
            Camera.main.orthographicSize - 0.5f,
            -1.0f);

        btExit.GetComponent<Button>().EvtClicked += Exit_Clicked;
        btAgain.GetComponent<Button>().EvtClicked += Again_Clicked;
        btShare.GetComponent<Button>().EvtClicked += Share_Clicked;
        BtnLeaderBoard.GetComponent<ESLBButton>().EvtClicked += LB_Clicked;

        string path = Application.persistentDataPath;
        float highScore;
        float score = PlayerPrefs.GetFloat("ClassicScore");
        Debug.Log(path);
        Debug.Log("data path= " + Application.dataPath);
        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.Load(path + "/highscore.xml");
            float.TryParse(xmlDoc.ChildNodes[1].ChildNodes[0].InnerText, out highScore);
            Debug.Log("*** = " + xmlDoc.ChildNodes[1].ChildNodes[0].InnerText);
            if (highScore < 0)
            {
                highScore = float.PositiveInfinity;
                m_NoHighScore = true;
            }
            if (score < highScore && PlayerPrefs.GetInt("Win") == 1)
            {
                highScore = score;
                m_NoHighScore = false;
                xmlDoc.ChildNodes[1].ChildNodes[0].InnerText = string.Format("{0:0.0000}", score);
                xmlDoc.ChildNodes[1].ChildNodes[0].Attributes["posted"].Value = "false";
                xmlDoc.Save(path + "/highscore.xml");
            }
        }
        catch
        {
            Debug.Log("Not found " + path);
            CreateHighscoreFile(path);
            xmlDoc.Load(path + "/highscore.xml");
            // float.TryParse(xmlDoc.ChildNodes[1].ChildNodes[0].InnerText, out highScore);
            highScore = float.PositiveInfinity;
            m_NoHighScore = true;
            if (score < highScore && PlayerPrefs.GetInt("Win") == 1)
            {
                m_NoHighScore = false;
                xmlDoc.ChildNodes[1].ChildNodes[0].InnerText = string.Format("{0:0.0000}", score);
                xmlDoc.ChildNodes[1].ChildNodes[0].Attributes["posted"].Value = "false";
                xmlDoc.Save(path + "/highscore.xml");
            }
        }
        string str = string.Format("{0:0.0000}\"", score);
        Score.GetComponent<TextMesh>().text = str;
        ScoreShadow.GetComponent<TextMesh>().text = str;
        if (m_NoHighScore == false)
        {
            string best_str = "Best: " + string.Format("{0:0.0000}\"", highScore);
            BestScore.GetComponent<TextMesh>().text = best_str;
            BestScoreShadow.GetComponent<TextMesh>().text = best_str;
        }
        else
        {
            BestScore.renderer.enabled = false;
            BestScoreShadow.renderer.enabled = false;
        }
        Sprite spr = Resources.Load<Sprite>("Sprites/trollface" + Random.Range(1, 6).ToString());
        EndImage.GetComponent<SpriteRenderer>().sprite = spr;
    }

    private void ShiftSceneCallback()
    {
        m_DisableFunction = false;
    }

    // Use this for initialization
    void Start()
    {
        m_ShiftScene = Instantiate(PrefabShiftScene) as GameObject;
        m_ShiftScene.GetComponent<ShiftScene>().ShiftInWhenReady = true;
        m_ShiftScene.GetComponent<ShiftScene>().ShiftInCallback += ShiftSceneCallback;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_DisableFunction)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_DisableFunction = true;
                StopAllCoroutines();
                m_ShiftScene.GetComponent<ShiftScene>().DoShiftOut("");
                Application.LoadLevelAdditive("MenuScene");
            }
        }
    }
}
