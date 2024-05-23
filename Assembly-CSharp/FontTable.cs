using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class FontTable : MonoBehaviour
{
	// Token: 0x06001DFB RID: 7675 RVA: 0x0008B810 File Offset: 0x00089A10
	private void Awake()
	{
		FontTable.s_instance = this;
		if (ApplicationMgr.Get() != null)
		{
			ApplicationMgr.Get().WillReset += new Action(this.WillReset);
		}
	}

	// Token: 0x06001DFC RID: 7676 RVA: 0x0008B84C File Offset: 0x00089A4C
	private void OnDestroy()
	{
		if (ApplicationMgr.Get() != null)
		{
			ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		}
		FontTable.s_instance = null;
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x0008B885 File Offset: 0x00089A85
	public static FontTable Get()
	{
		return FontTable.s_instance;
	}

	// Token: 0x06001DFE RID: 7678 RVA: 0x0008B88C File Offset: 0x00089A8C
	public FontDef GetFontDef(Font enUSFont)
	{
		string fontDefName = this.GetFontDefName(enUSFont);
		return this.GetFontDef(fontDefName);
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x0008B8A8 File Offset: 0x00089AA8
	public FontDef GetFontDef(string name)
	{
		FontDef result = null;
		this.m_defs.TryGetValue(name, out result);
		return result;
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x0008B8C8 File Offset: 0x00089AC8
	public void Reset()
	{
		foreach (KeyValuePair<string, FontDef> keyValuePair in this.m_defs)
		{
			if (!(keyValuePair.Value == null))
			{
				Object.Destroy(keyValuePair.Value.gameObject);
			}
		}
		this.m_defs.Clear();
		foreach (Transform transform in base.GetComponentsInChildren<Transform>())
		{
			if (!(transform == base.transform))
			{
				Object.Destroy(transform.gameObject);
			}
		}
		this.Initialize();
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x0008B99C File Offset: 0x00089B9C
	public bool IsInitialized()
	{
		return this.m_initialized;
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x0008B9A4 File Offset: 0x00089BA4
	public void Initialize()
	{
		this.m_initialized = false;
		this.m_initialDefsLoading = this.m_Entries.Count;
		if (this.m_initialDefsLoading == 0)
		{
			this.FinishInitialization();
			return;
		}
		foreach (FontTableEntry fontTableEntry in this.m_Entries)
		{
			AssetLoader.Get().LoadFontDef(fontTableEntry.m_FontDefName, new AssetLoader.GameObjectCallback(this.OnFontDefLoaded), null, false);
		}
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x0008BA40 File Offset: 0x00089C40
	public void AddInitializedCallback(FontTable.InitializedCallback callback)
	{
		this.AddInitializedCallback(callback, null);
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x0008BA4C File Offset: 0x00089C4C
	public void AddInitializedCallback(FontTable.InitializedCallback callback, object userData)
	{
		FontTable.InitializedListener initializedListener = new FontTable.InitializedListener();
		initializedListener.SetCallback(callback);
		initializedListener.SetUserData(userData);
		if (this.m_initializedListeners.Contains(initializedListener))
		{
			return;
		}
		this.m_initializedListeners.Add(initializedListener);
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x0008BA8B File Offset: 0x00089C8B
	public bool RemoveInitializedCallback(FontTable.InitializedCallback callback)
	{
		return this.RemoveInitializedCallback(callback, null);
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x0008BA98 File Offset: 0x00089C98
	public bool RemoveInitializedCallback(FontTable.InitializedCallback callback, object userData)
	{
		FontTable.InitializedListener initializedListener = new FontTable.InitializedListener();
		initializedListener.SetCallback(callback);
		initializedListener.SetUserData(userData);
		return this.m_initializedListeners.Remove(initializedListener);
	}

	// Token: 0x06001E07 RID: 7687 RVA: 0x0008BAC8 File Offset: 0x00089CC8
	private void OnFontDefLoaded(string name, GameObject go, object userData)
	{
		Log.Kyle.Print("OnFontDefLoaded", new object[0]);
		if (go == null)
		{
			this.OnInitialDefLoaded();
			return;
		}
		FontDef component = go.GetComponent<FontDef>();
		if (component == null)
		{
			string text = GameStrings.Format("GLOBAL_ERROR_ASSET_INCORRECT_DATA", new object[]
			{
				name
			});
			Error.AddFatal(text);
			string text2 = string.Format("FontTable.OnFontDefLoaded() - name={0} message={1}", name, text);
			Debug.LogError(text2);
			this.OnInitialDefLoaded();
			return;
		}
		component.transform.parent = base.transform;
		this.m_defs.Add(name, component);
		this.OnInitialDefLoaded();
	}

	// Token: 0x06001E08 RID: 7688 RVA: 0x0008BB69 File Offset: 0x00089D69
	private void OnInitialDefLoaded()
	{
		this.m_initialDefsLoading--;
		if (this.m_initialDefsLoading > 0)
		{
			return;
		}
		this.FinishInitialization();
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x0008BB8C File Offset: 0x00089D8C
	private void FinishInitialization()
	{
		this.m_initialized = true;
		this.FireInitializedCallbacks();
	}

	// Token: 0x06001E0A RID: 7690 RVA: 0x0008BB9C File Offset: 0x00089D9C
	private void FireInitializedCallbacks()
	{
		FontTable.InitializedListener[] array = this.m_initializedListeners.ToArray();
		this.m_initializedListeners.Clear();
		foreach (FontTable.InitializedListener initializedListener in array)
		{
			initializedListener.Fire();
		}
	}

	// Token: 0x06001E0B RID: 7691 RVA: 0x0008BBE0 File Offset: 0x00089DE0
	private string GetFontDefName(Font font)
	{
		if (font == null)
		{
			return null;
		}
		return this.GetFontDefName(font.name);
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x0008BBFC File Offset: 0x00089DFC
	private string GetFontDefName(string fontName)
	{
		foreach (FontTableEntry fontTableEntry in this.m_Entries)
		{
			if (fontTableEntry.m_FontName == fontName)
			{
				return fontTableEntry.m_FontDefName;
			}
		}
		return null;
	}

	// Token: 0x06001E0D RID: 7693 RVA: 0x0008BC70 File Offset: 0x00089E70
	private void WillReset()
	{
		this.m_defs.Clear();
	}

	// Token: 0x040010C1 RID: 4289
	public List<FontTableEntry> m_Entries;

	// Token: 0x040010C2 RID: 4290
	private static FontTable s_instance;

	// Token: 0x040010C3 RID: 4291
	private bool m_initialized;

	// Token: 0x040010C4 RID: 4292
	private List<FontTable.InitializedListener> m_initializedListeners = new List<FontTable.InitializedListener>();

	// Token: 0x040010C5 RID: 4293
	private int m_initialDefsLoading;

	// Token: 0x040010C6 RID: 4294
	private Map<string, FontDef> m_defs = new Map<string, FontDef>();

	// Token: 0x02000285 RID: 645
	// (Invoke) Token: 0x06002386 RID: 9094
	public delegate void InitializedCallback(object userData);

	// Token: 0x020004B1 RID: 1201
	private class InitializedListener : EventListener<FontTable.InitializedCallback>
	{
		// Token: 0x06003985 RID: 14725 RVA: 0x00117478 File Offset: 0x00115678
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}
}
