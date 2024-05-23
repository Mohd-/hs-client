using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020004A2 RID: 1186
[CustomEditClass]
public class AdventureWingProgressDisplay_LOE : AdventureWingProgressDisplay
{
	// Token: 0x06003877 RID: 14455 RVA: 0x00114120 File Offset: 0x00112320
	private void Awake()
	{
		AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_emptyStaffObjects, true);
		AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_rodObjects, false);
		AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_headObjects, false);
		AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_pearlObjects, false);
		AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_visibleStaffObjects, false);
		if (this.m_hangingSignHitArea != null)
		{
			this.m_hangingSignHitArea.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OnHangingSignClick();
			});
		}
		if (this.m_completeStaffHitArea != null)
		{
			this.m_completeStaffHitArea.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OnCompleteStaffClick();
			});
		}
	}

	// Token: 0x06003878 RID: 14456 RVA: 0x001141C0 File Offset: 0x001123C0
	private void Update()
	{
		if (!AdventureScene.Get().IsDevMode)
		{
			return;
		}
		if (Input.GetKeyDown(99))
		{
			base.StartCoroutine(this.PlayCompleteAnimationCoroutine(base.GetComponent<PlayMakerFSM>(), "OnWingDisappear", null, Option.INVALID));
		}
		else if (Input.GetKeyDown(118))
		{
			base.StartCoroutine(this.PlayCompleteAnimationCoroutine(base.GetComponent<PlayMakerFSM>(), "OnWingReappear", null, Option.INVALID));
		}
	}

	// Token: 0x06003879 RID: 14457 RVA: 0x00114230 File Offset: 0x00112430
	public override void UpdateProgress(WingDbId wingDbId, bool normalComplete)
	{
		switch (wingDbId)
		{
		case WingDbId.LOE_TEMPLE_OF_ORSIS:
			this.m_rodComplete = normalComplete;
			break;
		case WingDbId.LOE_ULDAMAN:
			this.m_headComplete = normalComplete;
			break;
		case WingDbId.LOE_RUINED_CITY:
			this.m_pearlComplete = normalComplete;
			break;
		case WingDbId.LOE_HALL_OF_EXPLORERS:
			this.m_finalWingComplete = normalComplete;
			break;
		}
		this.UpdatePartVisibility();
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x00114294 File Offset: 0x00112494
	public override bool HasProgressAnimationToPlay()
	{
		if (!this.m_rodComplete || !this.m_headComplete || !this.m_pearlComplete)
		{
			return false;
		}
		if (this.m_finalWingComplete)
		{
			return !Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_REAPPEAR, false);
		}
		return !Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_DISAPPEAR, false);
	}

	// Token: 0x0600387B RID: 14459 RVA: 0x001142F8 File Offset: 0x001124F8
	public override void PlayProgressAnimation(AdventureWingProgressDisplay.OnAnimationComplete onAnimComplete = null)
	{
		if (!this.m_rodComplete || !this.m_headComplete || !this.m_pearlComplete)
		{
			if (onAnimComplete != null)
			{
				onAnimComplete();
			}
			return;
		}
		PlayMakerFSM component = base.GetComponent<PlayMakerFSM>();
		if (component == null)
		{
			if (onAnimComplete != null)
			{
				onAnimComplete();
			}
			return;
		}
		if (!this.m_finalWingComplete)
		{
			bool @bool = Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_DISAPPEAR, false);
			if (@bool)
			{
				if (onAnimComplete != null)
				{
					onAnimComplete();
				}
				return;
			}
			base.StartCoroutine(this.PlayCompleteAnimationCoroutine(component, "OnWingDisappear", onAnimComplete, Option.HAS_SEEN_LOE_STAFF_DISAPPEAR));
		}
		else
		{
			bool bool2 = Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_REAPPEAR, false);
			if (bool2)
			{
				if (onAnimComplete != null)
				{
					onAnimComplete();
				}
				return;
			}
			base.StartCoroutine(this.PlayCompleteAnimationCoroutine(component, "OnWingReappear", onAnimComplete, Option.HAS_SEEN_LOE_STAFF_REAPPEAR));
		}
	}

	// Token: 0x0600387C RID: 14460 RVA: 0x001143E0 File Offset: 0x001125E0
	private void UpdatePartVisibility()
	{
		bool @bool = Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_DISAPPEAR, false);
		if (this.m_finalWingComplete)
		{
			bool bool2 = Options.Get().GetBool(Option.HAS_SEEN_LOE_STAFF_REAPPEAR, false);
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_emptyStaffObjects, false);
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_rodObjects, this.m_rodComplete && bool2);
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_headObjects, this.m_headComplete && bool2);
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_pearlObjects, this.m_pearlComplete && bool2);
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_visibleStaffObjects, true);
		}
		else
		{
			bool flag = this.m_rodComplete && !@bool;
			bool flag2 = this.m_headComplete && !@bool;
			bool flag3 = this.m_pearlComplete && !@bool;
			bool flag4 = flag || flag2 || flag3;
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_emptyStaffObjects, !flag4);
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_rodObjects, flag);
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_headObjects, flag2);
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_pearlObjects, flag3);
			AdventureWingProgressDisplay_LOE.SetObjectsVisibility(this.m_visibleStaffObjects, flag4);
		}
		if (this.m_hangingSignText != null)
		{
			this.m_hangingSignText.Text = ((!@bool) ? GameStrings.Get("GLUE_ADVENTURE_LOE_STAFF_RESERVED") : GameStrings.Get("GLUE_ADVENTURE_LOE_STAFF_DISAPPEARED"));
		}
		if (this.m_completeStaffHitArea != null)
		{
			this.m_completeStaffHitArea.gameObject.SetActive(this.m_finalWingComplete && this.m_rodComplete && this.m_headComplete && this.m_pearlComplete);
		}
		if (this.m_hangingSignHitArea != null)
		{
			this.m_hangingSignHitArea.SetEnabled(!this.m_finalWingComplete && !this.m_rodComplete && !this.m_headComplete && !this.m_pearlComplete);
		}
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x001145E4 File Offset: 0x001127E4
	private static void SetObjectsVisibility(List<GameObject> objs, bool show)
	{
		foreach (GameObject gameObject in objs)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(show);
			}
		}
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x00114648 File Offset: 0x00112848
	private IEnumerator PlayCompleteAnimationCoroutine(PlayMakerFSM fsm, string eventName, AdventureWingProgressDisplay.OnAnimationComplete onAnimComplete, Option seenOption)
	{
		FsmBool animComplete = fsm.FsmVariables.FindFsmBool("AnimationComplete");
		fsm.SendEvent(eventName);
		this.m_animating = true;
		if (animComplete != null)
		{
			while (!animComplete.Value)
			{
				yield return null;
			}
		}
		this.m_animating = false;
		if (seenOption != Option.INVALID)
		{
			Options.Get().SetBool(seenOption, true);
		}
		if (onAnimComplete != null)
		{
			onAnimComplete();
		}
		yield break;
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x001146A0 File Offset: 0x001128A0
	private void OnHangingSignClick()
	{
		if (this.m_animating)
		{
			return;
		}
		if (this.m_rodComplete || this.m_headComplete || this.m_pearlComplete)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.m_hangingSignQuotePrefab) || string.IsNullOrEmpty(this.m_hangingSignQuoteVOLine))
		{
			return;
		}
		NotificationManager.Get().CreateCharacterQuote(FileUtils.GameAssetPathToName(this.m_hangingSignQuotePrefab), GameStrings.Get(this.m_hangingSignQuoteVOLine), this.m_hangingSignQuoteVOLine, true, 0f, CanvasAnchor.BOTTOM_LEFT);
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x0011472C File Offset: 0x0011292C
	private void OnCompleteStaffClick()
	{
		if (this.m_animating)
		{
			return;
		}
		if (!this.m_rodComplete || !this.m_headComplete || !this.m_pearlComplete || !this.m_finalWingComplete)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.m_completeStaffQuotePrefab) || string.IsNullOrEmpty(this.m_completeStaffQuoteVOLine))
		{
			return;
		}
		NotificationManager.Get().CreateCharacterQuote(FileUtils.GameAssetPathToName(this.m_completeStaffQuotePrefab), GameStrings.Get(this.m_completeStaffQuoteVOLine), this.m_completeStaffQuoteVOLine, true, 0f, CanvasAnchor.BOTTOM_LEFT);
	}

	// Token: 0x0400243E RID: 9278
	private const string s_WingDisappearAnimateEventName = "OnWingDisappear";

	// Token: 0x0400243F RID: 9279
	private const string s_WingReappearAnimateEventName = "OnWingReappear";

	// Token: 0x04002440 RID: 9280
	private const string s_CompleteAnimationVarName = "AnimationComplete";

	// Token: 0x04002441 RID: 9281
	public UberText m_hangingSignText;

	// Token: 0x04002442 RID: 9282
	public PegUIElement m_hangingSignHitArea;

	// Token: 0x04002443 RID: 9283
	public PegUIElement m_completeStaffHitArea;

	// Token: 0x04002444 RID: 9284
	public List<GameObject> m_emptyStaffObjects = new List<GameObject>();

	// Token: 0x04002445 RID: 9285
	public List<GameObject> m_visibleStaffObjects = new List<GameObject>();

	// Token: 0x04002446 RID: 9286
	public List<GameObject> m_rodObjects = new List<GameObject>();

	// Token: 0x04002447 RID: 9287
	public List<GameObject> m_headObjects = new List<GameObject>();

	// Token: 0x04002448 RID: 9288
	public List<GameObject> m_pearlObjects = new List<GameObject>();

	// Token: 0x04002449 RID: 9289
	[CustomEditField(Sections = "VO")]
	public string m_hangingSignQuotePrefab;

	// Token: 0x0400244A RID: 9290
	[CustomEditField(Sections = "VO")]
	public string m_hangingSignQuoteVOLine;

	// Token: 0x0400244B RID: 9291
	[CustomEditField(Sections = "VO")]
	public string m_completeStaffQuotePrefab;

	// Token: 0x0400244C RID: 9292
	[CustomEditField(Sections = "VO")]
	public string m_completeStaffQuoteVOLine;

	// Token: 0x0400244D RID: 9293
	private bool m_rodComplete;

	// Token: 0x0400244E RID: 9294
	private bool m_headComplete;

	// Token: 0x0400244F RID: 9295
	private bool m_pearlComplete;

	// Token: 0x04002450 RID: 9296
	private bool m_finalWingComplete;

	// Token: 0x04002451 RID: 9297
	private bool m_animating;
}
