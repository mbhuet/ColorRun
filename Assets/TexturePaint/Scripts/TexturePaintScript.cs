//Created by Phillip Heckinger, 2013.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

using UnityEngine;
using System.Collections;

[AddComponentMenu("TexturePaint/Texture Paint Grid")]
public class TexturePaintScript : MonoBehaviour
{
public RenderTexture colorRenderTextureSource;
private RenderTexture colorRenderTextureDestination;

private RenderTexture unityProTestRenderTexture;

public Texture2D colorRenderTextureSourceFree;
private Color32[] colorRenderTextureSourceFreeTempSA;
private int colorRenderTextureSourceFreeTempSCX = 0;
private int colorRenderTextureSourceFreeTempSCY = 0;

private float[] floatZeroRenderTextureSourceFreeTempSA;
private float[] floatOneRenderTextureSourceFreeTempSA;
private Vector2[] vector2ZeroRenderTextureSourceFreeTempSA;

public Color startingColor = Color.black;
public Texture2D startingColorTextureSource;

public Texture2D colorDissipationTextureSource;

private Texture2D tempStorageBufferTexture;
private Color32[] tempStorageArray = new Color32[64];
private int tempColorStorageCounter = 0;

private Material impulseLocationMat;
private Material impulseLocation4Mat;
private Material impulseLocation8Mat;
private Material impulseLocationTexMat;
private Material colorDissipationMat;
private Material colorDissipationTexMat;

private Material initializeToValueMat;
private Material initializeToTextureMat;

public colorInfluenceClass[] colorActorDynamicArray;

private int i;
private int n;

public float colorUpdateFPS = 25.0f;
public int resolution = 64;
private int resolutionHidden = 64;
public int resolutionIndex = 1;
public float colorStrength = 1.0f;
public bool useColorDissipation = true;
public float colorDissipation = 0.001f;
public bool useColorDissipationTexture = false;
public float oldColorDissipation = 0.001f;
public Color colorDissipateTo;

private Vector3 tempLocation;
private float tempColorSize;

private Transform myTransform;
private Vector3 myTransformPosition;
private Vector3 myTransformLocalScale;

public int dynamicInputArrayLength;

private Vector3 myBoundsSize;
private float myBoundsMagnitude;

private Vector3 tempPositionOffset;

private float textureRotationAngle;
private Quaternion textureRotationQuaternion;
private Matrix4x4 textureRotationMatrix;

public bool useStartColorTexture = false;
		
private GameObject colorConnectorObject;
private TexturePaintConnector colorConnectorScript;
		
private bool isTerrain = true;

public int outputTextureNum = 1;
private int outputTextureNumHidden = 1;
		
public Material materialTarget;
public string materialTextureSlot = "_MainTex";

private Mesh myMesh;

private int frameDelay = 0;

private bool updateInfluenceArray = false;

private bool blitColorShader = false;

private float kEncodeBit = 1.0f/255.0f;
private Vector2 enc2;
private Vector2 kEncodeMul2 = new Vector2(1.0f, 255.0f);
private Vector2 kDecodeDot2 = new Vector2(1.0f, 1.0f/255.0f);

private Color tempColor1;
private Color32 tempCompressedColor1;
private Color32 tempCompressedColor2;

private float freeColorPositionX = 0.0f;
private float freeColorPositionY = 0.0f;
private float freeColorSize = 0.0f;
private float freeColorFalloff = 0.0f;

private float freeTempFloat = 0.0f;

private Vector4 tempShaderVectorData;

public bool updateColorWithFluid = true;
private bool updateColorWithFluidHidden = true;

private float freeColorCalcAlpha = 0.0f;
private int tempColorPixelCount = 0;

private Color[] tempColorArray;

private int resolutionMin1 = 0;
private int resolution2Min1 = 0;
private int resolution2 = 0;

private Vector2 vector2Zero = new Vector2(0,0);

private Vector2 tempVector2;

private Color tempColor;

private int freeMinX = 0;
private int freeMinY = 0;
private int freeMaxX = 0;
private int freeMaxY = 0;

private Color32 color32Zero = new Color32(0,0,0,0);

private RaycastHit hitInfo;

public bool useMyMaterial = true;
		
public bool syncWithUpdate = false;

public bool useUnityProMethod = true;
		
public bool use3DPaint = false;
		
public float uvScaleBias = 1.0f;

public enum texturePaintResOptionsData
{
_32x32 = 0,
_64x64 = 1,
_128x128 = 2,
_256x256 = 3,
_512x512 = 4,
_1024x1024 = 5,
_2048x2048 = 6,
_4096x4096 = 7
}

public texturePaintResOptionsData resOptions = texturePaintResOptionsData._256x256;

//========

void Awake()
{
	if(GameObject.Find("dynamiclyCreatedTexturePaintConnector"))
	{
		colorConnectorObject = GameObject.Find("dynamiclyCreatedTexturePaintConnector");

        colorConnectorScript = colorConnectorObject.GetComponent<TexturePaintConnector>();
		
		colorConnectorScript.RegisterColorSimActor(this);
	}
	else
	{
		colorConnectorObject = new GameObject();
		
		colorConnectorObject.name = "dynamiclyCreatedTexturePaintConnector";

        colorConnectorScript = colorConnectorObject.AddComponent<TexturePaintConnector>();
		
		colorConnectorScript.RegisterColorSimActor(this);
	}
	
	if(colorConnectorObject == null)
	{
		Debug.LogError("TexturePaintScript failed to find or create a TexturePaintConnector object.  Make sure the TexturePaintConnector script exists and can by found by the TexturePaintScript.");
	}
	
	if(colorConnectorScript == null)
	{
		Debug.LogError("TexturePaintScript failed to find or create a TexturePaintConnector script.  Make sure the TexturePaintConnector script exists and can by found by the TexturePaintScript.");
	}
}

//========

void Start()
{
	myTransform = transform;
	myTransformPosition = transform.position;
	myTransformLocalScale = myTransform.localScale;

    myMesh = GetComponent<MeshFilter>().mesh;
	
	unityProTestRenderTexture = new RenderTexture(4, 4, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
	unityProTestRenderTexture.Create();

	if(!unityProTestRenderTexture.IsCreated())
	{
		useUnityProMethod = false;
			
		Debug.LogWarning("RenderTextures failed to create.  RenderTextures are only available in Unity Pro and are required for TexturePaint to work on the GPU.  TexturePaint will use the non-RenderTexture method on the CPU instead.");
	}
	else
	{
		unityProTestRenderTexture.Release();
	}

	//Prevents bugs related to unexpected resolution changes and output texture changes.  Resolution and output texture can only be changed/set before a TexturePaint is active.
	resolutionHidden = resolution;

	outputTextureNumHidden = outputTextureNum;
	
	if(useUnityProMethod)
	{
	//create a temp camera
        GameObject tempCameraObject = new GameObject();
        Camera tempCameraComp;
        tempCameraComp = tempCameraObject.AddComponent<Camera>();
		
	//render texture setup
		colorRenderTextureSource = new RenderTexture(resolutionHidden, resolutionHidden, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		colorRenderTextureSource.wrapMode = TextureWrapMode.Clamp;
		tempCameraComp.targetTexture = colorRenderTextureSource;
		tempCameraComp.Render();

		colorRenderTextureDestination = new RenderTexture(resolutionHidden, resolutionHidden, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		colorRenderTextureDestination.wrapMode = TextureWrapMode.Clamp;
		tempCameraComp.targetTexture = colorRenderTextureDestination;
		tempCameraComp.Render();

	//release temp files
		tempCameraComp = null;
		Destroy(tempCameraObject);
	}
		
	if(useUnityProMethod)
	{
		//material instance safety checks and setup
		if(!Shader.Find("TexturePaint/TexturePaintImpulseLocationShader"))
		{
			Debug.LogError("TexturePaint/TexturePaintImpulseLocationShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		else
		{
			impulseLocationMat = new Material(Shader.Find("TexturePaint/TexturePaintImpulseLocationShader"));
		}
		if(!Shader.Find("TexturePaint/TexturePaintImpulseLocation4Shader"))
		{
			Debug.LogError("TexturePaint/TexturePaintImpulseLocation4Shader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		else
		{
			impulseLocation4Mat = new Material(Shader.Find("TexturePaint/TexturePaintImpulseLocation4Shader"));
		}
		if(!Shader.Find("TexturePaint/TexturePaintImpulseLocation8Shader"))
		{
			Debug.LogError("TexturePaint/TexturePaintImpulseLocation8Shader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		else
		{
			impulseLocation8Mat = new Material(Shader.Find("TexturePaint/TexturePaintImpulseLocation8Shader"));
		}
		if(!Shader.Find("TexturePaint/TexturePaintImpulseLocationTexShader"))
		{
			Debug.LogError("TexturePaint/TexturePaintImpulseLocationTexShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		else
		{
			impulseLocationTexMat = new Material(Shader.Find("TexturePaint/TexturePaintImpulseLocationTexShader"));
		}
		if(!Shader.Find("TexturePaint/TexturePaintInitializeToValueShader"))
		{
			Debug.LogError("TexturePaint/TexturePaintInitializeToValueShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		else
		{
			initializeToValueMat = new Material(Shader.Find("TexturePaint/TexturePaintInitializeToValueShader"));
		}
		if(!Shader.Find("TexturePaint/TexturePaintInitializeToTextureShader"))
		{
            Debug.LogError("TexturePaint/TexturePaintInitializeToTextureShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		else
		{
			initializeToTextureMat = new Material(Shader.Find("TexturePaint/TexturePaintInitializeToTextureShader"));
		}
		if(!Shader.Find("TexturePaint/TexturePaintColorDissipationShader"))
		{
            Debug.LogError("TexturePaint/TexturePaintColorDissipationShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		else
		{
			colorDissipationMat = new Material(Shader.Find("TexturePaint/TexturePaintColorDissipationShader"));
		}
		if(!Shader.Find("TexturePaint/TexturePaintColorDissipationTexShader"))
		{
            Debug.LogError("TexturePaint/TexturePaintColorDissipationTexShader could not be found.  TexturePaint cannot be simulated without this shader.");
		}
		else
		{
			colorDissipationTexMat = new Material(Shader.Find("TexturePaint/TexturePaintColorDissipationTexShader"));
		}
	}
	
	if(!useUnityProMethod)
	{
		resolutionMin1 = resolutionHidden - 1;
		resolution2Min1 = (resolutionHidden * resolutionHidden) - 1;
		resolution2 = resolutionHidden * resolutionHidden;
	
		colorRenderTextureSourceFree = new Texture2D(resolutionHidden, resolutionHidden, TextureFormat.ARGB32, false, true);
		colorRenderTextureSourceFree.wrapMode = TextureWrapMode.Clamp;
		colorRenderTextureSourceFreeTempSA = new Color32[resolution2];
		
		floatZeroRenderTextureSourceFreeTempSA = new float[resolution2];
		floatOneRenderTextureSourceFreeTempSA = new float[resolution2];
		vector2ZeroRenderTextureSourceFreeTempSA = new Vector2[resolution2];
		
		for(i = 0; i < resolution2; i++)
		{
			floatZeroRenderTextureSourceFreeTempSA[i] = 0.0f;
			floatOneRenderTextureSourceFreeTempSA[i]  = 1.0f;
			vector2ZeroRenderTextureSourceFreeTempSA[i] = Vector2.zero;
		}
		
		if(tempColorArray == null)
		{
			tempColorArray = new Color[resolutionHidden * resolutionHidden];
		}
	}

	if(useUnityProMethod)
	{
		tempStorageBufferTexture = new Texture2D(16, 4, TextureFormat.ARGB32, false, true);
		tempStorageBufferTexture.filterMode = FilterMode.Point;
		tempStorageBufferTexture.wrapMode = TextureWrapMode.Clamp;
	}
	
	if(useColorDissipation)
	{
		colorDissipation = oldColorDissipation;
	}
	else
	{
		colorDissipation = 0;
	}

    if (gameObject.GetComponent<Terrain>())
	{
		isTerrain = true;
		Debug.LogError("TexturePaint Script doesn't support processing on Terrain.  Process TexturePaint on a GameObject with UVs and apply the resulting texture to the Terrain.");
	}
	else
	{
		isTerrain = false;
		
		if(materialTarget == null)
		{
			materialTarget = (Material)Resources.Load("TexturePaintSimpleMat");
			
			if(materialTarget == null)
			{
				Debug.LogError("Material Target is null and Unity failed to find \"TexturePaintSimpleMat\".  Make sure TexturePaintSimpleMat can be found, or assign a Material Target to the TexturePaintGrid.");
			}
		}
		
		if(useMyMaterial)
		{
			materialTarget = GetComponent<Renderer>().material;
		}
		
		if(materialTarget)
		{
			if(!materialTarget.HasProperty(materialTextureSlot))
			{
				Debug.LogError("The Material Texture Slot entered in TexturePaint could not be found in the material TexturePaint is currently trying to use.  Make sure the texture slot name is correct and the material being used has that slot name.");
			}
		}
		else
		{
			Debug.LogError("Material Target is null on TexturePaint.  Please make sure TexturePaint has a material target set, or the material TexturePaintSimSimpleMat is in the resources folder.");
		}
		
		if(useUnityProMethod)
		{
			materialTarget.SetTexture(materialTextureSlot,  colorRenderTextureSource);
		}
		else
		{
			materialTarget.SetTexture(materialTextureSlot,  colorRenderTextureSourceFree);
		}
		
		myBoundsSize = myMesh.bounds.size;
		myTransformLocalScale = myTransform.localScale;
		myBoundsSize = new Vector3(myBoundsSize.x * myTransformLocalScale.x, myBoundsSize.y * myTransformLocalScale.y, myBoundsSize.z * myTransformLocalScale.z);
		myBoundsMagnitude = myBoundsSize.magnitude;
	}
	
	if(useUnityProMethod)
	{
		if(startingColorTextureSource && useStartColorTexture)
		{
		    initializeToTextureMat.SetTexture("initialTexture", startingColorTextureSource);
		    Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, initializeToTextureMat);
		    Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		else
		{
		    initializeToValueMat.SetVector("initialValue", startingColor);
		    Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, initializeToValueMat);
		    Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		
		impulseLocationMat.SetTexture("fluidTex", colorRenderTextureSource);
		impulseLocationMat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
		impulseLocationMat.SetFloat("halfStorageRDX", (1.0f/16.0f) * 0.5f);
		impulseLocationMat.SetFloat("globalStrength", colorStrength);
		
		impulseLocation4Mat.SetTexture("fluidTex", colorRenderTextureSource);
		impulseLocation4Mat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
        impulseLocation4Mat.SetFloat("halfStorageRDX", (1.0f/16.0f) * 0.5f);
		impulseLocation4Mat.SetFloat("globalStrength", colorStrength);
		
		impulseLocation8Mat.SetTexture("fluidTex", colorRenderTextureSource);
		impulseLocation8Mat.SetTexture("tempStorageBuffer", tempStorageBufferTexture);
        impulseLocation8Mat.SetFloat("halfStorageRDX", (1.0f/16.0f) * 0.5f);
		impulseLocation8Mat.SetFloat("globalStrength", colorStrength);
		
		impulseLocationTexMat.SetTexture("fluidTex", colorRenderTextureSource);
		impulseLocationTexMat.SetFloat("globalStrength", colorStrength);
		
		colorDissipationMat.SetTexture("fluidTex", colorRenderTextureSource);
		colorDissipationMat.SetFloat("dissipation", colorDissipation);
		colorDissipationMat.SetVector("dissipationColor", colorDissipateTo);
	
		colorDissipationTexMat.SetTexture("fluidTex", colorRenderTextureSource);
		colorDissipationTexMat.SetFloat("dissipation", colorDissipation);
		colorDissipationTexMat.SetTexture("dissipationTex", colorDissipationTextureSource);
		colorDissipationTexMat.SetVector("dissipationColor", colorDissipateTo);
	}
	
	if(!useUnityProMethod)
	{
	//clear and setup color and velocity fields
		if(useStartColorTexture)
		{
			initializeToTextureMatFree(startingColorTextureSource);
		}
		else
		{
			initializeToValueMatFree(startingColor);
		}
	}
	
	updateColorWithFluidHidden = updateColorWithFluid;

//A delayed call on static collision setup so that all of the collision actors can "report in" first.
	if(useUnityProMethod)
	{
		if(colorUpdateFPS > 0 && !syncWithUpdate)
		{
			InvokeRepeating("UpdateColor", 0.13f, 1.0f / colorUpdateFPS);
		}
	}
	else
	{		
		if(colorUpdateFPS > 0 && !syncWithUpdate)
		{
			InvokeRepeating("UpdateColorFree", 0.13f, 1.0f / colorUpdateFPS);
		}
	}
	
	if(useUnityProMethod)
	{
		if(useColorDissipationTexture)
		{
			InvokeRepeating("DissipateColorTex", 0.13f, 0.05f);
		}
		else if(useColorDissipation)
		{
			InvokeRepeating("DissipateColor", 0.13f, 0.05f);
		}
	}
	else
	{
		if(useColorDissipation)
		{
			InvokeRepeating("DissipateColorFree", 0.13f, 0.05f);
		}
	}
}

//========

void Update()
{
	if(syncWithUpdate)
	{
		if(useUnityProMethod)
		{
			UpdateColor();
		}
		else
		{		
			UpdateColorFree();
		}
	}
}

//========

void DissipateColor()
{
    Graphics.Blit(colorRenderTextureSource, colorRenderTextureDestination, colorDissipationMat);
    Graphics.Blit(colorRenderTextureDestination, colorRenderTextureSource);
}

//========

void DissipateColorTex()
{
    Graphics.Blit(colorRenderTextureSource, colorRenderTextureDestination, colorDissipationTexMat);
    Graphics.Blit(colorRenderTextureDestination, colorRenderTextureSource);
}

//========

void Update3DColorActors()
{
//set color for each input object
					//length is defined in the texturePaintConnector script
	for (n = 0; n < dynamicInputArrayLength; n++)
	{
		if(colorActorDynamicArray[n] != null)
		{
			if(colorActorDynamicArray[n].addColor)
			{
				if(colorActorDynamicArray[n].multiplySizeByScale)
				{
					tempColorSize = colorActorDynamicArray[n].colorSize * (colorActorDynamicArray[n].myTransform.localScale.magnitude * 0.577f);
				}
				else
				{
					tempColorSize = colorActorDynamicArray[n].colorSize;
				}
				
				if((colorActorDynamicArray[n].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempColorSize)
                {
					if(Physics.Raycast(colorActorDynamicArray[n].myTransform.position + (colorActorDynamicArray[n].myTransform.up * tempColorSize), colorActorDynamicArray[n].myTransform.up * -1.0f, out hitInfo, tempColorSize * 2))
                    {
                        if (hitInfo.transform == myTransform)
                        {
                            if (colorActorDynamicArray[n].useColorMaskTexture && colorActorDynamicArray[n].colorMaskTexture)
                            {
                                //locationX, locationY
                                tempShaderVectorData.x = ((hitInfo.textureCoord.x + 0.25f) * 0.5f) + 0.125f;
                                tempShaderVectorData.y = ((hitInfo.textureCoord.y + 0.25f) * 0.5f) + 0.125f;
                                //Color alpha value
                                tempShaderVectorData.z = colorActorDynamicArray[n].colorValue.a;
                                impulseLocationTexMat.SetVector("textureData", tempShaderVectorData);
                                impulseLocationTexMat.SetTexture("colorTexMask", colorActorDynamicArray[n].colorMaskTexture);
                                impulseLocationTexMat.SetVector("modColor", colorActorDynamicArray[n].colorValue);
                                textureRotationAngle = colorActorDynamicArray[n].myTransform.localRotation.eulerAngles.y;
                                textureRotationQuaternion = Quaternion.Euler(0, 0, textureRotationAngle);
                                textureRotationMatrix = Matrix4x4.TRS(vector2Zero, textureRotationQuaternion, new Vector2(1.0f / (1.8f * (tempColorSize / myBoundsSize.x) * uvScaleBias), 1 / (1.8f * (tempColorSize / myBoundsSize.z) * uvScaleBias)));
                                impulseLocationTexMat.SetMatrix("rotationMatrix", textureRotationMatrix);

                                Graphics.Blit(colorRenderTextureSource, colorRenderTextureDestination, impulseLocationTexMat);
                                Graphics.Blit(colorRenderTextureDestination, colorRenderTextureSource);
                            }
                            else
                            {
                                //positionX, positionY
                                tempCompressedColor1 = EncodeFloatRG(((hitInfo.textureCoord.x + 0.25f) * 0.5f) + 0.125f);
                                tempCompressedColor2 = EncodeFloatRG(((hitInfo.textureCoord.y + 0.25f) * 0.5f) + 0.125f);
                                tempStorageArray[tempColorStorageCounter] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, tempCompressedColor2.g);
                                //color and alpha
                                tempColor1 = colorActorDynamicArray[n].colorValue;
                                tempCompressedColor1 = new Color32((byte)(tempColor1.r * 255), (byte)(tempColor1.g * 255), (byte)(tempColor1.b * 255), (byte)(tempColor1.a * 255));
                                tempStorageArray[tempColorStorageCounter + 16] = tempCompressedColor1;
                                //color size, fallOff
                                tempCompressedColor1 = EncodeFloatRG(uvScaleBias * (tempColorSize / myBoundsSize.x) * 0.5f);
                                tempCompressedColor2.r = (byte)(colorActorDynamicArray[n].colorFalloff * 255);
                                tempStorageArray[tempColorStorageCounter + 32] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, 0);

                                tempColorStorageCounter++;

                                if (tempColorStorageCounter > 15)
                                {
                                    tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
                                    tempStorageBufferTexture.Apply(false);

                                    Graphics.Blit(colorRenderTextureSource, colorRenderTextureDestination, impulseLocationMat);
                                    Graphics.Blit(colorRenderTextureDestination, colorRenderTextureSource);

                                    blitColorShader = false;
                                    tempColorStorageCounter = 0;
                                }
                                else
                                {
                                    blitColorShader = true;
                                }
                            }
                        }
					}
				}
			}
		}
		else
		{
			updateInfluenceArray = true;
		}
	}
	
	if(blitColorShader)
	{
		//zero out remaining pixels (if its not a full load) to prevent possible bugs with deleted actors
		for(n = tempColorStorageCounter; n < 16; n++)
		{
			tempStorageArray[n] = color32Zero;
			tempStorageArray[n + 16] = color32Zero;
			tempStorageArray[n + 32] = color32Zero;
		}
		
		tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
		tempStorageBufferTexture.Apply(false);
		
		if(tempColorStorageCounter <= 4)
		{
		    Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, impulseLocation4Mat);
		    Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		else if(tempColorStorageCounter <= 8)
		{
		    Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, impulseLocation8Mat);
		    Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		else
		{
		    Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, impulseLocationMat);
		    Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		
		blitColorShader = false;

		tempColorStorageCounter = 0;
	}
	
	if(updateInfluenceArray)
	{
		colorConnectorScript.SortActorArray();
		colorConnectorScript.GetActorArrayUpdate(this);
		updateInfluenceArray = false;
	}
}



//========

void UpdateColorActors()
{
//set color for each input object
					//length is defined in the texturePaintConnector script
	for (n = 0; n < dynamicInputArrayLength; n++)
	{
		if(colorActorDynamicArray[n] != null)
		{
			if(colorActorDynamicArray[n].addColor)
			{
				if(colorActorDynamicArray[n].multiplySizeByScale)
				{
					tempColorSize = colorActorDynamicArray[n].colorSize * (colorActorDynamicArray[n].myTransform.localScale.magnitude * 0.577f);
				}
				else
				{
					tempColorSize = colorActorDynamicArray[n].colorSize;
				}
				
				if((colorActorDynamicArray[n].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempColorSize)
                {
					if(colorActorDynamicArray[n].useColorMaskTexture && colorActorDynamicArray[n].colorMaskTexture)
					{
						tempLocation = myTransform.InverseTransformPoint(colorActorDynamicArray[n].myTransform.position);
						//locationX, locationY
						tempShaderVectorData.x = ((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1.0f) + 1.0f) * 0.5f;
						tempShaderVectorData.y = ((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1.0f) + 1.0f) * 0.5f;
						//Color alpha value
						tempShaderVectorData.z = colorActorDynamicArray[n].colorValue.a;
						impulseLocationTexMat.SetVector("textureData", tempShaderVectorData);
						impulseLocationTexMat.SetTexture("colorTexMask", colorActorDynamicArray[n].colorMaskTexture);
						impulseLocationTexMat.SetVector("modColor", colorActorDynamicArray[n].colorValue);
						textureRotationAngle = Vector3.Angle(colorActorDynamicArray[n].myTransform.forward, myTransform.forward);
						if(Vector3.Dot(colorActorDynamicArray[n].myTransform.forward, myTransform.right) < 0)
						{
							textureRotationAngle *= -1;
						}
						textureRotationQuaternion = Quaternion.Euler(0, 0, textureRotationAngle);
						textureRotationMatrix = Matrix4x4.TRS(vector2Zero, textureRotationQuaternion, new Vector2(1 / (1.8f * (tempColorSize / myBoundsSize.x) * uvScaleBias), 1 / (1.8f * (tempColorSize / myBoundsSize.z) * uvScaleBias)));
			        	impulseLocationTexMat.SetMatrix("rotationMatrix", textureRotationMatrix);

			        	Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, impulseLocationTexMat);
			        	Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
					}
					else
					{
						tempLocation = myTransform.InverseTransformPoint(colorActorDynamicArray[n].myTransform.position);
						//positionX, positionY
						tempCompressedColor1 = EncodeFloatRG(((((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) * -1.0f) + 1.0f) * 0.5f);
						tempCompressedColor2 = EncodeFloatRG(((((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) * -1.0f) + 1.0f) * 0.5f);
						tempStorageArray[tempColorStorageCounter] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, tempCompressedColor2.g);
						//color and alpha
						tempColor1 = colorActorDynamicArray[n].colorValue;
						tempCompressedColor1 = new Color32((byte)(tempColor1.r * 255), (byte)(tempColor1.g * 255), (byte)(tempColor1.b * 255), (byte)(tempColor1.a * 255));
						tempStorageArray[tempColorStorageCounter + 16] = tempCompressedColor1;
						//color size, fallOff
						tempCompressedColor1 = EncodeFloatRG(uvScaleBias * (tempColorSize / myBoundsSize.x) * 0.5f);
						tempCompressedColor2.r = (byte)(colorActorDynamicArray[n].colorFalloff * 255);
						tempStorageArray[tempColorStorageCounter + 32] = new Color32(tempCompressedColor1.r, tempCompressedColor1.g, tempCompressedColor2.r, 0);

						tempColorStorageCounter++;
						
						if(tempColorStorageCounter > 15)
						{
							tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
							tempStorageBufferTexture.Apply(false);
							
							Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, impulseLocationMat);
							Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);

							blitColorShader = false;
							tempColorStorageCounter = 0;
						}
						else
						{
							blitColorShader = true;
						}
					}
				}
			}
		}
		else
		{
			updateInfluenceArray = true;
		}
	}
	
	if(blitColorShader)
	{
		//zero out remaining pixels (if its not a full load) to prevent possible bugs with deleted actors
		for(n = tempColorStorageCounter; n < 16; n++)
		{
			tempStorageArray[n] = color32Zero;
			tempStorageArray[n + 16] = color32Zero;
			tempStorageArray[n + 32] = color32Zero;
		}
		
		tempStorageBufferTexture.SetPixels32(tempStorageArray, 0);
		tempStorageBufferTexture.Apply(false);
		
		if(tempColorStorageCounter <= 4)
		{
		    Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, impulseLocation4Mat);
		    Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		else if(tempColorStorageCounter <= 8)
		{
		    Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, impulseLocation8Mat);
		    Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		else
		{
		    Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, impulseLocationMat);
		    Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		
		blitColorShader = false;

		tempColorStorageCounter = 0;
	}
	
	if(updateInfluenceArray)
	{
		colorConnectorScript.SortActorArray();
		colorConnectorScript.GetActorArrayUpdate(this);
		updateInfluenceArray = false;
	}
}

//========

void UpdateColor()
{
	frameDelay++;

	switch(frameDelay)
	{
		case 1:
			myTransformPosition = myTransform.position;
			break;
		case 2:
			myTransformLocalScale = myTransform.localScale;
			break;
		case 3:
			if(isTerrain == false)
			{
				myBoundsSize = myMesh.bounds.size;
				myBoundsSize = new Vector3(myBoundsSize.x * myTransformLocalScale.x, myBoundsSize.y * myTransformLocalScale.y, myBoundsSize.z * myTransformLocalScale.z);
				myBoundsMagnitude = myBoundsSize.magnitude;
			}
			
			frameDelay = 0;
			break;
	}

	if(use3DPaint)
	{
		Update3DColorActors();
	}
	else
	{
		UpdateColorActors();
	}
}

//===================================== Helper Functions Below This Line =====================================

// Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly... so we clamp it.
Color32 EncodeFloatRG(float v)
{
	v = Mathf.Clamp(v, 0f, 0.9999f);
	enc2 = new Vector2(kEncodeMul2.x * v, kEncodeMul2.y * v);
	enc2 = new Vector2(enc2.x % 1, enc2.y % 1);   //same as doing frac()... but frac doesnt exist and % has precision loss of about 0.00000005
	enc2.x -= enc2.y * kEncodeBit;
	return new Color32((byte)Mathf.RoundToInt(enc2.x * 255), (byte)(enc2.y * 255), (byte)(0), (byte)(0));
}

//Decode is only used for testing and could be removed without breaking the simulation.
float DecodeFloatRG(Color32 colorV)
{
	return Vector2.Dot(new Vector2(colorV.r, colorV.g), kDecodeDot2);
}

//========

public void ForceColorUpdate()
{
	if(useUnityProMethod)
	{
		UpdateColor();
	}
	else
	{
		UpdateColorFree();
	}
}

//========

public void ChangeColorStrength(float tempStrength)
{
	colorStrength = tempStrength;
	
	if(useUnityProMethod)
	{
		impulseLocationMat.SetFloat("globalStrength", colorStrength);
		impulseLocation4Mat.SetFloat("globalStrength", colorStrength);
		impulseLocation8Mat.SetFloat("globalStrength", colorStrength);
		impulseLocationTexMat.SetFloat("globalStrength", colorStrength);
	}
}

//========

public void ChangeColorDissipationStrength(float dissipation)
{
	colorDissipation = dissipation;
	oldColorDissipation = dissipation;
		
	if(useColorDissipation)
	{
		if(useUnityProMethod)
		{
			colorDissipationMat.SetFloat("dissipation", colorDissipation);
		
			colorDissipationTexMat.SetFloat("dissipation", colorDissipation);
		}
	}
	else
	{
		Debug.LogWarning("TexturePaint Color dissipation is currently disabled. To see color dissipation, make sure it is enabled.");
	}
}

//========

public void ChangeColorDissipateTo(Color dissipateColor)
{
		colorDissipateTo = dissipateColor;

		if(useUnityProMethod)
		{
			colorDissipationMat.SetVector("dissipationColor", colorDissipateTo);

			colorDissipationTexMat.SetVector("dissipationColor", colorDissipateTo);
		}
}

//========

public void ChangeColorDissipationBool(bool value)
{
	useColorDissipation = value;
	
	if(useColorDissipation)
	{
		colorDissipation = oldColorDissipation;
		
		if(useUnityProMethod)
		{
			colorDissipationMat.SetFloat("dissipation", colorDissipation);

			colorDissipationTexMat.SetFloat("dissipation", colorDissipation);
		}
	}
	else
	{
		oldColorDissipation = colorDissipation;
		
		colorDissipation = 0.0f;
		
		if(useUnityProMethod)
		{
			colorDissipationMat.SetFloat("dissipation", colorDissipation);

			colorDissipationTexMat.SetFloat("dissipation", colorDissipation);
		}
	}
}

//========

public void SetColorSyncWithUpdate(bool syncBool)
{
	syncWithUpdate = syncBool;
	
	if(use3DPaint)
	{
		if(syncWithUpdate)
		{
			CancelInvoke("Update3DColorActors");
		}
		else
		{
			CancelInvoke("Update3DColorActors");
				
			if(colorUpdateFPS > 0)
			{
				InvokeRepeating("Update3DColorActors", 0.1f, 1 / colorUpdateFPS);
			}
			else
			{
				Debug.LogWarning("ColorUpdateFPS is set to zero and will not be set to update.");
			}
		}
	}
	else
	{
		if(syncWithUpdate)
		{
			CancelInvoke("UpdateColorActors");
		}
		else
		{
			CancelInvoke("UpdateColorActors");
				
			if(colorUpdateFPS > 0)
			{
				InvokeRepeating("UpdateColorActors", 0.1f, 1 / colorUpdateFPS);
			}
			else
			{
				Debug.LogWarning("ColorUpdateFPS is set to zero and will not be set to update.");
			}
		}
	}
}

//========

public void SetColorUpdateFPS(float fps)
{
	colorUpdateFPS = fps;
	
	if(use3DPaint)
	{
		if(useUnityProMethod)
		{
			if(!syncWithUpdate)
			{
				CancelInvoke("Update3DColorActors");
				
				if(colorUpdateFPS > 0)
				{
					InvokeRepeating("Update3DColorActors", 0.1f, 1 / colorUpdateFPS);
				}
			}
			else
			{
				Debug.LogWarning("Color Update is currently synced with Update(); to use SetColorUpdateFPS you need to turn off color sync with Update() first.  Try using SetColorSyncWithUpdate(boolean) first.");
			}
		}
		else
		{
			if(!syncWithUpdate)
			{
				CancelInvoke("Update3DColorActorsFree");
				
				if(colorUpdateFPS > 0)
				{
					InvokeRepeating("Update3DColorActorsFree", 0.1f, 1 / colorUpdateFPS);
				}
			}
			else
			{
				Debug.LogWarning("Color Update is currently synced with Update(); to use SetColorUpdateFPS you need to turn off color sync with Update() first.  Try using SetColorSyncWithUpdate(boolean) first.");
			}
		}
	}
	else
	{
		if(useUnityProMethod)
		{
			if(!syncWithUpdate)
			{
				CancelInvoke("UpdateColorActors");
				
				if(colorUpdateFPS > 0)
				{
					InvokeRepeating("UpdateColorActors", 0.1f, 1 / colorUpdateFPS);
				}
			}
			else
			{
				Debug.LogWarning("Color Update is currently synced with Update(); to use SetColorUpdateFPS you need to turn off color sync with Update() first.  Try using SetColorSyncWithUpdate(boolean) first.");
			}
		}
		else
		{
			if(!syncWithUpdate)
			{
				CancelInvoke("UpdateColorActorsFree");
				
				if(colorUpdateFPS > 0)
				{
					InvokeRepeating("UpdateColorActorsFree", 0.1f, 1 / colorUpdateFPS);
				}
			}
			else
			{
				Debug.LogWarning("Color Update is currently synced with Update(); to use SetColorUpdateFPS you need to turn off color sync with Update() first.  Try using SetColorSyncWithUpdate(boolean) first.");
			}
		}
	}
}

//========

public void ClearColorBuffer(Color defaultColor)
{
	if(useUnityProMethod)
	{
		if(defaultColor != null)
		{
			initializeToValueMat.SetVector("initialValue", new Vector3(defaultColor.r, defaultColor.g, defaultColor.b));
			Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, initializeToValueMat);
			Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		else
		{
			Debug.LogError("ClearColorBuffer() was called without supplying a defaultColor value to the function.  The TexturePaint Color buffer has not been cleared");
		}
	}
	else
	{
		if(defaultColor != null)
		{
			clearColorToValueMatFree(defaultColor);
		}
		else
		{
			Debug.LogError("ClearColorBuffer() was called without supplying a defaultColor value to the function.  The TexturePaint Color buffer has not been cleared");
		}
	}
}

//========

public void ClearColorBufferToTexture(Texture2D colorTexture)
{
	if(useUnityProMethod)
	{
		if(colorTexture != null)
		{
			initializeToTextureMat.SetTexture("initialTexture", colorTexture);
			Graphics.Blit (colorRenderTextureSource, colorRenderTextureDestination, initializeToTextureMat);
			Graphics.Blit (colorRenderTextureDestination, colorRenderTextureSource);
		}
		else
		{
			Debug.LogError("ClearColorBufferToTexture() was called without supplying a colorTexture to the function.  The TexturePaint Color buffer has not been cleared to the texture value.");
		}
	}
	else
	{
		if(colorTexture != null)
		{
			initializeToTextureMatFree(colorTexture);
		}
		else
		{
			Debug.LogError("ClearColorBufferToTexture() was called without supplying a colorTexture to the function.  The TexturePaint Color buffer has not been cleared to the texture value.");
		}
	}
}

//====================================== UNITY FREE TEXTUREPAINT FUNCTIONS ONLY BELOW THIS LINE ======================================

void DissipateColorFree()
{
	for(i = 0; i < resolution2Min1; i++)
	{
		colorRenderTextureSourceFreeTempSA[i] = Color32.Lerp(colorRenderTextureSourceFreeTempSA[i], colorDissipateTo, colorDissipation);
	}
}
	
	
//========

void Update3DColorActorsFree()
{
//set color for each input object
					//length is defined in the texturePaintConnector script
	for (n = 0; n < dynamicInputArrayLength; n++)
	{
		if(colorActorDynamicArray[n] != null)
		{
			if(colorActorDynamicArray[n].addColor)
			{
				if(colorActorDynamicArray[n].multiplySizeByScale)
				{
					tempColorSize = colorActorDynamicArray[n].colorSize * (colorActorDynamicArray[n].myTransform.localScale.magnitude * 0.577f);
				}
				else
				{
					tempColorSize = colorActorDynamicArray[n].colorSize;
				}
				
				if((colorActorDynamicArray[n].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempColorSize)
				{
					if(Physics.Raycast(colorActorDynamicArray[n].myTransform.position + (colorActorDynamicArray[n].myTransform.up * tempColorSize), colorActorDynamicArray[n].myTransform.up * -1.0f, out hitInfo, tempColorSize * 2.0f))
					{
                        if (hitInfo.transform == myTransform)
                        {
                            //positionX, positionY
                            freeColorPositionX = hitInfo.textureCoord.x;
                            freeColorPositionY = hitInfo.textureCoord.y;
                            tempVector2 = new Vector2(freeColorPositionX, freeColorPositionY);
                            //color and alpha
                            tempColor1 = colorActorDynamicArray[n].colorValue;
                            //color size, fallOff
                            freeColorSize = (tempColorSize / myBoundsSize.x) * uvScaleBias;
                            freeColorFalloff = colorActorDynamicArray[n].colorFalloff * 150;

                            freeMinY = (int)((freeColorPositionY - (freeColorSize * 0.975f)) * resolutionHidden);
                            freeMinX = (int)((freeColorPositionX - (freeColorSize * 0.975f)) * resolutionHidden);

                            if (freeMinY < 0)
                            {
                                freeMinY = 0;
                            }

                            if (freeMinX < 0)
                            {
                                freeMinX = 0;
                            }

                            freeMaxY = (int)((freeColorPositionY + (freeColorSize * 1.2f)) * resolutionHidden);
                            freeMaxX = (int)((freeColorPositionX + (freeColorSize * 1.2f)) * resolutionHidden);

                            if (freeMaxY > resolutionHidden)
                            {
                                freeMaxY = resolutionHidden;
                            }

                            if (freeMaxX > resolutionHidden)
                            {
                                freeMaxX = resolutionHidden;
                            }

                            tempColorPixelCount = 0;

                            for (colorRenderTextureSourceFreeTempSCY = freeMinY; colorRenderTextureSourceFreeTempSCY < freeMaxY; colorRenderTextureSourceFreeTempSCY++)
                            {
                                //fake clamp function
                                tempColorPixelCount = (colorRenderTextureSourceFreeTempSCY * resolutionHidden) + freeMinX;

                                if (tempColorPixelCount >= 0)
                                {
                                    for (colorRenderTextureSourceFreeTempSCX = freeMinX; colorRenderTextureSourceFreeTempSCX < freeMaxX; colorRenderTextureSourceFreeTempSCX++)
                                    {
                                        if (tempColorPixelCount <= resolution2Min1)
                                        {
                                            //fake clamp function
                                            freeTempFloat = (Vector2.Distance(new Vector2(colorRenderTextureSourceFreeTempSCX / (float)resolutionHidden, colorRenderTextureSourceFreeTempSCY / (float)resolutionHidden), tempVector2) - freeColorSize) * -freeColorFalloff;
                                            if (freeTempFloat < 0)
                                            {
                                                freeTempFloat = 0;
                                            }
                                            else if (freeTempFloat > 1)
                                            {
                                                freeTempFloat = 1;
                                            }

                                            freeColorCalcAlpha = tempColor1.a * freeTempFloat;

                                            colorRenderTextureSourceFreeTempSA[tempColorPixelCount] = Color32.Lerp(colorRenderTextureSourceFreeTempSA[tempColorPixelCount], tempColor1, freeColorCalcAlpha);

                                            tempColorPixelCount++;
                                        }
                                    }
                                }
                            }
                        }
					}
				}
			}
		}
		else
		{
			updateInfluenceArray = true;
		}
	}

	if(updateInfluenceArray)
	{
		colorConnectorScript.SortActorArray();
		colorConnectorScript.GetActorArrayUpdate(this);
		updateInfluenceArray = false;
	}
}

//========

void UpdateColorActorsFree()
{
//set color for each input object
					//length is defined in the texturePaintConnector script
	for (n = 0; n < dynamicInputArrayLength; n++)
	{
		if(colorActorDynamicArray[n] != null)
		{
			if(colorActorDynamicArray[n].addColor)
			{
				if(colorActorDynamicArray[n].multiplySizeByScale)
				{
					tempColorSize = colorActorDynamicArray[n].colorSize * (colorActorDynamicArray[n].myTransform.localScale.magnitude * 0.577f);
				}
				else
				{
					tempColorSize = colorActorDynamicArray[n].colorSize;
				}
				
				if((colorActorDynamicArray[n].myTransform.position - myTransformPosition).magnitude < (myBoundsMagnitude * 0.525f) + tempColorSize)
				{
					tempLocation = myTransform.InverseTransformPoint(colorActorDynamicArray[n].myTransform.position);
					//positionX, positionY
					freeColorPositionX = (((tempLocation.x * myTransformLocalScale.x) / myBoundsSize.x) - 0.5f) * -1;
					freeColorPositionY = (((tempLocation.z * myTransformLocalScale.z) / myBoundsSize.z) - 0.5f) * -1;
					tempVector2 = new Vector2(freeColorPositionX, freeColorPositionY);
					//color and alpha
					tempColor1 = colorActorDynamicArray[n].colorValue;
					//color size, fallOff
					freeColorSize = (tempColorSize / myBoundsSize.x) * uvScaleBias;
					freeColorFalloff = colorActorDynamicArray[n].colorFalloff * 150;
					
					freeMinY = (int)((freeColorPositionY - (freeColorSize * 0.975f)) * resolutionHidden);
					freeMinX = (int)((freeColorPositionX - (freeColorSize * 0.975f)) * resolutionHidden);
					
					if(freeMinY < 0)
					{
						freeMinY = 0;
					}
					
					if(freeMinX < 0)
					{
						freeMinX = 0;
					}
					
					freeMaxY = (int)((freeColorPositionY + (freeColorSize * 1.2f)) * resolutionHidden);
					freeMaxX = (int)((freeColorPositionX + (freeColorSize * 1.2f)) * resolutionHidden);

					if(freeMaxY > resolutionHidden)
					{
						freeMaxY = resolutionHidden;
					}
					
					if(freeMaxX > resolutionHidden)
					{
						freeMaxX = resolutionHidden;
					}
			
					tempColorPixelCount = 0;

					for(colorRenderTextureSourceFreeTempSCY = freeMinY; colorRenderTextureSourceFreeTempSCY < freeMaxY; colorRenderTextureSourceFreeTempSCY++)
					{
						//fake clamp method
						tempColorPixelCount = (colorRenderTextureSourceFreeTempSCY * resolutionHidden) + freeMinX;
						
						if(tempColorPixelCount >= 0)
						{
							for(colorRenderTextureSourceFreeTempSCX = freeMinX; colorRenderTextureSourceFreeTempSCX < freeMaxX; colorRenderTextureSourceFreeTempSCX++)
							{
								if(tempColorPixelCount <= resolution2Min1)
								{
									//fake clamp method
									freeTempFloat = (Vector2.Distance(new Vector2(colorRenderTextureSourceFreeTempSCX / (float)resolutionHidden, colorRenderTextureSourceFreeTempSCY / (float)resolutionHidden), tempVector2) - freeColorSize) * -freeColorFalloff;

                                    if(freeTempFloat < 0)
									{
										freeTempFloat = 0;
									}
									else if(freeTempFloat > 1)
									{
										freeTempFloat = 1;
									}
									
									freeColorCalcAlpha = tempColor1.a * freeTempFloat;
									
									colorRenderTextureSourceFreeTempSA[tempColorPixelCount] = Color32.Lerp(colorRenderTextureSourceFreeTempSA[tempColorPixelCount], tempColor1, freeColorCalcAlpha);
								
									tempColorPixelCount++;
								}
							}
						}
					}
				}
			}
		}
		else
		{
			updateInfluenceArray = true;
		}
	}

	if(updateInfluenceArray)
	{
		colorConnectorScript.SortActorArray();
		colorConnectorScript.GetActorArrayUpdate(this);
		updateInfluenceArray = false;
	}
}
	
//========

void UpdateColorFree()
{
	frameDelay++;

	switch(frameDelay)
	{
		case 1:
			myTransformPosition = myTransform.position;
			break;
		case 2:
			myTransformLocalScale = myTransform.localScale;
			break;
		case 3:
			if(isTerrain == false)
			{
				myBoundsSize = myMesh.bounds.size;
				myBoundsSize = new Vector3(myBoundsSize.x * myTransformLocalScale.x, myBoundsSize.y * myTransformLocalScale.y, myBoundsSize.z * myTransformLocalScale.z);
				myBoundsMagnitude = myBoundsSize.magnitude;
			}
			
			frameDelay = 0;	
			break;
	}

	if(use3DPaint)
	{
		Update3DColorActorsFree();
	}
	else
	{
		UpdateColorActorsFree();
	}
	
	UpdateDisplayFree();
}

//========

void UpdateDisplayFree()
{
//Output texture to screen
	colorRenderTextureSourceFree.SetPixels32(colorRenderTextureSourceFreeTempSA);
	colorRenderTextureSourceFree.Apply();
}

//========

void clearColorToValueMatFree(Color tempValue)
{
	tempColorPixelCount = 0;

	for(colorRenderTextureSourceFreeTempSCY = 0; colorRenderTextureSourceFreeTempSCY < resolutionHidden; colorRenderTextureSourceFreeTempSCY++)
	{
		for(colorRenderTextureSourceFreeTempSCX = 0; colorRenderTextureSourceFreeTempSCX < resolutionHidden; colorRenderTextureSourceFreeTempSCX++)
		{
			colorRenderTextureSourceFreeTempSA[tempColorPixelCount] = tempValue;
			
			tempColorPixelCount++;
		}
	}
}

//========

void initializeToTextureMatFree(Texture2D tempTexture)
{
	tempColorPixelCount = 0;
	
	if(tempTexture)
	{
		for(colorRenderTextureSourceFreeTempSCY = 0; colorRenderTextureSourceFreeTempSCY < resolutionHidden; colorRenderTextureSourceFreeTempSCY++)
		{
			for(colorRenderTextureSourceFreeTempSCX = 0; colorRenderTextureSourceFreeTempSCX < resolutionHidden; colorRenderTextureSourceFreeTempSCX++)
			{
				tempColor = tempTexture.GetPixelBilinear(colorRenderTextureSourceFreeTempSCX / resolutionHidden, colorRenderTextureSourceFreeTempSCY / resolutionHidden);

				colorRenderTextureSourceFreeTempSA[tempColorPixelCount] = tempColor;
				
				tempColorPixelCount++;
			}
		}
	}
	
	UpdateDisplayFree();
}

//========

void initializeToValueMatFree(Color tempColor)
{
	tempColorPixelCount = 0;

	for(colorRenderTextureSourceFreeTempSCY = 0; colorRenderTextureSourceFreeTempSCY < resolutionHidden; colorRenderTextureSourceFreeTempSCY++)
	{
		for(colorRenderTextureSourceFreeTempSCX = 0; colorRenderTextureSourceFreeTempSCX < resolutionHidden; colorRenderTextureSourceFreeTempSCX++)
		{
			colorRenderTextureSourceFreeTempSA[tempColorPixelCount] = tempColor;

			tempColorPixelCount++;
		}
	}
	
	UpdateDisplayFree();
}
}