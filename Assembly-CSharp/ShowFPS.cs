using System;
using UnityEngine;

// Token: 0x02000EE6 RID: 3814
[ExecuteInEditMode]
public class ShowFPS : MonoBehaviour
{
	// Token: 0x0600722E RID: 29230 RVA: 0x00218F5E File Offset: 0x0021715E
	private void Awake()
	{
		ShowFPS.s_instance = this;
		if (ApplicationMgr.IsPublic())
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}

	// Token: 0x0600722F RID: 29231 RVA: 0x00218F7B File Offset: 0x0021717B
	private void OnDestroy()
	{
		ShowFPS.s_instance = null;
	}

	// Token: 0x06007230 RID: 29232 RVA: 0x00218F83 File Offset: 0x00217183
	public static ShowFPS Get()
	{
		return ShowFPS.s_instance;
	}

	// Token: 0x06007231 RID: 29233 RVA: 0x00218F8C File Offset: 0x0021718C
	[ContextMenu("Start Frame Count")]
	public void StartFrameCount()
	{
		this.m_FrameCountLastTime = Time.realtimeSinceStartup;
		this.m_FrameCountTime = 0f;
		this.m_FrameCount = 0;
		this.m_FrameCountActive = true;
	}

	// Token: 0x06007232 RID: 29234 RVA: 0x00218FBD File Offset: 0x002171BD
	[ContextMenu("Stop Frame Count")]
	public void StopFrameCount()
	{
		this.m_FrameCountActive = false;
	}

	// Token: 0x06007233 RID: 29235 RVA: 0x00218FC8 File Offset: 0x002171C8
	[ContextMenu("Clear Frame Count")]
	public void ClearFrameCount()
	{
		this.m_FrameCountLastTime = 0f;
		this.m_FrameCountTime = 0f;
		this.m_FrameCount = 0;
		this.m_FrameCountActive = false;
	}

	// Token: 0x06007234 RID: 29236 RVA: 0x00218FFC File Offset: 0x002171FC
	private void Start()
	{
		this.m_LastInterval = (double)Time.realtimeSinceStartup;
		this.frames = 0;
		this.UpdateEnabled();
		Options.Get().RegisterChangedListener(Option.HUD, new Options.ChangedCallback(this.OnHudOptionChanged));
	}

	// Token: 0x06007235 RID: 29237 RVA: 0x0021903C File Offset: 0x0021723C
	private void OnDisable()
	{
		if (this.m_GuiText)
		{
			Object.DestroyImmediate(this.m_GuiText.gameObject);
		}
		Time.captureFramerate = 0;
	}

	// Token: 0x06007236 RID: 29238 RVA: 0x00219070 File Offset: 0x00217270
	private void Update()
	{
		bool flag = false;
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			FullScreenEffects component = camera.GetComponent<FullScreenEffects>();
			if (!(component == null))
			{
				if (component.enabled)
				{
					flag = true;
				}
			}
		}
		if (!this.m_GuiText)
		{
			GameObject gameObject = new GameObject("FPS");
			gameObject.transform.position = Vector3.zero;
			this.m_GuiText = gameObject.AddComponent<GUIText>();
			SceneUtils.SetHideFlags(gameObject, 61);
			this.m_GuiText.pixelOffset = new Vector2((float)Screen.width * 0.7f, 15f);
		}
		this.frames++;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if ((double)realtimeSinceStartup > this.m_LastInterval + (double)this.m_UpdateInterval)
		{
			float num = (float)this.frames / (float)((double)realtimeSinceStartup - this.m_LastInterval);
			if (this.m_verbose)
			{
				this.m_fpsText = string.Format("{0:f2} - {1} frames over {2}sec", num, this.frames, this.m_UpdateInterval);
			}
			else
			{
				this.m_fpsText = string.Format("{0:f2}", num);
			}
			this.frames = 0;
			this.m_LastInterval = (double)realtimeSinceStartup;
		}
		string text = this.m_fpsText;
		if (this.m_FrameCountActive || this.m_FrameCount > 0)
		{
			if (this.m_FrameCountActive)
			{
				this.m_FrameCountTime += (realtimeSinceStartup - this.m_FrameCountLastTime) / 60f * Time.timeScale;
				if (this.m_FrameCountLastTime == 0f)
				{
					this.m_FrameCountLastTime = realtimeSinceStartup;
				}
				this.m_FrameCount = Mathf.CeilToInt(this.m_FrameCountTime * 60f);
			}
			text = string.Format("{0} - Frame Count: {1}", text, this.m_FrameCount);
		}
		if (flag)
		{
			text = string.Format("{0} - FSE", text);
		}
		if (ScreenEffectsMgr.Get() != null)
		{
			int activeScreenEffectsCount = ScreenEffectsMgr.Get().GetActiveScreenEffectsCount();
			if (activeScreenEffectsCount > 0 && ScreenEffectsMgr.Get().gameObject.activeSelf)
			{
				text = string.Format("{0} - ScreenEffects Active: {1}", text, activeScreenEffectsCount);
			}
		}
		this.m_GuiText.text = text;
	}

	// Token: 0x06007237 RID: 29239 RVA: 0x002192D6 File Offset: 0x002174D6
	private void OnHudOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		this.UpdateEnabled();
	}

	// Token: 0x06007238 RID: 29240 RVA: 0x002192DE File Offset: 0x002174DE
	private void UpdateEnabled()
	{
		base.enabled = Options.Get().GetBool(Option.HUD);
	}

	// Token: 0x04005C48 RID: 23624
	private GUIText m_GuiText;

	// Token: 0x04005C49 RID: 23625
	private float m_UpdateInterval = 0.5f;

	// Token: 0x04005C4A RID: 23626
	private double m_LastInterval;

	// Token: 0x04005C4B RID: 23627
	private int frames;

	// Token: 0x04005C4C RID: 23628
	private bool m_FrameCountActive;

	// Token: 0x04005C4D RID: 23629
	private float m_FrameCountTime;

	// Token: 0x04005C4E RID: 23630
	private float m_FrameCountLastTime;

	// Token: 0x04005C4F RID: 23631
	private int m_FrameCount;

	// Token: 0x04005C50 RID: 23632
	private bool m_verbose;

	// Token: 0x04005C51 RID: 23633
	private string m_fpsText;

	// Token: 0x04005C52 RID: 23634
	private static ShowFPS s_instance;
}
