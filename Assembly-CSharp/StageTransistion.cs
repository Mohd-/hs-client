using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000F43 RID: 3907
public class StageTransistion : MonoBehaviour
{
	// Token: 0x06007410 RID: 29712 RVA: 0x00222B90 File Offset: 0x00220D90
	private void Start()
	{
		this.rays.SetActive(false);
		this.flash.SetActive(false);
		this.entireObj.SetActive(true);
		this.inplayObj.SetActive(false);
		this.hlBase.GetComponent<Renderer>().material.SetFloat("_Amount", 0f);
		this.hlEdge.GetComponent<Renderer>().material.SetFloat("_Amount", 0f);
	}

	// Token: 0x06007411 RID: 29713 RVA: 0x00222C0C File Offset: 0x00220E0C
	private void OnGUI()
	{
		Event current = Event.current;
		if (current.isKey)
		{
			this.amountchange = true;
		}
	}

	// Token: 0x06007412 RID: 29714 RVA: 0x00222C34 File Offset: 0x00220E34
	private void OnMouseEnter()
	{
		if (this.FxStartAnim)
		{
			return;
		}
		base.StopCoroutine("fxOnExit");
		this.FxStartStop = false;
		this.FxStartAnim = true;
		this.fxmovefwd = true;
		base.StartCoroutine(this.fxStartEnd());
		this.fxEmitterA.GetComponent<ParticleEmitter>().emit = true;
		this.powerchange = true;
		this.fxEmitterAScale = true;
	}

	// Token: 0x06007413 RID: 29715 RVA: 0x00222C9C File Offset: 0x00220E9C
	private void OnMouseExit()
	{
		if (this.FxStartStop)
		{
			return;
		}
		base.StopCoroutine("fxStartEnd");
		this.FxStartAnim = false;
		this.FxStartStop = true;
		this.fxmovefwd = false;
		this.fxEmitterA.GetComponent<ParticleEmitter>().emit = true;
		base.StartCoroutine(this.fxOnExit());
	}

	// Token: 0x06007414 RID: 29716 RVA: 0x00222CF4 File Offset: 0x00220EF4
	private void OnMouseDown()
	{
		int num = this.stage;
		if (num != 0)
		{
			if (num == 1)
			{
				this.RaysOn();
			}
		}
		else
		{
			this.ManaUse();
		}
		this.stage++;
	}

	// Token: 0x06007415 RID: 29717 RVA: 0x00222D3E File Offset: 0x00220F3E
	private void RaysOn()
	{
		this.rays.SetActive(true);
		this.flash.SetActive(true);
		this.rayschange = true;
	}

	// Token: 0x06007416 RID: 29718 RVA: 0x00222D60 File Offset: 0x00220F60
	private IEnumerator destroyParticle()
	{
		yield return new WaitForSeconds(this.FxEmitterAKillTime);
		this.fxEmitterA.GetComponent<ParticleEmitter>().emit = false;
		yield break;
	}

	// Token: 0x06007417 RID: 29719 RVA: 0x00222D7C File Offset: 0x00220F7C
	private IEnumerator fxStartEnd()
	{
		yield return new WaitForSeconds(this.FxEmitterATimer);
		this.FxStartAnim = false;
		this.fxEmitterA.GetComponent<ParticleEmitter>().emit = false;
		yield break;
	}

	// Token: 0x06007418 RID: 29720 RVA: 0x00222D98 File Offset: 0x00220F98
	private IEnumerator fxOnExit()
	{
		yield return new WaitForSeconds(this.FxEmitterATimer);
		this.FxStartStop = false;
		this.fxEmitterA.GetComponent<ParticleEmitter>().emit = false;
		yield break;
	}

	// Token: 0x06007419 RID: 29721 RVA: 0x00222DB3 File Offset: 0x00220FB3
	private void OnSelected()
	{
		base.StartCoroutine(this.destroyParticle());
	}

	// Token: 0x0600741A RID: 29722 RVA: 0x00222DC2 File Offset: 0x00220FC2
	private void ManaUse()
	{
		this.colorchange = true;
	}

	// Token: 0x0600741B RID: 29723 RVA: 0x00222DCC File Offset: 0x00220FCC
	private void Update()
	{
		if (this.amountchange)
		{
			float num = Time.deltaTime / 0.5f;
			float num2 = num * 0.6954f;
			float num3 = num * 0.6954f;
			float num4 = this.hlEdge.GetComponent<Renderer>().material.GetFloat("_Amount") + num3;
			Debug.Log("amount edge " + num4);
			this.hlBase.GetComponent<Renderer>().material.SetFloat("_Amount", this.hlBase.GetComponent<Renderer>().material.GetFloat("_Amount") + num2);
			if (this.hlBase.GetComponent<Renderer>().material.GetFloat("_Amount") >= 0.6954f)
			{
				this.amountchange = false;
			}
			this.hlEdge.GetComponent<Renderer>().material.SetFloat("_Amount", this.hlEdge.GetComponent<Renderer>().material.GetFloat("_Amount") + num3);
		}
		if (this.colorchange)
		{
			float num5 = Time.deltaTime / 0.5f;
			Color color = this.hlBase.GetComponent<Renderer>().material.color;
			this.hlBase.GetComponent<Renderer>().material.color = Color.Lerp(color, this.endColor, num5);
		}
		if (this.powerchange)
		{
			float num6 = Time.deltaTime / 0.5f;
			float num7 = num6 * 18f;
			float num8 = num6 * 0.6954f;
			this.hlBase.GetComponent<Renderer>().material.SetFloat("_power", this.hlBase.GetComponent<Renderer>().material.GetFloat("_power") + num7);
			if (this.hlBase.GetComponent<Renderer>().material.GetFloat("_power") >= 29f)
			{
				this.powerchange = false;
			}
			this.hlBase.GetComponent<Renderer>().material.SetFloat("_Amount", this.hlBase.GetComponent<Renderer>().material.GetFloat("_Amount") + num8);
			if (this.hlBase.GetComponent<Renderer>().material.GetFloat("_Amount") >= 1.12f)
			{
				this.amountchange = false;
			}
		}
		if (this.rayschange)
		{
			float num9 = Time.deltaTime / 0.5f;
			float num10 = num9 * this.RayTime;
			this.rays.transform.localScale += new Vector3(0f, num10, 0f);
			if (!this.raysdone && this.rays.transform.localScale.y >= 20f)
			{
				this.rays.SetActive(false);
				base.GetComponent<Renderer>().enabled = false;
				this.inplayObj.SetActive(true);
				this.inplayObj.GetComponent<Animation>().Play();
				this.fxEmitterB.GetComponent<ParticleEmitter>().emit = true;
				this.fxEmitterA.SetActive(false);
				this.raysdone = true;
			}
		}
		if (this.raysdone)
		{
			float num11 = this.flash.GetComponent<Renderer>().material.GetFloat("_InvFade") - Time.deltaTime;
			this.flash.GetComponent<Renderer>().material.SetFloat("_InvFade", num11);
			Debug.Log("InvFade " + num11);
			if (num11 <= 0.01f)
			{
				this.entireObj.SetActive(false);
			}
		}
		if (this.FxStartAnim || this.FxStartStop)
		{
			if (this.fxmovefwd)
			{
				Particle[] particles = this.fxEmitterA.GetComponent<ParticleEmitter>().particles;
				for (int i = 0; i < particles.Length; i++)
				{
					Particle[] array = particles;
					int num12 = i;
					array[num12].position = array[num12].position + particles[i].position * Time.deltaTime / 0.2f;
				}
				this.fxEmitterA.GetComponent<ParticleEmitter>().particles = particles;
			}
			else
			{
				Particle[] particles2 = this.fxEmitterA.GetComponent<ParticleEmitter>().particles;
				for (int j = 0; j < particles2.Length; j++)
				{
					Particle[] array2 = particles2;
					int num13 = j;
					array2[num13].position = array2[num13].position - particles2[j].position * Time.deltaTime / 0.2f;
				}
				this.fxEmitterA.GetComponent<ParticleEmitter>().particles = particles2;
			}
		}
		if (this.fxEmitterAScale)
		{
			float num14 = Time.deltaTime / 0.5f;
			float num15 = num14 * this.fxATime;
			this.fxEmitterA.transform.localScale += new Vector3(num15, num15, num15);
		}
	}

	// Token: 0x04005EA9 RID: 24233
	public GameObject hlBase;

	// Token: 0x04005EAA RID: 24234
	public GameObject hlEdge;

	// Token: 0x04005EAB RID: 24235
	public GameObject entireObj;

	// Token: 0x04005EAC RID: 24236
	public GameObject inplayObj;

	// Token: 0x04005EAD RID: 24237
	public GameObject rays;

	// Token: 0x04005EAE RID: 24238
	public GameObject flash;

	// Token: 0x04005EAF RID: 24239
	public GameObject fxEmitterA;

	// Token: 0x04005EB0 RID: 24240
	public GameObject fxEmitterB;

	// Token: 0x04005EB1 RID: 24241
	public float FxEmitterAKillTime = 1f;

	// Token: 0x04005EB2 RID: 24242
	private Shader shaderBucket;

	// Token: 0x04005EB3 RID: 24243
	private bool colorchange;

	// Token: 0x04005EB4 RID: 24244
	private bool powerchange;

	// Token: 0x04005EB5 RID: 24245
	private bool amountchange;

	// Token: 0x04005EB6 RID: 24246
	private bool turnon;

	// Token: 0x04005EB7 RID: 24247
	private bool rayschange;

	// Token: 0x04005EB8 RID: 24248
	private bool flashchange;

	// Token: 0x04005EB9 RID: 24249
	public Color endColor;

	// Token: 0x04005EBA RID: 24250
	public Color flashendColor;

	// Token: 0x04005EBB RID: 24251
	private int stage;

	// Token: 0x04005EBC RID: 24252
	public float RayTime = 10f;

	// Token: 0x04005EBD RID: 24253
	public float fxATime = 1f;

	// Token: 0x04005EBE RID: 24254
	private bool fxmovefwd = true;

	// Token: 0x04005EBF RID: 24255
	public float FxEmitterAWaitTime = 1f;

	// Token: 0x04005EC0 RID: 24256
	public float FxEmitterATimer = 2f;

	// Token: 0x04005EC1 RID: 24257
	private bool FxStartAnim;

	// Token: 0x04005EC2 RID: 24258
	private bool FxStartStop;

	// Token: 0x04005EC3 RID: 24259
	private bool fxEmitterAScale;

	// Token: 0x04005EC4 RID: 24260
	private bool raysdone;
}
