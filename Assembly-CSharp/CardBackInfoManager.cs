using System;
using UnityEngine;

// Token: 0x020006EC RID: 1772
[CustomEditClass]
public class CardBackInfoManager : MonoBehaviour
{
	// Token: 0x06004927 RID: 18727 RVA: 0x0015D7B0 File Offset: 0x0015B9B0
	public static CardBackInfoManager Get()
	{
		if (CardBackInfoManager.s_instance == null)
		{
			string name = (!UniversalInputManager.UsePhoneUI) ? "CardBackInfoManager" : "CardBackInfoManager_phone";
			CardBackInfoManager.s_instance = AssetLoader.Get().LoadGameObject(name, true, false).GetComponent<CardBackInfoManager>();
		}
		return CardBackInfoManager.s_instance;
	}

	// Token: 0x06004928 RID: 18728 RVA: 0x0015D808 File Offset: 0x0015BA08
	private void Awake()
	{
		this.m_previewPane.SetActive(false);
		this.SetupUI();
	}

	// Token: 0x06004929 RID: 18729 RVA: 0x0015D81C File Offset: 0x0015BA1C
	private void OnDestroy()
	{
		CardBackInfoManager.s_instance = null;
	}

	// Token: 0x0600492A RID: 18730 RVA: 0x0015D824 File Offset: 0x0015BA24
	public void EnterPreview(CollectionCardVisual cardVisual)
	{
		Actor actor = cardVisual.GetActor();
		if (actor == null)
		{
			Debug.LogError("Unable to obtain actor from card visual.");
			return;
		}
		CollectionCardBack component = actor.GetComponent<CollectionCardBack>();
		if (component == null)
		{
			Debug.LogError("Actor does not contain a CollectionCardBack component!");
			return;
		}
		this.EnterPreview(component.GetCardBackId(), cardVisual);
	}

	// Token: 0x0600492B RID: 18731 RVA: 0x0015D87C File Offset: 0x0015BA7C
	public void EnterPreview(int cardBackIdx, CollectionCardVisual cardVisual)
	{
		if (this.m_animating)
		{
			return;
		}
		if (this.m_currentCardBack != null)
		{
			Object.Destroy(this.m_currentCardBack);
			this.m_currentCardBack = null;
		}
		this.m_animating = true;
		CardBackDbfRecord record = GameDbf.CardBack.GetRecord(cardBackIdx);
		this.m_title.Text = record.Name;
		this.m_description.Text = record.Description;
		bool flag = false;
		if (!CollectionManager.Get().IsInEditMode())
		{
			NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
			int defaultCardBack = netObject.DefaultCardBack;
			flag = (CardBackManager.Get().IsCardBackOwned(cardBackIdx) && defaultCardBack != cardBackIdx);
		}
		this.m_favoriteButton.SetEnabled(flag);
		this.m_favoriteButton.Flip(flag);
		this.m_currentCardBackIdx = cardBackIdx;
		if (!CardBackManager.Get().LoadCardBackByIndex(cardBackIdx, delegate(CardBackManager.LoadCardBackData cardBackData)
		{
			GameObject gameObject = cardBackData.m_GameObject;
			gameObject.name = "CARD_BACK_" + cardBackIdx;
			SceneUtils.SetLayer(gameObject, this.m_cardBackContainer.gameObject.layer);
			GameUtils.SetParent(gameObject, this.m_cardBackContainer, false);
			this.m_currentCardBack = gameObject;
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_currentCardBack.transform.localPosition = Vector3.zero;
			}
			else
			{
				this.m_currentCardBack.transform.position = cardVisual.transform.position;
				iTween.MoveTo(this.m_currentCardBack.gameObject, iTween.Hash(new object[]
				{
					"name",
					"FinishBigCardMove",
					"position",
					this.m_cardBackContainer.transform.position,
					"time",
					this.m_animationTime
				}));
				iTween.ScaleTo(this.m_currentCardBack.gameObject, iTween.Hash(new object[]
				{
					"scale",
					Vector3.one,
					"time",
					this.m_animationTime,
					"easeType",
					iTween.EaseType.easeOutQuad
				}));
				iTween.PunchRotation(this.m_currentCardBack, iTween.Hash(new object[]
				{
					"amount",
					new Vector3(0f, 0f, 75f),
					"time",
					2.5f
				}));
			}
			this.m_currentCardBack.transform.localScale = Vector3.one;
			this.m_currentCardBack.transform.localRotation = Quaternion.identity;
			this.m_previewPane.SetActive(true);
			this.m_offClicker.gameObject.SetActive(true);
			iTween.ScaleFrom(this.m_previewPane, iTween.Hash(new object[]
			{
				"scale",
				new Vector3(0.01f, 0.01f, 0.01f),
				"time",
				this.m_animationTime,
				"easeType",
				iTween.EaseType.easeOutCirc,
				"oncomplete",
				delegate(object e)
				{
					this.m_animating = false;
				}
			}));
		}, "Card_Hidden"))
		{
			Debug.LogError(string.Format("Unable to load card back ID {0} for preview.", cardBackIdx));
			this.m_animating = false;
		}
		if (!string.IsNullOrEmpty(this.m_enterPreviewSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_enterPreviewSound));
		}
		FullScreenFXMgr.Get().StartStandardBlurVignette(this.m_animationTime);
	}

	// Token: 0x0600492C RID: 18732 RVA: 0x0015DA14 File Offset: 0x0015BC14
	public void CancelPreview()
	{
		if (this.m_animating)
		{
			return;
		}
		Vector3 origScale = this.m_previewPane.transform.localScale;
		this.m_animating = true;
		iTween.ScaleTo(this.m_previewPane, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.01f, 0.01f, 0.01f),
			"time",
			this.m_animationTime,
			"easeType",
			iTween.EaseType.easeOutCirc,
			"oncomplete",
			delegate(object e)
			{
				this.m_animating = false;
				this.m_previewPane.transform.localScale = origScale;
				this.m_previewPane.SetActive(false);
				this.m_offClicker.gameObject.SetActive(false);
			}
		}));
		iTween.ScaleTo(this.m_currentCardBack, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.01f, 0.01f, 0.01f),
			"time",
			this.m_animationTime,
			"easeType",
			iTween.EaseType.easeOutCirc,
			"oncomplete",
			delegate(object e)
			{
				this.m_currentCardBack.SetActive(false);
			}
		}));
		if (!string.IsNullOrEmpty(this.m_exitPreviewSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_exitPreviewSound));
		}
		FullScreenFXMgr.Get().EndStandardBlurVignette(this.m_animationTime, null);
	}

	// Token: 0x0600492D RID: 18733 RVA: 0x0015DB80 File Offset: 0x0015BD80
	private void SetupUI()
	{
		this.m_favoriteButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.SetFavorite();
			this.CancelPreview();
		});
		this.m_offClicker.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.CancelPreview();
		});
		this.m_offClicker.AddEventListener(UIEventType.RIGHTCLICK, delegate(UIEvent e)
		{
			this.CancelPreview();
		});
	}

	// Token: 0x0600492E RID: 18734 RVA: 0x0015DBD8 File Offset: 0x0015BDD8
	private void SetFavorite()
	{
		NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
		if (this.m_currentCardBackIdx != netObject.DefaultCardBack)
		{
			ConnectAPI.SetDefaultCardBack(this.m_currentCardBackIdx);
		}
	}

	// Token: 0x0400303E RID: 12350
	public GameObject m_previewPane;

	// Token: 0x0400303F RID: 12351
	public GameObject m_cardBackContainer;

	// Token: 0x04003040 RID: 12352
	public UberText m_title;

	// Token: 0x04003041 RID: 12353
	public UberText m_description;

	// Token: 0x04003042 RID: 12354
	public UIBButton m_favoriteButton;

	// Token: 0x04003043 RID: 12355
	public PegUIElement m_offClicker;

	// Token: 0x04003044 RID: 12356
	public float m_animationTime = 0.5f;

	// Token: 0x04003045 RID: 12357
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_enterPreviewSound;

	// Token: 0x04003046 RID: 12358
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_exitPreviewSound;

	// Token: 0x04003047 RID: 12359
	private int m_currentCardBackIdx;

	// Token: 0x04003048 RID: 12360
	private GameObject m_currentCardBack;

	// Token: 0x04003049 RID: 12361
	private bool m_animating;

	// Token: 0x0400304A RID: 12362
	private static CardBackInfoManager s_instance;
}
