using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class W8Touch : MonoBehaviour
{
	// Token: 0x14000007 RID: 7
	// (add) Token: 0x06000EB8 RID: 3768 RVA: 0x0003F1B0 File Offset: 0x0003D3B0
	// (remove) Token: 0x06000EB9 RID: 3769 RVA: 0x0003F1C9 File Offset: 0x0003D3C9
	public event Action VirtualKeyboardDidShow;

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x06000EBA RID: 3770 RVA: 0x0003F1E2 File Offset: 0x0003D3E2
	// (remove) Token: 0x06000EBB RID: 3771 RVA: 0x0003F1FB File Offset: 0x0003D3FB
	public event Action VirtualKeyboardDidHide;

	// Token: 0x06000EBC RID: 3772
	[DllImport("User32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	// Token: 0x06000EBD RID: 3773 RVA: 0x0003F214 File Offset: 0x0003D414
	private void Start()
	{
		this.m_touchState = new W8Touch.TouchState[5];
		for (int i = 0; i < 5; i++)
		{
			this.m_touchState[i] = W8Touch.TouchState.None;
		}
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x0003F248 File Offset: 0x0003D448
	private void Awake()
	{
		W8Touch.s_instance = this;
		if (this.LoadW8TouchDLL())
		{
			W8Touch.s_isWindows8OrGreater = W8Touch.DLL_W8IsWindows8OrGreater();
		}
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x0003F275 File Offset: 0x0003D475
	private void Destroy()
	{
		W8Touch.s_instance = null;
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x0003F280 File Offset: 0x0003D480
	private void Update()
	{
		if (!W8Touch.IsInitialized())
		{
			return;
		}
		W8Touch.DLL_W8GetDesktopRect(out this.m_desktopRect);
		bool flag = W8Touch.DLL_W8IsVirtualKeyboardVisible();
		if (flag != this.m_isVirtualKeyboardVisible)
		{
			this.m_isVirtualKeyboardVisible = flag;
			if (flag && this.VirtualKeyboardDidShow != null)
			{
				this.VirtualKeyboardDidShow.Invoke();
			}
			else if (!flag && this.VirtualKeyboardDidHide != null)
			{
				this.VirtualKeyboardDidHide.Invoke();
			}
		}
		if (this.m_isVirtualKeyboardVisible)
		{
			this.m_isVirtualKeyboardShowRequested = false;
		}
		else
		{
			this.m_isVirtualKeyboardHideRequested = false;
		}
		W8Touch.PowerSource batteryMode = this.GetBatteryMode();
		if (batteryMode != this.m_lastPowerSourceState)
		{
			Log.Yim.Print("PowerSource Change Detected: {0}", new object[]
			{
				batteryMode
			});
			this.m_lastPowerSourceState = batteryMode;
			GraphicsManager.Get().RenderQualityLevel = (GraphicsQuality)Options.Get().GetInt(Option.GFX_QUALITY);
		}
		if ((!W8Touch.DLL_W8IsLastEventFromTouch() && UniversalInputManager.Get().IsTouchMode()) || (W8Touch.DLL_W8IsLastEventFromTouch() && !UniversalInputManager.Get().IsTouchMode()))
		{
			this.ToggleTouchMode();
		}
		if (this.m_touchState != null)
		{
			int num = W8Touch.DLL_W8GetTouchPointCount();
			for (int i = 0; i < 5; i++)
			{
				W8Touch.tTouchData tTouchData = new W8Touch.tTouchData();
				bool flag2 = false;
				if (i < num)
				{
					flag2 = W8Touch.DLL_W8GetTouchPoint(i, tTouchData);
				}
				if (flag2 && i == 0)
				{
					Vector2 vector = this.TransformTouchPosition(new Vector2((float)tTouchData.m_x, (float)tTouchData.m_y));
					if (this.m_touchPosition.x != -1f && this.m_touchPosition.y != -1f && this.m_touchState[i] == W8Touch.TouchState.Down)
					{
						this.m_touchDelta.x = vector.x - this.m_touchPosition.x;
						this.m_touchDelta.y = vector.y - this.m_touchPosition.y;
					}
					else
					{
						this.m_touchDelta.x = (this.m_touchDelta.y = 0f);
					}
					this.m_touchPosition.x = vector.x;
					this.m_touchPosition.y = vector.y;
				}
				if (flag2 && tTouchData.m_ID != -1)
				{
					if (this.m_touchState[i] == W8Touch.TouchState.Down || this.m_touchState[i] == W8Touch.TouchState.InitialDown)
					{
						this.m_touchState[i] = W8Touch.TouchState.Down;
					}
					else
					{
						this.m_touchState[i] = W8Touch.TouchState.InitialDown;
					}
				}
				else if (this.m_touchState[i] == W8Touch.TouchState.Down || this.m_touchState[i] == W8Touch.TouchState.InitialDown)
				{
					this.m_touchState[i] = W8Touch.TouchState.InitialUp;
				}
				else
				{
					this.m_touchState[i] = W8Touch.TouchState.None;
				}
			}
		}
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x0003F560 File Offset: 0x0003D760
	private void OnGUI()
	{
		if (!W8Touch.s_isWindows8OrGreater && W8Touch.s_DLL == IntPtr.Zero)
		{
			return;
		}
		if (!W8Touch.s_initialized)
		{
			this.InitializeDLL();
		}
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x0003F5A1 File Offset: 0x0003D7A1
	public static W8Touch Get()
	{
		return W8Touch.s_instance;
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x0003F5A8 File Offset: 0x0003D7A8
	private Vector2 TransformTouchPosition(Vector2 touchInput)
	{
		Vector2 result = default(Vector2);
		if (Screen.fullScreen)
		{
			float num = (float)Screen.width / (float)Screen.height;
			float num2 = (float)this.m_desktopRect.Right / (float)this.m_desktopRect.Bottom;
			if (Mathf.Abs(num - num2) < Mathf.Epsilon)
			{
				float num3 = (float)Screen.width / (float)this.m_desktopRect.Right;
				float num4 = (float)Screen.height / (float)this.m_desktopRect.Bottom;
				result.x = touchInput.x * num3;
				result.y = ((float)this.m_desktopRect.Bottom - touchInput.y) * num4;
			}
			else if (num < num2)
			{
				float num5 = (float)this.m_desktopRect.Bottom;
				float num6 = num5 * num;
				float num7 = (float)Screen.height / num5;
				float num8 = (float)Screen.width / num6;
				float num9 = ((float)this.m_desktopRect.Right - num6) / 2f;
				result.x = (touchInput.x - num9) * num8;
				result.y = ((float)this.m_desktopRect.Bottom - touchInput.y) * num7;
			}
			else
			{
				float num10 = (float)this.m_desktopRect.Right;
				float num11 = num10 / num;
				float num12 = (float)Screen.height / num11;
				float num13 = (float)Screen.width / num10;
				float num14 = ((float)this.m_desktopRect.Bottom - num11) / 2f;
				result.x = touchInput.x * num13;
				result.y = ((float)this.m_desktopRect.Bottom - touchInput.y - num14) * num12;
			}
		}
		else
		{
			result.x = touchInput.x;
			result.y = (float)Screen.height - touchInput.y;
		}
		return result;
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x0003F778 File Offset: 0x0003D978
	private void ToggleTouchMode()
	{
		if (!W8Touch.IsInitialized())
		{
			return;
		}
		bool @bool = Options.Get().GetBool(Option.TOUCH_MODE);
		Options.Get().SetBool(Option.TOUCH_MODE, !@bool);
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x0003F7B0 File Offset: 0x0003D9B0
	public void ShowKeyboard()
	{
		if (!W8Touch.IsInitialized() || this.m_isVirtualKeyboardShowRequested || (this.m_isVirtualKeyboardVisible && !this.m_isVirtualKeyboardHideRequested))
		{
			return;
		}
		if (this.m_isVirtualKeyboardHideRequested)
		{
			this.m_isVirtualKeyboardHideRequested = false;
		}
		W8Touch.KeyboardFlags keyboardFlags = (W8Touch.KeyboardFlags)W8Touch.DLL_W8ShowKeyboard();
		if ((keyboardFlags & W8Touch.KeyboardFlags.Shown) != W8Touch.KeyboardFlags.Shown)
		{
		}
		if ((keyboardFlags & W8Touch.KeyboardFlags.Shown) == W8Touch.KeyboardFlags.Shown && (keyboardFlags & W8Touch.KeyboardFlags.SuccessTabTip) == W8Touch.KeyboardFlags.SuccessTabTip)
		{
			this.m_isVirtualKeyboardShowRequested = true;
		}
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x0003F828 File Offset: 0x0003DA28
	public void HideKeyboard()
	{
		if (!W8Touch.IsInitialized() && !this.m_isVirtualKeyboardVisible)
		{
			return;
		}
		if (this.m_isVirtualKeyboardShowRequested)
		{
			this.m_isVirtualKeyboardShowRequested = false;
		}
		int num = W8Touch.DLL_W8HideKeyboard();
		if (num == 0)
		{
			this.m_isVirtualKeyboardHideRequested = true;
		}
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x0003F87C File Offset: 0x0003DA7C
	public void ShowOSK()
	{
		if (!W8Touch.IsInitialized())
		{
			return;
		}
		W8Touch.KeyboardFlags keyboardFlags = (W8Touch.KeyboardFlags)W8Touch.DLL_W8ShowOSK();
		if ((keyboardFlags & W8Touch.KeyboardFlags.Shown) != W8Touch.KeyboardFlags.Shown)
		{
		}
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x0003F8A8 File Offset: 0x0003DAA8
	public string GetIntelDeviceName()
	{
		if (!W8Touch.IsInitialized())
		{
			return null;
		}
		return W8Touch.IntelDevice.GetDeviceName(W8Touch.DLL_W8GetDeviceId());
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x0003F8C5 File Offset: 0x0003DAC5
	public W8Touch.PowerSource GetBatteryMode()
	{
		if (!W8Touch.IsInitialized())
		{
			return W8Touch.PowerSource.Unintialized;
		}
		return (W8Touch.PowerSource)W8Touch.DLL_W8GetBatteryMode();
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x0003F8DD File Offset: 0x0003DADD
	public int GetPercentBatteryLife()
	{
		if (!W8Touch.IsInitialized())
		{
			return -1;
		}
		return W8Touch.DLL_W8GetPercentBatteryLife();
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x0003F8F5 File Offset: 0x0003DAF5
	public bool IsVirtualKeyboardVisible()
	{
		return W8Touch.IsInitialized() && this.m_isVirtualKeyboardVisible;
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0003F909 File Offset: 0x0003DB09
	public bool GetTouch(int touchCount)
	{
		return W8Touch.IsInitialized() && this.m_touchState != null && touchCount < 5 && (this.m_touchState[touchCount] == W8Touch.TouchState.InitialDown || this.m_touchState[touchCount] == W8Touch.TouchState.Down);
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x0003F948 File Offset: 0x0003DB48
	public bool GetTouchDown(int touchCount)
	{
		return W8Touch.IsInitialized() && this.m_touchState != null && touchCount < 5 && this.m_touchState[touchCount] == W8Touch.TouchState.InitialDown;
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x0003F979 File Offset: 0x0003DB79
	public bool GetTouchUp(int touchCount)
	{
		return W8Touch.IsInitialized() && this.m_touchState != null && touchCount < 5 && this.m_touchState[touchCount] == W8Touch.TouchState.InitialUp;
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0003F9AC File Offset: 0x0003DBAC
	public Vector3 GetTouchPosition()
	{
		if (!W8Touch.IsInitialized() || this.m_touchState == null)
		{
			return new Vector3(0f, 0f, 0f);
		}
		return new Vector3(this.m_touchPosition.x, this.m_touchPosition.y, this.m_touchPosition.z);
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x0003FA0C File Offset: 0x0003DC0C
	public Vector2 GetTouchDelta()
	{
		if (!W8Touch.IsInitialized() || this.m_touchState == null)
		{
			return new Vector2(0f, 0f);
		}
		return new Vector2(this.m_touchDelta.x, this.m_touchDelta.y);
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x0003FA5C File Offset: 0x0003DC5C
	public Vector3 GetTouchPositionForGUI()
	{
		if (!W8Touch.IsInitialized() || this.m_touchState == null)
		{
			return new Vector3(0f, 0f, 0f);
		}
		Vector2 vector = this.TransformTouchPosition(this.m_touchPosition);
		return new Vector3(vector.x, vector.y, this.m_touchPosition.z);
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0003FAC4 File Offset: 0x0003DCC4
	private IntPtr GetFunction(string name)
	{
		IntPtr procAddress = DLLUtils.GetProcAddress(W8Touch.s_DLL, name);
		if (procAddress == IntPtr.Zero)
		{
			Debug.LogError("Could not load W8TouchDLL." + name + "()");
			W8Touch.AppQuit();
		}
		return procAddress;
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0003FB08 File Offset: 0x0003DD08
	private bool LoadW8TouchDLL()
	{
		if (Environment.OSVersion.Version.Major < 6 || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor < 2))
		{
			Log.Yim.Print("Windows Version is Pre-Windows 8", new object[0]);
			return false;
		}
		if (W8Touch.s_DLL == IntPtr.Zero)
		{
			W8Touch.s_DLL = FileUtils.LoadPlugin("W8TouchDLL", false);
			if (W8Touch.s_DLL == IntPtr.Zero)
			{
				Log.Yim.Print("Could not load W8TouchDLL.dll", new object[0]);
				return false;
			}
		}
		W8Touch.DLL_W8ShowKeyboard = (W8Touch.DelW8ShowKeyboard)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_ShowKeyboard"), typeof(W8Touch.DelW8ShowKeyboard));
		W8Touch.DLL_W8HideKeyboard = (W8Touch.DelW8HideKeyboard)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_HideKeyboard"), typeof(W8Touch.DelW8HideKeyboard));
		W8Touch.DLL_W8ShowOSK = (W8Touch.DelW8ShowOSK)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_ShowOSK"), typeof(W8Touch.DelW8ShowOSK));
		W8Touch.DLL_W8Initialize = (W8Touch.DelW8Initialize)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_Initialize"), typeof(W8Touch.DelW8Initialize));
		W8Touch.DLL_W8Shutdown = (W8Touch.DelW8Shutdown)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_Shutdown"), typeof(W8Touch.DelW8Shutdown));
		W8Touch.DLL_W8GetDeviceId = (W8Touch.DelW8GetDeviceId)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_GetDeviceId"), typeof(W8Touch.DelW8GetDeviceId));
		W8Touch.DLL_W8IsWindows8OrGreater = (W8Touch.DelW8IsWindows8OrGreater)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_IsWindows8OrGreater"), typeof(W8Touch.DelW8IsWindows8OrGreater));
		W8Touch.DLL_W8IsLastEventFromTouch = (W8Touch.DelW8IsLastEventFromTouch)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_IsLastEventFromTouch"), typeof(W8Touch.DelW8IsLastEventFromTouch));
		W8Touch.DLL_W8GetBatteryMode = (W8Touch.DelW8GetBatteryMode)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_GetBatteryMode"), typeof(W8Touch.DelW8GetBatteryMode));
		W8Touch.DLL_W8GetPercentBatteryLife = (W8Touch.DelW8GetPercentBatteryLife)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_GetPercentBatteryLife"), typeof(W8Touch.DelW8GetPercentBatteryLife));
		W8Touch.DLL_W8GetDesktopRect = (W8Touch.DelW8GetDesktopRect)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_GetDesktopRect"), typeof(W8Touch.DelW8GetDesktopRect));
		W8Touch.DLL_W8IsVirtualKeyboardVisible = (W8Touch.DelW8IsVirtualKeyboardVisible)Marshal.GetDelegateForFunctionPointer(this.GetFunction("W8_IsVirtualKeyboardVisible"), typeof(W8Touch.DelW8IsVirtualKeyboardVisible));
		W8Touch.DLL_W8GetTouchPointCount = (W8Touch.DelW8GetTouchPointCount)Marshal.GetDelegateForFunctionPointer(this.GetFunction("GetTouchPointCount"), typeof(W8Touch.DelW8GetTouchPointCount));
		W8Touch.DLL_W8GetTouchPoint = (W8Touch.DelW8GetTouchPoint)Marshal.GetDelegateForFunctionPointer(this.GetFunction("GetTouchPoint"), typeof(W8Touch.DelW8GetTouchPoint));
		return true;
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0003FDB4 File Offset: 0x0003DFB4
	public static void AppQuit()
	{
		Log.Yim.Print("W8Touch.AppQuit()", new object[0]);
		if (W8Touch.s_DLL == IntPtr.Zero)
		{
			return;
		}
		if (W8Touch.s_instance)
		{
			W8Touch.s_instance.ResetWindowFeedbackSetting();
		}
		if (W8Touch.DLL_W8Shutdown != null && W8Touch.s_initialized)
		{
			W8Touch.DLL_W8Shutdown();
			W8Touch.s_initialized = false;
		}
		if (!DLLUtils.FreeLibrary(W8Touch.s_DLL))
		{
			Debug.Log("Error unloading W8TouchDLL.dll");
		}
		W8Touch.s_DLL = IntPtr.Zero;
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0003FE4C File Offset: 0x0003E04C
	private static bool IsInitialized()
	{
		return W8Touch.s_DLL != IntPtr.Zero && W8Touch.s_isWindows8OrGreater && W8Touch.s_initialized;
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x0003FE80 File Offset: 0x0003E080
	private void InitializeDLL()
	{
		if (this.m_intializationAttemptCount >= 10)
		{
			return;
		}
		string windowName = GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE");
		int num = W8Touch.DLL_W8Initialize(windowName);
		if (num < 0)
		{
			this.m_intializationAttemptCount++;
		}
		else
		{
			Log.Yim.Print("W8Touch Start Success!", new object[0]);
			W8Touch.s_initialized = true;
			IntPtr intPtr = DLLUtils.LoadLibrary("User32.DLL");
			if (intPtr == IntPtr.Zero)
			{
				Log.Yim.Print("Could not load User32.DLL", new object[0]);
			}
			else
			{
				IntPtr procAddress = DLLUtils.GetProcAddress(intPtr, "SetWindowFeedbackSetting");
				if (procAddress == IntPtr.Zero)
				{
					Log.Yim.Print("Could not load User32.SetWindowFeedbackSetting()", new object[0]);
				}
				else
				{
					IntPtr intPtr2 = W8Touch.FindWindow(null, "Hearthstone");
					if (intPtr2 == IntPtr.Zero)
					{
						intPtr2 = W8Touch.FindWindow(null, GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE"));
					}
					if (intPtr2 == IntPtr.Zero)
					{
						Log.Yim.Print("Unable to retrieve Hearthstone window handle!", new object[0]);
					}
					else
					{
						W8Touch.DelSetWindowFeedbackSetting delSetWindowFeedbackSetting = (W8Touch.DelSetWindowFeedbackSetting)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(W8Touch.DelSetWindowFeedbackSetting));
						int num2 = Marshal.SizeOf(typeof(int));
						IntPtr intPtr3 = Marshal.AllocHGlobal(num2);
						Marshal.WriteInt32(intPtr3, 0, (!this.m_bWindowFeedbackSettingValue) ? 0 : 1);
						bool bIsWindowFeedbackDisabled = true;
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_CONTACTVISUALIZATION, 0U, Convert.ToUInt32(num2), intPtr3))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_CONTACTVISUALIZATION failed!", new object[0]);
							bIsWindowFeedbackDisabled = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_TAP, 0U, Convert.ToUInt32(num2), intPtr3))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_TAP failed!", new object[0]);
							bIsWindowFeedbackDisabled = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_PRESSANDHOLD, 0U, Convert.ToUInt32(num2), intPtr3))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_PRESSANDHOLD failed!", new object[0]);
							bIsWindowFeedbackDisabled = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_DOUBLETAP, 0U, Convert.ToUInt32(num2), intPtr3))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_DOUBLETAP failed!", new object[0]);
							bIsWindowFeedbackDisabled = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_RIGHTTAP, 0U, Convert.ToUInt32(num2), intPtr3))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_RIGHTTAP failed!", new object[0]);
							bIsWindowFeedbackDisabled = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_GESTURE_PRESSANDTAP, 0U, Convert.ToUInt32(num2), intPtr3))
						{
							Log.Yim.Print("FEEDBACK_GESTURE_PRESSANDTAP failed!", new object[0]);
							bIsWindowFeedbackDisabled = false;
						}
						this.m_bIsWindowFeedbackDisabled = bIsWindowFeedbackDisabled;
						if (this.m_bIsWindowFeedbackDisabled)
						{
							Log.Yim.Print("Windows 8 Feedback Touch Gestures Disabled!", new object[0]);
						}
						Marshal.FreeHGlobal(intPtr3);
					}
				}
				if (!DLLUtils.FreeLibrary(intPtr))
				{
					Log.Yim.Print("Error unloading User32.dll", new object[0]);
				}
			}
		}
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x00040174 File Offset: 0x0003E374
	private void ResetWindowFeedbackSetting()
	{
		if (W8Touch.s_initialized && this.m_bIsWindowFeedbackDisabled)
		{
			IntPtr intPtr = DLLUtils.LoadLibrary("User32.DLL");
			if (intPtr == IntPtr.Zero)
			{
				Log.Yim.Print("Could not load User32.DLL", new object[0]);
			}
			else
			{
				IntPtr procAddress = DLLUtils.GetProcAddress(intPtr, "SetWindowFeedbackSetting");
				if (procAddress == IntPtr.Zero)
				{
					Log.Yim.Print("Could not load User32.SetWindowFeedbackSetting()", new object[0]);
				}
				else
				{
					IntPtr intPtr2 = W8Touch.FindWindow(null, "Hearthstone");
					if (intPtr2 == IntPtr.Zero)
					{
						intPtr2 = W8Touch.FindWindow(null, GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE"));
					}
					if (intPtr2 == IntPtr.Zero)
					{
						Log.Yim.Print("Unable to retrieve Hearthstone window handle!", new object[0]);
					}
					else
					{
						W8Touch.DelSetWindowFeedbackSetting delSetWindowFeedbackSetting = (W8Touch.DelSetWindowFeedbackSetting)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(W8Touch.DelSetWindowFeedbackSetting));
						int num = Marshal.SizeOf(typeof(int));
						IntPtr intPtr3 = Marshal.AllocHGlobal(num);
						Marshal.WriteInt32(intPtr3, 0, (!this.m_bWindowFeedbackSettingValue) ? 0 : 1);
						bool flag = true;
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_CONTACTVISUALIZATION, 0U, 0U, IntPtr.Zero))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_CONTACTVISUALIZATION failed!", new object[0]);
							flag = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_TAP, 0U, 0U, IntPtr.Zero))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_TAP failed!", new object[0]);
							flag = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_PRESSANDHOLD, 0U, 0U, IntPtr.Zero))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_PRESSANDHOLD failed!", new object[0]);
							flag = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_DOUBLETAP, 0U, 0U, IntPtr.Zero))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_DOUBLETAP failed!", new object[0]);
							flag = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_TOUCH_RIGHTTAP, 0U, 0U, IntPtr.Zero))
						{
							Log.Yim.Print("FEEDBACK_TOUCH_RIGHTTAP failed!", new object[0]);
							flag = false;
						}
						if (!delSetWindowFeedbackSetting(intPtr2, W8Touch.FEEDBACK_TYPE.FEEDBACK_GESTURE_PRESSANDTAP, 0U, 0U, IntPtr.Zero))
						{
							Log.Yim.Print("FEEDBACK_GESTURE_PRESSANDTAP failed!", new object[0]);
							flag = false;
						}
						this.m_bIsWindowFeedbackDisabled = !flag;
						if (!this.m_bIsWindowFeedbackDisabled)
						{
							Log.Yim.Print("Windows 8 Feedback Touch Gestures Reset!", new object[0]);
						}
						Marshal.FreeHGlobal(intPtr3);
					}
				}
				if (!DLLUtils.FreeLibrary(intPtr))
				{
					Log.Yim.Print("Error unloading User32.dll", new object[0]);
				}
			}
		}
	}

	// Token: 0x040007CD RID: 1997
	private const int MaxTouches = 5;

	// Token: 0x040007CE RID: 1998
	private const int MaxInitializationAttempts = 10;

	// Token: 0x040007CF RID: 1999
	private static W8Touch s_instance;

	// Token: 0x040007D0 RID: 2000
	public static bool s_initialized = false;

	// Token: 0x040007D1 RID: 2001
	public static bool s_isWindows8OrGreater = false;

	// Token: 0x040007D2 RID: 2002
	private static IntPtr s_DLL = IntPtr.Zero;

	// Token: 0x040007D3 RID: 2003
	private int m_intializationAttemptCount;

	// Token: 0x040007D4 RID: 2004
	private W8Touch.TouchState[] m_touchState;

	// Token: 0x040007D5 RID: 2005
	private Vector3 m_touchPosition = new Vector3(-1f, -1f, 0f);

	// Token: 0x040007D6 RID: 2006
	private Vector2 m_touchDelta = new Vector2(0f, 0f);

	// Token: 0x040007D7 RID: 2007
	private W8Touch.RECT m_desktopRect = default(W8Touch.RECT);

	// Token: 0x040007D8 RID: 2008
	private bool m_isVirtualKeyboardVisible;

	// Token: 0x040007D9 RID: 2009
	private bool m_isVirtualKeyboardShowRequested;

	// Token: 0x040007DA RID: 2010
	private bool m_isVirtualKeyboardHideRequested;

	// Token: 0x040007DB RID: 2011
	private W8Touch.PowerSource m_lastPowerSourceState = W8Touch.PowerSource.Unintialized;

	// Token: 0x040007DC RID: 2012
	private bool m_bWindowFeedbackSettingValue;

	// Token: 0x040007DD RID: 2013
	private bool m_bIsWindowFeedbackDisabled;

	// Token: 0x040007DE RID: 2014
	private static W8Touch.DelW8ShowKeyboard DLL_W8ShowKeyboard;

	// Token: 0x040007DF RID: 2015
	private static W8Touch.DelW8HideKeyboard DLL_W8HideKeyboard;

	// Token: 0x040007E0 RID: 2016
	private static W8Touch.DelW8ShowOSK DLL_W8ShowOSK;

	// Token: 0x040007E1 RID: 2017
	private static W8Touch.DelW8Initialize DLL_W8Initialize;

	// Token: 0x040007E2 RID: 2018
	private static W8Touch.DelW8Shutdown DLL_W8Shutdown;

	// Token: 0x040007E3 RID: 2019
	private static W8Touch.DelW8GetDeviceId DLL_W8GetDeviceId;

	// Token: 0x040007E4 RID: 2020
	private static W8Touch.DelW8IsWindows8OrGreater DLL_W8IsWindows8OrGreater;

	// Token: 0x040007E5 RID: 2021
	private static W8Touch.DelW8IsLastEventFromTouch DLL_W8IsLastEventFromTouch;

	// Token: 0x040007E6 RID: 2022
	private static W8Touch.DelW8GetBatteryMode DLL_W8GetBatteryMode;

	// Token: 0x040007E7 RID: 2023
	private static W8Touch.DelW8GetPercentBatteryLife DLL_W8GetPercentBatteryLife;

	// Token: 0x040007E8 RID: 2024
	private static W8Touch.DelW8GetDesktopRect DLL_W8GetDesktopRect;

	// Token: 0x040007E9 RID: 2025
	private static W8Touch.DelW8IsVirtualKeyboardVisible DLL_W8IsVirtualKeyboardVisible;

	// Token: 0x040007EA RID: 2026
	private static W8Touch.DelW8GetTouchPointCount DLL_W8GetTouchPointCount;

	// Token: 0x040007EB RID: 2027
	private static W8Touch.DelW8GetTouchPoint DLL_W8GetTouchPoint;

	// Token: 0x020004BB RID: 1211
	[StructLayout(0, Pack = 1)]
	public class tTouchData
	{
		// Token: 0x0400250F RID: 9487
		public int m_x;

		// Token: 0x04002510 RID: 9488
		public int m_y;

		// Token: 0x04002511 RID: 9489
		public int m_ID;

		// Token: 0x04002512 RID: 9490
		public int m_Time;
	}

	// Token: 0x020004BC RID: 1212
	public struct RECT
	{
		// Token: 0x04002513 RID: 9491
		public int Left;

		// Token: 0x04002514 RID: 9492
		public int Top;

		// Token: 0x04002515 RID: 9493
		public int Right;

		// Token: 0x04002516 RID: 9494
		public int Bottom;
	}

	// Token: 0x020004BD RID: 1213
	[Flags]
	public enum KeyboardFlags
	{
		// Token: 0x04002518 RID: 9496
		Shown = 1,
		// Token: 0x04002519 RID: 9497
		NotShown = 2,
		// Token: 0x0400251A RID: 9498
		SuccessTabTip = 4,
		// Token: 0x0400251B RID: 9499
		SuccessOSK = 8,
		// Token: 0x0400251C RID: 9500
		ErrorTabTip = 16,
		// Token: 0x0400251D RID: 9501
		ErrorOSK = 32,
		// Token: 0x0400251E RID: 9502
		NotFoundTabTip = 64,
		// Token: 0x0400251F RID: 9503
		NotFoundOSK = 128
	}

	// Token: 0x020004BE RID: 1214
	public enum TouchState
	{
		// Token: 0x04002521 RID: 9505
		None,
		// Token: 0x04002522 RID: 9506
		InitialDown,
		// Token: 0x04002523 RID: 9507
		Down,
		// Token: 0x04002524 RID: 9508
		InitialUp
	}

	// Token: 0x020004BF RID: 1215
	public class IntelDevice
	{
		// Token: 0x060039DC RID: 14812 RVA: 0x0011A08C File Offset: 0x0011828C
		public static string GetDeviceName(int deviceId)
		{
			string result;
			if (!W8Touch.IntelDevice.DeviceIdMap.TryGetValue(deviceId, out result))
			{
				return string.Empty;
			}
			return result;
		}

		// Token: 0x04002525 RID: 9509
		private static readonly Map<int, string> DeviceIdMap = new Map<int, string>
		{
			{
				30720,
				"Auburn"
			},
			{
				28961,
				"Whitney"
			},
			{
				28963,
				"Whitney"
			},
			{
				28965,
				"Whitney"
			},
			{
				4402,
				"Solono"
			},
			{
				9570,
				"Brookdale"
			},
			{
				13698,
				"Montara"
			},
			{
				9586,
				"Springdale"
			},
			{
				9602,
				"Grantsdale"
			},
			{
				10114,
				"Grantsdale"
			},
			{
				9618,
				"Alviso"
			},
			{
				10130,
				"Alviso"
			},
			{
				10098,
				"Lakeport-G"
			},
			{
				10102,
				"Lakeport-G"
			},
			{
				10146,
				"Calistoga"
			},
			{
				10150,
				"Calistoga"
			},
			{
				10626,
				"Broadwater-G"
			},
			{
				10627,
				"Broadwater-G"
			},
			{
				10610,
				"Broadwater-G"
			},
			{
				10611,
				"Broadwater-G"
			},
			{
				10642,
				"Broadwater-G"
			},
			{
				10643,
				"Broadwater-G"
			},
			{
				10658,
				"Broadwater-G"
			},
			{
				10659,
				"Broadwater-G"
			},
			{
				10754,
				"Crestline"
			},
			{
				10755,
				"Crestline"
			},
			{
				10770,
				"Crestline"
			},
			{
				10771,
				"Crestline"
			},
			{
				10674,
				"Bearlake"
			},
			{
				10675,
				"Bearlake"
			},
			{
				10690,
				"Bearlake"
			},
			{
				10691,
				"Bearlake"
			},
			{
				10706,
				"Bearlake"
			},
			{
				10707,
				"Bearlake"
			},
			{
				10818,
				"Cantiga"
			},
			{
				10819,
				"Cantiga"
			},
			{
				11778,
				"Eaglelake"
			},
			{
				11779,
				"Eaglelake"
			},
			{
				11810,
				"Eaglelake"
			},
			{
				11811,
				"Eaglelake"
			},
			{
				11794,
				"Eaglelake"
			},
			{
				11795,
				"Eaglelake"
			},
			{
				11826,
				"Eaglelake"
			},
			{
				11827,
				"Eaglelake"
			},
			{
				11842,
				"Eaglelake"
			},
			{
				11843,
				"Eaglelake"
			},
			{
				11922,
				"Eaglelake"
			},
			{
				11923,
				"Eaglelake"
			},
			{
				70,
				"Arrandale"
			},
			{
				66,
				"Clarkdale"
			},
			{
				262,
				"Mobile_SandyBridge_GT1"
			},
			{
				278,
				"Mobile_SandyBridge_GT2"
			},
			{
				294,
				"Mobile_SandyBridge_GT2+"
			},
			{
				258,
				"DT_SandyBridge_GT2+"
			},
			{
				274,
				"DT_SandyBridge_GT2+"
			},
			{
				290,
				"DT_SandyBridge_GT2+"
			},
			{
				266,
				"SandyBridge_Server"
			},
			{
				270,
				"SandyBridge_Reserved"
			},
			{
				338,
				"Desktop_IvyBridge_GT1"
			},
			{
				342,
				"Mobile_IvyBridge_GT1"
			},
			{
				346,
				"Server_IvyBridge_GT1"
			},
			{
				350,
				"Reserved_IvyBridge_GT1"
			},
			{
				354,
				"Desktop_IvyBridge_GT2"
			},
			{
				358,
				"Mobile_IvyBridge_GT2"
			},
			{
				362,
				"Server_IvyBridge_GT2"
			},
			{
				1026,
				"Desktop_Haswell_GT1_Y6W"
			},
			{
				1030,
				"Mobile_Haswell_GT1_Y6W"
			},
			{
				1034,
				"Server_Haswell_GT1"
			},
			{
				1042,
				"Desktop_Haswell_GT2_U15W"
			},
			{
				1046,
				"Mobile_Haswell_GT2_U15W"
			},
			{
				1051,
				"Workstation_Haswell_GT2"
			},
			{
				1050,
				"Server_Haswell_GT2"
			},
			{
				1054,
				"Reserved_Haswell_DT_GT1.5_U15W"
			},
			{
				2566,
				"Mobile_Haswell_ULT_GT1_Y6W"
			},
			{
				2574,
				"Mobile_Haswell_ULX_GT1_Y6W"
			},
			{
				2582,
				"Mobile_Haswell_ULT_GT2_U15W"
			},
			{
				2590,
				"Mobile_Haswell_ULX_GT2_Y6W"
			},
			{
				2598,
				"Mobile_Haswell_ULT_GT3_U28W"
			},
			{
				2606,
				"Mobile_Haswell_ULT_GT3@28_U28W"
			},
			{
				3346,
				"Desktop_Haswell_GT2F"
			},
			{
				3350,
				"Mobile_Haswell_GT2F"
			},
			{
				3362,
				"Desktop_Crystal-Well_GT3"
			},
			{
				3366,
				"Mobile_Crystal-Well_GT3"
			},
			{
				3370,
				"Server_Crystal-Well_GT3"
			},
			{
				3889,
				"BayTrail"
			},
			{
				33032,
				"Poulsbo"
			},
			{
				33033,
				"Poulsbo"
			},
			{
				2255,
				"CloverTrail"
			},
			{
				40961,
				"CloverTrail"
			},
			{
				40962,
				"CloverTrail"
			},
			{
				40977,
				"CloverTrail"
			},
			{
				40978,
				"CloverTrail"
			}
		};
	}

	// Token: 0x020004C0 RID: 1216
	public enum PowerSource
	{
		// Token: 0x04002527 RID: 9511
		Unintialized = -1,
		// Token: 0x04002528 RID: 9512
		BatteryPower,
		// Token: 0x04002529 RID: 9513
		ACPower,
		// Token: 0x0400252A RID: 9514
		UndefinedPower = 255
	}

	// Token: 0x020004C1 RID: 1217
	public enum FEEDBACK_TYPE
	{
		// Token: 0x0400252C RID: 9516
		FEEDBACK_TOUCH_CONTACTVISUALIZATION = 1,
		// Token: 0x0400252D RID: 9517
		FEEDBACK_PEN_BARRELVISUALIZATION,
		// Token: 0x0400252E RID: 9518
		FEEDBACK_PEN_TAP,
		// Token: 0x0400252F RID: 9519
		FEEDBACK_PEN_DOUBLETAP,
		// Token: 0x04002530 RID: 9520
		FEEDBACK_PEN_PRESSANDHOLD,
		// Token: 0x04002531 RID: 9521
		FEEDBACK_PEN_RIGHTTAP,
		// Token: 0x04002532 RID: 9522
		FEEDBACK_TOUCH_TAP,
		// Token: 0x04002533 RID: 9523
		FEEDBACK_TOUCH_DOUBLETAP,
		// Token: 0x04002534 RID: 9524
		FEEDBACK_TOUCH_PRESSANDHOLD,
		// Token: 0x04002535 RID: 9525
		FEEDBACK_TOUCH_RIGHTTAP,
		// Token: 0x04002536 RID: 9526
		FEEDBACK_GESTURE_PRESSANDTAP
	}

	// Token: 0x020004C2 RID: 1218
	// (Invoke) Token: 0x060039DE RID: 14814
	[UnmanagedFunctionPointer(2)]
	private delegate int DelW8ShowKeyboard();

	// Token: 0x020004C3 RID: 1219
	// (Invoke) Token: 0x060039E2 RID: 14818
	[UnmanagedFunctionPointer(2)]
	private delegate int DelW8HideKeyboard();

	// Token: 0x020004C4 RID: 1220
	// (Invoke) Token: 0x060039E6 RID: 14822
	[UnmanagedFunctionPointer(2)]
	private delegate int DelW8ShowOSK();

	// Token: 0x020004C5 RID: 1221
	// (Invoke) Token: 0x060039EA RID: 14826
	[UnmanagedFunctionPointer(3, CharSet = 4)]
	private delegate int DelW8Initialize(string windowName);

	// Token: 0x020004C6 RID: 1222
	// (Invoke) Token: 0x060039EE RID: 14830
	[UnmanagedFunctionPointer(2)]
	private delegate void DelW8Shutdown();

	// Token: 0x020004C7 RID: 1223
	// (Invoke) Token: 0x060039F2 RID: 14834
	[UnmanagedFunctionPointer(2)]
	private delegate int DelW8GetDeviceId();

	// Token: 0x020004C8 RID: 1224
	// (Invoke) Token: 0x060039F6 RID: 14838
	[UnmanagedFunctionPointer(2)]
	private delegate bool DelW8IsWindows8OrGreater();

	// Token: 0x020004C9 RID: 1225
	// (Invoke) Token: 0x060039FA RID: 14842
	[UnmanagedFunctionPointer(2)]
	private delegate bool DelW8IsLastEventFromTouch();

	// Token: 0x020004CA RID: 1226
	// (Invoke) Token: 0x060039FE RID: 14846
	[UnmanagedFunctionPointer(2)]
	private delegate int DelW8GetBatteryMode();

	// Token: 0x020004CB RID: 1227
	// (Invoke) Token: 0x06003A02 RID: 14850
	[UnmanagedFunctionPointer(2)]
	private delegate int DelW8GetPercentBatteryLife();

	// Token: 0x020004CC RID: 1228
	// (Invoke) Token: 0x06003A06 RID: 14854
	[UnmanagedFunctionPointer(2)]
	private delegate void DelW8GetDesktopRect(out W8Touch.RECT desktopRect);

	// Token: 0x020004CD RID: 1229
	// (Invoke) Token: 0x06003A0A RID: 14858
	[UnmanagedFunctionPointer(2)]
	private delegate bool DelW8IsVirtualKeyboardVisible();

	// Token: 0x020004CE RID: 1230
	// (Invoke) Token: 0x06003A0E RID: 14862
	[UnmanagedFunctionPointer(2)]
	private delegate int DelW8GetTouchPointCount();

	// Token: 0x020004CF RID: 1231
	// (Invoke) Token: 0x06003A12 RID: 14866
	[UnmanagedFunctionPointer(2)]
	private delegate bool DelW8GetTouchPoint(int i, W8Touch.tTouchData n);

	// Token: 0x020004D0 RID: 1232
	// (Invoke) Token: 0x06003A16 RID: 14870
	[UnmanagedFunctionPointer(2)]
	private delegate bool DelSetWindowFeedbackSetting(IntPtr hwnd, W8Touch.FEEDBACK_TYPE feedback, uint dwFlags, uint size, IntPtr configuration);
}
