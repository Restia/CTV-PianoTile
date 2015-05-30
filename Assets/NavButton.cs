using UnityEngine;
using System.Collections;

public class NavButton : MonoBehaviour {

    public delegate void EventClicked();
    public EventClicked EvtClicked;

    void OnMouseUpAsButton()
    {
        if (EvtClicked != null)
            EvtClicked();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
