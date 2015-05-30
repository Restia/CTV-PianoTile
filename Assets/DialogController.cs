using UnityEngine;
using System.Collections;

public class DialogController : MonoBehaviour {

    public GameObject YesButton;
    public GameObject NoButton;

    public GameObject GetYesButton()
    {
        return YesButton;
    }

    public GameObject GetNoButton()
    {
        return NoButton;
    }

    private void SelfDestruct()
    {
        Object.Destroy(gameObject);
    }

	// Use this for initialization
	void Start () {
        YesButton.GetComponent<DlgButtonBg>().EvtClicked += SelfDestruct;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
