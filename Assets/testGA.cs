using UnityEngine;
using System.Collections;

public class testGA : MonoBehaviour {

    public GoogleAnalyticsV3 analytic;

	// Use this for itialization
	void Start () {
        analytic.StartSession();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("test");
            analytic.LogScreen("test");
        }
	}
}
