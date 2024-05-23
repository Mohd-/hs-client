using System;
using System.Collections;
using UnityEngine;

// Token: 0x020008E7 RID: 2279
public class HeroWeld : MonoBehaviour
{
	// Token: 0x0600559F RID: 21919 RVA: 0x0019A20C File Offset: 0x0019840C
	public void DoAnim()
	{
		base.gameObject.SetActive(true);
		this.m_lights = base.gameObject.GetComponentsInChildren<Light>();
		foreach (Light light in this.m_lights)
		{
			light.enabled = true;
		}
		string text = "HeroWeldIn";
		base.gameObject.GetComponent<Animation>().Stop(text);
		base.gameObject.GetComponent<Animation>().Play(text);
		base.StartCoroutine(this.DestroyWhenFinished());
	}

	// Token: 0x060055A0 RID: 21920 RVA: 0x0019A294 File Offset: 0x00198494
	private IEnumerator DestroyWhenFinished()
	{
		yield return new WaitForSeconds(5f);
		foreach (Light light in this.m_lights)
		{
			light.enabled = false;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04003BD1 RID: 15313
	private Light[] m_lights;
}
