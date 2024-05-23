using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000675 RID: 1653
public class ClockOverlayText : MonoBehaviour
{
	// Token: 0x0600467D RID: 18045 RVA: 0x00152E40 File Offset: 0x00151040
	public void Show()
	{
		Vector3 vector = this.m_maxScale;
		if (UniversalInputManager.UsePhoneUI)
		{
			vector = this.m_maxScale_phone;
		}
		iTween.Stop(base.gameObject);
		base.gameObject.SetActive(true);
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			vector,
			"time",
			0.4f,
			"easetype",
			iTween.EaseType.easeOutQuad
		});
		iTween.ScaleTo(base.gameObject, args);
	}

	// Token: 0x0600467E RID: 18046 RVA: 0x00152ED0 File Offset: 0x001510D0
	public void Hide()
	{
		iTween.Stop(base.gameObject);
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			this.m_minScale,
			"time",
			0.15f,
			"easetype",
			iTween.EaseType.easeInQuad,
			"oncomplete",
			delegate(object o)
			{
				base.gameObject.SetActive(false);
			}
		});
		iTween.ScaleTo(base.gameObject, args);
	}

	// Token: 0x0600467F RID: 18047 RVA: 0x00152F54 File Offset: 0x00151154
	public void HideImmediate()
	{
		base.gameObject.SetActive(false);
		base.transform.localScale = this.m_minScale;
	}

	// Token: 0x06004680 RID: 18048 RVA: 0x00152F80 File Offset: 0x00151180
	public void UpdateText(int step)
	{
		if (step == 0)
		{
			this.m_bannerYear.SetActive(false);
			this.m_detailsText.gameObject.SetActive(false);
			this.m_bannerStandard.SetActive(true);
			this.m_detailsTextStandard.gameObject.SetActive(true);
		}
		else
		{
			this.m_bannerStandard.SetActive(false);
			this.m_detailsTextStandard.gameObject.SetActive(false);
			this.m_bannerYear.SetActive(true);
			this.m_detailsText.gameObject.SetActive(true);
		}
	}

	// Token: 0x04002D91 RID: 11665
	public GameObject m_bannerStandard;

	// Token: 0x04002D92 RID: 11666
	public UberText m_detailsTextStandard;

	// Token: 0x04002D93 RID: 11667
	public GameObject m_bannerYear;

	// Token: 0x04002D94 RID: 11668
	public UberText m_detailsText;

	// Token: 0x04002D95 RID: 11669
	public Vector3 m_maxScale;

	// Token: 0x04002D96 RID: 11670
	public Vector3 m_maxScale_phone;

	// Token: 0x04002D97 RID: 11671
	private Vector3 m_minScale = new Vector3(0.01f, 0.01f, 0.01f);
}
