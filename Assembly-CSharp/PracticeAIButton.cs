using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007EB RID: 2027
public class PracticeAIButton : PegUIElement
{
	// Token: 0x06004EC7 RID: 20167 RVA: 0x00176265 File Offset: 0x00174465
	public int GetMissionID()
	{
		return this.m_missionID;
	}

	// Token: 0x06004EC8 RID: 20168 RVA: 0x0017626D File Offset: 0x0017446D
	public long GetDeckID()
	{
		return this.m_deckID;
	}

	// Token: 0x06004EC9 RID: 20169 RVA: 0x00176275 File Offset: 0x00174475
	public TAG_CLASS GetClass()
	{
		return this.m_class;
	}

	// Token: 0x06004ECA RID: 20170 RVA: 0x0017627D File Offset: 0x0017447D
	public void PlayUnlockGlow()
	{
		this.m_unlockEffect.GetComponent<Animation>().Play("AITileGlow");
	}

	// Token: 0x06004ECB RID: 20171 RVA: 0x00176298 File Offset: 0x00174498
	public void Lock(bool locked)
	{
		this.m_locked = locked;
		float num = (float)((!this.m_locked) ? 0 : 1);
		bool enabled = !this.m_locked;
		this.SetEnabled(enabled);
		this.GetShowingMaterial().SetFloat("_Desaturate", num);
		this.m_rootObject.GetComponent<Renderer>().materials[0].SetFloat("_Desaturate", num);
	}

	// Token: 0x06004ECC RID: 20172 RVA: 0x00176308 File Offset: 0x00174508
	public void SetInfo(string name, TAG_CLASS buttonClass, CardDef cardDef, int missionID, bool flip)
	{
		this.SetInfo(name, buttonClass, cardDef, missionID, 0L, flip);
	}

	// Token: 0x06004ECD RID: 20173 RVA: 0x00176319 File Offset: 0x00174519
	public void SetInfo(string name, TAG_CLASS buttonClass, CardDef cardDef, long deckID, bool flip)
	{
		this.SetInfo(name, buttonClass, cardDef, 0, deckID, flip);
	}

	// Token: 0x06004ECE RID: 20174 RVA: 0x0017632C File Offset: 0x0017452C
	public void CoverUp(bool flip)
	{
		this.m_covered = true;
		if (flip)
		{
			this.GetHiddenNameMesh().Text = string.Empty;
			this.GetHiddenCover().GetComponent<Renderer>().enabled = true;
			this.Flip();
		}
		else
		{
			this.GetShowingNameMesh().Text = string.Empty;
			this.GetShowingCover().GetComponent<Renderer>().enabled = true;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_coveredBone.localPosition,
			"time",
			0.25f,
			"isLocal",
			true,
			"easeType",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_rootObject, args);
		this.SetEnabled(false);
	}

	// Token: 0x06004ECF RID: 20175 RVA: 0x00176408 File Offset: 0x00174608
	public void Select()
	{
		SoundManager.Get().LoadAndPlay("select_AI_opponent", base.gameObject);
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		this.SetEnabled(false);
		this.Depress();
	}

	// Token: 0x06004ED0 RID: 20176 RVA: 0x00176445 File Offset: 0x00174645
	public void Deselect()
	{
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		if (this.m_covered)
		{
			return;
		}
		this.Raise();
		if (this.m_locked)
		{
			return;
		}
		this.SetEnabled(true);
	}

	// Token: 0x06004ED1 RID: 20177 RVA: 0x0017647A File Offset: 0x0017467A
	public void Raise()
	{
		this.Raise(0.1f);
	}

	// Token: 0x06004ED2 RID: 20178 RVA: 0x00176487 File Offset: 0x00174687
	public void ShowQuestBang(bool shown)
	{
		this.m_questBang.SetActive(shown);
	}

	// Token: 0x06004ED3 RID: 20179 RVA: 0x00176495 File Offset: 0x00174695
	private void Flip()
	{
		base.StopCoroutine(this.FLIP_COROUTINE);
		this.m_usingBackside = !this.m_usingBackside;
		base.StartCoroutine(this.FLIP_COROUTINE, this.m_usingBackside);
	}

	// Token: 0x06004ED4 RID: 20180 RVA: 0x001764CC File Offset: 0x001746CC
	private IEnumerator WaitThenFlip(bool flipToBackside)
	{
		iTween.StopByName(base.gameObject, "flip");
		yield return new WaitForEndOfFrame();
		float startXRotation = (!flipToBackside) ? 180f : 0f;
		this.m_rootObject.transform.localEulerAngles = new Vector3(startXRotation, 0f, 0f);
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(180f, 0f, 0f),
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeOutElastic,
			"space",
			1,
			"name",
			"flip"
		});
		iTween.RotateAdd(this.m_rootObject, args);
		float highlightTargetXRotation = (!flipToBackside) ? 0f : 180f;
		this.m_highlight.transform.localEulerAngles = new Vector3(highlightTargetXRotation, 0f, 0f);
		this.m_unlockEffect.transform.localPosition = ((!flipToBackside) ? this.GLOW_QUAD_NORMAL_LOCAL_POS : this.GLOW_QUAD_FLIPPED_LOCAL_POS);
		yield break;
	}

	// Token: 0x06004ED5 RID: 20181 RVA: 0x001764F5 File Offset: 0x001746F5
	private UberText GetShowingNameMesh()
	{
		return (!this.m_usingBackside) ? this.m_name : this.m_backsideName;
	}

	// Token: 0x06004ED6 RID: 20182 RVA: 0x00176513 File Offset: 0x00174713
	private UberText GetHiddenNameMesh()
	{
		return (!this.m_usingBackside) ? this.m_backsideName : this.m_name;
	}

	// Token: 0x06004ED7 RID: 20183 RVA: 0x00176534 File Offset: 0x00174734
	private Material GetShowingMaterial()
	{
		int num = (!this.m_usingBackside) ? 1 : 2;
		return this.m_rootObject.GetComponent<Renderer>().materials[num];
	}

	// Token: 0x06004ED8 RID: 20184 RVA: 0x00176568 File Offset: 0x00174768
	private void SetShowingMaterial(Material mat)
	{
		int materialIndex = (!this.m_usingBackside) ? 1 : 2;
		RenderUtils.SetMaterial(this.m_rootObject.GetComponent<Renderer>(), materialIndex, mat);
	}

	// Token: 0x06004ED9 RID: 20185 RVA: 0x0017659C File Offset: 0x0017479C
	private Material GetHiddenMaterial()
	{
		int num = (!this.m_usingBackside) ? 2 : 1;
		return this.m_rootObject.GetComponent<Renderer>().materials[num];
	}

	// Token: 0x06004EDA RID: 20186 RVA: 0x001765D0 File Offset: 0x001747D0
	private void SetHiddenMaterial(Material mat)
	{
		int materialIndex = (!this.m_usingBackside) ? 2 : 1;
		RenderUtils.SetMaterial(this.m_rootObject.GetComponent<Renderer>(), materialIndex, mat);
	}

	// Token: 0x06004EDB RID: 20187 RVA: 0x00176602 File Offset: 0x00174802
	private GameObject GetShowingCover()
	{
		return (!this.m_usingBackside) ? this.m_frontCover : this.m_backsideCover;
	}

	// Token: 0x06004EDC RID: 20188 RVA: 0x00176620 File Offset: 0x00174820
	private GameObject GetHiddenCover()
	{
		return (!this.m_usingBackside) ? this.m_backsideCover : this.m_frontCover;
	}

	// Token: 0x06004EDD RID: 20189 RVA: 0x00176640 File Offset: 0x00174840
	private void SetInfo(string name, TAG_CLASS buttonClass, CardDef cardDef, int missionID, long deckID, bool flip)
	{
		this.SetMissionID(missionID);
		this.SetDeckID(deckID);
		this.SetButtonClass(buttonClass);
		Material practiceAIPortrait = cardDef.GetPracticeAIPortrait();
		if (flip)
		{
			this.GetHiddenNameMesh().Text = name;
			if (practiceAIPortrait != null)
			{
				this.SetHiddenMaterial(practiceAIPortrait);
			}
			this.Flip();
		}
		else
		{
			if (this.m_infoSet)
			{
				Debug.LogWarning("PracticeAIButton.SetInfo() - button is being re-initialized!");
			}
			this.m_infoSet = true;
			if (practiceAIPortrait != null)
			{
				this.SetShowingMaterial(practiceAIPortrait);
			}
			this.GetShowingNameMesh().Text = name;
			base.SetOriginalLocalPosition();
		}
		this.m_covered = false;
		this.GetShowingCover().GetComponent<Renderer>().enabled = false;
	}

	// Token: 0x06004EDE RID: 20190 RVA: 0x001766F5 File Offset: 0x001748F5
	private void SetMissionID(int missionID)
	{
		this.m_missionID = missionID;
	}

	// Token: 0x06004EDF RID: 20191 RVA: 0x001766FE File Offset: 0x001748FE
	private void SetDeckID(long deckID)
	{
		this.m_deckID = deckID;
	}

	// Token: 0x06004EE0 RID: 20192 RVA: 0x00176707 File Offset: 0x00174907
	private void SetButtonClass(TAG_CLASS buttonClass)
	{
		this.m_class = buttonClass;
	}

	// Token: 0x06004EE1 RID: 20193 RVA: 0x00176710 File Offset: 0x00174910
	private void Raise(float time)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_upBone.localPosition,
			"time",
			time,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(this.m_rootObject, args);
	}

	// Token: 0x06004EE2 RID: 20194 RVA: 0x00176784 File Offset: 0x00174984
	private void Depress()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_downBone.localPosition,
			"time",
			0.1f,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(this.m_rootObject, args);
	}

	// Token: 0x06004EE3 RID: 20195 RVA: 0x001767FC File Offset: 0x001749FC
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("collection_manager_hero_mouse_over", base.gameObject);
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
	}

	// Token: 0x06004EE4 RID: 20196 RVA: 0x00176821 File Offset: 0x00174A21
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
	}

	// Token: 0x040035A8 RID: 13736
	private const float FLIPPED_X_ROTATION = 180f;

	// Token: 0x040035A9 RID: 13737
	private const float NORMAL_X_ROTATION = 0f;

	// Token: 0x040035AA RID: 13738
	public UberText m_name;

	// Token: 0x040035AB RID: 13739
	public UberText m_backsideName;

	// Token: 0x040035AC RID: 13740
	public GameObject m_frontCover;

	// Token: 0x040035AD RID: 13741
	public GameObject m_backsideCover;

	// Token: 0x040035AE RID: 13742
	public HighlightState m_highlight;

	// Token: 0x040035AF RID: 13743
	public GameObject m_unlockEffect;

	// Token: 0x040035B0 RID: 13744
	public GameObject m_questBang;

	// Token: 0x040035B1 RID: 13745
	public int m_PortraitMaterialIdx = -1;

	// Token: 0x040035B2 RID: 13746
	public GameObject m_rootObject;

	// Token: 0x040035B3 RID: 13747
	public Transform m_upBone;

	// Token: 0x040035B4 RID: 13748
	public Transform m_downBone;

	// Token: 0x040035B5 RID: 13749
	public Transform m_coveredBone;

	// Token: 0x040035B6 RID: 13750
	private int m_missionID;

	// Token: 0x040035B7 RID: 13751
	private long m_deckID;

	// Token: 0x040035B8 RID: 13752
	private bool m_covered;

	// Token: 0x040035B9 RID: 13753
	private bool m_locked;

	// Token: 0x040035BA RID: 13754
	private bool m_infoSet;

	// Token: 0x040035BB RID: 13755
	private bool m_usingBackside;

	// Token: 0x040035BC RID: 13756
	private TAG_CLASS m_class;

	// Token: 0x040035BD RID: 13757
	private readonly string FLIP_COROUTINE = "WaitThenFlip";

	// Token: 0x040035BE RID: 13758
	private readonly Vector3 GLOW_QUAD_NORMAL_LOCAL_POS = new Vector3(-0.1953466f, 1.336676f, 0.00721521f);

	// Token: 0x040035BF RID: 13759
	private readonly Vector3 GLOW_QUAD_FLIPPED_LOCAL_POS = new Vector3(-0.1953466f, -1.336676f, 0.00721521f);
}
