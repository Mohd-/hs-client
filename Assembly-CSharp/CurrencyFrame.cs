using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004DF RID: 1247
public class CurrencyFrame : MonoBehaviour
{
	// Token: 0x06003AA7 RID: 15015 RVA: 0x0011B4B4 File Offset: 0x001196B4
	private void Awake()
	{
		NetCache.Get().RegisterGoldBalanceListener(new NetCache.DelGoldBalanceListener(this.OnGoldBalanceChanged));
		this.m_mouseOverZone.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnFrameMouseOver));
		this.m_mouseOverZone.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnFrameMouseOut));
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
	}

	// Token: 0x06003AA8 RID: 15016 RVA: 0x0011B520 File Offset: 0x00119720
	private void OnDestroy()
	{
		if (NetCache.Get() != null)
		{
			NetCache.Get().RemoveGoldBalanceListener(new NetCache.DelGoldBalanceListener(this.OnGoldBalanceChanged));
		}
		if (SceneMgr.Get() != null)
		{
			SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		}
	}

	// Token: 0x06003AA9 RID: 15017 RVA: 0x0011B574 File Offset: 0x00119774
	public void DeactivateCurrencyFrame()
	{
		base.gameObject.SetActive(false);
		this.m_state = CurrencyFrame.State.HIDDEN;
	}

	// Token: 0x06003AAA RID: 15018 RVA: 0x0011B58C File Offset: 0x0011978C
	public void RefreshContents()
	{
		CurrencyFrame.CurrencyType currencyToShow = this.GetCurrencyToShow();
		this.UpdateAmount(currencyToShow);
		this.Show(currencyToShow);
	}

	// Token: 0x06003AAB RID: 15019 RVA: 0x0011B5B0 File Offset: 0x001197B0
	public void HideTemporarily()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeOutCubic
		});
		iTween.FadeTo(base.gameObject, args);
		this.m_showingCurrency = CurrencyFrame.CurrencyType.NONE;
	}

	// Token: 0x06003AAC RID: 15020 RVA: 0x0011B618 File Offset: 0x00119818
	public GameObject GetTooltipObject()
	{
		TooltipZone component = base.GetComponent<TooltipZone>();
		if (component != null)
		{
			return component.GetTooltipObject();
		}
		return null;
	}

	// Token: 0x06003AAD RID: 15021 RVA: 0x0011B640 File Offset: 0x00119840
	public void SetCurrencyOverride(CurrencyFrame.CurrencyType? type)
	{
		this.m_overrideCurrencyType = type;
		this.RefreshContents();
		BnetBar.Get().UpdateLayout();
	}

	// Token: 0x06003AAE RID: 15022 RVA: 0x0011B65C File Offset: 0x0011985C
	private void ShowImmediate(CurrencyFrame.CurrencyType currencyType)
	{
		bool flag = currencyType != CurrencyFrame.CurrencyType.NONE;
		this.m_showingCurrency = currencyType;
		base.gameObject.SetActive(flag);
		iTween.Stop(base.gameObject, true);
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		float num = (!flag) ? 0f : 1f;
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.material.color = new Color(1f, 1f, 1f, num);
		}
		float num2 = (currencyType != CurrencyFrame.CurrencyType.GOLD) ? 0f : 1f;
		float num3 = (currencyType != CurrencyFrame.CurrencyType.ARCANE_DUST) ? 0f : 1f;
		this.m_goldCoin.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, num2);
		this.m_dustJar.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, num3);
		this.m_state = ((!flag) ? CurrencyFrame.State.HIDDEN : CurrencyFrame.State.SHOWN);
	}

	// Token: 0x06003AAF RID: 15023 RVA: 0x0011B790 File Offset: 0x00119990
	private void Show(CurrencyFrame.CurrencyType currencyType)
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.ShowImmediate(currencyType);
			return;
		}
		bool flag = currencyType != CurrencyFrame.CurrencyType.NONE;
		if (!DemoMgr.Get().IsCurrencyEnabled())
		{
			flag = false;
		}
		if (flag)
		{
			if (this.m_state == CurrencyFrame.State.SHOWN || this.m_state == CurrencyFrame.State.ANIMATE_IN)
			{
				this.ShowCurrencyType(currencyType);
				return;
			}
			this.m_state = CurrencyFrame.State.ANIMATE_IN;
			base.gameObject.SetActive(true);
			Hashtable args = iTween.Hash(new object[]
			{
				"amount",
				1f,
				"delay",
				0f,
				"time",
				0.25f,
				"easeType",
				iTween.EaseType.easeOutCubic,
				"oncomplete",
				"ActivateCurrencyFrame",
				"oncompletetarget",
				base.gameObject
			});
			iTween.Stop(base.gameObject);
			iTween.FadeTo(base.gameObject, args);
			this.ShowCurrencyType(currencyType);
		}
		else
		{
			if (this.m_state == CurrencyFrame.State.HIDDEN || this.m_state == CurrencyFrame.State.ANIMATE_OUT)
			{
				this.ShowCurrencyType(currencyType);
				return;
			}
			this.m_state = CurrencyFrame.State.ANIMATE_OUT;
			Hashtable args2 = iTween.Hash(new object[]
			{
				"amount",
				0f,
				"delay",
				0f,
				"time",
				0.25f,
				"easeType",
				iTween.EaseType.easeOutCubic,
				"oncomplete",
				"DeactivateCurrencyFrame",
				"oncompletetarget",
				base.gameObject
			});
			iTween.Stop(base.gameObject);
			iTween.FadeTo(base.gameObject, args2);
			this.ShowCurrencyType(currencyType);
		}
	}

	// Token: 0x06003AB0 RID: 15024 RVA: 0x0011B970 File Offset: 0x00119B70
	private CurrencyFrame.CurrencyType GetCurrencyToShow()
	{
		if (this.m_overrideCurrencyType != null)
		{
			return this.m_overrideCurrencyType.Value;
		}
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		this.m_backgroundFaded = true;
		CurrencyFrame.CurrencyType currencyType;
		switch (mode)
		{
		case SceneMgr.Mode.HUB:
			currencyType = CurrencyFrame.CurrencyType.GOLD;
			this.m_backgroundFaded = false;
			goto IL_FF;
		case SceneMgr.Mode.COLLECTIONMANAGER:
			currencyType = ((!UniversalInputManager.UsePhoneUI) ? CurrencyFrame.CurrencyType.ARCANE_DUST : CurrencyFrame.CurrencyType.NONE);
			goto IL_FF;
		case SceneMgr.Mode.PACKOPENING:
		case SceneMgr.Mode.TOURNAMENT:
		case SceneMgr.Mode.FRIENDLY:
		case SceneMgr.Mode.DRAFT:
		case SceneMgr.Mode.ADVENTURE:
			currencyType = ((!UniversalInputManager.UsePhoneUI) ? CurrencyFrame.CurrencyType.GOLD : CurrencyFrame.CurrencyType.NONE);
			goto IL_FF;
		case SceneMgr.Mode.TAVERN_BRAWL:
			if (UniversalInputManager.UsePhoneUI)
			{
				currencyType = CurrencyFrame.CurrencyType.NONE;
			}
			else if (TavernBrawlDisplay.Get() != null && TavernBrawlDisplay.Get().IsInDeckEditMode())
			{
				currencyType = CurrencyFrame.CurrencyType.ARCANE_DUST;
			}
			else
			{
				currencyType = CurrencyFrame.CurrencyType.GOLD;
			}
			goto IL_FF;
		}
		currencyType = CurrencyFrame.CurrencyType.NONE;
		IL_FF:
		if (UniversalInputManager.UsePhoneUI && currencyType == CurrencyFrame.CurrencyType.ARCANE_DUST)
		{
			currencyType = CurrencyFrame.CurrencyType.NONE;
		}
		return currencyType;
	}

	// Token: 0x06003AB1 RID: 15025 RVA: 0x0011BA95 File Offset: 0x00119C95
	private void SetAmount(long amount)
	{
		this.m_amount.Text = amount.ToString();
	}

	// Token: 0x06003AB2 RID: 15026 RVA: 0x0011BAAC File Offset: 0x00119CAC
	private void ShowCurrencyType(CurrencyFrame.CurrencyType currencyType)
	{
		this.FadeBackground(this.m_backgroundFaded);
		if (this.m_showingCurrency == currencyType)
		{
			return;
		}
		this.m_showingCurrency = currencyType;
		iTween.FadeTo(this.m_amount.gameObject, 1f, 0.25f);
		switch (this.m_showingCurrency)
		{
		case CurrencyFrame.CurrencyType.GOLD:
			iTween.FadeTo(this.m_dustJar, 0f, 0.25f);
			iTween.FadeTo(this.m_goldCoin, 1f, 0.25f);
			return;
		case CurrencyFrame.CurrencyType.ARCANE_DUST:
			iTween.FadeTo(this.m_dustJar, 1f, 0.25f);
			iTween.FadeTo(this.m_goldCoin, 0f, 0.25f);
			return;
		}
		iTween.FadeTo(this.m_dustJar, 0f, 0.25f);
		iTween.FadeTo(this.m_goldCoin, 0f, 0.25f);
	}

	// Token: 0x06003AB3 RID: 15027 RVA: 0x0011BBA0 File Offset: 0x00119DA0
	private void FadeBackground(bool isFaded)
	{
		Hashtable args;
		if (isFaded)
		{
			args = iTween.Hash(new object[]
			{
				"amount",
				0.5f,
				"time",
				0.25f,
				"easeType",
				iTween.EaseType.easeOutCubic
			});
		}
		else
		{
			args = iTween.Hash(new object[]
			{
				"amount",
				1f,
				"time",
				0.25f,
				"easeType",
				iTween.EaseType.easeOutCubic
			});
		}
		iTween.FadeTo(this.m_background, args);
	}

	// Token: 0x06003AB4 RID: 15028 RVA: 0x0011BC52 File Offset: 0x00119E52
	private void ActivateCurrencyFrame()
	{
		this.m_state = CurrencyFrame.State.SHOWN;
	}

	// Token: 0x06003AB5 RID: 15029 RVA: 0x0011BC5C File Offset: 0x00119E5C
	private void OnFrameMouseOver(UIEvent e)
	{
		string text = string.Empty;
		string key = string.Empty;
		CurrencyFrame.CurrencyType showingCurrency = this.m_showingCurrency;
		if (showingCurrency != CurrencyFrame.CurrencyType.GOLD)
		{
			if (showingCurrency == CurrencyFrame.CurrencyType.ARCANE_DUST)
			{
				text = "GLUE_CRAFTING_ARCANEDUST";
				key = "GLUE_CRAFTING_ARCANEDUST_DESCRIPTION";
			}
		}
		else
		{
			text = "GLUE_TOOLTIP_GOLD_HEADER";
			key = "GLUE_TOOLTIP_GOLD_DESCRIPTION";
		}
		if (text == string.Empty)
		{
			return;
		}
		KeywordHelpPanel keywordHelpPanel = base.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get(text), GameStrings.Get(key), 0.7f, true);
		SceneUtils.SetLayer(keywordHelpPanel.gameObject, GameLayer.BattleNet);
		keywordHelpPanel.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
		keywordHelpPanel.transform.localScale = new Vector3(70f, 70f, 70f);
		if (UniversalInputManager.UsePhoneUI)
		{
			TransformUtil.SetPoint(keywordHelpPanel, Anchor.TOP, this.m_mouseOverZone, Anchor.BOTTOM, Vector3.zero);
		}
		else
		{
			TransformUtil.SetPoint(keywordHelpPanel, Anchor.BOTTOM, this.m_mouseOverZone, Anchor.TOP, Vector3.zero);
		}
	}

	// Token: 0x06003AB6 RID: 15030 RVA: 0x0011BD65 File Offset: 0x00119F65
	private void OnFrameMouseOut(UIEvent e)
	{
		base.GetComponent<TooltipZone>().HideTooltip();
	}

	// Token: 0x06003AB7 RID: 15031 RVA: 0x0011BD72 File Offset: 0x00119F72
	private void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance)
	{
		if (this.m_showingCurrency != CurrencyFrame.CurrencyType.GOLD)
		{
			return;
		}
		this.SetAmount(balance.GetTotal());
	}

	// Token: 0x06003AB8 RID: 15032 RVA: 0x0011BD90 File Offset: 0x00119F90
	private void UpdateAmount(CurrencyFrame.CurrencyType currencyType)
	{
		switch (currencyType)
		{
		case CurrencyFrame.CurrencyType.GOLD:
		{
			NetCache.NetCacheGoldBalance netObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
			long amount = netObject.GetTotal();
			this.SetAmount(amount);
			break;
		}
		case CurrencyFrame.CurrencyType.ARCANE_DUST:
		{
			long amount;
			if (CraftingManager.Get() != null)
			{
				amount = CraftingManager.Get().GetLocalArcaneDustBalance();
			}
			else
			{
				amount = NetCache.Get().GetNetObject<NetCache.NetCacheArcaneDustBalance>().Balance;
			}
			this.SetAmount(amount);
			break;
		}
		}
	}

	// Token: 0x06003AB9 RID: 15033 RVA: 0x0011BE18 File Offset: 0x0011A018
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		this.m_overrideCurrencyType = default(CurrencyFrame.CurrencyType?);
		this.m_amount.UpdateNow();
	}

	// Token: 0x0400255E RID: 9566
	public UberText m_amount;

	// Token: 0x0400255F RID: 9567
	public GameObject m_dustJar;

	// Token: 0x04002560 RID: 9568
	public GameObject m_dustFX;

	// Token: 0x04002561 RID: 9569
	public GameObject m_explodeFX_Common;

	// Token: 0x04002562 RID: 9570
	public GameObject m_explodeFX_Rare;

	// Token: 0x04002563 RID: 9571
	public GameObject m_explodeFX_Epic;

	// Token: 0x04002564 RID: 9572
	public GameObject m_explodeFX_Legendary;

	// Token: 0x04002565 RID: 9573
	public GameObject m_goldCoin;

	// Token: 0x04002566 RID: 9574
	public GameObject m_background;

	// Token: 0x04002567 RID: 9575
	public PegUIElement m_mouseOverZone;

	// Token: 0x04002568 RID: 9576
	private CurrencyFrame.State m_state = CurrencyFrame.State.SHOWN;

	// Token: 0x04002569 RID: 9577
	private CurrencyFrame.CurrencyType m_showingCurrency;

	// Token: 0x0400256A RID: 9578
	private bool m_backgroundFaded;

	// Token: 0x0400256B RID: 9579
	private CurrencyFrame.CurrencyType? m_overrideCurrencyType;

	// Token: 0x020004E5 RID: 1253
	public enum CurrencyType
	{
		// Token: 0x0400257C RID: 9596
		NONE,
		// Token: 0x0400257D RID: 9597
		GOLD,
		// Token: 0x0400257E RID: 9598
		ARCANE_DUST
	}

	// Token: 0x020004E8 RID: 1256
	public enum State
	{
		// Token: 0x04002591 RID: 9617
		ANIMATE_IN,
		// Token: 0x04002592 RID: 9618
		ANIMATE_OUT,
		// Token: 0x04002593 RID: 9619
		HIDDEN,
		// Token: 0x04002594 RID: 9620
		SHOWN
	}
}
