using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RainbowCircle : MonoBehaviour {
	public Color[] colors;
	private List<GameObject> circles;
	public float maxSize;
	public float speed = .01f;

	public GameObject circle;
	public Material circleMat;
	GameObject test;

	public float generationTime = 1;
	public float timer;
	int currentIndex = 0;



	// Use this for initialization
	void Start () {
		circles = new List<GameObject>();
		timer = generationTime;
		//(CircleCollider2D) myCollider = (CircleCollider2D)this.collider;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= generationTime){
			timer = 0;
			if (currentIndex >= colors.Length) currentIndex = 0;
			StartCoroutine(GrowCircle(colors[currentIndex]));
			currentIndex ++;


		}

		/*
		foreach (GameObject c in circles){
			if (c != null){
				c.transform.Translate(Vector3.forward * Time.deltaTime *.01f);
			c.transform.localScale += Vector3.one * speed;
			if (c.transform.localScale.x > maxSize){
					DestroyObject(c.gameObject);
				//c.transform.localScale = new Vector3(maxSize, maxSize, .001f);
			}
			}
		}
		*/
		/*
		for (int i = 0; i<circles.Count; i++){
			GameObject c = circles[i];
			if (c != null){
				c.transform.Translate(Vector3.forward * Time.deltaTime *.01f);
				c.transform.localScale += Vector3.one * speed;
				if (c.transform.localScale.x > maxSize){
					c.transform.localScale = new Vector3(maxSize, maxSize, .001f);
					if (i>0) DestroyObject(circles[i-1].gameObject);
				}
			}
		}
		*/


	
	}

	IEnumerator GrowCircle(Color color){
		//push all existing circles forward slightly, so the new one appears on top
		for (int i = 0; i<circles.Count; i++){
			GameObject c = circles[i];
			if (c != null){
				c.transform.Translate(Vector3.forward * Time.deltaTime *.01f);
			}
		}

		GameObject newCircle = GameObject.CreatePrimitive(PrimitiveType.Quad);
		newCircle.transform.position = this.transform.position;
		newCircle.transform.localScale = Vector3.one * .001f;
		newCircle.GetComponent<Renderer>().material = circleMat;
		newCircle.GetComponent<Renderer>().material.color = color;
		newCircle.transform.parent = this.transform;
		GameObject.Destroy (newCircle.GetComponent<Collider>());
		circles.Add(newCircle);

		while (newCircle.transform.localScale.x < maxSize) {
			yield return null;
			newCircle.transform.localScale += Vector3.one * speed;


		}

		newCircle.transform.localScale = new Vector3(maxSize, maxSize, .001f);

		yield return new WaitForSeconds (.5f);

		circles.Remove (newCircle);
		GameObject.Destroy (newCircle);
	}
}
