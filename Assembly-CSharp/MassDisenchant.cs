using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000547 RID: 1351
public class MassDisenchant : MonoBehaviour
{
	// Token: 0x06003E33 RID: 15923 RVA: 0x0012CDA0 File Offset: 0x0012AFA0
	private void Awake()
	{
		MassDisenchant.s_Instance = this;
		this.m_headlineText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_HEADLINE");
		this.m_detailsHeadlineText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_DETAILS_HEADLINE");
		this.m_disenchantButton.SetText(GameStrings.Get("GLUE_MASS_DISENCHANT_BUTTON_TEXT"));
		if (this.m_detailsText != null)
		{
			this.m_detailsText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_DETAILS");
		}
		if (this.m_singleSubHeadlineText != null)
		{
			this.m_singleSubHeadlineText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_SUB_HEADLINE_TEXT");
		}
		if (this.m_doubleSubHeadlineText != null)
		{
			this.m_doubleSubHeadlineText.Text = GameStrings.Get("GLUE_MASS_DISENCHANT_SUB_HEADLINE_TEXT");
		}
		this.m_disenchantButton.SetUserOverYOffset(-0.04409015f);
		foreach (DisenchantBar disenchantBar in this.m_singleDisenchantBars)
		{
			disenchantBar.Init();
		}
		foreach (DisenchantBar disenchantBar2 in this.m_doubleDisenchantBars)
		{
			disenchantBar2.Init();
		}
		CollectionManager.Get().RegisterMassDisenchantListener(new CollectionManager.OnMassDisenchant(this.OnMassDisenchant));
	}

	// Token: 0x06003E34 RID: 15924 RVA: 0x0012CF20 File Offset: 0x0012B120
	private void Start()
	{
		this.m_disenchantButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDisenchantButtonPressed));
		this.m_disenchantButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDisenchantButtonOver));
		this.m_disenchantButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnDisenchantButtonOut));
		if (this.m_infoButton != null)
		{
			this.m_infoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoButtonPressed));
		}
	}

	// Token: 0x06003E35 RID: 15925 RVA: 0x0012CFA2 File Offset: 0x0012B1A2
	public static MassDisenchant Get()
	{
		return MassDisenchant.s_Instance;
	}

	// Token: 0x06003E36 RID: 15926 RVA: 0x0012CFA9 File Offset: 0x0012B1A9
	public void Show()
	{
		this.m_root.SetActive(true);
	}

	// Token: 0x06003E37 RID: 15927 RVA: 0x0012CFB7 File Offset: 0x0012B1B7
	public void Hide()
	{
		this.m_root.SetActive(false);
	}

	// Token: 0x06003E38 RID: 15928 RVA: 0x0012CFC8 File Offset: 0x0012B1C8
	private void OnDestroy()
	{
		foreach (GameObject gameObject in this.m_cleanupObjects)
		{
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
		}
		CollectionManager.Get().RemoveMassDisenchantListener(new CollectionManager.OnMassDisenchant(this.OnMassDisenchant));
	}

	// Token: 0x06003E39 RID: 15929 RVA: 0x0012D044 File Offset: 0x0012B244
	public int GetTotalAmount()
	{
		return this.m_totalAmount;
	}

	// Token: 0x06003E3A RID: 15930 RVA: 0x0012D04C File Offset: 0x0012B24C
	public void UpdateContents(List<CollectibleCard> disenchantCards)
	{
		List<CollectibleCard> list = Enumerable.ToList<CollectibleCard>(Enumerable.Where<CollectibleCard>(disenchantCards, (CollectibleCard c) => c.PremiumType == TAG_PREMIUM.GOLDEN));
		this.m_useSingle = (list.Count == 0);
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_useSingle = true;
		}
		List<DisenchantBar> list2 = (!this.m_useSingle) ? this.m_doubleDisenchantBars : this.m_singleDisenchantBars;
		foreach (DisenchantBar disenchantBar in list2)
		{
			disenchantBar.Reset();
		}
		this.m_totalAmount = 0;
		this.m_totalCardsToDisenchant = 0;
		CollectibleCard card;
		foreach (CollectibleCard card2 in disenchantCards)
		{
			card = card2;
			NetCache.CardValue cardValue = CraftingManager.Get().GetCardValue(card.CardId, card.PremiumType);
			if (cardValue != null)
			{
				EntityDef entityDef = DefLoader.Get().GetEntityDef(card.CardId);
				int num = cardValue.Sell * card.DisenchantCount;
				DisenchantBar disenchantBar2 = list2.Find((DisenchantBar obj) => (obj.m_premiumType == card.PremiumType || UniversalInputManager.UsePhoneUI) && obj.m_rarity == entityDef.GetRarity());
				if (disenchantBar2 == null)
				{
					Debug.LogWarning(string.Format("MassDisenchant.UpdateContents(): Could not find {0} bar to modify for card {1} (premium {2}, disenchant count {3})", new object[]
					{
						(!this.m_useSingle) ? "double" : "single",
						entityDef,
						card.PremiumType,
						card.DisenchantCount
					}));
				}
				else
				{
					disenchantBar2.AddCards(card.DisenchantCount, num, card.PremiumType);
					this.m_totalCardsToDisenchant += card.DisenchantCount;
					this.m_totalAmount += num;
				}
			}
		}
		if (this.m_totalAmount > 0)
		{
			this.m_singleRoot.SetActive(this.m_useSingle);
			if (this.m_doubleRoot != null)
			{
				this.m_doubleRoot.SetActive(!this.m_useSingle);
			}
			this.m_disenchantButton.SetEnabled(true);
		}
		foreach (DisenchantBar disenchantBar3 in list2)
		{
			disenchantBar3.UpdateVisuals(this.m_totalCardsToDisenchant);
		}
		this.m_totalAmountText.Text = GameStrings.Format("GLUE_MASS_DISENCHANT_TOTAL_AMOUNT", new object[]
		{
			this.m_totalAmount
		});
	}

	// Token: 0x06003E3B RID: 15931 RVA: 0x0012D390 File Offset: 0x0012B590
	public IEnumerator StartHighlight()
	{
		yield return null;
		this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		yield break;
	}

	// Token: 0x06003E3C RID: 15932 RVA: 0x0012D3AC File Offset: 0x0012B5AC
	public void OnMassDisenchant(int amount)
	{
		GraphicsManager graphicsManager = GraphicsManager.Get();
		GraphicsQuality renderQualityLevel = graphicsManager.RenderQualityLevel;
		int maxGlowBalls;
		if (renderQualityLevel != GraphicsQuality.Low)
		{
			if (renderQualityLevel != GraphicsQuality.Medium)
			{
				maxGlowBalls = 10;
			}
			else
			{
				maxGlowBalls = 6;
			}
		}
		else
		{
			maxGlowBalls = 3;
		}
		this.BlockUI(true);
		base.StartCoroutine(this.DoDisenchantAnims(maxGlowBalls, amount));
	}

	// Token: 0x06003E3D RID: 15933 RVA: 0x0012D407 File Offset: 0x0012B607
	private void BlockUI(bool block = true)
	{
		this.m_FX.m_blockInteraction.SetActive(block);
	}

	// Token: 0x06003E3E RID: 15934 RVA: 0x0012D41C File Offset: 0x0012B61C
	private void OnDisenchantButtonOver(UIEvent e)
	{
		if (CollectionManagerDisplay.Get().m_pageManager.IsShowingMassDisenchant())
		{
			this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
			SoundManager.Get().LoadAndPlay("Hub_Mouseover");
		}
		else
		{
			this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		}
	}

	// Token: 0x06003E3F RID: 15935 RVA: 0x0012D478 File Offset: 0x0012B678
	private void OnDisenchantButtonOut(UIEvent e)
	{
		if (CollectionManagerDisplay.Get().m_pageManager.IsShowingMassDisenchant())
		{
			this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
		else
		{
			this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		}
	}

	// Token: 0x06003E40 RID: 15936 RVA: 0x0012D4C4 File Offset: 0x0012B6C4
	private void OnDisenchantButtonPressed(UIEvent e)
	{
		Options.Get().SetBool(Option.HAS_DISENCHANTED, true);
		this.m_disenchantButton.SetEnabled(false);
		this.m_FX.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		Network.MassDisenchant();
	}

	// Token: 0x06003E41 RID: 15937 RVA: 0x0012D4F8 File Offset: 0x0012B6F8
	private void OnInfoButtonPressed(UIEvent e)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_MASS_DISENCHANT_BUTTON_TEXT");
		popupInfo.m_text = string.Format("{0}\n\n{1}", GameStrings.Get("GLUE_MASS_DISENCHANT_DETAILS_HEADLINE"), GameStrings.Get("GLUE_MASS_DISENCHANT_DETAILS"));
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06003E42 RID: 15938 RVA: 0x0012D558 File Offset: 0x0012B758
	private void Unbloomify(List<GameObject> glows, float newVal)
	{
		foreach (GameObject gameObject in glows)
		{
			gameObject.GetComponent<RenderToTexture>().m_BloomIntensity = newVal;
		}
	}

	// Token: 0x06003E43 RID: 15939 RVA: 0x0012D5B4 File Offset: 0x0012B7B4
	private void UncolorTotal(float newVal)
	{
		this.m_totalAmountText.TextColor = Color.Lerp(Color.white, new Color(0.7f, 0.85f, 1f, 1f), newVal);
	}

	// Token: 0x06003E44 RID: 15940 RVA: 0x0012D5E8 File Offset: 0x0012B7E8
	private void SetGemSaturation(List<DisenchantBar> disenchantBars, float saturation, bool onlyActive = false, bool onlyInactive = false)
	{
		foreach (DisenchantBar disenchantBar in disenchantBars)
		{
			int numCards = disenchantBar.GetNumCards();
			if ((onlyActive && numCards != 0) || (onlyInactive && numCards == 0) || (!onlyInactive && !onlyActive))
			{
				disenchantBar.m_rarityGem.GetComponent<Renderer>().material.SetColor("_Fade", new Color(saturation, saturation, saturation, 1f));
			}
		}
	}

	// Token: 0x06003E45 RID: 15941 RVA: 0x0012D68C File Offset: 0x0012B88C
	private IEnumerator DoDisenchantAnims(int maxGlowBalls, int disenchantTotal)
	{
		if (disenchantTotal == 0)
		{
			yield return null;
		}
		this.m_origTotalScale = this.m_totalAmountText.transform.localScale;
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_origDustScale = ArcaneDustAmount.Get().m_dustJar.transform.localScale;
		}
		else
		{
			this.m_origDustScale = BnetBar.Get().m_currencyFrame.m_dustJar.transform.localScale;
		}
		List<DisenchantBar> disenchantBars = (!this.m_useSingle) ? this.m_doubleDisenchantBars : this.m_singleDisenchantBars;
		float vigTime = 0.2f;
		FullScreenFXMgr.Get().Vignette(0.8f, vigTime, iTween.EaseType.easeInOutCubic, null);
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"from",
			1f,
			"to",
			0.3f,
			"time",
			vigTime,
			"onupdate",
			delegate(object newVal)
			{
				this.SetGemSaturation(disenchantBars, (float)newVal, false, false);
			}
		}));
		if (this.m_sound.m_intro != string.Empty)
		{
			SoundManager.Get().LoadAndPlay(this.m_sound.m_intro);
		}
		yield return new WaitForSeconds(vigTime);
		float duration = 0.5f;
		float rate = duration / 20f;
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"from",
			0.3f,
			"to",
			1.75f,
			"time",
			1.5f * duration,
			"easeInType",
			iTween.EaseType.easeInCubic,
			"onupdate",
			delegate(object newVal)
			{
				this.SetGemSaturation(disenchantBars, (float)newVal, true, false);
			}
		}));
		List<GameObject> glows = new List<GameObject>();
		if (this.m_FX.m_glowTotal != null)
		{
			glows.Add(this.m_FX.m_glowTotal);
		}
		this.m_totalAmountText.transform.localScale = this.m_origTotalScale * 2.54f;
		iTween.ScaleTo(this.m_totalAmountText.gameObject, iTween.Hash(new object[]
		{
			"scale",
			this.m_origTotalScale,
			"time",
			3.0
		}));
		if (this.m_FX.m_glowTotal != null)
		{
			this.m_FX.m_glowTotal.SetActive(true);
		}
		this.m_highestGlowBalls = 0;
		Color glowColor = new Color(0.7f, 0.85f, 1f, 1f);
		float origYSpeed = 0f;
		float origXSpeed = 0f;
		float origInten = 0f;
		foreach (DisenchantBar bar in disenchantBars)
		{
			int numCards = bar.GetNumCards();
			if (numCards > this.m_highestGlowBalls)
			{
				this.m_highestGlowBalls = numCards;
			}
		}
		this.m_highestGlowBalls = ((this.m_highestGlowBalls <= maxGlowBalls) ? this.m_highestGlowBalls : maxGlowBalls);
		foreach (DisenchantBar bar2 in disenchantBars)
		{
			int numCards2 = bar2.GetNumCards();
			if (numCards2 > 0)
			{
				RarityFX rareFX = this.GetRarityFX(bar2);
				int maxBalls = (numCards2 <= maxGlowBalls) ? numCards2 : maxGlowBalls;
				for (int i = 0; i < maxBalls; i++)
				{
					base.StartCoroutine(this.LaunchGlowball(bar2, rareFX, i, maxBalls, this.m_highestGlowBalls));
				}
			}
		}
		int j = 0;
		while ((float)j < duration / rate)
		{
			float curDustTotal = 0f;
			foreach (DisenchantBar bar3 in disenchantBars)
			{
				RaritySound rareSound = this.GetRaritySound(bar3);
				int numCards3 = bar3.GetNumCards();
				if (j == 0 && numCards3 != 0)
				{
					if (rareSound.m_drainSound != string.Empty)
					{
						SoundManager.Get().LoadAndPlay(rareSound.m_drainSound);
					}
					if (bar3.m_numGoldText != null && bar3.m_numGoldText.gameObject.activeSelf)
					{
						bar3.m_numGoldText.gameObject.SetActive(false);
						TransformUtil.SetLocalPosX(bar3.m_numCardsText, 2.902672f);
					}
					Vector3 textScale = bar3.m_numCardsText.gameObject.transform.localScale;
					iTween.ScaleFrom(bar3.m_numCardsText.gameObject, iTween.Hash(new object[]
					{
						"x",
						textScale.x * 2.28f,
						"y",
						textScale.y * 2.28f,
						"z",
						textScale.z * 2.28f,
						"time",
						3.0
					}));
					bar3.m_numCardsText.TextColor = glowColor;
					iTween.ColorTo(bar3.m_numCardsText.gameObject, iTween.Hash(new object[]
					{
						"r",
						1f,
						"g",
						1f,
						"b",
						1f,
						"time",
						3.0
					}));
					if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.High && bar3.m_glow != null)
					{
						glows.Add(bar3.m_glow);
						bar3.m_glow.GetComponent<RenderToTexture>().m_BloomIntensity = 0.01f;
						bar3.m_glow.SetActive(true);
					}
					origYSpeed = bar3.m_rarityGem.GetComponent<Renderer>().material.GetFloat("_YSpeed");
					origXSpeed = bar3.m_rarityGem.GetComponent<Renderer>().material.GetFloat("_XSpeed");
					origInten = bar3.m_amountBar.GetComponent<Renderer>().material.GetFloat("_Intensity");
					bar3.m_rarityGem.GetComponent<Renderer>().material.SetFloat("_YSpeed", -10f);
					bar3.m_rarityGem.GetComponent<Renderer>().material.SetFloat("_XSpeed", 20f);
				}
			}
			if (j == 0)
			{
				if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.High)
				{
					iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
					{
						"from",
						1f,
						"to",
						0.1f,
						"time",
						duration * 3f,
						"onupdate",
						delegate(object newVal)
						{
							this.Unbloomify(glows, (float)newVal);
						}
					}));
				}
				iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
				{
					"from",
					1f,
					"to",
					0.1f,
					"time",
					duration * 3f,
					"onupdate",
					delegate(object newVal)
					{
						this.UncolorTotal((float)newVal);
					}
				}));
				float curTotal = (float)CraftingManager.Get().GetLocalArcaneDustBalance();
				iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
				{
					"from",
					curTotal,
					"to",
					curTotal + (float)disenchantTotal,
					"time",
					3f * duration,
					"onupdate",
					delegate(object newVal)
					{
						this.SetDustBalance((float)newVal);
					},
					"oncomplete",
					delegate(object newVal)
					{
						CollectionManagerDisplay.Get().m_pageManager.UpdateMassDisenchant();
						FullScreenFXMgr.Get().StopVignette(vigTime, iTween.EaseType.easeInOutCubic, null);
						this.BlockUI(false);
					}
				}));
			}
			foreach (DisenchantBar bar4 in disenchantBars)
			{
				if (bar4.GetNumCards() != 0)
				{
					bar4.m_amountBar.GetComponent<Renderer>().material.SetFloat("_Intensity", 2f);
					curDustTotal += this.DrainBarAndDust(bar4, j, duration, rate);
				}
			}
			this.m_totalAmountText.Text = Convert.ToInt32(curDustTotal).ToString();
			yield return new WaitForSeconds(rate / duration);
			j++;
		}
		if (this.m_FX.m_glowTotal != null)
		{
			this.m_FX.m_glowTotal.SetActive(false);
		}
		this.m_totalAmountText.Text = "0";
		this.m_totalAmountText.TextColor = Color.white;
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"from",
			0.3f,
			"to",
			1f,
			"time",
			duration,
			"delay",
			vigTime,
			"onupdate",
			delegate(object newVal)
			{
				this.SetGemSaturation(disenchantBars, (float)newVal, false, true);
			}
		}));
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"from",
			1.75f,
			"to",
			1f,
			"time",
			duration,
			"delay",
			vigTime,
			"onupdate",
			delegate(object newVal)
			{
				this.SetGemSaturation(disenchantBars, (float)newVal, true, false);
			}
		}));
		foreach (DisenchantBar bar5 in disenchantBars)
		{
			if (bar5.m_glow != null)
			{
				bar5.m_glow.SetActive(false);
			}
			bar5.m_numCardsText.TextColor = Color.white;
			bar5.m_rarityGem.GetComponent<Renderer>().material.SetFloat("_YSpeed", origYSpeed);
			bar5.m_rarityGem.GetComponent<Renderer>().material.SetFloat("_XSpeed", origXSpeed);
			bar5.m_amountBar.GetComponent<Renderer>().material.SetFloat("_Intensity", origInten);
		}
		yield break;
	}

	// Token: 0x06003E46 RID: 15942 RVA: 0x0012D6C4 File Offset: 0x0012B8C4
	private void SetDustBalance(float bal)
	{
		int num = (int)bal;
		if (UniversalInputManager.UsePhoneUI)
		{
			ArcaneDustAmount.Get().m_dustCount.Text = num.ToString();
		}
		else
		{
			BnetBar.Get().m_currencyFrame.m_amount.Text = num.ToString();
		}
	}

	// Token: 0x06003E47 RID: 15943 RVA: 0x0012D71C File Offset: 0x0012B91C
	private float DrainBarAndDust(DisenchantBar bar, int drainRun, float duration, float rate)
	{
		float num = (float)bar.GetNumCards();
		num -= (float)(drainRun + 1) * num / (duration / rate);
		if (num < 0f)
		{
			num = 0f;
		}
		float num2 = (float)bar.GetAmountDust();
		num2 -= (float)(drainRun + 1) * num2 / (duration / rate);
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		bar.m_numCardsText.Text = Convert.ToInt32(num).ToString();
		bar.m_amountBar.GetComponent<Renderer>().material.SetFloat("_Percent", num / (float)this.m_totalCardsToDisenchant);
		bar.m_amountText.Text = Convert.ToInt32(num2).ToString();
		return num2;
	}

	// Token: 0x06003E48 RID: 15944 RVA: 0x0012D7D0 File Offset: 0x0012B9D0
	private Vector3 GetRanBoxPt(GameObject box)
	{
		Vector3 localScale = box.transform.localScale;
		Vector3 position = box.transform.position;
		Vector3 vector;
		vector..ctor(Random.Range(-localScale.x / 2f, localScale.x / 2f), Random.Range(-localScale.y / 2f, localScale.y / 2f), Random.Range(-localScale.z / 2f, localScale.z / 2f));
		return position + vector;
	}

	// Token: 0x06003E49 RID: 15945 RVA: 0x0012D864 File Offset: 0x0012BA64
	private IEnumerator LaunchGlowball(DisenchantBar bar, RarityFX rareFX, int glowBallNum, int totalGlowBalls, int m_highestGlowBalls)
	{
		float duration = 1f;
		float pad = 0.02f;
		float gNum = (float)glowBallNum;
		float timeRange = (duration - pad * (float)m_highestGlowBalls) / (float)totalGlowBalls;
		float delayStart = gNum * timeRange + gNum * pad;
		float delayEnd = (gNum + 1f) * timeRange + (gNum + 1f) * pad;
		GameObject glowBall = Object.Instantiate<GameObject>(this.m_FX.m_glowBall);
		this.m_cleanupObjects.Add(glowBall);
		glowBall.GetComponent<Renderer>().sharedMaterial = rareFX.glowBallMat;
		glowBall.GetComponent<TrailRenderer>().material = rareFX.glowTrailMat;
		glowBall.transform.position = bar.m_rarityGem.transform.position;
		glowBall.transform.position = new Vector3(glowBall.transform.position.x, glowBall.transform.position.y + 0.5f, glowBall.transform.position.z);
		List<Vector3> curvePoints = new List<Vector3>();
		curvePoints.Add(glowBall.transform.position);
		if ((double)Random.Range(0f, 1f) < 0.5)
		{
			curvePoints.Add(this.GetRanBoxPt(this.m_FX.m_gemBoxLeft1));
			curvePoints.Add(this.GetRanBoxPt(this.m_FX.m_gemBoxLeft2));
		}
		else
		{
			curvePoints.Add(this.GetRanBoxPt(this.m_FX.m_gemBoxRight1));
			curvePoints.Add(this.GetRanBoxPt(this.m_FX.m_gemBoxRight2));
		}
		GameObject dustJar;
		if (UniversalInputManager.UsePhoneUI)
		{
			dustJar = ArcaneDustAmount.Get().m_dustJar;
			curvePoints.Add(dustJar.transform.position);
		}
		else
		{
			dustJar = BnetBar.Get().m_currencyFrame.m_dustJar;
			Camera.main.ViewportToWorldPoint(BaseUI.Get().m_BnetCamera.WorldToViewportPoint(dustJar.transform.position)).y = 20f;
			curvePoints.Add(Camera.main.ViewportToWorldPoint(BaseUI.Get().m_BnetCamera.WorldToViewportPoint(dustJar.transform.position)));
		}
		yield return new WaitForSeconds(Random.Range(delayStart, delayEnd));
		RaritySound rareSound = this.GetRaritySound(bar);
		if (rareSound.m_missileSound != string.Empty)
		{
			SoundManager.Get().LoadAndPlay(rareSound.m_missileSound);
		}
		if (glowBallNum == 0)
		{
			GameObject burst = Object.Instantiate<GameObject>(rareFX.burstFX);
			this.m_cleanupObjects.Add(burst);
			burst.transform.position = bar.m_rarityGem.transform.position;
			burst.GetComponent<ParticleSystem>().Play();
			Object.Destroy(burst, 3f);
		}
		float glowBallTime = 0.4f;
		glowBall.SetActive(true);
		iTween.MoveTo(glowBall, iTween.Hash(new object[]
		{
			"path",
			curvePoints.ToArray(),
			"time",
			glowBallTime,
			"easetype",
			iTween.EaseType.linear
		}));
		Object.Destroy(glowBall, glowBallTime);
		yield return new WaitForSeconds(glowBallTime);
		if (rareSound.m_jarSound != string.Empty)
		{
			SoundManager.Get().LoadAndPlay(rareSound.m_jarSound);
		}
		GameObject dustFX;
		if (UniversalInputManager.UsePhoneUI)
		{
			dustFX = ArcaneDustAmount.Get().m_dustFX;
		}
		else
		{
			dustFX = BnetBar.Get().m_currencyFrame.m_dustFX;
		}
		if (Random.Range(0f, 1f) > 0.7f)
		{
			GameObject receiveFX = Object.Instantiate<GameObject>(dustFX);
			this.m_cleanupObjects.Add(receiveFX);
			receiveFX.transform.parent = dustFX.transform.parent;
			receiveFX.transform.localPosition = dustFX.transform.localPosition;
			receiveFX.transform.localScale = dustFX.transform.localScale;
			receiveFX.transform.localRotation = dustFX.transform.localRotation;
			receiveFX.SetActive(true);
			receiveFX.GetComponent<ParticleSystem>().Play();
			Object.Destroy(receiveFX, 4.9f);
		}
		GameObject receiveBurstFX = Object.Instantiate<GameObject>(rareFX.explodeFX);
		this.m_cleanupObjects.Add(receiveBurstFX);
		receiveBurstFX.transform.parent = rareFX.explodeFX.transform.parent;
		receiveBurstFX.transform.localPosition = rareFX.explodeFX.transform.localPosition;
		receiveBurstFX.transform.localScale = rareFX.explodeFX.transform.localScale;
		receiveBurstFX.transform.localRotation = rareFX.explodeFX.transform.localRotation;
		receiveBurstFX.SetActive(true);
		receiveBurstFX.GetComponent<ParticleSystem>().Play();
		Object.Destroy(receiveBurstFX, 3f);
		Vector3 dustScale = dustJar.transform.localScale + ((!UniversalInputManager.UsePhoneUI) ? new Vector3(50f, 50f, 50f) : new Vector3(6f, 6f, 6f));
		iTween.ScaleTo(dustJar, iTween.Hash(new object[]
		{
			"scale",
			dustScale,
			"time",
			0.15f
		}));
		iTween.ScaleTo(dustJar, iTween.Hash(new object[]
		{
			"scale",
			this.m_origDustScale,
			"delay",
			0.1,
			"time",
			1f
		}));
		yield return null;
		yield break;
	}

	// Token: 0x06003E4A RID: 15946 RVA: 0x0012D8CC File Offset: 0x0012BACC
	private RarityFX GetRarityFX(DisenchantBar bar)
	{
		RarityFX result = default(RarityFX);
		switch (bar.m_rarity)
		{
		case TAG_RARITY.RARE:
			result.burstFX = this.m_FX.m_burstFX_Rare;
			result.explodeFX = ((!UniversalInputManager.UsePhoneUI) ? BnetBar.Get().m_currencyFrame.m_explodeFX_Rare : ArcaneDustAmount.Get().m_explodeFX_Rare);
			result.glowBallMat = this.m_FX.m_glowBallMat_Rare;
			result.glowTrailMat = this.m_FX.m_glowTrailMat_Rare;
			break;
		case TAG_RARITY.EPIC:
			result.burstFX = this.m_FX.m_burstFX_Epic;
			result.explodeFX = ((!UniversalInputManager.UsePhoneUI) ? BnetBar.Get().m_currencyFrame.m_explodeFX_Epic : ArcaneDustAmount.Get().m_explodeFX_Epic);
			result.glowBallMat = this.m_FX.m_glowBallMat_Epic;
			result.glowTrailMat = this.m_FX.m_glowTrailMat_Epic;
			break;
		case TAG_RARITY.LEGENDARY:
			result.burstFX = this.m_FX.m_burstFX_Legendary;
			result.explodeFX = ((!UniversalInputManager.UsePhoneUI) ? BnetBar.Get().m_currencyFrame.m_explodeFX_Legendary : ArcaneDustAmount.Get().m_explodeFX_Legendary);
			result.glowBallMat = this.m_FX.m_glowBallMat_Legendary;
			result.glowTrailMat = this.m_FX.m_glowTrailMat_Legendary;
			break;
		default:
			result.burstFX = this.m_FX.m_burstFX_Common;
			result.explodeFX = ((!UniversalInputManager.UsePhoneUI) ? BnetBar.Get().m_currencyFrame.m_explodeFX_Legendary : ArcaneDustAmount.Get().m_explodeFX_Legendary);
			result.glowBallMat = this.m_FX.m_glowBallMat_Common;
			result.glowTrailMat = this.m_FX.m_glowTrailMat_Common;
			break;
		}
		return result;
	}

	// Token: 0x06003E4B RID: 15947 RVA: 0x0012DAC0 File Offset: 0x0012BCC0
	private RaritySound GetRaritySound(DisenchantBar bar)
	{
		RaritySound raritySound = new RaritySound();
		switch (bar.m_rarity)
		{
		case TAG_RARITY.RARE:
			raritySound.m_drainSound = this.m_sound.m_rare.m_drainSound;
			raritySound.m_jarSound = this.m_sound.m_rare.m_jarSound;
			raritySound.m_missileSound = this.m_sound.m_rare.m_missileSound;
			break;
		case TAG_RARITY.EPIC:
			raritySound.m_drainSound = this.m_sound.m_epic.m_drainSound;
			raritySound.m_jarSound = this.m_sound.m_epic.m_jarSound;
			raritySound.m_missileSound = this.m_sound.m_epic.m_missileSound;
			break;
		case TAG_RARITY.LEGENDARY:
			raritySound.m_drainSound = this.m_sound.m_legendary.m_drainSound;
			raritySound.m_jarSound = this.m_sound.m_legendary.m_jarSound;
			raritySound.m_missileSound = this.m_sound.m_legendary.m_missileSound;
			break;
		default:
			raritySound.m_drainSound = this.m_sound.m_common.m_drainSound;
			raritySound.m_jarSound = this.m_sound.m_common.m_jarSound;
			raritySound.m_missileSound = this.m_sound.m_common.m_missileSound;
			break;
		}
		return raritySound;
	}

	// Token: 0x040027C2 RID: 10178
	public GameObject m_root;

	// Token: 0x040027C3 RID: 10179
	public GameObject m_disenchantContainer;

	// Token: 0x040027C4 RID: 10180
	public MassDisenchantFX m_FX;

	// Token: 0x040027C5 RID: 10181
	public MassDisenchantSound m_sound;

	// Token: 0x040027C6 RID: 10182
	public UberText m_headlineText;

	// Token: 0x040027C7 RID: 10183
	public UberText m_detailsHeadlineText;

	// Token: 0x040027C8 RID: 10184
	public UberText m_detailsText;

	// Token: 0x040027C9 RID: 10185
	public UberText m_totalAmountText;

	// Token: 0x040027CA RID: 10186
	public NormalButton m_disenchantButton;

	// Token: 0x040027CB RID: 10187
	public UberText m_singleSubHeadlineText;

	// Token: 0x040027CC RID: 10188
	public UberText m_doubleSubHeadlineText;

	// Token: 0x040027CD RID: 10189
	public GameObject m_singleRoot;

	// Token: 0x040027CE RID: 10190
	public GameObject m_doubleRoot;

	// Token: 0x040027CF RID: 10191
	public List<DisenchantBar> m_singleDisenchantBars;

	// Token: 0x040027D0 RID: 10192
	public List<DisenchantBar> m_doubleDisenchantBars;

	// Token: 0x040027D1 RID: 10193
	public UIBButton m_infoButton;

	// Token: 0x040027D2 RID: 10194
	public Material m_rarityBarNormalMaterial;

	// Token: 0x040027D3 RID: 10195
	public Material m_rarityBarGoldMaterial;

	// Token: 0x040027D4 RID: 10196
	public Mesh m_rarityBarNormalMesh;

	// Token: 0x040027D5 RID: 10197
	public Mesh m_rarityBarGoldMesh;

	// Token: 0x040027D6 RID: 10198
	private bool m_useSingle = true;

	// Token: 0x040027D7 RID: 10199
	private int m_totalAmount;

	// Token: 0x040027D8 RID: 10200
	private int m_totalCardsToDisenchant;

	// Token: 0x040027D9 RID: 10201
	private Vector3 m_origTotalScale;

	// Token: 0x040027DA RID: 10202
	private Vector3 m_origDustScale;

	// Token: 0x040027DB RID: 10203
	private int m_highestGlowBalls;

	// Token: 0x040027DC RID: 10204
	private List<GameObject> m_cleanupObjects = new List<GameObject>();

	// Token: 0x040027DD RID: 10205
	private static MassDisenchant s_Instance;
}
