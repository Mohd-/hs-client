using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002A0 RID: 672
public class AdventureClassChallengeButton : PegUIElement
{
	// Token: 0x0600244F RID: 9295 RVA: 0x000B23EB File Offset: 0x000B05EB
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("collection_manager_hero_mouse_over", base.gameObject);
		this.m_Highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
	}

	// Token: 0x06002450 RID: 9296 RVA: 0x000B2410 File Offset: 0x000B0610
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_Highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
	}

	// Token: 0x06002451 RID: 9297 RVA: 0x000B2420 File Offset: 0x000B0620
	public void Select(bool playSound)
	{
		if (playSound)
		{
			SoundManager.Get().LoadAndPlay("select_AI_opponent", base.gameObject);
		}
		this.m_Highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		this.SetEnabled(false);
		this.Depress();
	}

	// Token: 0x06002452 RID: 9298 RVA: 0x000B2464 File Offset: 0x000B0664
	public void Deselect()
	{
		this.m_Highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		this.Raise(0.1f);
		this.SetEnabled(true);
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x000B2494 File Offset: 0x000B0694
	public void SetPortraitMaterial(Material portraitMat)
	{
		Renderer component = this.m_RootObject.GetComponent<Renderer>();
		Material[] materials = component.materials;
		materials[1] = portraitMat;
		component.materials = materials;
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x000B24C0 File Offset: 0x000B06C0
	private void Raise(float time)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_UpBone.localPosition,
			"time",
			time,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(this.m_RootObject, args);
	}

	// Token: 0x06002455 RID: 9301 RVA: 0x000B2534 File Offset: 0x000B0734
	private void Depress()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_DownBone.localPosition,
			"time",
			0.1f,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(this.m_RootObject, args);
	}

	// Token: 0x04001576 RID: 5494
	public UberText m_Text;

	// Token: 0x04001577 RID: 5495
	public int m_ScenarioID;

	// Token: 0x04001578 RID: 5496
	public HighlightState m_Highlight;

	// Token: 0x04001579 RID: 5497
	public GameObject m_RootObject;

	// Token: 0x0400157A RID: 5498
	public GameObject m_Chest;

	// Token: 0x0400157B RID: 5499
	public GameObject m_Checkmark;

	// Token: 0x0400157C RID: 5500
	public Transform m_UpBone;

	// Token: 0x0400157D RID: 5501
	public Transform m_DownBone;
}
