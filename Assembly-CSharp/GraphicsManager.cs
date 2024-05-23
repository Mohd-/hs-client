using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public class GraphicsManager : MonoBehaviour
{
	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06001E1F RID: 7711 RVA: 0x0008C1A7 File Offset: 0x0008A3A7
	// (set) Token: 0x06001E20 RID: 7712 RVA: 0x0008C1AF File Offset: 0x0008A3AF
	public GraphicsQuality RenderQualityLevel
	{
		get
		{
			return this.m_GraphicsQuality;
		}
		set
		{
			this.m_GraphicsQuality = value;
			Options.Get().SetInt(Option.GFX_QUALITY, (int)this.m_GraphicsQuality);
			this.UpdateQualitySettings();
		}
	}

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06001E21 RID: 7713 RVA: 0x0008C1D0 File Offset: 0x0008A3D0
	public bool RealtimeShadows
	{
		get
		{
			return this.m_RealtimeShadows;
		}
	}

	// Token: 0x06001E22 RID: 7714 RVA: 0x0008C1D8 File Offset: 0x0008A3D8
	private void Awake()
	{
		GraphicsManager.s_instance = this;
		this.m_DisableLowQualityObjects = new List<GameObject>();
		if (!Options.Get().HasOption(Option.GFX_QUALITY))
		{
			string intelDeviceName = W8Touch.Get().GetIntelDeviceName();
			Log.Yim.Print("Intel Device Name = {0}", new object[]
			{
				intelDeviceName
			});
			if (intelDeviceName != null && intelDeviceName.Contains("Haswell") && intelDeviceName.Contains("U28W"))
			{
				if (Screen.currentResolution.height > 1080)
				{
					Options.Get().SetInt(Option.GFX_QUALITY, 0);
				}
			}
			else if (intelDeviceName != null && intelDeviceName.Contains("Crystal-Well"))
			{
				Options.Get().SetInt(Option.GFX_QUALITY, 2);
			}
			else if (intelDeviceName != null && intelDeviceName.Contains("BayTrail"))
			{
				Options.Get().SetInt(Option.GFX_QUALITY, 0);
			}
		}
		this.m_GraphicsQuality = (GraphicsQuality)Options.Get().GetInt(Option.GFX_QUALITY);
		this.InitializeScreen();
		this.UpdateQualitySettings();
		this.m_lastWidth = Screen.width;
		this.m_lastHeight = Screen.height;
	}

	// Token: 0x06001E23 RID: 7715 RVA: 0x0008C2FC File Offset: 0x0008A4FC
	private void OnDestroy()
	{
		if (!Screen.fullScreen)
		{
			Options.Get().SetInt(Option.GFX_WIDTH, Screen.width);
			Options.Get().SetInt(Option.GFX_HEIGHT, Screen.height);
			int[] windowPosition = GraphicsManager.GetWindowPosition();
			Options.Get().SetInt(Option.GFX_WIN_POSX, windowPosition[0]);
			Options.Get().SetInt(Option.GFX_WIN_POSY, windowPosition[1]);
		}
		GraphicsManager.s_instance = null;
	}

	// Token: 0x06001E24 RID: 7716 RVA: 0x0008C360 File Offset: 0x0008A560
	private void Start()
	{
		if (Options.Get().HasOption(Option.GFX_TARGET_FRAME_RATE))
		{
			Application.targetFrameRate = Options.Get().GetInt(Option.GFX_TARGET_FRAME_RATE);
		}
		else
		{
			Application.targetFrameRate = 30;
		}
		this.LogSystemInfo();
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x0008C3A4 File Offset: 0x0008A5A4
	private void Update()
	{
		if (Screen.fullScreen)
		{
			return;
		}
		if (this.m_lastWidth == Screen.width && this.m_lastHeight == Screen.height)
		{
			return;
		}
		if ((float)Screen.width / (float)Screen.height < 1.777777f && (float)Screen.width / (float)Screen.height > 1.333333f)
		{
			return;
		}
		int num = Screen.width;
		int height = Screen.height;
		if ((float)Screen.width / (float)Screen.height > 1.777777f)
		{
			num = (int)((float)Screen.height * 1.777777f);
		}
		else
		{
			num = (int)((float)Screen.height * 1.333333f);
		}
		int[] windowPosition = GraphicsManager.GetWindowPosition();
		int x = windowPosition[0];
		int y = windowPosition[1];
		Screen.SetResolution(num, height, Screen.fullScreen);
		this.m_lastWidth = num;
		this.m_lastHeight = height;
		base.StartCoroutine(this.SetPos(x, y, 0f));
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x0008C48E File Offset: 0x0008A68E
	public static GraphicsManager Get()
	{
		return GraphicsManager.s_instance;
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x0008C498 File Offset: 0x0008A698
	public void SetScreenResolution(int width, int height, bool fullscreen)
	{
		if (height > Screen.currentResolution.height - 60 && !fullscreen)
		{
			height = Screen.currentResolution.height - 60;
		}
		if (width > Screen.currentResolution.width - 20 && !fullscreen)
		{
			width = Screen.currentResolution.width - 20;
		}
		base.StartCoroutine(this.SetRes(width, height, fullscreen));
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x0008C511 File Offset: 0x0008A711
	public void RegisterLowQualityDisableObject(GameObject lowQualityObject)
	{
		if (this.m_DisableLowQualityObjects.Contains(lowQualityObject))
		{
			return;
		}
		this.m_DisableLowQualityObjects.Add(lowQualityObject);
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x0008C531 File Offset: 0x0008A731
	public void DeregisterLowQualityDisableObject(GameObject lowQualityObject)
	{
		if (this.m_DisableLowQualityObjects.Contains(lowQualityObject))
		{
			this.m_DisableLowQualityObjects.Remove(lowQualityObject);
		}
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x0008C551 File Offset: 0x0008A751
	public bool isVeryLowQualityDevice()
	{
		return this.m_VeryLowQuality;
	}

	// Token: 0x06001E2B RID: 7723 RVA: 0x0008C55C File Offset: 0x0008A75C
	private static void _GetLimits(ref GraphicsManager.GPULimits limits)
	{
		limits.highPrecisionBits = 16;
		limits.mediumPrecisionBits = 16;
		limits.lowPrecisionBits = 23;
		limits.maxFragmentTextureUnits = 16;
		limits.maxVertexTextureUnits = 16;
		limits.maxCombinedTextureUnits = 32;
		limits.maxTextureSize = 8192;
		limits.maxCubeMapSize = 8192;
		limits.maxRenderBufferSize = 8192;
		limits.maxFragmentUniforms = 256;
		limits.maxVertexUniforms = 256;
		limits.maxVaryings = 32;
		limits.maxVertexAttribs = 32;
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x0008C5E0 File Offset: 0x0008A7E0
	public GraphicsManager.GPULimits GetGPULimits()
	{
		GraphicsManager.GPULimits result = default(GraphicsManager.GPULimits);
		GraphicsManager._GetLimits(ref result);
		return result;
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x0008C600 File Offset: 0x0008A800
	private void InitializeScreen()
	{
		bool @bool = Options.Get().GetBool(Option.GFX_FULLSCREEN, Screen.fullScreen);
		int num;
		int num2;
		if (@bool)
		{
			num = Options.Get().GetInt(Option.GFX_WIDTH, Screen.currentResolution.width);
			num2 = Options.Get().GetInt(Option.GFX_HEIGHT, Screen.currentResolution.height);
			if (!Options.Get().HasOption(Option.GFX_WIDTH) || !Options.Get().HasOption(Option.GFX_HEIGHT))
			{
				string intelDeviceName = W8Touch.Get().GetIntelDeviceName();
				if (intelDeviceName != null && ((intelDeviceName.Contains("Haswell") && intelDeviceName.Contains("Y6W")) || (intelDeviceName.Contains("Haswell") && intelDeviceName.Contains("U15W"))) && Screen.currentResolution.height >= 1080)
				{
					num = 1920;
					num2 = 1080;
				}
			}
			if (num == Screen.currentResolution.width && num2 == Screen.currentResolution.height && @bool == Screen.fullScreen)
			{
				return;
			}
		}
		else
		{
			if (!Options.Get().HasOption(Option.GFX_WIDTH) || !Options.Get().HasOption(Option.GFX_HEIGHT))
			{
				return;
			}
			num = Options.Get().GetInt(Option.GFX_WIDTH);
			num2 = Options.Get().GetInt(Option.GFX_HEIGHT);
		}
		this.SetScreenResolution(num, num2, @bool);
		if (!@bool)
		{
			if (!Options.Get().HasOption(Option.GFX_WIN_POSX) || !Options.Get().HasOption(Option.GFX_WIN_POSY))
			{
				return;
			}
			int num3 = Options.Get().GetInt(Option.GFX_WIN_POSX);
			int num4 = Options.Get().GetInt(Option.GFX_WIN_POSY);
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			base.StartCoroutine(this.SetPos(num3, num4, 0.6f));
		}
	}

	// Token: 0x06001E2E RID: 7726 RVA: 0x0008C7E4 File Offset: 0x0008A9E4
	private void UpdateQualitySettings()
	{
		Log.Kyle.Print("GraphicsManager Update, Graphics Quality: " + this.m_GraphicsQuality.ToString(), new object[0]);
		this.UpdateRenderQualitySettings();
		this.UpdateAntiAliasing();
	}

	// Token: 0x06001E2F RID: 7727
	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

	// Token: 0x06001E30 RID: 7728
	[DllImport("user32.dll")]
	private static extern IntPtr FindWindow(string className, string windowName);

	// Token: 0x06001E31 RID: 7729 RVA: 0x0008C828 File Offset: 0x0008AA28
	private static bool SetWindowPosition(int x, int y, int resX = 0, int resY = 0)
	{
		IntPtr activeWindow = GraphicsManager.GetActiveWindow();
		IntPtr intPtr = GraphicsManager.FindWindow(null, "Hearthstone");
		if (activeWindow == intPtr)
		{
			GraphicsManager.SetWindowPos(activeWindow, 0, x, y, resX, resY, (resX * resY != 0) ? 0 : 1);
			return true;
		}
		return false;
	}

	// Token: 0x06001E32 RID: 7730
	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	// Token: 0x06001E33 RID: 7731 RVA: 0x0008C871 File Offset: 0x0008AA71
	private static IntPtr GetActiveWindow()
	{
		return GraphicsManager.GetForegroundWindow();
	}

	// Token: 0x06001E34 RID: 7732
	[DllImport("user32.dll")]
	[return: MarshalAs(2)]
	private static extern bool GetWindowRect(IntPtr hWnd, out GraphicsManager.RECT lpRect);

	// Token: 0x06001E35 RID: 7733 RVA: 0x0008C878 File Offset: 0x0008AA78
	private static int[] GetWindowPosition()
	{
		int[] array = new int[2];
		GraphicsManager.RECT rect = default(GraphicsManager.RECT);
		GraphicsManager.GetWindowRect(GraphicsManager.GetActiveWindow(), out rect);
		array[0] = rect.Left;
		array[1] = rect.Top;
		return array;
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x0008C8B8 File Offset: 0x0008AAB8
	private IEnumerator SetRes(int width, int height, bool fullscreen)
	{
		int[] oldPos = GraphicsManager.GetWindowPosition();
		int posX = oldPos[0];
		int posY = oldPos[1];
		CameraFade cameraFade = LoadingScreen.Get().GetCameraFade();
		Camera camera = LoadingScreen.Get().GetFxCamera();
		float prevDepth = camera.depth;
		Color prevColor = cameraFade.m_Color;
		float prevFade = cameraFade.m_Fade;
		bool prevROA = cameraFade.m_RenderOverAll;
		cameraFade.m_Color = Color.black;
		cameraFade.m_Fade = 1f;
		cameraFade.m_RenderOverAll = true;
		yield return null;
		Screen.SetResolution(width, height, fullscreen);
		yield return null;
		Screen.SetResolution(width, height, fullscreen);
		this.m_lastWidth = Screen.width;
		this.m_lastHeight = Screen.height;
		yield return null;
		camera.depth = prevDepth;
		cameraFade.m_Color = prevColor;
		cameraFade.m_Fade = prevFade;
		cameraFade.m_RenderOverAll = prevROA;
		if (posX + width + 20 > Screen.currentResolution.width)
		{
			posX = Screen.currentResolution.width - (width + 20);
		}
		if (posY + height + 60 > Screen.currentResolution.height)
		{
			posY = Screen.currentResolution.height - (height + 60);
		}
		if (posX < 0 || posX > Screen.currentResolution.width)
		{
			posX = 0;
		}
		if (posY + height + 120 > Screen.currentResolution.height)
		{
			posY = 0;
		}
		if (posY < 0 || posY > Screen.currentResolution.height)
		{
			posY = 0;
		}
		base.StartCoroutine(this.SetPos(posX, posY, 0f));
		yield break;
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x0008C900 File Offset: 0x0008AB00
	private IEnumerator SetPos(int x, int y, float delay = 0f)
	{
		yield return new WaitForSeconds(delay);
		this.m_winPosX = x;
		this.m_winPosY = y;
		int[] currentPos = GraphicsManager.GetWindowPosition();
		int[] newPos = new int[]
		{
			this.m_winPosX,
			this.m_winPosY
		};
		float startTime = Time.time;
		while (currentPos != newPos && Time.time < startTime + 1f)
		{
			newPos[0] = this.m_winPosX;
			newPos[1] = this.m_winPosY;
			if (!GraphicsManager.SetWindowPosition(this.m_winPosX, this.m_winPosY, 0, 0))
			{
				break;
			}
			currentPos = GraphicsManager.GetWindowPosition();
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001E38 RID: 7736 RVA: 0x0008C948 File Offset: 0x0008AB48
	private void UpdateAntiAliasing()
	{
		bool flag = false;
		int num = 0;
		if (this.m_GraphicsQuality == GraphicsQuality.Low)
		{
			num = 0;
			flag = false;
		}
		if (this.m_GraphicsQuality == GraphicsQuality.Medium)
		{
			num = 2;
			flag = false;
			if (W8Touch.Get() != null)
			{
				string intelDeviceName = W8Touch.Get().GetIntelDeviceName();
				if (intelDeviceName != null && (intelDeviceName.Equals("BayTrail") || intelDeviceName.Equals("Poulsbo") || intelDeviceName.Equals("CloverTrail") || (intelDeviceName.Contains("Haswell") && intelDeviceName.Contains("Y6W"))))
				{
					num = 0;
				}
			}
		}
		if (this.m_GraphicsQuality == GraphicsQuality.High)
		{
			switch (Localization.GetLocale())
			{
			case Locale.koKR:
			case Locale.ruRU:
			case Locale.zhTW:
			case Locale.zhCN:
			case Locale.plPL:
			case Locale.jaJP:
			case Locale.thTH:
				num = 2;
				flag = false;
				goto IL_F9;
			}
			num = 0;
			flag = true;
		}
		IL_F9:
		if (Options.Get().HasOption(Option.GFX_MSAA))
		{
			num = Options.Get().GetInt(Option.GFX_MSAA);
		}
		if (Options.Get().HasOption(Option.GFX_FXAA))
		{
			flag = Options.Get().GetBool(Option.GFX_FXAA);
		}
		if (flag)
		{
			num = 0;
		}
		if (num > 0)
		{
			flag = false;
		}
		QualitySettings.antiAliasing = num;
		FullScreenAntialiasing[] array = Object.FindObjectsOfType(typeof(FullScreenAntialiasing)) as FullScreenAntialiasing[];
		foreach (FullScreenAntialiasing fullScreenAntialiasing in array)
		{
			fullScreenAntialiasing.enabled = flag;
		}
	}

	// Token: 0x06001E39 RID: 7737 RVA: 0x0008CAE4 File Offset: 0x0008ACE4
	private void UpdateRenderQualitySettings()
	{
		int num = 30;
		int vSyncCount = 0;
		int num2 = 101;
		if (this.m_GraphicsQuality == GraphicsQuality.Low)
		{
			num = 30;
			vSyncCount = 0;
			this.m_RealtimeShadows = false;
			this.SetQualityByName("Low");
			num2 = 101;
		}
		if (this.m_GraphicsQuality == GraphicsQuality.Medium)
		{
			num = 30;
			vSyncCount = 0;
			this.m_RealtimeShadows = false;
			this.SetQualityByName("Medium");
			num2 = 201;
		}
		if (this.m_GraphicsQuality == GraphicsQuality.High)
		{
			num = 60;
			vSyncCount = 1;
			this.m_RealtimeShadows = true;
			this.SetQualityByName("High");
			num2 = 301;
		}
		Shader.DisableKeyword("LOW_QUALITY");
		if (Options.Get().HasOption(Option.GFX_TARGET_FRAME_RATE))
		{
			Application.targetFrameRate = Options.Get().GetInt(Option.GFX_TARGET_FRAME_RATE);
		}
		else
		{
			if (W8Touch.Get() != null && W8Touch.Get().GetBatteryMode() == W8Touch.PowerSource.BatteryPower && num > 30)
			{
				Log.Yim.Print("Battery Mode Detected - Clamping Target Frame Rate from {0} to 30", new object[]
				{
					num
				});
				num = 30;
				vSyncCount = 0;
			}
			Application.targetFrameRate = num;
		}
		if (Options.Get().HasOption(Option.GFX_VSYNC))
		{
			QualitySettings.vSyncCount = Options.Get().GetInt(Option.GFX_VSYNC);
		}
		else
		{
			QualitySettings.vSyncCount = vSyncCount;
		}
		Log.Kyle.Print(string.Format("Target frame rate: {0}", Application.targetFrameRate), new object[0]);
		ProjectedShadow[] array = Object.FindObjectsOfType(typeof(ProjectedShadow)) as ProjectedShadow[];
		foreach (ProjectedShadow projectedShadow in array)
		{
			projectedShadow.enabled = !this.m_RealtimeShadows;
		}
		RenderToTexture[] array3 = Object.FindObjectsOfType(typeof(RenderToTexture)) as RenderToTexture[];
		foreach (RenderToTexture renderToTexture in array3)
		{
			renderToTexture.ForceTextureRebuild();
		}
		Shader[] array5 = Object.FindObjectsOfType(typeof(Shader)) as Shader[];
		foreach (Shader shader in array5)
		{
			shader.maximumLOD = num2;
		}
		foreach (GameObject gameObject in this.m_DisableLowQualityObjects)
		{
			if (!(gameObject == null))
			{
				if (this.m_GraphicsQuality == GraphicsQuality.Low)
				{
					Log.Kyle.Print(string.Format("Low Quality Disable: {0}", gameObject.name), new object[0]);
					gameObject.SetActive(false);
				}
				else
				{
					Log.Kyle.Print(string.Format("Low Quality Enable: {0}", gameObject.name), new object[0]);
					gameObject.SetActive(true);
				}
			}
		}
		Shader.globalMaximumLOD = num2;
		this.SetScreenEffects();
	}

	// Token: 0x06001E3A RID: 7738 RVA: 0x0008CDD8 File Offset: 0x0008AFD8
	private void SetScreenEffects()
	{
		if (ScreenEffectsMgr.Get() != null)
		{
			if (this.m_GraphicsQuality == GraphicsQuality.Low)
			{
				ScreenEffectsMgr.Get().gameObject.SetActive(false);
			}
			else
			{
				ScreenEffectsMgr.Get().gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06001E3B RID: 7739 RVA: 0x0008CE28 File Offset: 0x0008B028
	private void SetQualityByName(string qualityName)
	{
		string[] names = QualitySettings.names;
		int num = -1;
		int i;
		for (i = 0; i < names.Length; i++)
		{
			if (names[i] == qualityName)
			{
				num = i;
			}
		}
		if (i < 0)
		{
			Debug.LogError(string.Format("GraphicsManager: Quality Level not found: {0}", qualityName));
			return;
		}
		QualitySettings.SetQualityLevel(num, true);
	}

	// Token: 0x06001E3C RID: 7740 RVA: 0x0008CE80 File Offset: 0x0008B080
	private void LogSystemInfo()
	{
		Debug.Log("System Info:");
		Debug.Log(string.Format("SystemInfo - Device Name: {0}", SystemInfo.deviceName));
		Debug.Log(string.Format("SystemInfo - Device Model: {0}", SystemInfo.deviceModel));
		Debug.Log(string.Format("SystemInfo - OS: {0}", SystemInfo.operatingSystem));
		Debug.Log(string.Format("SystemInfo - CPU Type: {0}", SystemInfo.processorType));
		Debug.Log(string.Format("SystemInfo - CPU Cores: {0}", SystemInfo.processorCount));
		Debug.Log(string.Format("SystemInfo - System Memory: {0}", SystemInfo.systemMemorySize));
		Debug.Log(string.Format("SystemInfo - Screen Resolution: {0}x{1}", Screen.currentResolution.width, Screen.currentResolution.height));
		Debug.Log(string.Format("SystemInfo - Screen DPI: {0}", Screen.dpi));
		Debug.Log(string.Format("SystemInfo - GPU ID: {0}", SystemInfo.graphicsDeviceID));
		Debug.Log(string.Format("SystemInfo - GPU Name: {0}", SystemInfo.graphicsDeviceName));
		Debug.Log(string.Format("SystemInfo - GPU Vendor: {0}", SystemInfo.graphicsDeviceVendor));
		Debug.Log(string.Format("SystemInfo - GPU Memory: {0}", SystemInfo.graphicsMemorySize));
		Debug.Log(string.Format("SystemInfo - GPU Shader Level: {0}", SystemInfo.graphicsShaderLevel));
		Debug.Log(string.Format("SystemInfo - GPU NPOT Support: {0}", SystemInfo.npotSupport));
		Debug.Log(string.Format("SystemInfo - Graphics API (version): {0}", SystemInfo.graphicsDeviceVersion));
		Debug.Log(string.Format("SystemInfo - Graphics API (type): {0}", SystemInfo.graphicsDeviceType));
		Debug.Log(string.Format("SystemInfo - Graphics Supported Render Target Count: {0}", SystemInfo.supportedRenderTargetCount));
		Debug.Log(string.Format("SystemInfo - Graphics Supports 3D Textures: {0}", SystemInfo.supports3DTextures));
		Debug.Log(string.Format("SystemInfo - Graphics Supports Compute Shaders: {0}", SystemInfo.supportsComputeShaders));
		Debug.Log(string.Format("SystemInfo - Graphics Supports Image Effects: {0}", SystemInfo.supportsImageEffects));
		Debug.Log(string.Format("SystemInfo - Graphics Supports Render Textures: {0}", SystemInfo.supportsRenderTextures));
		Debug.Log(string.Format("SystemInfo - Graphics Supports Render To Cubemap: {0}", SystemInfo.supportsRenderToCubemap));
		Debug.Log(string.Format("SystemInfo - Graphics Supports Shadows: {0}", SystemInfo.supportsShadows));
		Debug.Log(string.Format("SystemInfo - Graphics Supports Sparse Textures: {0}", SystemInfo.supportsSparseTextures));
		Debug.Log(string.Format("SystemInfo - Graphics Supports Stencil: {0}", SystemInfo.supportsStencil));
		Debug.Log(string.Format("SystemInfo - Graphics RenderTextureFormat.ARGBHalf: {0}", SystemInfo.SupportsRenderTextureFormat(2)));
		Debug.Log(string.Format("SystemInfo - Graphics Metal Support: {0}", SystemInfo.graphicsDeviceVersion.StartsWith("Metal")));
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x0008D13C File Offset: 0x0008B33C
	private void LogGPULimits()
	{
		GraphicsManager.GPULimits gpulimits = this.GetGPULimits();
		Debug.Log("GPU Limits:");
		Debug.Log(string.Format("GPU - Fragment High Precision: {0}", gpulimits.highPrecisionBits));
		Debug.Log(string.Format("GPU - Fragment Medium Precision: {0}", gpulimits.mediumPrecisionBits));
		Debug.Log(string.Format("GPU - Fragment Low Precision: {0}", gpulimits.lowPrecisionBits));
		Debug.Log(string.Format("GPU - Fragment Max Texture Units: {0}", gpulimits.maxFragmentTextureUnits));
		Debug.Log(string.Format("GPU - Vertex Max Texture Units: {0}", gpulimits.maxVertexTextureUnits));
		Debug.Log(string.Format("GPU - Combined Max Texture Units: {0}", gpulimits.maxCombinedTextureUnits));
		Debug.Log(string.Format("GPU - Max Texture Size: {0}", gpulimits.maxTextureSize));
		Debug.Log(string.Format("GPU - Max Cube-Map Texture Size: {0}", gpulimits.maxCubeMapSize));
		Debug.Log(string.Format("GPU - Max Renderbuffer Size: {0}", gpulimits.maxRenderBufferSize));
		Debug.Log(string.Format("GPU - Fragment Max Uniform Vectors: {0}", gpulimits.maxFragmentUniforms));
		Debug.Log(string.Format("GPU - Vertex Max Uniform Vectors: {0}", gpulimits.maxVertexUniforms));
		Debug.Log(string.Format("GPU - Max Varying Vectors: {0}", gpulimits.maxVaryings));
		Debug.Log(string.Format("GPU - Vertex Max Attribs: {0}", gpulimits.maxVertexAttribs));
	}

	// Token: 0x040010C8 RID: 4296
	private const int ANDROID_MIN_DPI_HIGH_RES_TEXTURES = 180;

	// Token: 0x040010C9 RID: 4297
	private const int REDUCE_MAX_WINDOW_SIZE_X = 20;

	// Token: 0x040010CA RID: 4298
	private const int REDUCE_MAX_WINDOW_SIZE_Y = 60;

	// Token: 0x040010CB RID: 4299
	private GraphicsQuality m_GraphicsQuality;

	// Token: 0x040010CC RID: 4300
	private bool m_RealtimeShadows;

	// Token: 0x040010CD RID: 4301
	private List<GameObject> m_DisableLowQualityObjects;

	// Token: 0x040010CE RID: 4302
	private bool m_VeryLowQuality;

	// Token: 0x040010CF RID: 4303
	private static GraphicsManager s_instance;

	// Token: 0x040010D0 RID: 4304
	private int m_lastWidth;

	// Token: 0x040010D1 RID: 4305
	private int m_lastHeight;

	// Token: 0x040010D2 RID: 4306
	private int m_winPosX;

	// Token: 0x040010D3 RID: 4307
	private int m_winPosY;

	// Token: 0x02000797 RID: 1943
	public struct GPULimits
	{
		// Token: 0x040033B2 RID: 13234
		public int highPrecisionBits;

		// Token: 0x040033B3 RID: 13235
		public int mediumPrecisionBits;

		// Token: 0x040033B4 RID: 13236
		public int lowPrecisionBits;

		// Token: 0x040033B5 RID: 13237
		public int maxFragmentTextureUnits;

		// Token: 0x040033B6 RID: 13238
		public int maxVertexTextureUnits;

		// Token: 0x040033B7 RID: 13239
		public int maxCombinedTextureUnits;

		// Token: 0x040033B8 RID: 13240
		public int maxTextureSize;

		// Token: 0x040033B9 RID: 13241
		public int maxCubeMapSize;

		// Token: 0x040033BA RID: 13242
		public int maxRenderBufferSize;

		// Token: 0x040033BB RID: 13243
		public int maxFragmentUniforms;

		// Token: 0x040033BC RID: 13244
		public int maxVertexUniforms;

		// Token: 0x040033BD RID: 13245
		public int maxVaryings;

		// Token: 0x040033BE RID: 13246
		public int maxVertexAttribs;
	}

	// Token: 0x02000798 RID: 1944
	private struct RECT
	{
		// Token: 0x040033BF RID: 13247
		public int Left;

		// Token: 0x040033C0 RID: 13248
		public int Top;

		// Token: 0x040033C1 RID: 13249
		public int Right;

		// Token: 0x040033C2 RID: 13250
		public int Bottom;
	}
}
