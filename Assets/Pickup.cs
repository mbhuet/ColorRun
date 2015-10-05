using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		Player p = col.GetComponent<Player>();
		if (p != null){
			if (p.isLeader){
				p.makeLeader();
			p.points ++;
			GameObject.Destroy(this.gameObject);
			}
		}
	}
}
