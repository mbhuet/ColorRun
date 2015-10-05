//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using System.Collections;

[AddComponentMenu("TexturePaint/(Auto Spawned) TexturePaint Connector")]
public class TexturePaintConnector : MonoBehaviour
{
public int dynamicArrayCount = 0;

private int colorActorArraySize = 128;
private int colorSurfacesArraySize = 128;

public colorInfluenceClass[] colorActorDynamicArray;
private colorInfluenceClass[] colorActorHistoryArray;
private colorInfluenceClass[] colorActorDynamicTempArray;
private colorInfluenceClass[] colorActorEmptyArray;
		
private TexturePaintScript[] colorSimScripts;
private int colorSimCount = 0;
private int totalColorActorId;
private int i = 0;
private int actorIdSlot = 0;
private bool actorAssigned;
private float lastSortTime = -1.0f;

//==========

void Awake()
{
    colorActorDynamicArray = new colorInfluenceClass[colorActorArraySize];
    colorActorHistoryArray = new colorInfluenceClass[colorActorArraySize];
    colorActorDynamicTempArray = new colorInfluenceClass[colorActorArraySize];
    colorActorEmptyArray = new colorInfluenceClass[colorActorArraySize];

    colorSimScripts = new TexturePaintScript[colorSurfacesArraySize];

	totalColorActorId = 0;
	
//creates a cleared array and clears all arrays to make sure there is zero junk data floating around.
	for(i = 0; i < colorSurfacesArraySize; i++)
	{
		colorSimScripts[i] = null;
	}

	for(i = 0; i < colorActorArraySize; i++)
	{
		colorActorEmptyArray[i] = null;
	}
	
	System.Array.Copy(colorActorEmptyArray, colorActorHistoryArray, colorActorArraySize);
	System.Array.Copy(colorActorEmptyArray, colorActorDynamicArray, colorActorArraySize);
	System.Array.Copy(colorActorEmptyArray, colorActorDynamicTempArray, colorActorArraySize);
}

//==========

public int AddInfluenceActor(colorInfluenceClass colorDetails)
{
    actorAssigned = false;

    for (i = 0; i < colorActorArraySize; i++)
    {
        if (colorActorHistoryArray[i] == null)
        {
            colorActorHistoryArray[i] = colorDetails;
            actorAssigned = true;
            actorIdSlot = i;
            i = 128;
        }
    }

    if (actorAssigned == false)
    {
        Debug.LogError("TexturePaintConnector tried to assign an Influence Actor but the array is full.  Texture Paint defaults to a maximum of 128 actors cached in the array.  If you need to use more than 128 actors, change the TexturePaintConnector script variable colorActorArraySize to a larger value.");
        actorIdSlot = 127;
    }

    SortActorArray();

    return Mathf.Clamp(actorIdSlot, 0, colorActorArraySize);
}

//==========

public void RemoveInfluenceActor(int colorActorId)
{
	colorActorHistoryArray[colorActorId] = null;
	
	SortActorArray();
}

//==========

public void SortActorArray()
{
	if((Time.time - lastSortTime) > 0.245f)
	{
		Invoke("SortActorArrayHidden", 0.25f);
		
		lastSortTime = Time.time;
	}
}

//==========

void SortActorArrayHidden()
{
	System.Array.Copy(colorActorEmptyArray, colorActorDynamicArray, colorActorArraySize);
	System.Array.Copy(colorActorEmptyArray, colorActorDynamicTempArray, colorActorArraySize);

    //Sort actors to arrays
    dynamicArrayCount = 0;

	for(i = 0; i <  colorActorHistoryArray.Length; i++)
	{
		if(colorActorHistoryArray[i] != null)
		{
			colorActorDynamicTempArray[dynamicArrayCount] = colorActorHistoryArray[i];
            dynamicArrayCount++;
		}
	}

	if(colorActorDynamicTempArray.Length > 0)
	{
		dynamicArrayCount = 0;
		
		for(i = 0; i < colorActorDynamicTempArray.Length; i++)
		{
			if(colorActorDynamicTempArray[i] != null && colorActorDynamicTempArray[i].actorPriority == 5)
			{
				colorActorDynamicArray[dynamicArrayCount] = colorActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				colorActorDynamicTempArray[i] = null;
			}
		}
		for(i = 0; i < colorActorDynamicTempArray.Length; i++)
		{
            if (colorActorDynamicTempArray[i] != null && colorActorDynamicTempArray[i].actorPriority == 4)
			{
				colorActorDynamicArray[dynamicArrayCount] = colorActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				colorActorDynamicTempArray[i] = null;
			}
		}
		for(i = 0; i < colorActorDynamicTempArray.Length; i++)
		{
            if (colorActorDynamicTempArray[i] != null && colorActorDynamicTempArray[i].actorPriority == 3)
			{
				colorActorDynamicArray[dynamicArrayCount] = colorActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				colorActorDynamicTempArray[i] = null;
			}
		}
		for(i = 0; i < colorActorDynamicTempArray.Length; i++)
		{
            if (colorActorDynamicTempArray[i] != null && colorActorDynamicTempArray[i].actorPriority == 2)
			{
				colorActorDynamicArray[dynamicArrayCount] = colorActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				colorActorDynamicTempArray[i] = null;
			}
		}
		for(i = 0; i < colorActorDynamicTempArray.Length; i++)
		{
            if (colorActorDynamicTempArray[i] != null && colorActorDynamicTempArray[i].actorPriority == 1)
			{
				colorActorDynamicArray[dynamicArrayCount] = colorActorDynamicTempArray[i];
				
				dynamicArrayCount++;
				
				colorActorDynamicTempArray[i] = null;
			}
		}
	}

//Tell paint actors about actor updates
	for(i = 0; i < colorSimScripts.Length; i++)
	{
		if(colorSimScripts[i])
		{
			GetActorArrayUpdate(colorSimScripts[i]);
		}
	}
}

//==========

    public void GetActorArrayUpdate(TexturePaintScript colorScriptRef)
    {
	    colorScriptRef.colorActorDynamicArray = colorActorDynamicArray;
	    colorScriptRef.dynamicInputArrayLength = dynamicArrayCount;
    }

//==========

    public void RegisterColorSimActor(TexturePaintScript passedColorScript)
    {
	    colorSimScripts[colorSimCount] = passedColorScript;
		
	    colorSimCount++;
    }
}