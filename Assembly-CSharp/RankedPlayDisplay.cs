using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200038B RID: 907
public class RankedPlayDisplay : MonoBehaviour
{
	// Token: 0x06002F74 RID: 12148 RVA: 0x000EEC5C File Offset: 0x000ECE5C
	private void Awake()
	{
		this.m_medal = Object.Instantiate<TournamentMedal>(this.m_medalPrefab);
		this.SetRankedMedalTransform(this.m_medalBone);
		this.m_medal.GetComponent<Collider>().enabled = false;
		this.m_usingWildVisuals = Options.Get().GetBool(Option.IN_WILD_MODE);
		this.m_casualButton.SetFormat(this.m_usingWildVisuals);
		this.m_rankedButton.SetFormat(this.m_usingWildVisuals);
		this.m_medal.SetFormat(this.m_usingWildVisuals);
		this.m_casualButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnCasualButtonOver));
		this.m_rankedButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRankedButtonOver));
		this.m_casualButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnCasualButtonOut));
		this.m_rankedButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRankedButtonOut));
	}

	// Token: 0x06002F75 RID: 12149 RVA: 0x000EED44 File Offset: 0x000ECF44
	public void UpdateMode()
	{
		bool flag = Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE);
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject != null && netObject.Games != null && !netObject.Games.Casual)
		{
			this.m_casualButton.SetEnabled(false);
			if (!flag)
			{
				flag = true;
				Options.Get().SetBool(Option.IN_RANKED_PLAY_MODE, true);
			}
		}
		else
		{
			this.m_casualButton.SetEnabled(true);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			DeckPickerTrayDisplay.Get().ToggleRankedDetailsTray(flag);
		}
		else
		{
			DeckPickerTrayDisplay.Get().UpdateRankedClassWinsPlate();
		}
		if (flag)
		{
			this.m_casualButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCasualButtonRelease));
			this.m_rankedButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRankedButtonRelease));
			this.m_casualButton.Up();
			this.m_rankedButton.Down();
		}
		else
		{
			this.m_casualButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCasualButtonRelease));
			this.m_rankedButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRankedButtonRelease));
			this.m_casualButton.Down();
			this.m_rankedButton.Up();
		}
	}

	// Token: 0x06002F76 RID: 12150 RVA: 0x000EEE84 File Offset: 0x000ED084
	public void StartSetRotationTutorial()
	{
		this.m_inSetRotationTutorial = true;
		bool @bool = Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE);
		if (UniversalInputManager.UsePhoneUI)
		{
			DeckPickerTrayDisplay.Get().ToggleRankedDetailsTray(@bool);
		}
		this.m_usingWildVisuals = false;
		this.m_casualButton.SetFormat(false);
		this.m_rankedButton.SetFormat(false);
		this.m_casualButton.Up();
		this.m_rankedButton.Up();
		this.m_casualButton.SetEnabled(false);
		this.m_rankedButton.SetEnabled(false);
		DeckPickerTrayDisplay.Get().SetPlayButtonText((!@bool) ? GameStrings.Get("GLOBAL_PLAY") : GameStrings.Get("GLOBAL_PLAY_RANKED"));
		DeckPickerTrayDisplay.Get().m_playButton.m_newPlayButtonText.TextAlpha = 0f;
		DeckPickerTrayDisplay.Get().UpdateRankedClassWinsPlate();
		this.m_medal.SetFormat(false);
	}

	// Token: 0x06002F77 RID: 12151 RVA: 0x000EEF65 File Offset: 0x000ED165
	public void EndSetRotationTutorial()
	{
		this.m_inSetRotationTutorial = false;
		this.m_casualButton.SetEnabled(true);
		this.m_rankedButton.SetEnabled(true);
		this.UpdateMode();
	}

	// Token: 0x06002F78 RID: 12152 RVA: 0x000EEF8C File Offset: 0x000ED18C
	public void SetRankedMedalTransform(Transform bone)
	{
		this.m_medal.transform.parent = bone;
		this.m_medal.transform.localScale = Vector3.one;
		this.m_medal.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06002F79 RID: 12153 RVA: 0x000EEFD4 File Offset: 0x000ED1D4
	public void SetRankedMedal(NetCache.NetCacheMedalInfo medal)
	{
		this.m_medal.SetMedal(medal);
	}

	// Token: 0x06002F7A RID: 12154 RVA: 0x000EEFE4 File Offset: 0x000ED1E4
	public void OnSwitchFormat()
	{
		if (!this.m_inSetRotationTutorial)
		{
			bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
			if (this.m_usingWildVisuals != @bool)
			{
				this.m_usingWildVisuals = @bool;
				base.StopCoroutine("WaitThenSetRankedButtonsFormat");
				if (this.m_formatSwitchGlowBurst != null)
				{
					string text = (!this.m_usingWildVisuals) ? "Glow" : "GlowNoFX";
					this.m_formatSwitchGlowBurst.SendEvent(text);
					base.StartCoroutine("WaitThenSetRankedButtonsFormat", this.m_usingWildVisuals);
				}
				else
				{
					this.m_casualButton.SetFormat(this.m_usingWildVisuals);
					this.m_rankedButton.SetFormat(this.m_usingWildVisuals);
					this.m_medal.SetFormat(this.m_usingWildVisuals);
				}
			}
			this.UpdateMode();
		}
	}

	// Token: 0x06002F7B RID: 12155 RVA: 0x000EF0B7 File Offset: 0x000ED2B7
	private void OnCasualButtonRelease(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("tournament_screen_select_hero");
		Options.Get().SetBool(Option.IN_RANKED_PLAY_MODE, false);
		this.UpdateMode();
	}

	// Token: 0x06002F7C RID: 12156 RVA: 0x000EF0DB File Offset: 0x000ED2DB
	private void OnRankedButtonRelease(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("tournament_screen_select_hero");
		Options.Get().SetBool(Option.IN_RANKED_PLAY_MODE, true);
		this.UpdateMode();
	}

	// Token: 0x06002F7D RID: 12157 RVA: 0x000EF100 File Offset: 0x000ED300
	private void OnCasualButtonOver(UIEvent e)
	{
		string key = (!Options.Get().GetBool(Option.IN_WILD_MODE)) ? "GLUE_TOURNAMENT_CASUAL" : "GLUE_TOURNAMENT_CASUAL_WILD";
		this.m_casualButton.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get(key), GameStrings.Get("GLUE_TOOLTIP_CASUAL_BUTTON"), 5f, true);
	}

	// Token: 0x06002F7E RID: 12158 RVA: 0x000EF158 File Offset: 0x000ED358
	private void OnRankedButtonOver(UIEvent e)
	{
		string key = (!Options.Get().GetBool(Option.IN_WILD_MODE)) ? "GLUE_TOURNAMENT_RANKED" : "GLUE_TOURNAMENT_RANKED_WILD";
		this.m_rankedButton.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get(key), GameStrings.Get("GLUE_TOOLTIP_RANKED_BUTTON"), 5f, true);
	}

	// Token: 0x06002F7F RID: 12159 RVA: 0x000EF1B0 File Offset: 0x000ED3B0
	private void OnCasualButtonOut(UIEvent e)
	{
		this.m_casualButton.GetComponent<TooltipZone>().HideTooltip();
	}

	// Token: 0x06002F80 RID: 12160 RVA: 0x000EF1C2 File Offset: 0x000ED3C2
	private void OnRankedButtonOut(UIEvent e)
	{
		this.m_rankedButton.GetComponent<TooltipZone>().HideTooltip();
	}

	// Token: 0x06002F81 RID: 12161 RVA: 0x000EF1D4 File Offset: 0x000ED3D4
	private IEnumerator WaitThenSetRankedButtonsFormat(bool isWild)
	{
		yield return new WaitForSeconds(this.m_medalSwitchFormatDelay);
		this.m_casualButton.SetFormat(isWild);
		this.m_rankedButton.SetFormat(isWild);
		this.m_medal.SetFormat(isWild);
		yield break;
	}

	// Token: 0x04001D72 RID: 7538
	public RankedPlayToggleButton m_casualButton;

	// Token: 0x04001D73 RID: 7539
	public RankedPlayToggleButton m_rankedButton;

	// Token: 0x04001D74 RID: 7540
	public Transform m_medalBone;

	// Token: 0x04001D75 RID: 7541
	public TournamentMedal m_medalPrefab;

	// Token: 0x04001D76 RID: 7542
	public PlayMakerFSM m_formatSwitchGlowBurst;

	// Token: 0x04001D77 RID: 7543
	public float m_medalSwitchFormatDelay;

	// Token: 0x04001D78 RID: 7544
	private TournamentMedal m_medal;

	// Token: 0x04001D79 RID: 7545
	private bool m_inSetRotationTutorial;

	// Token: 0x04001D7A RID: 7546
	private bool m_usingWildVisuals;
}
