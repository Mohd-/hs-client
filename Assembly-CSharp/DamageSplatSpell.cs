using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000848 RID: 2120
public class DamageSplatSpell : Spell
{
	// Token: 0x06005174 RID: 20852 RVA: 0x00185A55 File Offset: 0x00183C55
	protected override void Awake()
	{
		base.Awake();
		this.EnableAllRenderers(false);
	}

	// Token: 0x06005175 RID: 20853 RVA: 0x00185A64 File Offset: 0x00183C64
	public float GetDamage()
	{
		return (float)this.m_damage;
	}

	// Token: 0x06005176 RID: 20854 RVA: 0x00185A6D File Offset: 0x00183C6D
	public void SetDamage(int damage)
	{
		this.m_damage = damage;
	}

	// Token: 0x06005177 RID: 20855 RVA: 0x00185A78 File Offset: 0x00183C78
	public void DoSplatAnims()
	{
		base.StopAllCoroutines();
		iTween.Stop(base.gameObject);
		base.StartCoroutine(this.SplatAnimCoroutine());
	}

	// Token: 0x06005178 RID: 20856 RVA: 0x00185AA4 File Offset: 0x00183CA4
	private IEnumerator SplatAnimCoroutine()
	{
		this.UpdateElements();
		iTween.FadeTo(base.gameObject, 1f, 0f);
		base.transform.localScale = Vector3.zero;
		yield return null;
		this.OnSpellFinished();
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			Vector3.one,
			"time",
			1f,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
		yield return new WaitForSeconds(2f);
		iTween.FadeTo(base.gameObject, 0f, 1f);
		yield return new WaitForSeconds(1.1f);
		this.EnableAllRenderers(false);
		this.OnStateFinished();
		yield break;
	}

	// Token: 0x06005179 RID: 20857 RVA: 0x00185ABF File Offset: 0x00183CBF
	protected override void OnIdle(SpellStateType prevStateType)
	{
		this.UpdateElements();
		base.OnIdle(prevStateType);
	}

	// Token: 0x0600517A RID: 20858 RVA: 0x00185ACE File Offset: 0x00183CCE
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.UpdateElements();
		base.OnAction(prevStateType);
		this.DoSplatAnims();
	}

	// Token: 0x0600517B RID: 20859 RVA: 0x00185AE3 File Offset: 0x00183CE3
	protected override void OnNone(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		this.m_activeSplat = null;
	}

	// Token: 0x0600517C RID: 20860 RVA: 0x00185AF4 File Offset: 0x00183CF4
	protected override void ShowImpl()
	{
		base.ShowImpl();
		if (this.m_activeSplat == null)
		{
			return;
		}
		SceneUtils.EnableRenderers(this.m_activeSplat.gameObject, true);
		this.m_DamageTextMesh.gameObject.SetActive(true);
	}

	// Token: 0x0600517D RID: 20861 RVA: 0x00185B3B File Offset: 0x00183D3B
	protected override void HideImpl()
	{
		base.HideImpl();
		this.EnableAllRenderers(false);
	}

	// Token: 0x0600517E RID: 20862 RVA: 0x00185B4C File Offset: 0x00183D4C
	private void UpdateElements()
	{
		if (this.m_damage < 0)
		{
			this.m_DamageTextMesh.Text = string.Format("+{0}", Mathf.Abs(this.m_damage));
			this.m_activeSplat = this.m_HealSplat;
			SceneUtils.EnableRenderers(this.m_BloodSplat.gameObject, false);
			SceneUtils.EnableRenderers(this.m_HealSplat.gameObject, true);
		}
		else
		{
			this.m_DamageTextMesh.Text = string.Format("-{0}", this.m_damage);
			this.m_activeSplat = this.m_BloodSplat;
			SceneUtils.EnableRenderers(this.m_BloodSplat.gameObject, true);
			SceneUtils.EnableRenderers(this.m_HealSplat.gameObject, false);
		}
		this.m_DamageTextMesh.gameObject.SetActive(true);
	}

	// Token: 0x0600517F RID: 20863 RVA: 0x00185C1C File Offset: 0x00183E1C
	private void EnableAllRenderers(bool enabled)
	{
		SceneUtils.EnableRenderers(this.m_BloodSplat.gameObject, enabled);
		SceneUtils.EnableRenderers(this.m_HealSplat.gameObject, enabled);
		this.m_DamageTextMesh.gameObject.SetActive(enabled);
	}

	// Token: 0x0400381E RID: 14366
	private const float SCALE_IN_TIME = 1f;

	// Token: 0x0400381F RID: 14367
	public GameObject m_BloodSplat;

	// Token: 0x04003820 RID: 14368
	public GameObject m_HealSplat;

	// Token: 0x04003821 RID: 14369
	public UberText m_DamageTextMesh;

	// Token: 0x04003822 RID: 14370
	private GameObject m_activeSplat;

	// Token: 0x04003823 RID: 14371
	private int m_damage;
}
