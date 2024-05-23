using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200081A RID: 2074
public class ManaCostBar : MonoBehaviour
{
	// Token: 0x06004FF0 RID: 20464 RVA: 0x0017B420 File Offset: 0x00179620
	private void Start()
	{
		if (this.m_manaCostBarObject == null)
		{
			base.enabled = false;
		}
		if (this.m_ParticleStart != null)
		{
			this.m_particleStartPoint = this.m_ParticleStart.transform.localPosition;
		}
		if (this.m_ParticleEnd != null)
		{
			this.m_particleEndPoint = this.m_ParticleEnd.transform.localPosition;
		}
		this.m_barMaterial = this.m_manaCostBarObject.GetComponent<Renderer>().material;
		this.m_barMaterial.SetFloat("_Seed", Random.Range(0f, 1f));
	}

	// Token: 0x06004FF1 RID: 20465 RVA: 0x0017B4C8 File Offset: 0x001796C8
	private void Update()
	{
	}

	// Token: 0x06004FF2 RID: 20466 RVA: 0x0017B4CC File Offset: 0x001796CC
	public void SetBar(float newValue)
	{
		this.m_currentVal = newValue / this.m_maxValue;
		this.SetBarValue(this.m_currentVal);
		this.m_previousVal = this.m_currentVal;
	}

	// Token: 0x06004FF3 RID: 20467 RVA: 0x0017B500 File Offset: 0x00179700
	public void AnimateBar(float newValue)
	{
		if (newValue == 0f)
		{
			this.SetBarValue(0f);
			return;
		}
		this.m_currentVal = newValue / this.m_maxValue;
		if (this.m_manaCostBarObject == null)
		{
			return;
		}
		if (this.m_currentVal == this.m_previousVal)
		{
			return;
		}
		if (this.m_currentVal > this.m_previousVal)
		{
			this.m_factor = this.m_currentVal - this.m_previousVal;
		}
		else
		{
			this.m_factor = this.m_previousVal - this.m_currentVal;
		}
		this.m_factor = Mathf.Abs(this.m_factor);
		if (this.m_currentVal > this.m_previousVal)
		{
			this.IncreaseBar(this.m_currentVal, this.m_previousVal);
		}
		else
		{
			this.DecreaseBar(this.m_currentVal, this.m_previousVal);
		}
		this.m_previousVal = this.m_currentVal;
	}

	// Token: 0x06004FF4 RID: 20468 RVA: 0x0017B5EC File Offset: 0x001797EC
	private void SetBarValue(float val)
	{
		this.m_currentVal = val / this.m_maxValue;
		if (this.m_manaCostBarObject == null)
		{
			return;
		}
		if (this.m_currentVal == this.m_previousVal)
		{
			return;
		}
		this.BarPercent_OnUpdate(val);
		this.ParticlePosition_OnUpdate(val);
		if (val == 0f)
		{
			this.PlayParticles(false);
		}
		this.m_previousVal = this.m_currentVal;
	}

	// Token: 0x06004FF5 RID: 20469 RVA: 0x0017B658 File Offset: 0x00179858
	private void IncreaseBar(float newVal, float prevVal)
	{
		float num = this.m_increaseAnimTime * this.m_factor;
		this.PlayParticles(true);
		iTween.EaseType easeType = iTween.EaseType.easeInQuad;
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			prevVal,
			"to",
			newVal,
			"time",
			num,
			"easetype",
			easeType,
			"onupdate",
			"BarPercent_OnUpdate",
			"oncomplete",
			"Increase_OnComplete",
			"oncompletetarget",
			base.gameObject,
			"onupdatetarget",
			base.gameObject,
			"name",
			"IncreaseBarPercent"
		});
		iTween.StopByName(this.m_manaCostBarObject.gameObject, "IncreaseBarPercent");
		iTween.ValueTo(this.m_manaCostBarObject.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"from",
			prevVal,
			"to",
			newVal,
			"time",
			num,
			"easetype",
			easeType,
			"onupdate",
			"ParticlePosition_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"ParticlePos"
		});
		iTween.StopByName(this.m_manaCostBarObject.gameObject, "ParticlePos");
		iTween.ValueTo(this.m_manaCostBarObject.gameObject, args2);
		Hashtable args3 = iTween.Hash(new object[]
		{
			"from",
			this.m_BarIntensity,
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
			"Intensity"
		});
		iTween.StopByName(this.m_manaCostBarObject.gameObject, "Intensity");
		iTween.ValueTo(this.m_manaCostBarObject.gameObject, args3);
	}

	// Token: 0x06004FF6 RID: 20470 RVA: 0x0017B8B4 File Offset: 0x00179AB4
	private void DecreaseBar(float newVal, float prevVal)
	{
		float num = this.m_increaseAnimTime * this.m_factor;
		this.PlayParticles(true);
		iTween.EaseType easeType = iTween.EaseType.easeOutQuad;
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			prevVal,
			"to",
			newVal,
			"time",
			num,
			"easetype",
			easeType,
			"onupdate",
			"BarPercent_OnUpdate",
			"oncomplete",
			"Decrease_OnComplete",
			"onupdatetarget",
			base.gameObject,
			"name",
			"IncreaseBarPercent"
		});
		iTween.StopByName(this.m_manaCostBarObject.gameObject, "IncreaseBarPercent");
		iTween.ValueTo(this.m_manaCostBarObject.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"from",
			prevVal,
			"to",
			newVal,
			"time",
			num,
			"easetype",
			easeType,
			"onupdate",
			"ParticlePosition_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"ParticlePos"
		});
		iTween.StopByName(this.m_manaCostBarObject.gameObject, "ParticlePos");
		iTween.ValueTo(this.m_manaCostBarObject.gameObject, args2);
	}

	// Token: 0x06004FF7 RID: 20471 RVA: 0x0017BA44 File Offset: 0x00179C44
	private void BarPercent_OnUpdate(float val)
	{
		this.m_barMaterial.SetFloat("_Percent", val);
	}

	// Token: 0x06004FF8 RID: 20472 RVA: 0x0017BA58 File Offset: 0x00179C58
	private void ParticlePosition_OnUpdate(float val)
	{
		this.m_ParticleObject.transform.localPosition = Vector3.Lerp(this.m_particleStartPoint, this.m_particleEndPoint, val);
	}

	// Token: 0x06004FF9 RID: 20473 RVA: 0x0017BA87 File Offset: 0x00179C87
	private void Intensity_OnUpdate(float val)
	{
		this.m_barMaterial.SetFloat("_Intensity", val);
	}

	// Token: 0x06004FFA RID: 20474 RVA: 0x0017BA9A File Offset: 0x00179C9A
	private void Increase_OnComplete()
	{
		if (this.m_ParticleImpact != null)
		{
			this.m_ParticleImpact.GetComponent<ParticleSystem>().Play();
		}
		this.CoolDown();
	}

	// Token: 0x06004FFB RID: 20475 RVA: 0x0017BAC3 File Offset: 0x00179CC3
	private void Decrease_OnComplete()
	{
	}

	// Token: 0x06004FFC RID: 20476 RVA: 0x0017BAC8 File Offset: 0x00179CC8
	private void CoolDown()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			this.m_maxIntensity,
			"to",
			this.m_BarIntensity,
			"time",
			this.m_coolDownAnimTime,
			"easetype",
			iTween.EaseType.easeOutQuad,
			"onupdate",
			"Intensity_OnUpdate",
			"onupdatetarget",
			base.gameObject,
			"name",
			"CoolDownIntensity",
			"oncomplete",
			"CoolDown_OnComplete",
			"oncompletetarget",
			base.gameObject
		});
		iTween.StopByName(this.m_manaCostBarObject.gameObject, "CoolDownIntensity");
		iTween.ValueTo(this.m_manaCostBarObject.gameObject, args);
	}

	// Token: 0x06004FFD RID: 20477 RVA: 0x0017BBB6 File Offset: 0x00179DB6
	private void CoolDown_OnComplete()
	{
		iTween.StopByName(this.m_manaCostBarObject.gameObject, "CoolDownIntensity");
	}

	// Token: 0x06004FFE RID: 20478 RVA: 0x0017BBD0 File Offset: 0x00179DD0
	private void PlayParticles(bool state)
	{
		ParticleSystem[] componentsInChildren = this.m_ParticleObject.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			if (state && particleSystem != this.m_ParticleImpact.GetComponent<ParticleSystem>())
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
			particleSystem.enableEmission = state;
		}
	}

	// Token: 0x06004FFF RID: 20479 RVA: 0x0017BC37 File Offset: 0x00179E37
	public void TestIncrease()
	{
		this.AnimateBar(7f);
	}

	// Token: 0x06005000 RID: 20480 RVA: 0x0017BC44 File Offset: 0x00179E44
	public void TestDecrease()
	{
		this.AnimateBar(6f);
	}

	// Token: 0x06005001 RID: 20481 RVA: 0x0017BC51 File Offset: 0x00179E51
	public void TestReset()
	{
		this.SetBar(4f);
	}

	// Token: 0x040036BB RID: 14011
	public GameObject m_manaCostBarObject;

	// Token: 0x040036BC RID: 14012
	public GameObject m_ParticleObject;

	// Token: 0x040036BD RID: 14013
	public GameObject m_ParticleStart;

	// Token: 0x040036BE RID: 14014
	public GameObject m_ParticleEnd;

	// Token: 0x040036BF RID: 14015
	public GameObject m_ParticleImpact;

	// Token: 0x040036C0 RID: 14016
	public float m_maxValue = 10f;

	// Token: 0x040036C1 RID: 14017
	public float m_BarIntensity = 1.6f;

	// Token: 0x040036C2 RID: 14018
	public float m_maxIntensity = 2f;

	// Token: 0x040036C3 RID: 14019
	public float m_increaseAnimTime = 2f;

	// Token: 0x040036C4 RID: 14020
	public float m_coolDownAnimTime = 1f;

	// Token: 0x040036C5 RID: 14021
	private float m_previousVal;

	// Token: 0x040036C6 RID: 14022
	private float m_currentVal;

	// Token: 0x040036C7 RID: 14023
	private float m_factor;

	// Token: 0x040036C8 RID: 14024
	private Vector3 m_particleStartPoint = Vector3.zero;

	// Token: 0x040036C9 RID: 14025
	private Vector3 m_particleEndPoint = Vector3.zero;

	// Token: 0x040036CA RID: 14026
	private Material m_barMaterial;
}
