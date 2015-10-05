//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

var fpsFoldout = false;
var colorFoldout = true;

private var fpsFluidReport = "";
private var fpsColorReport = "";

private var colorUpdateText = "";

private var showFpsZeroWarning = false;

private var cautionDisplayed = false;

@MenuItem ("GameObject/Create Other/TexturePaint Grid")
static function TexturePaint_SimGrid()
{
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
	
	if(Resources.Load("TexturePaintObject"))
	{
		Instantiate(Resources.Load("TexturePaintObject"), menuSpawnPosition, Quaternion.identity);
	}
	else
	{
		Debug.LogError("TexturePaintObject was not found in the Resources folder and could not be created in the Scene.");
	}
}

@CustomEditor(TexturePaintScript)
@CanEditMultipleObjects
class TexturePaintScriptInterface extends Editor
{
    var colorUpdateFPSTemp : SerializedProperty;
    var colorStrengthTemp : SerializedProperty;
    var resolutionIndexTemp : SerializedProperty;
    var resolutionTemp : SerializedProperty;
    var colorResolutionIndexTemp : SerializedProperty;
    var outputTextureNumTemp : SerializedProperty;
    var materialTextureSlotTemp : SerializedProperty;
    var useColorDissipationTemp : SerializedProperty;
    var useColorDissipationTextureTemp : SerializedProperty;
    var colorDissipationTextureSourceTemp : SerializedProperty;
    var colorDissipationTemp : SerializedProperty;
    var oldColorDissipationTemp : SerializedProperty;
    var colorDissipateToTemp : SerializedProperty;
    var useStartColorTextureTemp : SerializedProperty;
    var startingColorTextureSourceTemp : SerializedProperty;
    var startingColorTemp : SerializedProperty;
	var resOptionsTemp : SerializedProperty;
	var uniqueColorResOptionsTemp : SerializedProperty;
	var outputTextureTemp : SerializedProperty;
	var materialTargetTemp : SerializedProperty;
	var useMyMaterialTemp : SerializedProperty;
	var useUnityProMethodTemp : SerializedProperty;
	var syncWithUpdateTemp : SerializedProperty;
	var use3DPaintTemp : SerializedProperty;
	var uvScaleBiasTemp : SerializedProperty;
	
	function OnEnable()
    {
	    colorUpdateFPSTemp = serializedObject.FindProperty("colorUpdateFPS");
	    colorStrengthTemp = serializedObject.FindProperty("colorStrength");
	    resolutionIndexTemp = serializedObject.FindProperty("resolutionIndex");
	    resolutionTemp = serializedObject.FindProperty("resolution");
	    colorResolutionIndexTemp = serializedObject.FindProperty("colorResolutionIndex");
	    outputTextureNumTemp = serializedObject.FindProperty("outputTextureNum");
	    materialTextureSlotTemp = serializedObject.FindProperty("materialTextureSlot");
	    useColorDissipationTemp = serializedObject.FindProperty("useColorDissipation");
	    useColorDissipationTextureTemp = serializedObject.FindProperty("useColorDissipationTexture");
	    colorDissipationTextureSourceTemp = serializedObject.FindProperty("colorDissipationTextureSource");
	    colorDissipationTemp = serializedObject.FindProperty("colorDissipation");
	    oldColorDissipationTemp = serializedObject.FindProperty("oldColorDissipation");
	    colorDissipateToTemp = serializedObject.FindProperty("colorDissipateTo");
	    useStartColorTextureTemp = serializedObject.FindProperty("useStartColorTexture");
	    startingColorTextureSourceTemp = serializedObject.FindProperty("startingColorTextureSource");
	    startingColorTemp = serializedObject.FindProperty("startingColor");
	    resOptionsTemp = serializedObject.FindProperty("resOptions");
		outputTextureTemp = serializedObject.FindProperty("outputTexture");
		materialTargetTemp = serializedObject.FindProperty("materialTarget");
		useMyMaterialTemp = serializedObject.FindProperty("useMyMaterial");
		useUnityProMethodTemp = serializedObject.FindProperty("useUnityProMethod");
		syncWithUpdateTemp = serializedObject.FindProperty("syncWithUpdate");
		use3DPaintTemp = serializedObject.FindProperty("use3DPaint");
		uvScaleBiasTemp = serializedObject.FindProperty("uvScaleBias");
		
		if(!Shader.Find("TexturePaint/TexturePaintImpulseLocationShader"))
		{
			Debug.LogError("TexturePaint/TexturePaintImpulseLocationShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		if(!Shader.Find("TexturePaint/TexturePaintImpulseLocation4Shader"))
		{
			Debug.LogError("TexturePaint/TexturePaintImpulseLocation4Shader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		if(!Shader.Find("TexturePaint/TexturePaintImpulseLocation8Shader"))
		{
			Debug.LogError("TexturePaint/TexturePaintImpulseLocation8Shader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
    	if(!Shader.Find("TexturePaint/TexturePaintInitializeToValueShader"))
		{
			Debug.LogError("TexturePaint/TexturePaintInitializeToValueShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		if(!Shader.Find("TexturePaint/TexturePaintInitializeToTextureShader"))
		{
			Debug.LogError("TexturePaint/TexturePaintInitializeToTextureShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		if(!Shader.Find("TexturePaint/TexturePaintColorDissipationShader"))
		{
			Debug.LogError("TexturePaint/TexturePaintColorDissipationShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		if(!Shader.Find("TexturePaint/TexturePaintColorDissipationTexShader"))
		{
			Debug.LogError("TexturePaint/TexturePaintColorDissipationTexShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
	}
	
    override function OnInspectorGUI()
    {
    	serializedObject.Update();
		
		//Works with the if() at the bottom to tell unity this object has been changed and it should update the sphere gizmo rendering.
    	GUI.changed = false;
    	
		//reset of the value check used below.
		cautionDisplayed = false;
		
    	EditorGUILayout.Space();

    	EditorGUILayout.PropertyField(useUnityProMethodTemp, GUIContent("Use GPU"));

		EditorGUILayout.HelpBox("GPU Features only work in Unity Pro.  If \"Use GPU\" is false, TexturePaint will only use the CPU.", MessageType.Info);
		
		EditorGUILayout.PropertyField(use3DPaintTemp, GUIContent("Paint on 3D objects"));
		
		EditorGUILayout.PropertyField(uvScaleBiasTemp, GUIContent("UV Scale Bias"));

    	EditorGUILayout.LabelField("_______________________");
    	EditorGUILayout.Space();

		EditorGUILayout.PropertyField(syncWithUpdateTemp, GUIContent("Sync Color to Update()"));
    	
    	if(!syncWithUpdateTemp.boolValue)
    	{
    		EditorGUILayout.Slider(colorUpdateFPSTemp, 0, 120, GUIContent("Color Update FPS"));
		}

    	EditorGUILayout.Slider(colorStrengthTemp, 0, 10, GUIContent("Color Strength"));

		EditorGUILayout.PropertyField(resOptionsTemp, GUIContent("Color Resolution"));
		
		switch(resOptionsTemp.intValue)
		{
			case 0 :
				resolutionTemp.intValue = 32;
				break;
			case 1 :
			    resolutionTemp.intValue = 64;
				break;
			case 2 :
			    resolutionTemp.intValue = 128;
				break;
			case 3 :
			    resolutionTemp.intValue = 256;
				break;
			case 4 :
			    resolutionTemp.intValue = 512;
				break;
			case 5 :
			    resolutionTemp.intValue = 1024;
				break;
			case 6 :
			    resolutionTemp.intValue = 2048;
				break;
			case 7 :
			    resolutionTemp.intValue = 4096;
		}

    	EditorGUILayout.PropertyField(useMyMaterialTemp, GUIContent("Use My Material"));
    	
    	if(!useMyMaterialTemp.boolValue)
    	{
    		EditorGUILayout.PropertyField(materialTargetTemp, GUIContent("Material Target"));
    	}
    	
    	EditorGUILayout.PropertyField(materialTextureSlotTemp, GUIContent("Material Texture Slot"));

		EditorGUILayout.LabelField("_______________________");
    	EditorGUILayout.Space();
		
    	colorFoldout = EditorGUILayout.Foldout(colorFoldout, "Color:");
    	
    	if(colorFoldout)
    	{
	    	EditorGUILayout.PropertyField(useColorDissipationTemp, GUIContent("Use Color Dissipation"));
	
			if(useUnityProMethodTemp.boolValue)
			{
				EditorGUILayout.PropertyField(useColorDissipationTextureTemp, GUIContent("Color Dissipation Mask"));
			}
			
			if(useColorDissipationTextureTemp.boolValue && useUnityProMethodTemp.boolValue)
			{
				if(!colorDissipationTextureSourceTemp.hasMultipleDifferentValues)
	    		{
					colorDissipationTextureSourceTemp.objectReferenceValue = EditorGUILayout.ObjectField("Color Dissp. Texture", colorDissipationTextureSourceTemp.objectReferenceValue, Texture2D, false);
				}
				else
				{
					EditorGUILayout.LabelField("Color Dissp. Texture");
					EditorGUILayout.LabelField("                                Multiple");
					EditorGUILayout.LabelField("                                Texture");
					EditorGUILayout.LabelField("                                Types");
				}
			}
			
				EditorGUILayout.Slider(oldColorDissipationTemp, 0.001, 1, GUIContent("Color Dissipation Rate"));
			
			
			if(!useColorDissipationTextureTemp.boolValue || !useUnityProMethodTemp.boolValue)
			{
				EditorGUILayout.PropertyField(colorDissipateToTemp, GUIContent("Dissipate To Color"));
			}
			
			if(!useColorDissipationTemp.boolValue)
			{
				colorDissipationTemp.floatValue = 0.0;
			}
	    	
			EditorGUILayout.PropertyField(useStartColorTextureTemp, GUIContent("Use Start Color Texture"));
			
	    	if(useStartColorTextureTemp.boolValue)
	    	{
	    		if(!startingColorTextureSourceTemp.hasMultipleDifferentValues)
	    		{
					startingColorTextureSourceTemp.objectReferenceValue = EditorGUILayout.ObjectField("Starting Color Texture", startingColorTextureSourceTemp.objectReferenceValue, Texture2D, false);;
				}
				else
				{
					EditorGUILayout.LabelField("Starting Color Texture");
					EditorGUILayout.LabelField("                                Multiple");
					EditorGUILayout.LabelField("                                Texture");
					EditorGUILayout.LabelField("                                Types");
				}
			}
			else
			{
				EditorGUILayout.PropertyField(startingColorTemp, GUIContent("Starting Color"));
			}
		}

		EditorGUILayout.Space();
		
		if(GUI.changed)
		{
            EditorUtility.SetDirty(target);
    	}
    	
    	serializedObject.ApplyModifiedProperties ();
    }
}
