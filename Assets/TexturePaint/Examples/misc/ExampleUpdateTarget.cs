//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using System.Collections;

public class ExampleUpdateTarget : MonoBehaviour
{
private Transform myTransform;

private RaycastHit hit;
private Ray ray;

public TexturePaintInfluenceActor paintScriptObject;

void Start()
{
	myTransform = transform;
	
	if(paintScriptObject == null)
	{
        paintScriptObject = GetComponent<TexturePaintInfluenceActor>();
	}
}

void Update()
{
//raycast and get the surface location and rotation of the texture input.
	ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        
    if(Physics.Raycast(ray, out hit))
    {
        myTransform.position = hit.point;

        myTransform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
    }

//on mouse click, turn on the paint actor, update the color texture and turn off the paint actor after update.
	if(Input.GetMouseButton(0))
	{   
		if(hit.transform)
		{
			paintScriptObject.colorDetails.addColor = true;
			
        	hit.transform.gameObject.SendMessage("ForceColorUpdate");
        	
        	paintScriptObject.colorDetails.addColor = false;
    	}
    }
}
}