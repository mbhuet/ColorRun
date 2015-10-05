using UnityEngine;
using System.Collections;

public class LeaderZone : MonoBehaviour {
	public Player myPlayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		Player p = col.gameObject.GetComponent<Player>();
		if (p != null){
			if (myPlayer.isLeader && myPlayer.newLeaderTimer <= 0){
				p.makeLeader();
				myPlayer.makeChaser();
				Debug.Log("New Leader is " + p);
			}

		}
	}
}
