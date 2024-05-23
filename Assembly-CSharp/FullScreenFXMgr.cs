using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002BB RID: 699
public class FullScreenFXMgr : MonoBehaviour
{
	// Token: 0x060025AA RID: 9642 RVA: 0x000B8B26 File Offset: 0x000B6D26
	private void Awake()
	{
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
		FullScreenFXMgr.s_instance = this;
	}

	// Token: 0x060025AB RID: 9643 RVA: 0x000B8B44 File Offset: 0x000B6D44
	private void OnDestroy()
	{
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		FullScreenFXMgr.s_instance = null;
	}

	// Token: 0x060025AC RID: 9644 RVA: 0x000B8B64 File Offset: 0x000B6D64
	public void WillReset()
	{
		this.m_ActiveEffectsCount = 0;
		Camera main = Camera.main;
		if (main == null)
		{
			return;
		}
		FullScreenEffects component = main.GetComponent<FullScreenEffects>();
		if (component == null)
		{
			return;
		}
		component.BlurEnabled = false;
		component.VignettingEnable = false;
		component.DesaturationEnabled = false;
		component.BlendToColorEnable = false;
		this.m_StdBlurVignetteCount = 0;
	}

	// Token: 0x060025AD RID: 9645 RVA: 0x000B8BC2 File Offset: 0x000B6DC2
	public static FullScreenFXMgr Get()
	{
		return FullScreenFXMgr.s_instance;
	}

	// Token: 0x060025AE RID: 9646 RVA: 0x000B8BC9 File Offset: 0x000B6DC9
	public bool isFullScreenEffectActive()
	{
		return !(this.m_FullScreenEffects == null) && this.m_FullScreenEffects.isActive();
	}

	// Token: 0x060025AF RID: 9647 RVA: 0x000B8BEC File Offset: 0x000B6DEC
	private void BeginEffect(string name, string onUpdate, string onComplete, float start, float end, float time, iTween.EaseType easeType)
	{
		Log.FullScreenFX.Print(string.Concat(new object[]
		{
			"BeginEffect ",
			name,
			" ",
			start,
			" => ",
			end
		}), new object[0]);
		Hashtable hashtable = new Hashtable();
		hashtable["name"] = name;
		hashtable["onupdate"] = onUpdate;
		hashtable["onupdatetarget"] = base.gameObject;
		hashtable["from"] = start;
		if (!string.IsNullOrEmpty(onComplete))
		{
			hashtable["oncomplete"] = onComplete;
			hashtable["oncompletetarget"] = base.gameObject;
		}
		hashtable["to"] = end;
		hashtable["time"] = time;
		hashtable["easetype"] = easeType;
		iTween.StopByName(base.gameObject, name);
		iTween.ValueTo(base.gameObject, hashtable);
	}

	// Token: 0x060025B0 RID: 9648 RVA: 0x000B8CFC File Offset: 0x000B6EFC
	public void StopAllEffects(float delay = 0f)
	{
		this.GetCurrEffects();
		if (this.m_FullScreenEffects == null || !this.m_FullScreenEffects.isActive())
		{
			return;
		}
		Log.FullScreenFX.Print("StopAllEffects", new object[0]);
		base.StartCoroutine(this.StopAllEffectsCoroutine(this.m_FullScreenEffects, delay));
	}

	// Token: 0x060025B1 RID: 9649 RVA: 0x000B8D5C File Offset: 0x000B6F5C
	private IEnumerator StopAllEffectsCoroutine(FullScreenEffects effects, float delay)
	{
		float stopEffectsTime = 0.25f;
		yield return new WaitForSeconds(delay);
		Log.FullScreenFX.Print("StopAllEffectsCoroutine stopping effects now", new object[0]);
		if (effects == null)
		{
			yield break;
		}
		if (effects.BlurEnabled)
		{
			this.StopBlur(stopEffectsTime, iTween.EaseType.linear, null);
		}
		if (effects.VignettingEnable)
		{
			this.StopVignette(stopEffectsTime, iTween.EaseType.linear, null);
		}
		if (effects.BlendToColorEnable)
		{
			this.StopBlendToColor(stopEffectsTime, iTween.EaseType.linear, null);
		}
		if (effects.DesaturationEnabled)
		{
			this.StopDesaturate(stopEffectsTime, iTween.EaseType.linear, null);
		}
		this.m_StdBlurVignetteCount = 0;
		yield return new WaitForSeconds(stopEffectsTime);
		if (effects == null)
		{
			yield break;
		}
		effects.Disable();
		yield break;
	}

	// Token: 0x060025B2 RID: 9650 RVA: 0x000B8D94 File Offset: 0x000B6F94
	public void Vignette(float endVal, float time, iTween.EaseType easeType, FullScreenFXMgr.EffectListener listener = null)
	{
		this.GetCurrEffects();
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.VignettingEnable = true;
		this.m_vignetteListener = listener;
		this.BeginEffect("vignette", "OnVignette", "OnVignetteComplete", 0f, endVal, time, easeType);
	}

	// Token: 0x060025B3 RID: 9651 RVA: 0x000B8DEC File Offset: 0x000B6FEC
	public void StopVignette(float time, iTween.EaseType easeType, FullScreenFXMgr.EffectListener listener = null)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		if (!this.m_FullScreenEffects.isActive())
		{
			return;
		}
		this.m_FullScreenEffects.VignettingEnable = true;
		this.m_vignetteListener = listener;
		this.BeginEffect("vignette", "OnVignette", "OnVignetteClear", this.m_FullScreenEffects.VignettingIntensity, 0f, time, easeType);
	}

	// Token: 0x060025B4 RID: 9652 RVA: 0x000B8E56 File Offset: 0x000B7056
	public void OnVignette(float val)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.VignettingIntensity = val;
	}

	// Token: 0x060025B5 RID: 9653 RVA: 0x000B8E76 File Offset: 0x000B7076
	public void OnVignetteComplete()
	{
		if (this.m_vignetteListener != null)
		{
			this.m_vignetteListener();
		}
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x000B8E90 File Offset: 0x000B7090
	public void OnVignetteClear()
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.VignettingEnable = false;
		this.OnVignetteComplete();
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x000B8EC4 File Offset: 0x000B70C4
	public void Desaturate(float endVal, float time, iTween.EaseType easeType, FullScreenFXMgr.EffectListener listener = null)
	{
		this.GetCurrEffects();
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.DesaturationEnabled = true;
		this.m_desatListener = listener;
		this.BeginEffect("desat", "OnDesat", "OnDesatComplete", this.m_FullScreenEffects.Desaturation, endVal, time, easeType);
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x000B8F24 File Offset: 0x000B7124
	public void StopDesaturate(float time, iTween.EaseType easeType, FullScreenFXMgr.EffectListener listener = null)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		if (!this.m_FullScreenEffects.isActive())
		{
			return;
		}
		this.m_FullScreenEffects.DesaturationEnabled = true;
		this.m_desatListener = listener;
		this.BeginEffect("desat", "OnDesat", "OnDesatClear", this.m_FullScreenEffects.Desaturation, 0f, time, easeType);
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x000B8F8E File Offset: 0x000B718E
	public void OnDesat(float val)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.Desaturation = val;
	}

	// Token: 0x060025BA RID: 9658 RVA: 0x000B8FAE File Offset: 0x000B71AE
	public void OnDesatComplete()
	{
		if (this.m_desatListener != null)
		{
			this.m_desatListener();
		}
	}

	// Token: 0x060025BB RID: 9659 RVA: 0x000B8FC8 File Offset: 0x000B71C8
	public void OnDesatClear()
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.DesaturationEnabled = false;
		this.OnDesatComplete();
	}

	// Token: 0x060025BC RID: 9660 RVA: 0x000B8FF9 File Offset: 0x000B71F9
	public void SetBlurAmount(float val)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.BlurAmount = val;
	}

	// Token: 0x060025BD RID: 9661 RVA: 0x000B9019 File Offset: 0x000B7219
	public void SetBlurBrightness(float val)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.BlurBrightness = val;
	}

	// Token: 0x060025BE RID: 9662 RVA: 0x000B9039 File Offset: 0x000B7239
	public void SetBlurDesaturation(float val)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.BlurDesaturation = val;
	}

	// Token: 0x060025BF RID: 9663 RVA: 0x000B905C File Offset: 0x000B725C
	public void Blur(float blurVal, float time, iTween.EaseType easeType, FullScreenFXMgr.EffectListener listener = null)
	{
		this.m_ActiveEffectsCount++;
		this.GetCurrEffects();
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.BlurEnabled = true;
		this.m_blurListener = listener;
		this.BeginEffect("blur", "OnBlur", "OnBlurComplete", this.m_FullScreenEffects.BlurBlend, blurVal, time, easeType);
	}

	// Token: 0x060025C0 RID: 9664 RVA: 0x000B90C8 File Offset: 0x000B72C8
	public void StopBlur(float time, iTween.EaseType easeType, FullScreenFXMgr.EffectListener listener = null)
	{
		this.m_ActiveEffectsCount--;
		if (this.m_ActiveEffectsCount > 0)
		{
			return;
		}
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		if (!this.m_FullScreenEffects.isActive())
		{
			return;
		}
		this.m_FullScreenEffects.BlurEnabled = true;
		this.m_blurListener = listener;
		this.BeginEffect("blur", "OnBlur", "OnBlurClear", this.m_FullScreenEffects.BlurBlend, 0f, time, easeType);
	}

	// Token: 0x060025C1 RID: 9665 RVA: 0x000B914D File Offset: 0x000B734D
	public void DisableBlur()
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.BlurEnabled = false;
		this.m_FullScreenEffects.BlurBlend = 0f;
		this.m_FullScreenEffects.BlurAmount = 0f;
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x000B918D File Offset: 0x000B738D
	public void OnBlur(float val)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.BlurBlend = val;
	}

	// Token: 0x060025C3 RID: 9667 RVA: 0x000B91AD File Offset: 0x000B73AD
	public void OnBlurComplete()
	{
		if (this.m_blurListener != null)
		{
			this.m_blurListener();
		}
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x000B91C8 File Offset: 0x000B73C8
	public void OnBlurClear()
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.BlurEnabled = false;
		this.OnBlurComplete();
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x000B91FC File Offset: 0x000B73FC
	public void BlendToColor(Color blendColor, float endVal, float time, iTween.EaseType easeType, FullScreenFXMgr.EffectListener listener = null)
	{
		this.GetCurrEffects();
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.enabled = true;
		this.m_FullScreenEffects.BlendToColorEnable = true;
		this.m_FullScreenEffects.BlendToColor = blendColor;
		this.m_blendToColorListener = listener;
		this.BeginEffect("blendtocolor", "OnBlendToColor", "OnBlendToColorComplete", 0f, endVal, time, easeType);
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x000B926C File Offset: 0x000B746C
	public void StopBlendToColor(float time, iTween.EaseType easeType, FullScreenFXMgr.EffectListener listener = null)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		if (!this.m_FullScreenEffects.isActive())
		{
			return;
		}
		this.m_FullScreenEffects.BlendToColorEnable = true;
		this.m_blendToColorListener = listener;
		this.BeginEffect("blendtocolor", "OnBlendToColor", "OnBlendToColorClear", this.m_FullScreenEffects.BlendToColorAmount, 0f, time, easeType);
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x000B92D6 File Offset: 0x000B74D6
	public void OnBlendToColor(float val)
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.BlendToColorAmount = val;
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x000B92F6 File Offset: 0x000B74F6
	public void OnBlendToColorComplete()
	{
		if (this.m_blendToColorListener != null)
		{
			this.m_blendToColorListener();
		}
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x000B9310 File Offset: 0x000B7510
	public void OnBlendToColorClear()
	{
		if (this.m_FullScreenEffects == null)
		{
			return;
		}
		this.m_FullScreenEffects.BlendToColorEnable = false;
		this.OnBlendToColorComplete();
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x000B9344 File Offset: 0x000B7544
	public void StartStandardBlurVignette(float time)
	{
		if (this.m_StdBlurVignetteCount == 0)
		{
			this.SetBlurBrightness(1f);
			this.SetBlurDesaturation(0f);
			this.Vignette(0.4f, time, iTween.EaseType.easeOutCirc, null);
			this.Blur(1f, time, iTween.EaseType.easeOutCirc, null);
		}
		this.m_StdBlurVignetteCount++;
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x000B939E File Offset: 0x000B759E
	public void EndStandardBlurVignette(float time, FullScreenFXMgr.EffectListener listener = null)
	{
		if (this.m_StdBlurVignetteCount == 0)
		{
			return;
		}
		this.m_StdBlurVignetteCount--;
		if (this.m_StdBlurVignetteCount == 0)
		{
			this.StopBlur(time, iTween.EaseType.easeOutCirc, null);
			this.StopVignette(time, iTween.EaseType.easeOutCirc, listener);
		}
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000B93DC File Offset: 0x000B75DC
	private FullScreenEffects GetCurrEffects()
	{
		if (Camera.main == null)
		{
			Debug.LogError("Camera.main is null!");
			return null;
		}
		FullScreenEffects component = Camera.main.GetComponent<FullScreenEffects>();
		if (component == null)
		{
			Debug.LogError("fullScreenEffects is nulll!");
			return null;
		}
		this.m_FullScreenEffects = component;
		return component;
	}

	// Token: 0x04001648 RID: 5704
	private static FullScreenFXMgr s_instance;

	// Token: 0x04001649 RID: 5705
	private FullScreenFXMgr.EffectListener m_vignetteListener;

	// Token: 0x0400164A RID: 5706
	private FullScreenFXMgr.EffectListener m_desatListener;

	// Token: 0x0400164B RID: 5707
	private FullScreenFXMgr.EffectListener m_blurListener;

	// Token: 0x0400164C RID: 5708
	private FullScreenFXMgr.EffectListener m_blendToColorListener;

	// Token: 0x0400164D RID: 5709
	private int m_ActiveEffectsCount;

	// Token: 0x0400164E RID: 5710
	private FullScreenEffects m_FullScreenEffects;

	// Token: 0x0400164F RID: 5711
	private int m_StdBlurVignetteCount;

	// Token: 0x020002BC RID: 700
	// (Invoke) Token: 0x060025CE RID: 9678
	public delegate void EffectListener();
}
