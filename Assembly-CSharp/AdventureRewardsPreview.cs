using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003BC RID: 956
[CustomEditClass]
public class AdventureRewardsPreview : MonoBehaviour
{
	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x06003223 RID: 12835 RVA: 0x000FBE5C File Offset: 0x000FA05C
	// (set) Token: 0x06003224 RID: 12836 RVA: 0x000FBE64 File Offset: 0x000FA064
	[CustomEditField(Sections = "Cards Preview")]
	public float CardWidth
	{
		get
		{
			return this.m_CardWidth;
		}
		set
		{
			this.m_CardWidth = value;
			this.UpdateCardPositions();
		}
	}

	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x06003225 RID: 12837 RVA: 0x000FBE73 File Offset: 0x000FA073
	// (set) Token: 0x06003226 RID: 12838 RVA: 0x000FBE7B File Offset: 0x000FA07B
	[CustomEditField(Sections = "Cards Preview")]
	public float CardSpacing
	{
		get
		{
			return this.m_CardSpacing;
		}
		set
		{
			this.m_CardSpacing = value;
			this.UpdateCardPositions();
		}
	}

	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x06003227 RID: 12839 RVA: 0x000FBE8A File Offset: 0x000FA08A
	// (set) Token: 0x06003228 RID: 12840 RVA: 0x000FBE92 File Offset: 0x000FA092
	[CustomEditField(Sections = "Cards Preview")]
	public float CardClumpAngleIncrement
	{
		get
		{
			return this.m_CardClumpAngleIncrement;
		}
		set
		{
			this.m_CardClumpAngleIncrement = value;
			this.UpdateCardPositions();
		}
	}

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x06003229 RID: 12841 RVA: 0x000FBEA1 File Offset: 0x000FA0A1
	// (set) Token: 0x0600322A RID: 12842 RVA: 0x000FBEA9 File Offset: 0x000FA0A9
	[CustomEditField(Sections = "Cards Preview")]
	public Vector3 CardClumpSpacing
	{
		get
		{
			return this.m_CardClumpSpacing;
		}
		set
		{
			this.m_CardClumpSpacing = value;
			this.UpdateCardPositions();
		}
	}

	// Token: 0x0600322B RID: 12843 RVA: 0x000FBEB8 File Offset: 0x000FA0B8
	private void Awake()
	{
		if (this.m_BackButton != null)
		{
			this.m_BackButton.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
			{
				Navigation.GoBack();
			});
		}
	}

	// Token: 0x0600322C RID: 12844 RVA: 0x000FBEF5 File Offset: 0x000FA0F5
	public void AddHideListener(AdventureRewardsPreview.OnHide dlg)
	{
		this.m_OnHideListeners.Add(dlg);
	}

	// Token: 0x0600322D RID: 12845 RVA: 0x000FBF03 File Offset: 0x000FA103
	public void RemoveHideListener(AdventureRewardsPreview.OnHide dlg)
	{
		this.m_OnHideListeners.Remove(dlg);
	}

	// Token: 0x0600322E RID: 12846 RVA: 0x000FBF12 File Offset: 0x000FA112
	private bool OnNavigateBack()
	{
		this.Show(false);
		return true;
	}

	// Token: 0x0600322F RID: 12847 RVA: 0x000FBF1C File Offset: 0x000FA11C
	public void SetHeaderText(string text)
	{
		this.m_HeaderTextObject.Text = GameStrings.Format("GLUE_ADVENTURE_REWARDS_PREVIEW_HEADER", new object[]
		{
			text
		});
	}

	// Token: 0x06003230 RID: 12848 RVA: 0x000FBF40 File Offset: 0x000FA140
	public void AddSpecificCards(List<string> cardIds)
	{
		foreach (string text in cardIds)
		{
			List<string> list = new List<string>();
			list.Add(text);
			this.AddCardBatch(list);
		}
	}

	// Token: 0x06003231 RID: 12849 RVA: 0x000FBFA4 File Offset: 0x000FA1A4
	public void AddCardBatch(int scenarioId)
	{
		List<CardRewardData> immediateCardRewardsForDefeatingScenario = AdventureProgressMgr.Get().GetImmediateCardRewardsForDefeatingScenario(scenarioId);
		this.AddCardBatch(immediateCardRewardsForDefeatingScenario);
	}

	// Token: 0x06003232 RID: 12850 RVA: 0x000FBFC4 File Offset: 0x000FA1C4
	public void AddCardBatch(List<CardRewardData> rewards)
	{
		List<string> list = new List<string>();
		foreach (CardRewardData cardRewardData in rewards)
		{
			list.Add(cardRewardData.CardID);
		}
		this.AddCardBatch(list);
	}

	// Token: 0x06003233 RID: 12851 RVA: 0x000FC02C File Offset: 0x000FA22C
	public void AddCardBatch(List<string> cardIds)
	{
		List<Actor> list = new List<Actor>();
		this.m_CardBatches.Add(list);
		this.AddCardBatch(cardIds, list);
	}

	// Token: 0x06003234 RID: 12852 RVA: 0x000FC053 File Offset: 0x000FA253
	public void SetHiddenCardCount(int hiddenCardCount)
	{
		this.m_HiddenCardCount = hiddenCardCount;
	}

	// Token: 0x06003235 RID: 12853 RVA: 0x000FC05C File Offset: 0x000FA25C
	public void Reset()
	{
		foreach (List<Actor> list in this.m_CardBatches)
		{
			foreach (Actor actor in list)
			{
				if (actor != null)
				{
					Object.Destroy(actor.gameObject);
				}
			}
		}
		this.m_HiddenCardCount = 0;
		this.m_CardBatches.Clear();
	}

	// Token: 0x06003236 RID: 12854 RVA: 0x000FC118 File Offset: 0x000FA318
	public void Show(bool show)
	{
		if (this.m_ClickBlocker != null)
		{
			this.m_ClickBlocker.SetActive(show);
		}
		if (this.m_DisableScrollbar != null)
		{
			this.m_DisableScrollbar.Enable(!show);
		}
		if (show)
		{
			this.UpdateCardPositions();
			FullScreenFXMgr.Get().StartStandardBlurVignette(this.m_ShowHideAnimationTime);
			base.gameObject.SetActive(true);
			iTween.ScaleFrom(base.gameObject, iTween.Hash(new object[]
			{
				"scale",
				Vector3.one * 0.05f,
				"time",
				this.m_ShowHideAnimationTime
			}));
			if (!string.IsNullOrEmpty(this.m_PreviewAppearSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_PreviewAppearSound));
			}
			Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
		}
		else
		{
			Vector3 origScale = base.transform.localScale;
			iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
			{
				"scale",
				Vector3.one * 0.05f,
				"time",
				this.m_ShowHideAnimationTime,
				"oncomplete",
				delegate(object o)
				{
					this.gameObject.SetActive(false);
					this.transform.localScale = origScale;
					this.FireHideEvent();
				}
			}));
			if (!string.IsNullOrEmpty(this.m_PreviewShrinkSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_PreviewShrinkSound));
			}
			FullScreenFXMgr.Get().EndStandardBlurVignette(this.m_ShowHideAnimationTime, null);
		}
	}

	// Token: 0x06003237 RID: 12855 RVA: 0x000FC2C8 File Offset: 0x000FA4C8
	private void AddCardBatch(List<string> cardIds, List<Actor> cardBatch)
	{
		if (cardIds == null || cardIds.Count == 0)
		{
			return;
		}
		string cardId;
		foreach (string cardId2 in cardIds)
		{
			cardId = cardId2;
			FullDef fullDef = DefLoader.Get().GetFullDef(cardId, null);
			GameObject gameObject = AssetLoader.Get().LoadActor(ActorNames.GetHandActor(fullDef.GetEntityDef(), TAG_PREMIUM.NORMAL), false, false);
			Actor actor = gameObject.GetComponent<Actor>();
			actor.SetCardDef(fullDef.GetCardDef());
			actor.SetEntityDef(fullDef.GetEntityDef());
			GameUtils.SetParent(actor, this.m_CardsContainer, false);
			SceneUtils.SetLayer(actor, this.m_CardsContainer.gameObject.layer);
			cardBatch.Add(actor);
			if (this.m_PreviewCardsExpandable && this.m_CardsPreviewDisplay != null)
			{
				PegUIElement pegUIElement = actor.m_cardMesh.gameObject.AddComponent<PegUIElement>();
				pegUIElement.GetComponent<Collider>().enabled = true;
				pegUIElement.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
				{
					if (!this.m_CardsPreviewDisplay.IsShowing())
					{
						List<string> list = new List<string>();
						list.Add(cardId);
						this.m_CardsPreviewDisplay.ShowCards(list, actor.transform.position, new Vector3?(actor.transform.position));
					}
				});
			}
		}
	}

	// Token: 0x06003238 RID: 12856 RVA: 0x000FC450 File Offset: 0x000FA650
	private void UpdateCardPositions()
	{
		int num = this.m_CardBatches.Count;
		bool flag = this.m_HiddenCardCount > 0;
		bool flag2 = this.m_HiddenCardsLabelObject != null;
		if (flag && flag2)
		{
			num++;
		}
		float num2 = (float)(num - 1) * this.m_CardSpacing + (float)num * this.m_CardWidth;
		float num3 = num2 * 0.5f - this.m_CardWidth * 0.5f;
		int num4 = 0;
		foreach (List<Actor> list in this.m_CardBatches)
		{
			if (list.Count != 0)
			{
				int num5 = 0;
				foreach (Actor actor in list)
				{
					if (!(actor == null))
					{
						Vector3 localPosition = this.m_CardClumpSpacing * (float)num5;
						localPosition.x += (float)num4 * (this.m_CardSpacing + this.m_CardWidth) - num3;
						actor.transform.localScale = Vector3.one * 5f;
						actor.transform.localRotation = Quaternion.identity;
						actor.transform.Rotate(new Vector3(0f, 1f, 0f), (float)num5 * this.m_CardClumpAngleIncrement);
						actor.transform.localPosition = localPosition;
						actor.SetUnlit();
						actor.ContactShadow(true);
						actor.UpdateAllComponents();
						actor.Show();
						num5++;
					}
				}
				num4++;
			}
		}
		if (flag && flag2)
		{
			Vector3 zero = Vector3.zero;
			zero.x += (float)num4 * (this.m_CardSpacing + this.m_CardWidth) - num3;
			this.m_HiddenCardsLabelObject.transform.localPosition = zero;
			this.m_HiddenCardsLabel.Text = string.Format("+{0}", this.m_HiddenCardCount);
		}
		if (flag2)
		{
			this.m_HiddenCardsLabelObject.SetActive(flag);
		}
	}

	// Token: 0x06003239 RID: 12857 RVA: 0x000FC6C0 File Offset: 0x000FA8C0
	private void FireHideEvent()
	{
		AdventureRewardsPreview.OnHide[] array = this.m_OnHideListeners.ToArray();
		foreach (AdventureRewardsPreview.OnHide onHide in array)
		{
			onHide();
		}
	}

	// Token: 0x04001F59 RID: 8025
	[CustomEditField(Sections = "Cards Preview")]
	public GameObject m_CardsContainer;

	// Token: 0x04001F5A RID: 8026
	[SerializeField]
	private float m_CardWidth = 30f;

	// Token: 0x04001F5B RID: 8027
	[SerializeField]
	private float m_CardSpacing = 5f;

	// Token: 0x04001F5C RID: 8028
	[SerializeField]
	private float m_CardClumpAngleIncrement = 10f;

	// Token: 0x04001F5D RID: 8029
	[SerializeField]
	private Vector3 m_CardClumpSpacing = Vector3.zero;

	// Token: 0x04001F5E RID: 8030
	[CustomEditField(Sections = "Cards Preview")]
	public UberText m_HeaderTextObject;

	// Token: 0x04001F5F RID: 8031
	[CustomEditField(Sections = "Cards Preview")]
	public PegUIElement m_BackButton;

	// Token: 0x04001F60 RID: 8032
	[CustomEditField(Sections = "Cards Preview")]
	public GameObject m_ClickBlocker;

	// Token: 0x04001F61 RID: 8033
	[CustomEditField(Sections = "Cards Preview")]
	public UIBScrollable m_DisableScrollbar;

	// Token: 0x04001F62 RID: 8034
	[CustomEditField(Sections = "Cards Preview")]
	public float m_ShowHideAnimationTime = 0.15f;

	// Token: 0x04001F63 RID: 8035
	[CustomEditField(Sections = "Cards Preview")]
	public bool m_PreviewCardsExpandable;

	// Token: 0x04001F64 RID: 8036
	[CustomEditField(Sections = "Cards Preview/Hidden Cards")]
	public GameObject m_HiddenCardsLabelObject;

	// Token: 0x04001F65 RID: 8037
	[CustomEditField(Sections = "Cards Preview/Hidden Cards")]
	public UberText m_HiddenCardsLabel;

	// Token: 0x04001F66 RID: 8038
	[CustomEditField(Sections = "Cards Preview", Parent = "m_PreviewCardsExpandable")]
	public AdventureRewardsDisplayArea m_CardsPreviewDisplay;

	// Token: 0x04001F67 RID: 8039
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_PreviewAppearSound;

	// Token: 0x04001F68 RID: 8040
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_PreviewShrinkSound;

	// Token: 0x04001F69 RID: 8041
	private List<List<Actor>> m_CardBatches = new List<List<Actor>>();

	// Token: 0x04001F6A RID: 8042
	private List<AdventureRewardsPreview.OnHide> m_OnHideListeners = new List<AdventureRewardsPreview.OnHide>();

	// Token: 0x04001F6B RID: 8043
	private int m_HiddenCardCount;

	// Token: 0x020003C6 RID: 966
	// (Invoke) Token: 0x06003260 RID: 12896
	public delegate void OnHide();
}
