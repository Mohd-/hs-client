using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200080F RID: 2063
public class HeroLabel : MonoBehaviour
{
	// Token: 0x06004FCA RID: 20426 RVA: 0x0017AC9E File Offset: 0x00178E9E
	public void UpdateText(string nameText, string classText)
	{
		this.m_nameText.Text = nameText;
		this.m_classText.Text = classText;
	}

	// Token: 0x06004FCB RID: 20427 RVA: 0x0017ACB8 File Offset: 0x00178EB8
	public void FadeOut()
	{
		iTween.FadeTo(this.m_nameText.gameObject, 0f, 0.5f);
		iTween.FadeTo(this.m_classText.gameObject, 0f, 0.5f);
		base.StartCoroutine(this.FinishFade());
	}

	// Token: 0x06004FCC RID: 20428 RVA: 0x0017AD08 File Offset: 0x00178F08
	private IEnumerator FinishFade()
	{
		yield return new WaitForSeconds(1f);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04003690 RID: 13968
	public UberText m_nameText;

	// Token: 0x04003691 RID: 13969
	public UberText m_classText;
}
