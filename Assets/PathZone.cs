using UnityEngine;
using System.Collections;

public class PathZone : MonoBehaviour {
	public float countdown;
	public bool overrideTimeout = false;

	// Use this for initialization
	void Start () {
		if (countdown == 0)
		countdown = GameHandler.pathDuration_static;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (countdown < 0 && GameHandler.pathTimeout_static && !overrideTimeout){
			GameObject.Destroy(this.gameObject);
		}

		countdown -= Time.deltaTime;



	}

	void OnTriggerStay2D(Collider2D col){

	}
}
