using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004B8 RID: 1208
public class ScreenResizeDetector : MonoBehaviour
{
	// Token: 0x060039BA RID: 14778 RVA: 0x00118D36 File Offset: 0x00116F36
	private void Awake()
	{
		this.SaveScreenSize();
	}

	// Token: 0x060039BB RID: 14779 RVA: 0x00118D40 File Offset: 0x00116F40
	private void OnPreCull()
	{
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (object.Equals(this.m_screenWidth, num) && object.Equals(this.m_screenHeight, num2))
		{
			return;
		}
		this.SaveScreenSize();
		this.FireSizeChangedEvent();
	}

	// Token: 0x060039BC RID: 14780 RVA: 0x00118D9E File Offset: 0x00116F9E
	public bool AddSizeChangedListener(ScreenResizeDetector.SizeChangedCallback callback)
	{
		return this.AddSizeChangedListener(callback, null);
	}

	// Token: 0x060039BD RID: 14781 RVA: 0x00118DA8 File Offset: 0x00116FA8
	public bool AddSizeChangedListener(ScreenResizeDetector.SizeChangedCallback callback, object userData)
	{
		ScreenResizeDetector.SizeChangedListener sizeChangedListener = new ScreenResizeDetector.SizeChangedListener();
		sizeChangedListener.SetCallback(callback);
		sizeChangedListener.SetUserData(userData);
		if (this.m_sizeChangedListeners.Contains(sizeChangedListener))
		{
			return false;
		}
		this.m_sizeChangedListeners.Add(sizeChangedListener);
		return true;
	}

	// Token: 0x060039BE RID: 14782 RVA: 0x00118DE9 File Offset: 0x00116FE9
	public bool RemoveSizeChangedListener(ScreenResizeDetector.SizeChangedCallback callback)
	{
		return this.RemoveSizeChangedListener(callback, null);
	}

	// Token: 0x060039BF RID: 14783 RVA: 0x00118DF4 File Offset: 0x00116FF4
	public bool RemoveSizeChangedListener(ScreenResizeDetector.SizeChangedCallback callback, object userData)
	{
		ScreenResizeDetector.SizeChangedListener sizeChangedListener = new ScreenResizeDetector.SizeChangedListener();
		sizeChangedListener.SetCallback(callback);
		sizeChangedListener.SetUserData(userData);
		return this.m_sizeChangedListeners.Remove(sizeChangedListener);
	}

	// Token: 0x060039C0 RID: 14784 RVA: 0x00118E21 File Offset: 0x00117021
	private void SaveScreenSize()
	{
		this.m_screenWidth = (float)Screen.width;
		this.m_screenHeight = (float)Screen.height;
	}

	// Token: 0x060039C1 RID: 14785 RVA: 0x00118E3C File Offset: 0x0011703C
	private void FireSizeChangedEvent()
	{
		ScreenResizeDetector.SizeChangedListener[] array = this.m_sizeChangedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire();
		}
	}

	// Token: 0x040024F7 RID: 9463
	private float m_screenWidth;

	// Token: 0x040024F8 RID: 9464
	private float m_screenHeight;

	// Token: 0x040024F9 RID: 9465
	private List<ScreenResizeDetector.SizeChangedListener> m_sizeChangedListeners = new List<ScreenResizeDetector.SizeChangedListener>();

	// Token: 0x020004B9 RID: 1209
	// (Invoke) Token: 0x060039C3 RID: 14787
	public delegate void SizeChangedCallback(object userData);

	// Token: 0x02000F36 RID: 3894
	private class SizeChangedListener : EventListener<ScreenResizeDetector.SizeChangedCallback>
	{
		// Token: 0x060073CA RID: 29642 RVA: 0x00221A0D File Offset: 0x0021FC0D
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}
}
