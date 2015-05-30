using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class servertest : MonoBehaviour {

    IEnumerator FirstRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("First");
    }

    IEnumerator SecondRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Second");
    }

    IEnumerator AllInOne()
    {
        yield return StartCoroutine(FirstRoutine());
        yield return StartCoroutine(SecondRoutine());
        Debug.Log("Done.");
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(AllInOne());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
