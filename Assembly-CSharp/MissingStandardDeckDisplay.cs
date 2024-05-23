using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200037C RID: 892
public class MissingStandardDeckDisplay : MonoBehaviour
{
	// Token: 0x06002D77 RID: 11639 RVA: 0x000E3C58 File Offset: 0x000E1E58
	public void Awake()
	{
		this.m_isShown = false;
		this.m_missingStandardDeckObject.transform.position = this.m_hiddenBone.transform.position;
		this.m_missingStandardDeckObject.SetActive(false);
		this.m_darkenQuad.gameObject.SetActive(false);
		this.m_darkenQuad.material.color = this.m_quadHiddenColor;
		this.m_text.SetGameStringText("GLUE_TOURNAMENT_NO_STANDARD_DECKS");
	}

	// Token: 0x06002D78 RID: 11640 RVA: 0x000E3CCF File Offset: 0x000E1ECF
	public bool IsShown()
	{
		return this.m_isShown;
	}

	// Token: 0x06002D79 RID: 11641 RVA: 0x000E3CD8 File Offset: 0x000E1ED8
	public void ShowImmediately()
	{
		this.m_isShown = true;
		this.m_darkenQuad.gameObject.SetActive(true);
		this.m_missingStandardDeckObject.SetActive(true);
		iTween.Stop(this.m_missingStandardDeckObject);
		iTween.Stop(this.m_darkenQuad.gameObject);
		this.m_currentQuadFade = 1f;
		Color color = Color.Lerp(this.m_quadHiddenColor, this.m_quadShownColor, this.m_currentQuadFade);
		this.m_darkenQuad.material.color = color;
		this.m_missingStandardDeckObject.transform.position = this.m_shownBone.transform.position;
	}

	// Token: 0x06002D7A RID: 11642 RVA: 0x000E3D78 File Offset: 0x000E1F78
	public void Show()
	{
		if (!this.m_isShown)
		{
			this.m_isShown = true;
			this.m_darkenQuad.gameObject.SetActive(true);
			this.m_missingStandardDeckObject.SetActive(true);
			iTween.Stop(this.m_missingStandardDeckObject);
			iTween.Stop(this.m_darkenQuad.gameObject);
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				this.m_shownBone.transform.position,
				"time",
				this.m_animateInTime,
				"easetype",
				iTween.EaseType.easeOutBounce
			});
			iTween.MoveTo(this.m_missingStandardDeckObject, args);
			Hashtable args2 = iTween.Hash(new object[]
			{
				"from",
				this.m_currentQuadFade,
				"to",
				1f,
				"time",
				this.m_animateInTime,
				"onupdate",
				"DarkenQuadFade_Update",
				"onupdatetarget",
				base.gameObject
			});
			iTween.ValueTo(this.m_darkenQuad.gameObject, args2);
		}
	}

	// Token: 0x06002D7B RID: 11643 RVA: 0x000E3EB0 File Offset: 0x000E20B0
	public void Hide()
	{
		if (this.m_isShown)
		{
			this.m_isShown = false;
			iTween.Stop(this.m_missingStandardDeckObject);
			iTween.Stop(this.m_darkenQuad.gameObject);
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				this.m_hiddenBone.transform.position,
				"time",
				this.m_animateOutTime,
				"easetype",
				iTween.EaseType.linear,
				"oncomplete",
				"OnHideComplete",
				"oncompletetarget",
				base.gameObject
			});
			iTween.MoveTo(this.m_missingStandardDeckObject, args);
			Hashtable args2 = iTween.Hash(new object[]
			{
				"from",
				this.m_currentQuadFade,
				"to",
				0f,
				"time",
				this.m_animateOutTime,
				"onupdate",
				"DarkenQuadFade_Update",
				"onupdatetarget",
				base.gameObject
			});
			iTween.ValueTo(this.m_darkenQuad.gameObject, args2);
		}
	}

	// Token: 0x06002D7C RID: 11644 RVA: 0x000E3FF0 File Offset: 0x000E21F0
	private void DarkenQuadFade_Update(float fade)
	{
		this.m_currentQuadFade = fade;
		Color color = Color.Lerp(this.m_quadHiddenColor, this.m_quadShownColor, this.m_currentQuadFade);
		this.m_darkenQuad.material.color = color;
	}

	// Token: 0x06002D7D RID: 11645 RVA: 0x000E402D File Offset: 0x000E222D
	private void OnHideComplete()
	{
		this.m_missingStandardDeckObject.SetActive(false);
		this.m_darkenQuad.gameObject.SetActive(false);
	}

	// Token: 0x04001C50 RID: 7248
	public GameObject m_missingStandardDeckObject;

	// Token: 0x04001C51 RID: 7249
	public UberText m_text;

	// Token: 0x04001C52 RID: 7250
	public MeshRenderer m_darkenQuad;

	// Token: 0x04001C53 RID: 7251
	public Transform m_shownBone;

	// Token: 0x04001C54 RID: 7252
	public Transform m_hiddenBone;

	// Token: 0x04001C55 RID: 7253
	public float m_animateInTime = 1f;

	// Token: 0x04001C56 RID: 7254
	public float m_animateOutTime = 0.5f;

	// Token: 0x04001C57 RID: 7255
	private bool m_isShown;

	// Token: 0x04001C58 RID: 7256
	private Color m_quadHiddenColor = Color.white;

	// Token: 0x04001C59 RID: 7257
	private Color m_quadShownColor = new Color(0.53f, 0.53f, 0.53f, 1f);

	// Token: 0x04001C5A RID: 7258
	private float m_currentQuadFade;
}
