using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000866 RID: 2150
public class RankChangeStar : MonoBehaviour
{
	// Token: 0x06005299 RID: 21145 RVA: 0x0018A4A4 File Offset: 0x001886A4
	public void BlackOut()
	{
		this.m_starMeshRenderer.enabled = false;
	}

	// Token: 0x0600529A RID: 21146 RVA: 0x0018A4B2 File Offset: 0x001886B2
	public void UnBlackOut()
	{
		this.m_starMeshRenderer.enabled = true;
	}

	// Token: 0x0600529B RID: 21147 RVA: 0x0018A4C0 File Offset: 0x001886C0
	public void FadeIn()
	{
		base.GetComponent<PlayMakerFSM>().SendEvent("FadeIn");
	}

	// Token: 0x0600529C RID: 21148 RVA: 0x0018A4D2 File Offset: 0x001886D2
	public void Spawn()
	{
		base.GetComponent<PlayMakerFSM>().SendEvent("Spawn");
	}

	// Token: 0x0600529D RID: 21149 RVA: 0x0018A4E4 File Offset: 0x001886E4
	public void Reset()
	{
		base.GetComponent<PlayMakerFSM>().SendEvent("Reset");
	}

	// Token: 0x0600529E RID: 21150 RVA: 0x0018A4F6 File Offset: 0x001886F6
	public void Blink(float delay)
	{
		base.StartCoroutine(this.DelayedBlink(delay));
	}

	// Token: 0x0600529F RID: 21151 RVA: 0x0018A508 File Offset: 0x00188708
	public IEnumerator DelayedBlink(float delay)
	{
		yield return new WaitForSeconds(delay);
		base.GetComponent<PlayMakerFSM>().SendEvent("Blink");
		yield break;
	}

	// Token: 0x060052A0 RID: 21152 RVA: 0x0018A531 File Offset: 0x00188731
	public void Burst(float delay)
	{
		base.StartCoroutine(this.DelayedBurst(delay));
	}

	// Token: 0x060052A1 RID: 21153 RVA: 0x0018A544 File Offset: 0x00188744
	public IEnumerator DelayedBurst(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.UnBlackOut();
		base.GetComponent<PlayMakerFSM>().SendEvent("Burst");
		yield break;
	}

	// Token: 0x060052A2 RID: 21154 RVA: 0x0018A570 File Offset: 0x00188770
	public IEnumerator DelayedDespawn(float delay)
	{
		yield return new WaitForSeconds(delay);
		base.GetComponent<PlayMakerFSM>().SendEvent("DeSpawn");
		yield break;
	}

	// Token: 0x060052A3 RID: 21155 RVA: 0x0018A599 File Offset: 0x00188799
	public void Despawn()
	{
		base.GetComponent<PlayMakerFSM>().SendEvent("DeSpawn");
	}

	// Token: 0x060052A4 RID: 21156 RVA: 0x0018A5AB File Offset: 0x001887AB
	public void Wipe(float delay)
	{
		base.StartCoroutine(this.DelayedWipe(delay));
	}

	// Token: 0x060052A5 RID: 21157 RVA: 0x0018A5BC File Offset: 0x001887BC
	public IEnumerator DelayedWipe(float delay)
	{
		yield return new WaitForSeconds(delay);
		base.GetComponent<PlayMakerFSM>().SendEvent("Wipe");
		yield break;
	}

	// Token: 0x040038DA RID: 14554
	public MeshRenderer m_starMeshRenderer;

	// Token: 0x040038DB RID: 14555
	public MeshRenderer m_bottomGlowRenderer;

	// Token: 0x040038DC RID: 14556
	public MeshRenderer m_topGlowRenderer;
}
