using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public class UniversalInputManager : MonoBehaviour
{
	// Token: 0x0600079B RID: 1947 RVA: 0x0001DE9E File Offset: 0x0001C09E
	private void Update()
	{
		this.UpdateMouseOnOrOffScreen();
		this.UpdateInput();
		this.CleanDeadCameras();
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x0001DEB4 File Offset: 0x0001C0B4
	private void Awake()
	{
		UniversalInputManager.s_instance = this;
		this.CreateHitTestPriorityMap();
		this.m_FullscreenEffectsCamera = CameraUtils.FindFullScreenEffectsCamera(true);
		if (this.m_FullscreenEffectsCamera != null)
		{
			this.m_FullscreenEffectsCameraActive = true;
		}
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x0001DEF1 File Offset: 0x0001C0F1
	private void Start()
	{
		this.m_mouseOnScreen = InputUtil.IsMouseOnScreen();
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x0001DEFE File Offset: 0x0001C0FE
	private void OnDestroy()
	{
		UniversalInputManager.s_instance = null;
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x0001DF06 File Offset: 0x0001C106
	private void OnGUI()
	{
		this.IgnoreGUIInput();
		this.HandleGUIInputInactive();
		this.HandleGUIInputActive();
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0001DF1B File Offset: 0x0001C11B
	public static UniversalInputManager Get()
	{
		return UniversalInputManager.s_instance;
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x0001DF24 File Offset: 0x0001C124
	public void SetGUISkin(GUISkinContainer skinContainer)
	{
		if (this.m_skinContainer != null)
		{
			Object.Destroy(this.m_skinContainer.gameObject);
		}
		this.m_skinContainer = skinContainer;
		this.m_skinContainer.transform.parent = base.transform;
		this.m_skin = skinContainer.GetGUISkin();
		this.m_defaultInputAlignment = this.m_skin.textField.alignment;
		this.m_defaultInputFont = this.m_skin.textField.font;
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0001DFA7 File Offset: 0x0001C1A7
	public bool IsTouchMode()
	{
		return UniversalInputManager.IsTouchDevice || Options.Get().GetBool(Option.TOUCH_MODE);
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0001DFC8 File Offset: 0x0001C1C8
	public bool WasTouchCanceled()
	{
		if (!UniversalInputManager.IsTouchDevice)
		{
			return false;
		}
		foreach (Touch touch in Input.touches)
		{
			if (touch.phase == 4)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0001E01D File Offset: 0x0001C21D
	public bool IsMouseOnScreen()
	{
		return this.m_mouseOnScreen;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0001E028 File Offset: 0x0001C228
	public bool RegisterMouseOnOrOffScreenListener(UniversalInputManager.MouseOnOrOffScreenCallback listener)
	{
		if (this.m_mouseOnOrOffScreenListeners.Contains(listener))
		{
			return false;
		}
		this.m_mouseOnOrOffScreenListeners.Add(listener);
		return true;
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0001E055 File Offset: 0x0001C255
	public bool UnregisterMouseOnOrOffScreenListener(UniversalInputManager.MouseOnOrOffScreenCallback listener)
	{
		return this.m_mouseOnOrOffScreenListeners.Remove(listener);
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0001E063 File Offset: 0x0001C263
	public void SetGameDialogActive(bool active)
	{
		this.m_gameDialogActive = active;
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0001E06C File Offset: 0x0001C26C
	public void SetSystemDialogActive(bool active)
	{
		this.m_systemDialogActive = active;
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0001E078 File Offset: 0x0001C278
	public void UseTextInput(UniversalInputManager.TextInputParams parms, bool force = false)
	{
		if (!force && parms.m_owner == this.m_inputOwner)
		{
			return;
		}
		if (this.m_inputOwner != null && this.m_inputOwner != parms.m_owner)
		{
			this.ObjectCancelTextInput(parms.m_owner);
		}
		this.m_inputOwner = parms.m_owner;
		this.m_inputUpdatedCallback = parms.m_updatedCallback;
		this.m_inputPreprocessCallback = parms.m_preprocessCallback;
		this.m_inputCompletedCallback = parms.m_completedCallback;
		this.m_inputCanceledCallback = parms.m_canceledCallback;
		this.m_inputPassword = parms.m_password;
		this.m_inputNumber = parms.m_number;
		this.m_inputMultiLine = parms.m_multiLine;
		this.m_inputActive = true;
		this.m_inputFocused = false;
		this.m_inputText = (parms.m_text ?? string.Empty);
		this.m_inputNormalizedRect = parms.m_rect;
		this.m_inputInitialScreenSize.x = (float)Screen.width;
		this.m_inputInitialScreenSize.y = (float)Screen.height;
		this.m_inputMaxCharacters = parms.m_maxCharacters;
		this.m_inputColor = parms.m_color;
		TextAnchor? alignment = parms.m_alignment;
		this.m_inputAlignment = ((alignment == null) ? this.m_defaultInputAlignment : alignment.Value);
		this.m_inputFont = (parms.m_font ?? this.m_defaultInputFont);
		this.m_inputNeedsFocus = true;
		this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.INVALID;
		this.m_inputKeepFocusOnComplete = parms.m_inputKeepFocusOnComplete;
		if (this.IsTextInputPassword())
		{
			Input.imeCompositionMode = 2;
		}
		this.m_hideVirtualKeyboardOnComplete = parms.m_hideVirtualKeyboardOnComplete;
		if (UniversalInputManager.Get().IsTouchMode() && parms.m_showVirtualKeyboard)
		{
			W8Touch.Get().ShowKeyboard();
		}
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x0001E240 File Offset: 0x0001C440
	public void CancelTextInput(GameObject requester, bool force = false)
	{
		if (!this.IsTextInputActive())
		{
			return;
		}
		if (!force && requester != this.m_inputOwner)
		{
			return;
		}
		this.ObjectCancelTextInput(requester);
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0001E270 File Offset: 0x0001C470
	public void FocusTextInput(GameObject owner)
	{
		if (owner != this.m_inputOwner)
		{
			return;
		}
		if (this.m_tabKeyDown)
		{
			this.m_inputNeedsFocusFromTabKeyDown = true;
		}
		else
		{
			this.m_inputNeedsFocus = true;
		}
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x0001E2AD File Offset: 0x0001C4AD
	public void UpdateTextInputRect(GameObject owner, Rect rect)
	{
		if (owner != this.m_inputOwner)
		{
			return;
		}
		this.m_inputNormalizedRect = rect;
		this.m_inputInitialScreenSize.x = (float)Screen.width;
		this.m_inputInitialScreenSize.y = (float)Screen.height;
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0001E2EA File Offset: 0x0001C4EA
	public bool IsTextInputPassword()
	{
		return this.m_inputPassword;
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0001E2F2 File Offset: 0x0001C4F2
	public bool IsTextInputActive()
	{
		return this.m_inputActive;
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0001E2FA File Offset: 0x0001C4FA
	public string GetInputText()
	{
		return this.m_inputText;
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0001E302 File Offset: 0x0001C502
	public void SetInputText(string text)
	{
		this.m_inputText = (text ?? string.Empty);
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0001E318 File Offset: 0x0001C518
	public bool InputIsOver(GameObject gameObj)
	{
		RaycastHit raycastHit;
		return this.InputIsOver(gameObj, out raycastHit);
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0001E330 File Offset: 0x0001C530
	public bool InputIsOver(GameObject gameObj, out RaycastHit hitInfo)
	{
		LayerMask mask = ((GameLayer)gameObj.layer).LayerBit();
		Camera camera;
		return this.Raycast(null, mask, out camera, out hitInfo, false) && hitInfo.collider.gameObject == gameObj;
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x0001E374 File Offset: 0x0001C574
	public bool InputIsOver(GameObject gameObj, int layerMask, out RaycastHit hitInfo)
	{
		Camera camera;
		return this.Raycast(null, layerMask, out camera, out hitInfo, false) && hitInfo.collider.gameObject == gameObj;
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0001E3AC File Offset: 0x0001C5AC
	public bool InputIsOver(Camera camera, GameObject gameObj)
	{
		RaycastHit raycastHit;
		return this.InputIsOver(camera, gameObj, out raycastHit);
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0001E3C4 File Offset: 0x0001C5C4
	public bool InputIsOver(Camera camera, GameObject gameObj, out RaycastHit hitInfo)
	{
		LayerMask mask = ((GameLayer)gameObj.layer).LayerBit();
		Camera camera2;
		return this.Raycast(camera, mask, out camera2, out hitInfo, false) && hitInfo.collider.gameObject == gameObj;
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x0001E408 File Offset: 0x0001C608
	public bool ForcedInputIsOver(Camera camera, GameObject gameObj)
	{
		RaycastHit raycastHit;
		return this.ForcedInputIsOver(camera, gameObj, out raycastHit);
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x0001E420 File Offset: 0x0001C620
	public bool ForcedInputIsOver(Camera camera, GameObject gameObj, out RaycastHit hitInfo)
	{
		LayerMask layerMask = ((GameLayer)gameObj.layer).LayerBit();
		return CameraUtils.Raycast(camera, this.GetMousePosition(), layerMask, out hitInfo) && hitInfo.collider.gameObject == gameObj;
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x0001E464 File Offset: 0x0001C664
	public bool InputHitAnyObject(GameLayer layer)
	{
		RaycastHit raycastHit;
		return this.GetInputHitInfo(layer, out raycastHit);
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0001E47C File Offset: 0x0001C67C
	public bool InputHitAnyObject(LayerMask layerMask)
	{
		RaycastHit raycastHit;
		return this.GetInputHitInfo(layerMask, out raycastHit);
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x0001E494 File Offset: 0x0001C694
	public bool InputHitAnyObject(Camera requestedCamera)
	{
		RaycastHit raycastHit;
		if (requestedCamera == null)
		{
			return this.GetInputHitInfo(out raycastHit);
		}
		return this.GetInputHitInfo(requestedCamera, requestedCamera.cullingMask, out raycastHit);
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0001E4CC File Offset: 0x0001C6CC
	public bool InputHitAnyObject(Camera requestedCamera, GameLayer layer)
	{
		RaycastHit raycastHit;
		return this.GetInputHitInfo(requestedCamera, layer, out raycastHit);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0001E4E4 File Offset: 0x0001C6E4
	public bool InputHitAnyObject(Camera requestedCamera, LayerMask mask)
	{
		RaycastHit raycastHit;
		return this.GetInputHitInfo(requestedCamera, mask, out raycastHit);
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0001E4FB File Offset: 0x0001C6FB
	public bool GetInputHitInfo(out RaycastHit hitInfo)
	{
		return this.GetInputHitInfo(GameLayer.Default, out hitInfo);
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0001E505 File Offset: 0x0001C705
	public bool GetInputHitInfo(GameLayer layer, out RaycastHit hitInfo)
	{
		return this.GetInputHitInfo(layer.LayerBit(), out hitInfo);
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x0001E51C File Offset: 0x0001C71C
	public bool GetInputHitInfo(LayerMask mask, out RaycastHit hitInfo)
	{
		Camera requestedCamera = this.GuessBestHitTestCamera(mask);
		return this.GetInputHitInfo(requestedCamera, mask, out hitInfo);
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x0001E53C File Offset: 0x0001C73C
	public bool GetInputHitInfo(Camera requestedCamera, out RaycastHit hitInfo)
	{
		if (requestedCamera == null)
		{
			return this.GetInputHitInfo(out hitInfo);
		}
		return this.GetInputHitInfo(requestedCamera, requestedCamera.cullingMask, out hitInfo);
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x0001E570 File Offset: 0x0001C770
	public bool GetInputHitInfo(Camera requestedCamera, GameLayer layer, out RaycastHit hitInfo)
	{
		Camera camera;
		return this.Raycast(requestedCamera, layer.LayerBit(), out camera, out hitInfo, false);
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0001E594 File Offset: 0x0001C794
	public bool GetInputHitInfo(Camera requestedCamera, LayerMask mask, out RaycastHit hitInfo)
	{
		Camera camera;
		return this.Raycast(requestedCamera, mask, out camera, out hitInfo, false);
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0001E5AD File Offset: 0x0001C7AD
	public bool GetInputPointOnPlane(Vector3 origin, out Vector3 point)
	{
		return this.GetInputPointOnPlane(GameLayer.Default, origin, out point);
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0001E5B8 File Offset: 0x0001C7B8
	public bool GetInputPointOnPlane(GameLayer layer, Vector3 origin, out Vector3 point)
	{
		point = Vector3.zero;
		LayerMask mask = layer.LayerBit();
		Camera camera;
		RaycastHit raycastHit;
		if (!this.Raycast(null, mask, out camera, out raycastHit, false))
		{
			return false;
		}
		Ray ray = camera.ScreenPointToRay(this.GetMousePosition());
		Vector3 vector = -camera.transform.forward;
		Plane plane;
		plane..ctor(vector, origin);
		float num;
		if (!plane.Raycast(ray, ref num))
		{
			return false;
		}
		point = ray.GetPoint(num);
		return true;
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0001E63A File Offset: 0x0001C83A
	public bool CanHitTestOffCamera(GameLayer layer)
	{
		return this.CanHitTestOffCamera(layer.LayerBit());
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x0001E64D File Offset: 0x0001C84D
	public bool CanHitTestOffCamera(LayerMask layerMask)
	{
		return (this.m_offCameraHitTestMask & layerMask) != 0;
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x0001E662 File Offset: 0x0001C862
	public void EnableHitTestOffCamera(GameLayer layer, bool enable)
	{
		this.EnableHitTestOffCamera(layer.LayerBit(), enable);
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x0001E678 File Offset: 0x0001C878
	public void EnableHitTestOffCamera(LayerMask mask, bool enable)
	{
		if (enable)
		{
			this.m_offCameraHitTestMask |= mask;
		}
		else
		{
			this.m_offCameraHitTestMask &= ~mask;
		}
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x0001E6B7 File Offset: 0x0001C8B7
	public void SetFullScreenEffectsCamera(Camera camera, bool active)
	{
		this.m_FullscreenEffectsCamera = camera;
		this.m_FullscreenEffectsCameraActive = false;
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0001E6C8 File Offset: 0x0001C8C8
	public bool GetMouseButton(int button)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			return W8Touch.Get().GetTouch(button);
		}
		return Input.GetMouseButton(button);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0001E6F8 File Offset: 0x0001C8F8
	public bool GetMouseButtonDown(int button)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			return W8Touch.Get().GetTouchDown(button);
		}
		return Input.GetMouseButtonDown(button);
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0001E728 File Offset: 0x0001C928
	public bool GetMouseButtonUp(int button)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			return W8Touch.Get().GetTouchUp(button);
		}
		return Input.GetMouseButtonUp(button);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0001E756 File Offset: 0x0001C956
	public Vector3 GetMousePosition()
	{
		if (this.IsTouchMode())
		{
			return W8Touch.Get().GetTouchPosition();
		}
		return Input.mousePosition;
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0001E774 File Offset: 0x0001C974
	public bool AddCameraMaskCamera(Camera camera)
	{
		if (this.m_CameraMaskCameras.Contains(camera))
		{
			return false;
		}
		this.m_CameraMaskCameras.Add(camera);
		return true;
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0001E7A1 File Offset: 0x0001C9A1
	public bool RemoveCameraMaskCamera(Camera camera)
	{
		return this.m_CameraMaskCameras.Remove(camera);
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0001E7B0 File Offset: 0x0001C9B0
	public bool AddIgnoredCamera(Camera camera)
	{
		if (this.m_ignoredCameras.Contains(camera))
		{
			return false;
		}
		this.m_ignoredCameras.Add(camera);
		return true;
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0001E7DD File Offset: 0x0001C9DD
	public bool RemoveIgnoredCamera(Camera camera)
	{
		return this.m_ignoredCameras.Remove(camera);
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0001E7EC File Offset: 0x0001C9EC
	private void CreateHitTestPriorityMap()
	{
		this.m_hitTestPriorityMap = new Map<GameLayer, int>();
		int num = 1;
		for (int i = 0; i < UniversalInputManager.HIT_TEST_PRIORITY_ORDER.Length; i++)
		{
			GameLayer key = UniversalInputManager.HIT_TEST_PRIORITY_ORDER[i];
			this.m_hitTestPriorityMap.Add(key, num++);
		}
		foreach (object obj in Enum.GetValues(typeof(GameLayer)))
		{
			GameLayer key2 = (GameLayer)((int)obj);
			if (!this.m_hitTestPriorityMap.ContainsKey(key2))
			{
				this.m_hitTestPriorityMap.Add(key2, 0);
			}
		}
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0001E8BC File Offset: 0x0001CABC
	private void CleanDeadCameras()
	{
		GeneralUtils.CleanDeadObjectsFromList<Camera>(this.m_CameraMaskCameras);
		GeneralUtils.CleanDeadObjectsFromList<Camera>(this.m_ignoredCameras);
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0001E8D4 File Offset: 0x0001CAD4
	private Camera GuessBestHitTestCamera(LayerMask mask)
	{
		foreach (Camera camera in Camera.allCameras)
		{
			if (!this.m_ignoredCameras.Contains(camera))
			{
				if ((camera.cullingMask & mask) != 0)
				{
					return camera;
				}
			}
		}
		return null;
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0001E930 File Offset: 0x0001CB30
	private bool Raycast(Camera requestedCamera, LayerMask mask, out Camera camera, out RaycastHit hitInfo, bool ignorePriority = false)
	{
		hitInfo = default(RaycastHit);
		if (!ignorePriority)
		{
			foreach (Camera camera2 in this.m_CameraMaskCameras)
			{
				camera = camera2;
				LayerMask mask2 = GameLayer.CameraMask.LayerBit();
				if (this.RaycastWithPriority(camera2, mask2, out hitInfo))
				{
					return true;
				}
			}
			camera = this.m_FullscreenEffectsCamera;
			if (!(camera != null))
			{
				goto IL_A0;
			}
			LayerMask mask3 = GameLayer.IgnoreFullScreenEffects.LayerBit();
			if (this.RaycastWithPriority(camera, mask3, out hitInfo))
			{
				return true;
			}
		}
		IL_A0:
		camera = requestedCamera;
		if (camera != null)
		{
			return this.RaycastWithPriority(camera, mask, out hitInfo);
		}
		camera = Camera.main;
		return this.RaycastWithPriority(camera, mask, out hitInfo);
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0001EA20 File Offset: 0x0001CC20
	private bool RaycastWithPriority(Camera camera, LayerMask mask, out RaycastHit hitInfo)
	{
		hitInfo = default(RaycastHit);
		if (camera == null)
		{
			return false;
		}
		if (!this.FilteredRaycast(camera, this.GetMousePosition(), mask, out hitInfo))
		{
			return false;
		}
		GameLayer layer = (GameLayer)hitInfo.collider.gameObject.layer;
		return !this.HigherPriorityCollisionExists(layer);
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0001EA78 File Offset: 0x0001CC78
	private bool FilteredRaycast(Camera camera, Vector3 screenPoint, LayerMask mask, out RaycastHit hitInfo)
	{
		if (this.CanHitTestOffCamera(mask))
		{
			Ray ray = camera.ScreenPointToRay(screenPoint);
			if (!Physics.Raycast(ray, ref hitInfo, camera.farClipPlane, mask))
			{
				return false;
			}
		}
		else if (!CameraUtils.Raycast(camera, screenPoint, mask, out hitInfo))
		{
			return false;
		}
		return true;
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0001EACC File Offset: 0x0001CCCC
	private bool HigherPriorityCollisionExists(GameLayer layer)
	{
		if (this.m_systemDialogActive && this.m_hitTestPriorityMap[layer] < this.m_hitTestPriorityMap[GameLayer.UI])
		{
			return true;
		}
		if (this.m_gameDialogActive && this.m_hitTestPriorityMap[layer] < this.m_hitTestPriorityMap[GameLayer.IgnoreFullScreenEffects])
		{
			return true;
		}
		if (this.m_FullscreenEffectsCameraActive && this.m_hitTestPriorityMap[layer] < this.m_hitTestPriorityMap[GameLayer.IgnoreFullScreenEffects])
		{
			return true;
		}
		LayerMask higherPriorityLayerMask = this.GetHigherPriorityLayerMask(layer);
		foreach (Camera camera in Camera.allCameras)
		{
			if (!this.m_ignoredCameras.Contains(camera))
			{
				if ((camera.cullingMask & higherPriorityLayerMask) != 0)
				{
					RaycastHit raycastHit;
					if (this.FilteredRaycast(camera, this.GetMousePosition(), higherPriorityLayerMask, out raycastHit))
					{
						GameLayer layer2 = (GameLayer)raycastHit.collider.gameObject.layer;
						if ((camera.cullingMask & layer2.LayerBit()) != 0)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0001EBF4 File Offset: 0x0001CDF4
	private LayerMask GetHigherPriorityLayerMask(GameLayer layer)
	{
		int num = this.m_hitTestPriorityMap[layer];
		LayerMask layerMask = 0;
		foreach (KeyValuePair<GameLayer, int> keyValuePair in this.m_hitTestPriorityMap)
		{
			GameLayer key = keyValuePair.Key;
			int value = keyValuePair.Value;
			if (value > num)
			{
				layerMask |= key.LayerBit();
			}
		}
		return layerMask;
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0001EC90 File Offset: 0x0001CE90
	private void UpdateMouseOnOrOffScreen()
	{
		bool flag = InputUtil.IsMouseOnScreen();
		if (flag == this.m_mouseOnScreen)
		{
			return;
		}
		this.m_mouseOnScreen = flag;
		UniversalInputManager.MouseOnOrOffScreenCallback[] array = this.m_mouseOnOrOffScreenListeners.ToArray();
		foreach (UniversalInputManager.MouseOnOrOffScreenCallback mouseOnOrOffScreenCallback in array)
		{
			mouseOnOrOffScreenCallback(flag);
		}
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0001ECEC File Offset: 0x0001CEEC
	private void UpdateInput()
	{
		if (this.UpdateTextInput())
		{
			return;
		}
		InputManager inputManager = InputManager.Get();
		if (inputManager != null && inputManager.HandleKeyboardInput())
		{
			return;
		}
		CheatMgr cheatMgr = CheatMgr.Get();
		if (cheatMgr != null && cheatMgr.HandleKeyboardInput())
		{
			return;
		}
		Cheats cheats = Cheats.Get();
		if (cheats != null && cheats.HandleKeyboardInput())
		{
			return;
		}
		DialogManager dialogManager = DialogManager.Get();
		if (dialogManager != null && dialogManager.HandleKeyboardInput())
		{
			return;
		}
		CollectionInputMgr collectionInputMgr = CollectionInputMgr.Get();
		if (collectionInputMgr != null && collectionInputMgr.HandleKeyboardInput())
		{
			return;
		}
		DraftInputManager draftInputManager = DraftInputManager.Get();
		if (draftInputManager != null && draftInputManager.HandleKeyboardInput())
		{
			return;
		}
		PackOpening packOpening = PackOpening.Get();
		if (packOpening != null && packOpening.HandleKeyboardInput())
		{
			return;
		}
		if (SceneMgr.Get() != null)
		{
			Scene scene = SceneMgr.Get().GetScene();
			if (scene != null && scene.HandleKeyboardInput())
			{
				return;
			}
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null && baseUI.HandleKeyboardInput())
		{
			return;
		}
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0001EE34 File Offset: 0x0001D034
	private bool UpdateTextInput()
	{
		if (Input.imeIsSelected || !string.IsNullOrEmpty(Input.compositionString))
		{
			UniversalInputManager.IsIMEEverUsed = true;
		}
		if (this.m_inputNeedsFocusFromTabKeyDown)
		{
			this.m_inputNeedsFocusFromTabKeyDown = false;
			this.m_inputNeedsFocus = true;
		}
		return this.m_inputActive && this.m_inputFocused;
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0001EE8C File Offset: 0x0001D08C
	private void UserCancelTextInput()
	{
		this.CancelTextInput(true, null);
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0001EE96 File Offset: 0x0001D096
	private void ObjectCancelTextInput(GameObject requester)
	{
		this.CancelTextInput(false, requester);
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0001EEA0 File Offset: 0x0001D0A0
	private void CancelTextInput(bool userRequested, GameObject requester)
	{
		if (this.IsTextInputPassword())
		{
			Input.imeCompositionMode = 0;
		}
		if (requester != null && requester == this.m_inputOwner)
		{
			this.ClearTextInputVars();
		}
		else
		{
			UniversalInputManager.TextInputCanceledCallback inputCanceledCallback = this.m_inputCanceledCallback;
			this.ClearTextInputVars();
			if (inputCanceledCallback != null)
			{
				inputCanceledCallback(userRequested, requester);
			}
		}
		if (this.IsTouchMode())
		{
			W8Touch.Get().HideKeyboard();
		}
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0001EF18 File Offset: 0x0001D118
	private void CompleteTextInput()
	{
		if (this.IsTextInputPassword())
		{
			Input.imeCompositionMode = 0;
		}
		UniversalInputManager.TextInputCompletedCallback inputCompletedCallback = this.m_inputCompletedCallback;
		if (!this.m_inputKeepFocusOnComplete)
		{
			this.ClearTextInputVars();
		}
		if (inputCompletedCallback != null)
		{
			inputCompletedCallback(this.m_inputText);
		}
		this.m_inputText = string.Empty;
		if (this.IsTouchMode() && this.m_hideVirtualKeyboardOnComplete)
		{
			W8Touch.Get().HideKeyboard();
		}
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0001EF8B File Offset: 0x0001D18B
	private void ClearTextInputVars()
	{
		this.m_inputActive = false;
		this.m_inputFocused = false;
		this.m_inputOwner = null;
		this.m_inputMaxCharacters = 0;
		this.m_inputUpdatedCallback = null;
		this.m_inputCompletedCallback = null;
		this.m_inputCanceledCallback = null;
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0001EFC0 File Offset: 0x0001D1C0
	private bool IgnoreGUIInput()
	{
		if (this.m_inputIgnoreState == UniversalInputManager.TextInputIgnoreState.INVALID)
		{
			return false;
		}
		if (Event.current.type != 5)
		{
			return false;
		}
		KeyCode keyCode = Event.current.keyCode;
		if (keyCode == 13)
		{
			if (this.m_inputIgnoreState == UniversalInputManager.TextInputIgnoreState.COMPLETE_KEY_UP)
			{
				this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.NEXT_CALL;
			}
			return true;
		}
		if (keyCode != 27)
		{
			return false;
		}
		if (this.m_inputIgnoreState == UniversalInputManager.TextInputIgnoreState.CANCEL_KEY_UP)
		{
			this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.NEXT_CALL;
		}
		return true;
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0001F038 File Offset: 0x0001D238
	private void HandleGUIInputInactive()
	{
		if (this.m_inputActive)
		{
			return;
		}
		if (this.m_inputIgnoreState != UniversalInputManager.TextInputIgnoreState.INVALID)
		{
			if (this.m_inputIgnoreState == UniversalInputManager.TextInputIgnoreState.NEXT_CALL)
			{
				this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.INVALID;
			}
			return;
		}
		if (ChatMgr.Get() != null)
		{
			ChatMgr.Get().HandleGUIInput();
		}
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0001F08C File Offset: 0x0001D28C
	private void HandleGUIInputActive()
	{
		if (!this.m_inputActive)
		{
			return;
		}
		if (!this.PreprocessGUITextInput())
		{
			return;
		}
		Vector2 screenSize;
		screenSize..ctor((float)Screen.width, (float)Screen.height);
		Rect inputScreenRect = this.ComputeTextInputRect(screenSize);
		this.SetupTextInput(screenSize, inputScreenRect);
		string text = this.ShowTextInput(inputScreenRect);
		if (this.IsTouchMode() && !W8Touch.Get().IsVirtualKeyboardVisible() && this.GetMouseButtonDown(0) && inputScreenRect.Contains(W8Touch.Get().GetTouchPositionForGUI()))
		{
			W8Touch.Get().ShowKeyboard();
		}
		this.UpdateTextInputFocus();
		if (!this.m_inputFocused)
		{
			return;
		}
		if (this.m_inputText != text)
		{
			if (this.m_inputNumber)
			{
				text = StringUtils.StripNonNumbers(text);
			}
			if (!this.m_inputMultiLine)
			{
				text = StringUtils.StripNewlines(text);
			}
			this.m_inputText = text;
			if (this.m_inputUpdatedCallback != null)
			{
				this.m_inputUpdatedCallback(text);
			}
		}
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0001F188 File Offset: 0x0001D388
	private bool PreprocessGUITextInput()
	{
		this.UpdateTabKeyDown();
		if (this.m_inputPreprocessCallback != null)
		{
			this.m_inputPreprocessCallback(Event.current);
			if (!this.m_inputActive)
			{
				return false;
			}
		}
		return !this.ProcessTextInputFinishKeys();
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0001F1D4 File Offset: 0x0001D3D4
	private void UpdateTabKeyDown()
	{
		this.m_tabKeyDown = (Event.current.type == 4 && Event.current.keyCode == 9);
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0001F208 File Offset: 0x0001D408
	private bool ProcessTextInputFinishKeys()
	{
		if (!this.m_inputFocused)
		{
			return false;
		}
		if (Event.current.type != 4)
		{
			return false;
		}
		KeyCode keyCode = Event.current.keyCode;
		if (keyCode == 13)
		{
			this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.COMPLETE_KEY_UP;
			this.CompleteTextInput();
			return true;
		}
		if (keyCode != 27)
		{
			return false;
		}
		this.m_inputIgnoreState = UniversalInputManager.TextInputIgnoreState.CANCEL_KEY_UP;
		this.UserCancelTextInput();
		return true;
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0001F274 File Offset: 0x0001D474
	private void SetupTextInput(Vector2 screenSize, Rect inputScreenRect)
	{
		GUI.skin = this.m_skin;
		GUI.skin.textField.font = this.m_inputFont;
		int fontSize = this.ComputeTextInputFontSize(screenSize, inputScreenRect.height);
		GUI.skin.textField.fontSize = fontSize;
		Color? inputColor = this.m_inputColor;
		if (inputColor != null)
		{
			Color? inputColor2 = this.m_inputColor;
			GUI.color = inputColor2.Value;
		}
		GUI.skin.textField.alignment = this.m_inputAlignment;
		GUI.SetNextControlName("UniversalInputManagerTextInput");
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0001F308 File Offset: 0x0001D508
	private string ShowTextInput(Rect inputScreenRect)
	{
		string result;
		if (this.m_inputPassword)
		{
			if (this.m_inputMaxCharacters <= 0)
			{
				result = GUI.PasswordField(inputScreenRect, this.m_inputText, '*');
			}
			else
			{
				result = GUI.PasswordField(inputScreenRect, this.m_inputText, '*', this.m_inputMaxCharacters);
			}
		}
		else if (this.m_inputMaxCharacters <= 0)
		{
			result = GUI.TextField(inputScreenRect, this.m_inputText);
		}
		else
		{
			result = GUI.TextField(inputScreenRect, this.m_inputText, this.m_inputMaxCharacters);
		}
		return result;
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0001F38C File Offset: 0x0001D58C
	private void UpdateTextInputFocus()
	{
		if (this.m_inputNeedsFocus)
		{
			GUI.FocusControl("UniversalInputManagerTextInput");
			this.m_inputFocused = true;
			this.m_inputNeedsFocus = false;
		}
		else
		{
			this.m_inputFocused = (GUI.GetNameOfFocusedControl() == "UniversalInputManagerTextInput");
		}
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0001F3CC File Offset: 0x0001D5CC
	private Rect ComputeTextInputRect(Vector2 screenSize)
	{
		float num = screenSize.x / screenSize.y;
		float num2 = this.m_inputInitialScreenSize.x / this.m_inputInitialScreenSize.y;
		float num3 = num2 / num;
		float num4 = screenSize.y / this.m_inputInitialScreenSize.y;
		float num5 = (0.5f - this.m_inputNormalizedRect.x) * this.m_inputInitialScreenSize.x;
		float num6 = num5 * num4;
		Rect result;
		result..ctor(screenSize.x * 0.5f - num6, this.m_inputNormalizedRect.y * screenSize.y - 1.5f, this.m_inputNormalizedRect.width * screenSize.x * num3, this.m_inputNormalizedRect.height * screenSize.y + 1.5f);
		return result;
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0001F4A0 File Offset: 0x0001D6A0
	private int ComputeTextInputFontSize(Vector2 screenSize, float rectHeight)
	{
		int num = Mathf.CeilToInt(rectHeight);
		if (Localization.IsIMELocale() || UniversalInputManager.IsIMEEverUsed)
		{
			num -= 9;
		}
		else
		{
			num -= 4;
		}
		return Mathf.Clamp(num, 2, 32);
	}

	// Token: 0x040003FF RID: 1023
	private const float TEXT_INPUT_RECT_HEIGHT_OFFSET = 3f;

	// Token: 0x04000400 RID: 1024
	private const int TEXT_INPUT_MAX_FONT_SIZE = 32;

	// Token: 0x04000401 RID: 1025
	private const int TEXT_INPUT_MIN_FONT_SIZE = 2;

	// Token: 0x04000402 RID: 1026
	private const int TEXT_INPUT_FONT_SIZE_INSET = 4;

	// Token: 0x04000403 RID: 1027
	private const int TEXT_INPUT_IME_FONT_SIZE_INSET = 9;

	// Token: 0x04000404 RID: 1028
	private const string TEXT_INPUT_NAME = "UniversalInputManagerTextInput";

	// Token: 0x04000405 RID: 1029
	private static readonly PlatformDependentValue<bool> IsTouchDevice = new PlatformDependentValue<bool>(PlatformCategory.Input)
	{
		Mouse = false,
		Touch = true
	};

	// Token: 0x04000406 RID: 1030
	private static readonly GameLayer[] HIT_TEST_PRIORITY_ORDER = new GameLayer[]
	{
		GameLayer.IgnoreFullScreenEffects,
		GameLayer.BackgroundUI,
		GameLayer.PerspectiveUI,
		GameLayer.CameraMask,
		GameLayer.UI,
		GameLayer.BattleNet,
		GameLayer.BattleNetFriendList,
		GameLayer.BattleNetChat,
		GameLayer.BattleNetDialog,
		GameLayer.HighPriorityUI
	};

	// Token: 0x04000407 RID: 1031
	private static UniversalInputManager s_instance;

	// Token: 0x04000408 RID: 1032
	private static bool IsIMEEverUsed = false;

	// Token: 0x04000409 RID: 1033
	private bool m_mouseOnScreen;

	// Token: 0x0400040A RID: 1034
	private List<UniversalInputManager.MouseOnOrOffScreenCallback> m_mouseOnOrOffScreenListeners = new List<UniversalInputManager.MouseOnOrOffScreenCallback>();

	// Token: 0x0400040B RID: 1035
	private Map<GameLayer, int> m_hitTestPriorityMap;

	// Token: 0x0400040C RID: 1036
	private bool m_gameDialogActive;

	// Token: 0x0400040D RID: 1037
	private bool m_systemDialogActive;

	// Token: 0x0400040E RID: 1038
	private int m_offCameraHitTestMask;

	// Token: 0x0400040F RID: 1039
	private Camera m_FullscreenEffectsCamera;

	// Token: 0x04000410 RID: 1040
	private List<Camera> m_CameraMaskCameras = new List<Camera>();

	// Token: 0x04000411 RID: 1041
	private bool m_FullscreenEffectsCameraActive;

	// Token: 0x04000412 RID: 1042
	private List<Camera> m_ignoredCameras = new List<Camera>();

	// Token: 0x04000413 RID: 1043
	private GameObject m_inputOwner;

	// Token: 0x04000414 RID: 1044
	private UniversalInputManager.TextInputUpdatedCallback m_inputUpdatedCallback;

	// Token: 0x04000415 RID: 1045
	private UniversalInputManager.TextInputPreprocessCallback m_inputPreprocessCallback;

	// Token: 0x04000416 RID: 1046
	private UniversalInputManager.TextInputCompletedCallback m_inputCompletedCallback;

	// Token: 0x04000417 RID: 1047
	private UniversalInputManager.TextInputCanceledCallback m_inputCanceledCallback;

	// Token: 0x04000418 RID: 1048
	private bool m_inputPassword;

	// Token: 0x04000419 RID: 1049
	private bool m_inputNumber;

	// Token: 0x0400041A RID: 1050
	private bool m_inputMultiLine;

	// Token: 0x0400041B RID: 1051
	private bool m_inputActive;

	// Token: 0x0400041C RID: 1052
	private bool m_inputFocused;

	// Token: 0x0400041D RID: 1053
	private bool m_inputKeepFocusOnComplete;

	// Token: 0x0400041E RID: 1054
	private string m_inputText;

	// Token: 0x0400041F RID: 1055
	private Rect m_inputNormalizedRect;

	// Token: 0x04000420 RID: 1056
	private Vector2 m_inputInitialScreenSize;

	// Token: 0x04000421 RID: 1057
	private int m_inputMaxCharacters;

	// Token: 0x04000422 RID: 1058
	private Font m_inputFont;

	// Token: 0x04000423 RID: 1059
	private TextAnchor m_inputAlignment;

	// Token: 0x04000424 RID: 1060
	private Color? m_inputColor;

	// Token: 0x04000425 RID: 1061
	private Font m_defaultInputFont;

	// Token: 0x04000426 RID: 1062
	private TextAnchor m_defaultInputAlignment;

	// Token: 0x04000427 RID: 1063
	private bool m_inputNeedsFocus;

	// Token: 0x04000428 RID: 1064
	private bool m_tabKeyDown;

	// Token: 0x04000429 RID: 1065
	private bool m_inputNeedsFocusFromTabKeyDown;

	// Token: 0x0400042A RID: 1066
	private UniversalInputManager.TextInputIgnoreState m_inputIgnoreState;

	// Token: 0x0400042B RID: 1067
	public bool m_hideVirtualKeyboardOnComplete = true;

	// Token: 0x0400042C RID: 1068
	private GUISkinContainer m_skinContainer;

	// Token: 0x0400042D RID: 1069
	private GUISkin m_skin;

	// Token: 0x0400042E RID: 1070
	public static readonly PlatformDependentValue<bool> UsePhoneUI = new PlatformDependentValue<bool>(PlatformCategory.Screen)
	{
		Phone = true,
		Tablet = false,
		PC = false
	};

	// Token: 0x020001FB RID: 507
	public class TextInputParams
	{
		// Token: 0x040010E9 RID: 4329
		public GameObject m_owner;

		// Token: 0x040010EA RID: 4330
		public bool m_password;

		// Token: 0x040010EB RID: 4331
		public bool m_number;

		// Token: 0x040010EC RID: 4332
		public bool m_multiLine;

		// Token: 0x040010ED RID: 4333
		public Rect m_rect;

		// Token: 0x040010EE RID: 4334
		public UniversalInputManager.TextInputUpdatedCallback m_updatedCallback;

		// Token: 0x040010EF RID: 4335
		public UniversalInputManager.TextInputPreprocessCallback m_preprocessCallback;

		// Token: 0x040010F0 RID: 4336
		public UniversalInputManager.TextInputCompletedCallback m_completedCallback;

		// Token: 0x040010F1 RID: 4337
		public UniversalInputManager.TextInputCanceledCallback m_canceledCallback;

		// Token: 0x040010F2 RID: 4338
		public int m_maxCharacters;

		// Token: 0x040010F3 RID: 4339
		public Font m_font;

		// Token: 0x040010F4 RID: 4340
		public TextAnchor? m_alignment;

		// Token: 0x040010F5 RID: 4341
		public string m_text;

		// Token: 0x040010F6 RID: 4342
		public bool m_touchScreenKeyboardHideInput;

		// Token: 0x040010F7 RID: 4343
		public int m_touchScreenKeyboardType;

		// Token: 0x040010F8 RID: 4344
		public bool m_inputKeepFocusOnComplete;

		// Token: 0x040010F9 RID: 4345
		public Color? m_color;

		// Token: 0x040010FA RID: 4346
		public bool m_showVirtualKeyboard = true;

		// Token: 0x040010FB RID: 4347
		public bool m_hideVirtualKeyboardOnComplete = true;

		// Token: 0x040010FC RID: 4348
		public bool m_useNativeKeyboard;
	}

	// Token: 0x020001FC RID: 508
	// (Invoke) Token: 0x06001E43 RID: 7747
	public delegate void TextInputUpdatedCallback(string input);

	// Token: 0x020001FD RID: 509
	// (Invoke) Token: 0x06001E47 RID: 7751
	public delegate bool TextInputPreprocessCallback(Event e);

	// Token: 0x020001FE RID: 510
	// (Invoke) Token: 0x06001E4B RID: 7755
	public delegate void TextInputCompletedCallback(string input);

	// Token: 0x020001FF RID: 511
	// (Invoke) Token: 0x06001E4F RID: 7759
	public delegate void TextInputCanceledCallback(bool userRequested, GameObject requester);

	// Token: 0x02000200 RID: 512
	// (Invoke) Token: 0x06001E53 RID: 7763
	public delegate void MouseOnOrOffScreenCallback(bool onScreen);

	// Token: 0x02000201 RID: 513
	private enum TextInputIgnoreState
	{
		// Token: 0x040010FE RID: 4350
		INVALID,
		// Token: 0x040010FF RID: 4351
		COMPLETE_KEY_UP,
		// Token: 0x04001100 RID: 4352
		CANCEL_KEY_UP,
		// Token: 0x04001101 RID: 4353
		NEXT_CALL
	}
}
