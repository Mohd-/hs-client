using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000445 RID: 1093
[CustomEditClass]
public class AdventureRewardsCardback : MonoBehaviour
{
	// Token: 0x0600366D RID: 13933 RVA: 0x0010C094 File Offset: 0x0010A294
	public void ShowCardBackReward()
	{
		this.HideCardBackReward();
		if (this.m_cardBackAppearAnimation == null || string.IsNullOrEmpty(this.m_cardBackAppearAnimationName) || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		base.StopCoroutine("AnimateCardBackIn");
		base.StartCoroutine("AnimateCardBackIn");
	}

	// Token: 0x0600366E RID: 13934 RVA: 0x0010C0F0 File Offset: 0x0010A2F0
	public void HideCardBackReward()
	{
		if (this.m_cardBackObject != null)
		{
			this.m_cardBackObject.SetActive(false);
		}
		if (this.m_cardBackText != null)
		{
			this.m_cardBackText.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600366F RID: 13935 RVA: 0x0010C13C File Offset: 0x0010A33C
	private void Awake()
	{
		if (Application.isPlaying)
		{
			this.LoadCardBackWithId();
		}
		this.m_cardBackTextOrigScale = this.m_cardBackText.transform.localScale;
	}

	// Token: 0x06003670 RID: 13936 RVA: 0x0010C170 File Offset: 0x0010A370
	private void LoadCardBackWithId()
	{
		if (this.m_cardBackObject != null)
		{
			Object.Destroy(this.m_cardBackObject);
		}
		if (this.m_cardBackId < 0)
		{
			Debug.LogError("Card back ID must be a positive number");
			return;
		}
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
			this.HideCardBackReward();
			this.m_cardBackObjectLoading = false;
		}, "Card_Hidden");
	}

	// Token: 0x06003671 RID: 13937 RVA: 0x0010C1D8 File Offset: 0x0010A3D8
	private IEnumerator AnimateCardBackIn()
	{
		this.m_cardBackAppearAnimation.Stop(this.m_cardBackAppearAnimationName);
		this.m_cardBackAppearAnimation.Rewind(this.m_cardBackAppearAnimationName);
		yield return new WaitForSeconds(this.m_cardBackAppearDelay);
		this.m_cardBackAppearAnimation.Play(this.m_cardBackAppearAnimationName);
		if (!string.IsNullOrEmpty(this.m_cardBackAppearSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_cardBackAppearSound));
		}
		yield return new WaitForSeconds(this.m_cardBackAppearTime);
		while (this.m_cardBackObjectLoading)
		{
			yield return null;
		}
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

	// Token: 0x040021D8 RID: 8664
	public GeneralStoreAdventureContent m_parentContent;

	// Token: 0x040021D9 RID: 8665
	public GameObject m_cardBackContainer;

	// Token: 0x040021DA RID: 8666
	public Animation m_cardBackAppearAnimation;

	// Token: 0x040021DB RID: 8667
	public UberText m_cardBackText;

	// Token: 0x040021DC RID: 8668
	public string m_cardBackAppearAnimationName;

	// Token: 0x040021DD RID: 8669
	public float m_cardBackAppearDelay = 0.5f;

	// Token: 0x040021DE RID: 8670
	public float m_cardBackAppearTime = 0.5f;

	// Token: 0x040021DF RID: 8671
	public int m_cardBackId = -1;

	// Token: 0x040021E0 RID: 8672
	public float m_driftRadius = 0.1f;

	// Token: 0x040021E1 RID: 8673
	public float m_driftTime = 10f;

	// Token: 0x040021E2 RID: 8674
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_cardBackAppearSound;

	// Token: 0x040021E3 RID: 8675
	private GameObject m_cardBackObject;

	// Token: 0x040021E4 RID: 8676
	private bool m_cardBackObjectLoading;

	// Token: 0x040021E5 RID: 8677
	private Vector3 m_cardBackTextOrigScale;
}
