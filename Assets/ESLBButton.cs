using UnityEngine;
using System.Collections;

public class ESLBButton : MonoBehaviour {

    public Sprite Norm;
    public Sprite Down;

    public delegate void EventClicked();
    public EventClicked EvtClicked;

    void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().sprite = Down;
    }

    void OnMouseUp()
    {
        GetComponent<SpriteRenderer>().sprite = Norm;
    }

    void OnMouseUpAsButton()
    {
        GetComponent<SpriteRenderer>().sprite = Norm;
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
