using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
		TrailRenderer trail;
		TexturePaintInfluenceActor painter;

		public float speedFactor = 0;
		private float maxSpeed = 10;
		private float minSpeed = 10;

	public static float acceleration = 5;
	public static float maxSpeedChaser = 14;
	public static float maxSpeedLeader = 12;
	public static float maxSpeedLeaderBoost = 20;

		public bool isLeader;
		public int playerNum;
		public static float pathWidth = 2;
		public GameObject PathZone;
		public float insideTimer;
		//public float boostTimer;
		LeaderZone leadZone;
		public float newLeaderTimer = 0;
		public ParticleSystem leaderBurst;
		public Color color;
		public float leaderTime = 0;
		public Material white;
		public Material black;
	public int points = 0;

	public AudioClip chime;

	PlayerIndex gamepadNum;

		
		// Use this for initialization
		void Start ()
		{
				leadZone = transform.GetComponentInChildren<LeaderZone> ();
				trail = this.GetComponent<TrailRenderer> ();
				painter = this.GetComponent<TexturePaintInfluenceActor> ();

				GetComponent<Renderer>().material.color = color;
				//painter.colorDetails.colorValue = color;
				leaderBurst.startColor = color;
			
				if (playerNum == 1)
						makeLeader ();
				else
						makeChaser ();

			switch (playerNum){
		case 1: gamepadNum = PlayerIndex.One;
			break;
		case 2: gamepadNum = PlayerIndex.Two;
			break;
		case 3: gamepadNum = PlayerIndex.Three;
			break;
		case 4 : gamepadNum = PlayerIndex.Four;
			break;
			}
		}
	
		// Update is called once per frame
		void Update ()
		{
				
				//create path
				if (isLeader) {
						leaderTime += Time.deltaTime;
						makePath();

						
						if (newLeaderTimer > 0) {
								newLeaderTimer -= Time.deltaTime;
						} else
								maxSpeed = maxSpeedLeader;
				}


				Collider2D[] cols = Physics2D.OverlapCircleAll (new Vector2 (transform.position.x, transform.position.y), this.transform.localScale.x / 2);
				bool outside = true;
				foreach (Collider2D col in cols) {
						if (col.gameObject.GetComponent<PathZone> ()) {
								outside = false;
						}
				}

				if (outside) {
						leadZone.GetComponent<Renderer>().material = black;
						speedFactor -= acceleration * Time.deltaTime * 3;
						if (GameHandler.rumbleOutside_static)
							GamePad.SetVibration(gamepadNum, .8f, .8f);
				} else {
						leadZone.GetComponent<Renderer>().material = white;
						speedFactor += acceleration * Time.deltaTime;
						if (GameHandler.rumbleOutside_static)
							GamePad.SetVibration(gamepadNum, 0, 0);
				}
				Vector3 move = getInput ();


				speedFactor = Mathf.Clamp (speedFactor, minSpeed, maxSpeed);
				transform.Translate (move * speedFactor * Time.deltaTime);

				//check to see if the player is touching a path
				if (Input.GetButtonDown("Fire"+playerNum)){
			if (isLeader){
				Boost();
			}
			else{
				GroupBoost();
			}
		}
				/* Old Movement

		else
			speedFactor += acceleration;

		speedFactor = Mathf.Clamp(speedFactor, .01f, .3f);

				move = getInput ();
				this.transform.Translate (move);
				*/
		}

		Vector3 getInput ()
		{
				float moveX = 0;
				float moveY = 0;

		GamePadState controller = GamePad.GetState(gamepadNum);
		moveX = controller.ThumbSticks.Left.X;
		moveY = controller.ThumbSticks.Left.Y;
			
		if (playerNum == 1){
			moveX += Input.GetAxis ("Horizontal");
			moveY += Input.GetAxis ("Vertical");
		}
		/*
				switch (playerNum) {
				case 1:
						moveX = Input.GetAxis ("Horizontal");
						moveY = Input.GetAxis ("Vertical");
						break;
				case 2:
						moveX = Input.GetAxis ("Horizontal2");
						moveY = Input.GetAxis ("Vertical2");
						break;
				case 3:
						moveX = Input.GetAxis ("Horizontal3");
						moveY = Input.GetAxis ("Vertical3");
						break;
				case 4:
						moveX = Input.GetAxis ("Horizontal4");
						moveY = Input.GetAxis ("Vertical4");
						break;
				default:
						break;
				}
		*/
				Vector3 moveInput = new Vector3 (moveX, moveY);
				if (moveInput.magnitude>1)
					moveInput.Normalize();
				return moveInput;
		}

		public void makeLeader ()
		{
		AudioSource.PlayClipAtPoint(chime, Camera.main.transform.position);
				maxSpeed = maxSpeedLeaderBoost;
				speedFactor = maxSpeed;

				leaderBurst.Emit (30);
				//painter.enabled = true;
				isLeader = true;
				newLeaderTimer = 1;
		}

		public void makeChaser ()
		{
				maxSpeed = maxSpeedChaser;
				//painter.enabled = false;
				isLeader = false;
		}

	public void makePath()
	{
		GameObject newPath = GameObject.Instantiate (PathZone) as GameObject;
		newPath.name = "PathZone";
		newPath.transform.position = this.transform.position;
		float size = getInput().magnitude * speedFactor/maxSpeed * 2;
		if (GameHandler.varialbePathWidth_static){
			newPath.transform.localScale = new Vector3 (size, size, newPath.transform.localScale.z);
		}
		else{
			newPath.transform.localScale = new Vector3 (this.transform.localScale.x, this.transform.localScale.y, newPath.transform.localScale.z);
		}
		if (GameHandler.coloredPaths_static){			
			newPath.GetComponent<Renderer>().material.color = color;
		}
	}

	void OnApplicationQuit(){
		GamePad.SetVibration(gamepadNum, 0, 0);
	}

	void Boost(){
	}

	void GroupBoost(){
	}


}
