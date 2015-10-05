using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameHandler : MonoBehaviour {
	Camera camera;
	public List<Player> players;
	public float cameraMargin;

	public int numPlayers;
	public float[] playerTimes;

	public bool goofy_overlap_hack;

	public static bool coloredPaths_static;
	public bool coloredPaths;

	public bool displayLeaderTime;
	public bool displayPoints;

	public static bool varialbePathWidth_static;
	public bool variablePathWidth;

	public static bool rumbleOutside_static;
	public bool rumbleOutside;

	public static bool pathTimeout_static;
	public bool pathTimeout;

	public static float pathDuration_static;
	public float pathDuration;

	public GameObject pickup;
	float spawnTimer = 10;

	public Texture2D RedX;
	public Texture2D GreenCheck;

	public Vector3 average_midpoint;


	public Transform movObj;


	// Use this for initialization
	void Start () {
		//Set up the camera
		camera = Camera.main;
		playerTimes = new float[numPlayers];

		//Remove extra players
		if (numPlayers > players.Count){
			numPlayers = players.Count;
		}
		else {
			for (int i = numPlayers; i< players.Count; i++){
				players[i].gameObject.SetActive(false);
			}
		}
		average_midpoint = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)){
			Application.LoadLevel(0);
		}

		coloredPaths_static = coloredPaths;
		varialbePathWidth_static = variablePathWidth;
		rumbleOutside_static = rumbleOutside;
		pathTimeout_static = pathTimeout;
		pathDuration_static = pathDuration;

		//manage the camera
		float maxX = players[0].transform.position.x;
		float maxY = players[0].transform.position.y;
		float minX = players[0].transform.position.x;
		float minY = players[0].transform.position.y;
		for (int i = 0; i<numPlayers; i++){
			maxX = Mathf.Max(maxX, players[i].transform.position.x);
			maxY = Mathf.Max(maxY, players[i].transform.position.y);

			minX = Mathf.Min(minX, players[i].transform.position.x);
			minY = Mathf.Min(minY, players[i].transform.position.y);
		}

		Vector3 midpoint = new Vector3((maxX + minX)/2, (maxY + minY)/2, camera.transform.position.z);
		average_midpoint = midpoint;
		camera.transform.position = Vector3.Lerp(camera.transform.position, midpoint, .5f);

		float newCamSize = 0;
		if ((maxY - minY+ cameraMargin*2) * camera.aspect > (maxX - minX + cameraMargin*2) ){
			newCamSize = (maxY - minY + cameraMargin*2)/2;
		}
		else{
			newCamSize = (maxX - minX + cameraMargin*2) / (camera.aspect * 2);
		}

		if (newCamSize < 5) newCamSize = 5;
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, newCamSize, .5f);

		if (goofy_overlap_hack){
			foreach (Player p in players){
				p.transform.Translate(Vector3.back * .001f);
				camera.transform.Translate(Vector3.back * .001f);
			}
			movObj.Translate(Vector3.back * .001f);
		}

		spawnTimer -= Time.deltaTime;
		if (spawnTimer <= 0){
			spawnTimer = 10;
			SpawnPickup();
		}

	}

	void OnGUI(){
		GUIStyle style = new GUIStyle(GUI.skin.button);

		Rect timeRect = new Rect(0,0, 100,80);
		if (displayLeaderTime){
			string timeString = "";
			for (int i = 0; i<numPlayers; i++){
				timeString += ("\n" + "Player " + (i+1)+ ": " + players[i].leaderTime.ToString("F2"));
			}
			if (GUI.Button(timeRect, timeString)){
				displayLeaderTime = false;
			}
		}
		else{
			if (GUI.Button(new Rect(0,0,100,20), "Leader Times")){
				displayLeaderTime = true;
			}
		}

		Rect pointRect = new Rect(100,0, 100,80);
		if (displayPoints){
			string pointString = "";
			for (int i = 0; i<numPlayers; i++){
				pointString += ("\n" + "Player " + (i+1)+ ": " + players[i].points);
			}

			if (GUI.Button(pointRect, pointString)){
				displayPoints = false;
			}
		}
		else{
			if (GUI.Button(new Rect(100,0,100,20), "Points")){
				displayPoints = true;
			}
		}

		style.normal.textColor = Color.black;

		if (coloredPaths) style.normal.background = GreenCheck;
		else style.normal.background = RedX;
		Rect colorRect = new Rect(Screen.width - 100,0, 100,60);

			if (GUI.Button(colorRect, "Toggle Colored\nPaths", style)){
				coloredPaths = !coloredPaths;
			}

		if (variablePathWidth) style.normal.background = GreenCheck;
		else style.normal.background = RedX;
		Rect widthRect = new Rect(Screen.width - 200,0, 100,60);
		if (GUI.Button(widthRect, "Toggle Variable\nPath Width", style)){
			variablePathWidth = !variablePathWidth;
		}

		if (pathTimeout) style.normal.background = GreenCheck;
		else style.normal.background = RedX;
		Rect pathDestroyRect = new Rect(Screen.width - 300,0, 100,60);
		if (GUI.Button(pathDestroyRect, "Toggle Path\nDestruction", style)){
			pathTimeout = !pathTimeout;
		}
		
		
		
	}

	void SpawnPickup(){
		GameObject newItem = GameObject.Instantiate(pickup, Vector3.zero, Quaternion.identity) as GameObject;
		newItem.transform.position = new Vector3(average_midpoint.x, average_midpoint.y, movObj.transform.position.z);
		newItem.transform.parent = movObj;
	}


}
