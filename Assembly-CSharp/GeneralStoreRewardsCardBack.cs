using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000454 RID: 1108
[CustomEditClass]
public class GeneralStoreRewardsCardBack : MonoBehaviour
{
	// Token: 0x060036DE RID: 14046 RVA: 0x0010DD0D File Offset: 0x0010BF0D
	public void SetCardBack(int id)
	{
		if (id == -1 || id == this.m_cardBackId)
		{
			return;
		}
		this.LoadCardBackWithId(id);
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x0010DD2A File Offset: 0x0010BF2A
	public void SetPreorderText(string text)
	{
		this.m_cardBackText.Text = text;
	}

	// Token: 0x060036E0 RID: 14048 RVA: 0x0010DD38 File Offset: 0x0010BF38
	public void ShowCardBackReward()
	{
		this.HideCardBackReward();
		if (this.m_cardBackId == -1)
		{
			return;
		}
		if (this.m_cardBackAppearAnimation == null || string.IsNullOrEmpty(this.m_cardBackAppearAnimationName) || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		base.StartCoroutine("AnimateCardBackIn");
	}

	// Token: 0x060036E1 RID: 14049 RVA: 0x0010DD98 File Offset: 0x0010BF98
	public void HideCardBackReward()
	{
		base.StopCoroutine("AnimateCardBackIn");
		if (this.m_cardBackContainer != null)
		{
			this.m_cardBackContainer.SetActive(false);
		}
	}

	// Token: 0x060036E2 RID: 14050 RVA: 0x0010DDCD File Offset: 0x0010BFCD
	private void Awake()
	{
		this.m_cardBackTextOrigScale = this.m_cardBackText.transform.localScale;
	}

	// Token: 0x060036E3 RID: 14051 RVA: 0x0010DDE8 File Offset: 0x0010BFE8
	private void LoadCardBackWithId(int cardBackId)
	{
		if (this.m_cardBackObject != null)
		{
			Object.Destroy(this.m_cardBackObject);
		}
		if (cardBackId < 0)
		{
			Debug.LogError("Card back ID must be a positive number");
			return;
		}
		this.m_cardBackId = cardBackId;
		this.m_cardBackObjectLoading = CardBackManager.Get().LoadCardBackByIndex(this.m_cardBackId, delegate(CardBackManager.LoadCardBackData cardBackData)
		{
			GameObject gameObject = cardBackData.m_GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.name = "CARD_BACK_" + cardBackData.m_CardBackIndex;
			Actor component = gameObject.GetComponent<Actor>();
			if (component != null)
			{
				GameObject cardMesh = component.m_cardMesh;
				component.SetCardbackUpdateIgnore(true);
				component.SetUnlit();
				if (cardMesh != null)
				{
					Material material = cardMesh.GetComponent<Renderer>().material;
					if (material.HasProperty("_SpecularIntensity"))
					{
						material.SetFloat("_SpecularIntensity", 0f);
					}
				}
			}
			this.m_cardBackObject = gameObject;
			SceneUtils.SetLayer(this.m_cardBackObject, this.m_cardBackContainer.gameObject.layer);
			GameUtils.SetParent(this.m_cardBackObject, this.m_cardBackContainer, false);
			this.m_cardBackObject.transform.localPosition = Vector3.zero;
			this.m_cardBackObject.transform.localScale = Vector3.one;
			this.m_cardBackObject.transform.localRotation = Quaternion.identity;
			AnimationUtil.FloatyPosition(this.m_cardBackContainer, this.m_driftRadius, this.m_driftTime);
			if (this.m_cardBackContainer != null)
			{
				this.m_cardBackContainer.SetActive(false);
			}
			this.m_cardBackObjectLoading = false;
		}, "Card_Hidden");
	}

	// Token: 0x060036E4 RID: 14052 RVA: 0x0010DE54 File Offset: 0x0010C054
	private IEnumerator AnimateCardBackIn()
	{
		this.m_cardBackText.gameObject.SetActive(false);
		this.m_cardBackAppearAnimation.Stop(this.m_cardBackAppearAnimationName);
		this.m_cardBackAppearAnimation.Rewind(this.m_cardBackAppearAnimationName);
		yield return new WaitForSeconds(this.m_cardBackAppearDelay);
		while (this.m_cardBackObjectLoading)
		{
			yield return null;
		}
		this.m_cardBackContainer.SetActive(true);
		this.m_cardBackAppearAnimation.Play(this.m_cardBackAppearAnimationName);
		if (!string.IsNullOrEmpty(this.m_cardBackAppearSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_cardBackAppearSound));
		}
		yield return new WaitForSeconds(this.m_cardBackAppearTime);
		if (this.m_cardBackObject != null)
		{
			this.m_cardBackObject.SetActive(true);
		}
		this.m_cardBackText.gameObject.SetActive(true);
		this.m_cardBackText.transform.localScale = Vector3.one * 0.01f;
		iTween.ScaleTo(this.m_cardBackText.gameObject, iTween.Hash(new object[]
		{
			"scale",
			this.m_cardBackTextOrigScale,
			"time",
			this.m_cardBackAppearTime
		}));
		yield break;
	}

	// Token: 0x04002224 RID: 8740
	public GameObject m_cardBackContainer;

	// Token: 0x04002225 RID: 8741
	public Animation m_cardBackAppearAnimation;

	// Token: 0x04002226 RID: 8742
	public UberText m_cardBackText;

	// Token: 0x04002227 RID: 8743
	public string m_cardBackAppearAnimationName;

	// Token: 0x04002228 RID: 8744
	public float m_cardBackAppearDelay = 0.5f;

	// Token: 0x04002229 RID: 8745
	public float m_cardBackAppearTime = 0.5f;

	// Token: 0x0400222A RID: 8746
	public float m_driftRadius = 0.1f;

	// Token: 0x0400222B RID: 8747
	public float m_driftTime = 10f;

	// Token: 0x0400222C RID: 8748
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_cardBackAppearSound;

	// Token: 0x0400222D RID: 8749
	private int m_cardBackId = -1;

	// Token: 0x0400222E RID: 8750
	private GameObject m_cardBackObject;

	// Token: 0x0400222F RID: 8751
	private bool m_cardBackObjectLoading;

	// Token: 0x04002230 RID: 8752
	private Vector3 m_cardBackTextOrigScale;
}
