//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

@MenuItem ("GameObject/Create Other/TexturePaint InfluenceActor")
static function TexturePaint_InfluenceActor()
{
//Uses the current viewport camera for spawn location, or the world zero if the current camera isnt valid.
 	var menuSpawnPosition = Vector3.zero;
	var menuSpawnCamera : Camera;

	if(Selection.activeGameObject)
	{
		menuSpawnPosition = Selection.activeGameObject.transform.position;
	}
	else
	{
 		menuSpawnCamera = Camera.current;
		
		if(menuSpawnCamera)
		{
			menuSpawnPosition = menuSpawnCamera.ViewportToWorldPoint(Vector3(0.5, 0.5, 10.0));
		}
		else
		{
			menuSpawnPosition = Vector3.zero;
		}
	}
	
	if(Resources.Load("TexturePaintInfluenceActorObject"))
	{
		Instantiate(Resources.Load("TexturePaintInfluenceActorObject"), menuSpawnPosition, Quaternion.identity);
	}
	else
	{
		Debug.LogError("TexturePaintInfluenceActorObject was not found in the Resources folder and could not be created in the Scene.");
	}
}

@CustomEditor(TexturePaintInfluenceActor)
@CanEditMultipleObjects
class TexturePaintInfluenceActorInterface extends Editor
{
	var showGizmosTemp : SerializedProperty;
	var colorFoldoutTemp : SerializedProperty;
	var addColorTemp : SerializedProperty;
	var useColorMaskTextureTemp : SerializedProperty;
	var colorMaskTextureTemp : SerializedProperty;
	var colorValueTemp : SerializedProperty;
	var colorSizeTemp : SerializedProperty;
	var colorFalloffTemp : SerializedProperty;
	var myTransformTemp : SerializedProperty;
	var lastFramePositionTemp : SerializedProperty;
	var actorPriorityTemp : SerializedProperty;
	var multiplySizeByScaleTemp : SerializedProperty;
	
    function OnEnable()
    {
    	showGizmosTemp = serializedObject.FindProperty("showGizmos");
		colorFoldoutTemp = serializedObject.FindProperty("colorFoldout");
		addColorTemp = serializedObject.FindProperty("colorDetails.addColor");
		useColorMaskTextureTemp = serializedObject.FindProperty("colorDetails.useColorMaskTexture");
		colorMaskTextureTemp = serializedObject.FindProperty("colorDetails.colorMaskTexture");
		colorValueTemp = serializedObject.FindProperty("colorDetails.colorValue");
		colorSizeTemp = serializedObject.FindProperty("colorDetails.colorSize");
		colorFalloffTemp = serializedObject.FindProperty("colorDetails.colorFalloff");
		actorPriorityTemp = serializedObject.FindProperty("colorDetails.actorPriority");
		multiplySizeByScaleTemp = serializedObject.FindProperty("colorDetails.multiplySizeByScale");
    }

    override function OnInspectorGUI()
    {
    	serializedObject.Update();
    	
    //Works with the if() at the bottom to tell unity this object has been changed and it should update the sphere gizmo rendering.
    	GUI.changed = false;
    	
    	EditorGUILayout.Space();
    	
    	EditorGUILayout.PropertyField(showGizmosTemp, GUIContent("Show Size Gizmos"));
    	
    	EditorGUILayout.PropertyField(multiplySizeByScaleTemp, GUIContent("Scale Size by localScale"));

		EditorGUILayout.IntSlider(actorPriorityTemp, 1, 5, GUIContent("Influence Priority"));

    	EditorGUILayout.LabelField("_______________________");
    	EditorGUILayout.Space();

		if(addColorTemp.boolValue)
		{
			isColorOn = "(On)";
		}
		else
		{
			isColorOn = "(Off)";
		}

		colorFoldoutTemp.boolValue = EditorGUILayout.Foldout(colorFoldoutTemp.boolValue, "Influence Color " + isColorOn);
    	
    	if(colorFoldoutTemp.boolValue)
    	{
	    	EditorGUILayout.PropertyField(addColorTemp, GUIContent("Add Color"));

		    EditorGUILayout.PropertyField(useColorMaskTextureTemp, GUIContent("Use Texture"));
		    
		    if(useColorMaskTextureTemp.boolValue)
	    	{
	    		if(!colorMaskTextureTemp.hasMultipleDifferentValues)
	    		{
					colorMaskTextureTemp.objectReferenceValue = EditorGUILayout.ObjectField("Color Texture", colorMaskTextureTemp.objectReferenceValue, Texture2D, false);
				}
				else
				{
					EditorGUILayout.LabelField("Color Mask Texture");
					EditorGUILayout.LabelField("                                Multiple");
					EditorGUILayout.LabelField("                                Texture");
					EditorGUILayout.LabelField("                                Types");
				}
			}

			EditorGUILayout.PropertyField(colorValueTemp, GUIContent("Color (Alpha = Strength)"));

	    	EditorGUILayout.Slider(colorSizeTemp, 0, 500, "Color Size");
			
			if(!useColorMaskTextureTemp.boolValue)
			{
				EditorGUILayout.Slider(colorFalloffTemp, 0.01, 1, GUIContent("Color Falloff"));
			}
		}

		EditorGUILayout.Space();
		
    	if(GUI.changed)
		{
            EditorUtility.SetDirty(target);
    	}

    	serializedObject.ApplyModifiedProperties();
    }
}
