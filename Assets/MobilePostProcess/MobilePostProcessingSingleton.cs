/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using UnityEngine;
using UnityEngine.UI;

public class MobilePostProcessingSingleton : MonoBehaviour{
    public static MobilePostProcessingSingleton Instance { get; set; }
	public Shader Shader;
	public bool Blur=true;
	[Range(2, 3)]
	public int iterations = 2;
	[Range(0, 10)]
	public float BlurAmount = 3f;
	[Range(0, 5)]
	public float BloomAmount = 3f;
	[Range(0, 1)]
	public float BloomThreshold = 0.07f;
	[Range(1, 8)]
	public int filterWidth = 4;
	[Range(1, 8)]
	public int filterHeight = 4;
	public bool LUT=true;
	[Range(0, 1)]
	public float LutAmount = 1.0f;


	static readonly int lutTextureString = Shader.PropertyToID ("_LutTex");
	static readonly int amountString = Shader.PropertyToID ("_Amount");
	static readonly int maskTextureString = Shader.PropertyToID ("_MaskTex");
	static readonly int blAmountString = Shader.PropertyToID ("_BloomAmount");
	static readonly int blThresholdString= Shader.PropertyToID ("_BloomThreshold");
	static readonly int scrResString= Shader.PropertyToID ("_ScrRes");
	static readonly int blurTexString= Shader.PropertyToID ("_BlurTex");

	static readonly int scrWidth=Screen.width;
	static readonly int scrHeight=Screen.height;


	private Shader previousShader;
	private Material material;
	public Texture2D sourceLut = null;
	private Texture2D prevSorceLut;
	private Texture2D converted2DLut = null;
	public Texture2D maskText;
	private int lutSize;
	public void Awake(){
		Instance = this;
	}
	public void Start(){
		if (maskText.Equals (null)) {
			Shader.SetGlobalTexture (maskTextureString, Texture2D.whiteTexture);
		} else Shader.SetGlobalTexture (maskTextureString, maskText);
	}

	private void CreateMaterial()
	{
		if (Shader == null)
		{
			material = null;
			Debug.LogError("Must set a shader to use LUT");
			return;
		}

		material = new Material(Shader);
		material.hideFlags = HideFlags.DontSave;
	}

	private void OnEnable()
	{
		if (GetComponent<Camera>() == null)
		{
			Debug.LogError("This script must be attached to a Camera");
		}
	}

	private void Update()
	{
		if (Shader != previousShader)
		{
			previousShader = Shader;
			CreateMaterial();
		}

		if (sourceLut != prevSorceLut)
		{
			prevSorceLut = sourceLut;
			Convert(sourceLut,"");
		}

	}

	private void OnDestroy()
	{
		if (converted2DLut != null)
		{
			DestroyImmediate(converted2DLut);
		}
		converted2DLut = null;
	}

	public void  SetIdentityLut (){
		int dim = 16;
		Color[] newC = new Color[dim*dim*dim*dim];
		float oneOverDim = 1.0f / (1.0f * dim - 1.0f);

		for(int i = 0; i < dim; i++) {
			for(int j = 0; j < dim; j++) {
				for(int x = 0; x < dim; x++) {
					for(int y = 0; y < dim; y++) 
					{
						newC[x + i * dim + y * dim * dim + j * dim * dim * dim] = 
							new Color(x * oneOverDim, y * oneOverDim, (j * dim + i) / (dim * dim - 1.0f), 1);
					}
				}
			}
		}

		if (converted2DLut)
			DestroyImmediate (converted2DLut);
		converted2DLut = new Texture2D (dim * dim, dim * dim, TextureFormat.ARGB32, false);
		converted2DLut.SetPixels (newC);
		converted2DLut.Apply ();
	}
	public bool ValidDimensions ( Texture2D tex2d  ){
		if (!tex2d) return false;
		int h = tex2d.height;
		if (h != Mathf.FloorToInt(Mathf.Sqrt(tex2d.width))) {
			return false;				
		}
		// we do not support other sizes than 256x16 
		if (h != 16) {
			return false;				
		}
		return true;
	}
	public void  Convert ( Texture2D temp2DTex ,   string path  ){
		if (temp2DTex) {
			int dim = temp2DTex.width * temp2DTex.height;
			dim = temp2DTex.height;
			if (!ValidDimensions(temp2DTex)) {
				Debug.LogWarning ("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.");				
				return;				
			}

			Color[] c = temp2DTex.GetPixels();
			Color[] newC = new Color[dim * dim * dim * dim];

			for(int i = 0; i < dim; i++) {
				for(int j = 0; j < dim; j++) 
				{
					for(int x = 0; x < dim; x++) {
						for(int y = 0; y < dim; y++) 
						{
							float b = (i + j * dim * 1.0f) / dim;
							int bi0 = Mathf.FloorToInt(b);
							int bi1 = Mathf.Min(bi0 + 1, dim - 1);
							float f = b - bi0;
							int index = x + (dim - y - 1) * dim * dim;
							Color col1 = c[index + bi0 * dim];
							Color col2 = c[index + bi1 * dim];

							newC[x + i * dim + y * dim * dim + j * dim * dim * dim] = 
								Color.Lerp(col1, col2, f);
						}
					}
				}
			}

			if (converted2DLut)
				DestroyImmediate (converted2DLut);
			converted2DLut = new Texture2D (dim * dim, dim * dim, TextureFormat.ARGB32, false);
			converted2DLut.SetPixels (newC);
			converted2DLut.Apply ();
		}		
		else {
			Debug.LogError ("Couldn't color correct with 2D LUT texture. Image Effect will be disabled.");
		}		
	}


	void  OnRenderImage ( RenderTexture source ,   RenderTexture destination  ){	
		if (LUT && Blur) {	
			material.SetTexture (lutTextureString, converted2DLut);
			material.SetFloat (amountString, LutAmount);
			material.SetFloat (blThresholdString, BloomThreshold);
			material.SetFloat (blAmountString, BloomAmount);
			material.SetVector(scrResString, new Vector2(BlurAmount /scrWidth, BlurAmount / scrHeight));
			RenderTexture buffer = RenderTexture.GetTemporary(scrWidth / filterWidth, scrHeight / filterHeight, 0);
			Graphics.Blit (source, buffer, material, iterations);
			material.SetTexture (blurTexString, buffer);
			RenderTexture.ReleaseTemporary (buffer);
			Graphics.Blit (source, destination, material, 0);
		} else if (LUT) {
			material.SetTexture (lutTextureString, converted2DLut);
			material.SetFloat (amountString, LutAmount);
			Graphics.Blit (source, destination, material, 1);
		} else if (Blur) {
			material.SetFloat (blAmountString, BloomAmount);
			material.SetFloat (blThresholdString, BloomThreshold);
			material.SetVector (scrResString, new Vector2 (BlurAmount / scrWidth, BlurAmount / scrHeight));
			RenderTexture buffer = RenderTexture.GetTemporary (scrWidth / filterWidth, scrHeight / filterHeight, 0);
			Graphics.Blit (source, buffer, material, iterations);
			material.SetTexture (blurTexString, buffer);
			RenderTexture.ReleaseTemporary (buffer);
			Graphics.Blit (source, destination,material,4);
		}

	}	
	public void Blurr(Slider slider){
		BlurAmount = slider.value;
	}
}
