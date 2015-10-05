//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using System.Collections;

[System.Serializable]
public class colorInfluenceClass
{
    public bool addColor = true;
    public bool useColorMaskTexture = false;
    public Texture2D colorMaskTexture;
    public Color colorValue = Color.white;
    public float colorSize = 0.5f;
    public float colorFalloff = 0.5f;
	
    public Transform myTransform;
    public Vector3 lastFramePosition;
    public float lastFrameTime = 0.0f;
    public int actorPriority = 3;
    public bool multiplySizeByScale = false;
}

[AddComponentMenu("TexturePaint/TexturePaint Influence Actor")]
public class TexturePaintInfluenceActor : MonoBehaviour
{
public bool useGPUfeatures = true;

public bool showGizmos = true;
public colorInfluenceClass colorDetails;

private GameObject colorConnectorObject;
private TexturePaintConnector colorConnectorScript;

private int colorActorId;

private bool tempAddColor;

private bool hasBeenDisabled = false;

public bool colorFoldout = true;

//=============

void OnDrawGizmos()
{
	if(showGizmos)
	{
		colorDetails.myTransform = transform;
		
	    if(colorDetails.addColor)
	    {
	    	Gizmos.color = Color.yellow;
	    	
	    	if(colorDetails.multiplySizeByScale)
	    	{
	    		Gizmos.DrawWireSphere(colorDetails.myTransform.position, colorDetails.colorSize * (transform.localScale.magnitude * 0.577f));
		        Gizmos.DrawLine(colorDetails.myTransform.position + (colorDetails.myTransform.up * 0.1f), colorDetails.myTransform.position - ((colorDetails.myTransform.up * 0.6f) * (colorDetails.colorSize * (transform.localScale.magnitude))));
			}
			else
			{
				Gizmos.DrawWireSphere(transform.position, colorDetails.colorSize);
				Gizmos.DrawLine(colorDetails.myTransform.position + (colorDetails.myTransform.up * 0.1f), colorDetails.myTransform.position - ((colorDetails.myTransform.up) * colorDetails.colorSize));
			}
		}
	}
}

//=============

void Start()
{
	colorDetails.myTransform = transform;
	colorDetails.lastFramePosition = transform.position;

	if(colorConnectorScript == null)
		{
		if(GameObject.Find("dynamiclyCreatedTexturePaintConnector"))
		{
			colorConnectorObject = GameObject.Find("dynamiclyCreatedTexturePaintConnector");

            colorConnectorScript = colorConnectorObject.GetComponent<TexturePaintConnector>();
		}
		else
		{
			colorConnectorObject = new GameObject();
			
			colorConnectorObject.name = "dynamiclyCreatedTexturePaintConnector";

            colorConnectorScript = colorConnectorObject.AddComponent<TexturePaintConnector>();
		}
	}
	
	if(colorConnectorObject == null)
	{
		Debug.LogError("TexturePaintInfluenceActor failed to find or create a TexturePaintConnector object.  Make sure the TexturePaintConnector script exists and can by found by the TexturePaintInfluenceActor.");
	}
	
	if(colorConnectorScript == null)
	{
		Debug.LogError("TexturePaintInfluenceActor failed to find or create a TexturePaintConnector script.  Make sure the TexturePaintConnector script exists and can by found by the TexturePaintInfluenceActor.");
	}
	else
	{
		colorActorId = colorConnectorScript.AddInfluenceActor(colorDetails);
	}
}

//==============

void OnEnable()
{
	if(hasBeenDisabled)
	{
		colorDetails.addColor = tempAddColor;
	}
}

//==============

void OnDisable()
{
	tempAddColor = colorDetails.addColor;

	colorDetails.addColor = false;
	
	hasBeenDisabled = true;
}

//============== HELPER FUNCTIONS BELOW THIS LINE =================

public void ChangeActorPriority(int tempInt)
{
    tempInt = Mathf.Clamp(tempInt, 1, 5);

    colorDetails.actorPriority = tempInt;

    colorConnectorScript.SortActorArray();
}

//==============

public void DestroyColorInfluenceActor()
{
    colorConnectorScript.RemoveInfluenceActor(colorActorId);

    Destroy(gameObject);
}
}