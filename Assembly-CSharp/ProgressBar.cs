using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000533 RID: 1331
public class ProgressBar : MonoBehaviour
{
	// Token: 0x06003DCE RID: 15822 RVA: 0x0012AF95 File Offset: 0x00129195
	public void Awake()
	{
		this.m_barMaterial = base.GetComponent<Renderer>().material;
	}

	// Token: 0x06003DCF RID: 15823 RVA: 0x0012AFA8 File Offset: 0x001291A8
	public void AnimateProgress(float prevVal, float currVal)
	{
		this.m_prevVal = prevVal;
		this.m_currVal = currVal;
		if (this.m_currVal > this.m_prevVal)
		{
			this.m_factor = this.m_currVal - this.m_prevVal;
		}
		else
		{
			this.m_factor = this.m_prevVal - this.m_currVal;
		}
		this.m_factor = Mathf.Abs(this.m_factor);
		if (this.m_currVal > this.m_prevVal)
		{
			this.IncreaseProgress(this.m_currVal, this.m_prevVal);
		}
		else
		{
			this.DecreaseProgress(this.m_currVal, this.m_prevVal);
		}
	}

	// Token: 0x06003DD0 RID: 15824 RVA: 0x0012B04A File Offset: 0x0012924A
	public void SetProgressBar(float progress)
	{
		this.m_barMaterial.SetFloat("_Intensity", this.m_barIntensity);
		this.m_barMaterial.SetFloat("_Percent", progress);
	}

	// Token: 0x06003DD1 RID: 15825 RVA: 0x0012B073 File Offset: 0x00129273
	public float GetAnimationTime()
	{
		return this.m_animationTime;
	}

	// Token: 0x06003DD2 RID: 15826 RVA: 0x0012B07C File Offset: 0x0012927C
	public void SetLabel(string text)
	{
		if (this.m_uberLabel != null)
		{
			this.m_uberLabel.Text = text;
		}
		if (this.m_label != null)
		{
			this.m_label.text = text;
		}
	}

	// Token: 0x06003DD3 RID: 15827 RVA: 0x0012B0C3 File Offset: 0x001292C3
	public void SetBarTexture(Texture texture)
	{
		this.m_barMaterial.SetTexture("_NoiseTex", texture);
	}

	// Token: 0x06003DD4 RID: 15828 RVA: 0x0012B0D8 File Offset: 0x001292D8
	private void IncreaseProgress(float currProgress, float prevProgress)
	{
		float num = this.m_increaseAnimTime * this.m_factor;
		this.m_animationTime = num;
		iTween.EaseType easeType = iTween.EaseType.easeOutQuad;
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			prevProgress,
			"to",
			currProgress,
			"time",
			num,
			"easetype",
			easeType,
			"onupdate",
			"Progress_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"IncreaseProgress"
		});
		iTween.StopByName(base.gameObject, "IncreaseProgress");
		iTween.ValueTo(base.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"from",
			0f,
			"to",
			0.005f,
			"time",
			num,
			"easetype",
			iTween.EaseType.easeOutQuad,
			"onupdate",
			"ScrollSpeed_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"UVSpeed"
		});
		iTween.StopByName(base.gameObject, "UVSpeed");
		iTween.ValueTo(base.gameObject, args2);
		this.m_maxIntensity = this.m_barIntensity + (this.m_barIntensityIncreaseMax - this.m_barIntensity) * this.m_factor;
		Hashtable args3 = iTween.Hash(new object[]
		{
			"from",
			this.m_barIntensity,
			"to",
			this.m_maxIntensity,
			"time",
			num,
			"easetype",
			easeType,
			"onupdate",
			"Intensity_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"Intensity",
			"oncomplete",
			"Intensity_OnComplete",
			"oncompletetarget",
			base.gameObject
		});
		iTween.StopByName(base.gameObject, "Intensity");
		iTween.ValueTo(base.gameObject, args3);
		if (base.GetComponent<AudioSource>())
		{
			SoundManager.Get().SetVolume(base.GetComponent<AudioSource>(), 0f);
			SoundManager.Get().SetPitch(base.GetComponent<AudioSource>(), this.m_increasePitchStart);
			SoundManager.Get().Play(base.GetComponent<AudioSource>());
		}
		Hashtable args4 = iTween.Hash(new object[]
		{
			"from",
			0,
			"to",
			1,
			"time",
			num * this.m_audioFadeInOut,
			"delay",
			0,
			"easetype",
			easeType,
			"onupdate",
			"AudioVolume_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"barVolumeStart"
		});
		iTween.StopByName(base.gameObject, "barVolumeStart");
		iTween.ValueTo(base.gameObject, args4);
		Hashtable args5 = iTween.Hash(new object[]
		{
			"from",
			1,
			"to",
			0,
			"time",
			num * this.m_audioFadeInOut,
			"delay",
			num * (1f - this.m_audioFadeInOut),
			"easetype",
			easeType,
			"onupdate",
			"AudioVolume_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"oncomplete",
			"AudioVolume_OnComplete",
			"name",
			"barVolumeEnd"
		});
		iTween.StopByName(base.gameObject, "barVolumeEnd");
		iTween.ValueTo(base.gameObject, args5);
		Hashtable args6 = iTween.Hash(new object[]
		{
			"from",
			this.m_increasePitchStart,
			"to",
			this.m_increasePitchEnd,
			"time",
			num,
			"delay",
			0,
			"easetype",
			easeType,
			"onupdate",
			"AudioPitch_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"barPitch"
		});
		iTween.StopByName(base.gameObject, "barPitch");
		iTween.ValueTo(base.gameObject, args6);
	}

	// Token: 0x06003DD5 RID: 15829 RVA: 0x0012B5EA File Offset: 0x001297EA
	private void Progress_OnUpdate(float val)
	{
		this.m_barMaterial.SetFloat("_Percent", val);
	}

	// Token: 0x06003DD6 RID: 15830 RVA: 0x0012B600 File Offset: 0x00129800
	private void Intensity_OnComplete()
	{
		iTween.StopByName(base.gameObject, "Increase");
		iTween.StopByName(base.gameObject, "Intensity");
		iTween.StopByName(base.gameObject, "UVSpeed");
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			this.m_maxIntensity,
			"to",
			this.m_barIntensity,
			"time",
			this.m_coolDownAnimTime,
			"easetype",
			iTween.EaseType.easeOutQuad,
			"onupdate",
			"Intensity_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"Intensity"
		});
		iTween.ValueTo(base.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"from",
			0.005f,
			"to",
			0f,
			"time",
			this.m_coolDownAnimTime,
			"easetype",
			iTween.EaseType.easeOutQuad,
			"onupdate",
			"ScrollSpeed_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"UVSpeed"
		});
		iTween.ValueTo(base.gameObject, args2);
	}

	// Token: 0x06003DD7 RID: 15831 RVA: 0x0012B77F File Offset: 0x0012997F
	private void Intensity_OnUpdate(float val)
	{
		this.m_barMaterial.SetFloat("_Intensity", val);
	}

	// Token: 0x06003DD8 RID: 15832 RVA: 0x0012B794 File Offset: 0x00129994
	private void ScrollSpeed_OnUpdate(float val)
	{
		this.m_Uadd += val;
		this.m_barMaterial.SetFloat("_Uadd", this.m_Uadd);
	}

	// Token: 0x06003DD9 RID: 15833 RVA: 0x0012B7C8 File Offset: 0x001299C8
	private void AudioVolume_OnUpdate(float val)
	{
		if (base.GetComponent<AudioSource>())
		{
			SoundManager.Get().SetVolume(base.GetComponent<AudioSource>(), val);
		}
	}

	// Token: 0x06003DDA RID: 15834 RVA: 0x0012B7F8 File Offset: 0x001299F8
	private void AudioVolume_OnComplete()
	{
		if (base.GetComponent<AudioSource>())
		{
			SoundManager.Get().Stop(base.GetComponent<AudioSource>());
		}
	}

	// Token: 0x06003DDB RID: 15835 RVA: 0x0012B828 File Offset: 0x00129A28
	private void AudioPitch_OnUpdate(float val)
	{
		if (base.GetComponent<AudioSource>())
		{
			SoundManager.Get().SetPitch(base.GetComponent<AudioSource>(), val);
		}
	}

	// Token: 0x06003DDC RID: 15836 RVA: 0x0012B858 File Offset: 0x00129A58
	private void DecreaseProgress(float currProgress, float prevProgress)
	{
		float num = this.m_decreaseAnimTime * this.m_factor;
		this.m_animationTime = num;
		iTween.EaseType easeType = iTween.EaseType.easeInOutCubic;
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			prevProgress,
			"to",
			currProgress,
			"time",
			num,
			"easetype",
			easeType,
			"onupdate",
			"Progress_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"Decrease"
		});
		iTween.StopByName(base.gameObject, "Decrease");
		iTween.ValueTo(base.gameObject, args);
		if (base.GetComponent<AudioSource>())
		{
			SoundManager.Get().SetVolume(base.GetComponent<AudioSource>(), 0f);
			SoundManager.Get().SetPitch(base.GetComponent<AudioSource>(), this.m_decreasePitchStart);
			SoundManager.Get().Play(base.GetComponent<AudioSource>());
		}
		Hashtable args2 = iTween.Hash(new object[]
		{
			"from",
			0,
			"to",
			1,
			"time",
			num * this.m_audioFadeInOut,
			"delay",
			0,
			"easetype",
			easeType,
			"onupdate",
			"AudioVolume_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"barVolumeStart"
		});
		iTween.StopByName(base.gameObject, "barVolumeStart");
		iTween.ValueTo(base.gameObject, args2);
		Hashtable args3 = iTween.Hash(new object[]
		{
			"from",
			1,
			"to",
			0,
			"time",
			num * this.m_audioFadeInOut,
			"delay",
			num * (1f - this.m_audioFadeInOut),
			"easetype",
			easeType,
			"onupdate",
			"AudioVolume_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"oncomplete",
			"AudioVolume_OnComplete",
			"name",
			"barVolumeEnd"
		});
		iTween.StopByName(base.gameObject, "barVolumeEnd");
		iTween.ValueTo(base.gameObject, args3);
		Hashtable args4 = iTween.Hash(new object[]
		{
			"from",
			this.m_decreasePitchStart,
			"to",
			this.m_decreasePitchEnd,
			"time",
			num,
			"delay",
			0,
			"easetype",
			easeType,
			"onupdate",
			"AudioPitch_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"barPitch"
		});
		iTween.StopByName(base.gameObject, "barPitch");
		iTween.ValueTo(base.gameObject, args4);
	}

	// Token: 0x0400275E RID: 10078
	public TextMesh m_label;

	// Token: 0x0400275F RID: 10079
	public UberText m_uberLabel;

	// Token: 0x04002760 RID: 10080
	public float m_increaseAnimTime = 2f;

	// Token: 0x04002761 RID: 10081
	public float m_decreaseAnimTime = 1f;

	// Token: 0x04002762 RID: 10082
	public float m_coolDownAnimTime = 1f;

	// Token: 0x04002763 RID: 10083
	public float m_barIntensity = 1.2f;

	// Token: 0x04002764 RID: 10084
	public float m_barIntensityIncreaseMax = 3f;

	// Token: 0x04002765 RID: 10085
	public float m_audioFadeInOut = 0.2f;

	// Token: 0x04002766 RID: 10086
	public float m_increasePitchStart = 1f;

	// Token: 0x04002767 RID: 10087
	public float m_increasePitchEnd = 1.2f;

	// Token: 0x04002768 RID: 10088
	public float m_decreasePitchStart = 1f;

	// Token: 0x04002769 RID: 10089
	public float m_decreasePitchEnd = 0.8f;

	// Token: 0x0400276A RID: 10090
	private Material m_barMaterial;

	// Token: 0x0400276B RID: 10091
	private float m_prevVal;

	// Token: 0x0400276C RID: 10092
	private float m_currVal;

	// Token: 0x0400276D RID: 10093
	private float m_factor;

	// Token: 0x0400276E RID: 10094
	private float m_maxIntensity;

	// Token: 0x0400276F RID: 10095
	private float m_Uadd;

	// Token: 0x04002770 RID: 10096
	private float m_animationTime;
}
