using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000EEC RID: 3820
public class AnimPositionResetter : MonoBehaviour
{
	// Token: 0x06007256 RID: 29270 RVA: 0x00219948 File Offset: 0x00217B48
	private void Awake()
	{
		this.m_initialPosition = base.transform.position;
	}

	// Token: 0x06007257 RID: 29271 RVA: 0x0021995C File Offset: 0x00217B5C
	public static AnimPositionResetter OnAnimStarted(GameObject go, float animTime)
	{
		if (animTime <= 0f)
		{
			return null;
		}
		AnimPositionResetter animPositionResetter = AnimPositionResetter.RegisterResetter(go);
		animPositionResetter.OnAnimStarted(animTime);
		return animPositionResetter;
	}

	// Token: 0x06007258 RID: 29272 RVA: 0x00219985 File Offset: 0x00217B85
	public Vector3 GetInitialPosition()
	{
		return this.m_initialPosition;
	}

	// Token: 0x06007259 RID: 29273 RVA: 0x0021998D File Offset: 0x00217B8D
	public float GetEndTimestamp()
	{
		return this.m_endTimestamp;
	}

	// Token: 0x0600725A RID: 29274 RVA: 0x00219995 File Offset: 0x00217B95
	public float GetDelay()
	{
		return this.m_delay;
	}

	// Token: 0x0600725B RID: 29275 RVA: 0x002199A0 File Offset: 0x00217BA0
	private static AnimPositionResetter RegisterResetter(GameObject go)
	{
		if (go == null)
		{
			return null;
		}
		AnimPositionResetter component = go.GetComponent<AnimPositionResetter>();
		if (component != null)
		{
			return component;
		}
		return go.AddComponent<AnimPositionResetter>();
	}

	// Token: 0x0600725C RID: 29276 RVA: 0x002199D8 File Offset: 0x00217BD8
	private void OnAnimStarted(float animTime)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup + animTime;
		float num2 = num - this.m_endTimestamp;
		if (num2 <= 0f)
		{
			return;
		}
		this.m_delay = Mathf.Min(num2, animTime);
		this.m_endTimestamp = num;
		base.StopCoroutine("ResetPosition");
		base.StartCoroutine("ResetPosition");
	}

	// Token: 0x0600725D RID: 29277 RVA: 0x00219A30 File Offset: 0x00217C30
	private IEnumerator ResetPosition()
	{
		yield return new WaitForSeconds(this.m_delay);
		base.transform.position = this.m_initialPosition;
		Object.Destroy(this);
		yield break;
	}

	// Token: 0x04005C6A RID: 23658
	private Vector3 m_initialPosition;

	// Token: 0x04005C6B RID: 23659
	private float m_endTimestamp;

	// Token: 0x04005C6C RID: 23660
	private float m_delay;
}
