using UnityEngine;
using OscJack;
using System.Collections;

public class Move : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var update = new Vector3(Mathf.Sin(Time.time) * 2f, Mathf.Sin(Time.time * 0.1f) * 3f, Mathf.Cos(Time.time * .5f));
        gameObject.transform.localPosition = update;
        OscMaster.SendData("/source/position 1 " + update.x + " " + update.y + " " + update.z);
	}
}
