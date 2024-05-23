using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007DE RID: 2014
public class RankedPlayToggleButton : PegUIElement
{
	// Token: 0x06004E77 RID: 20087 RVA: 0x00175358 File Offset: 0x00173558
	public void Up()
	{
		this.m_isDown = false;
		this.m_glowQuad.enabled = false;
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_upBone.localPosition,
			"isLocal",
			true,
			"time",
			0.1,
			"easeType",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_button, args);
		this.m_button.GetComponent<Renderer>().material = this.GetCorrectMaterial(this.m_isWild);
		this.m_highlight.ChangeState(ActorStateType.NONE);
		this.UpdateMaterialTogglers();
	}

	// Token: 0x06004E78 RID: 20088 RVA: 0x00175418 File Offset: 0x00173618
	public void Down()
	{
		this.m_isDown = true;
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_downBone.localPosition,
			"isLocal",
			true,
			"time",
			0.1,
			"easeType",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_button, args);
		this.m_button.GetComponent<Renderer>().material = this.GetCorrectMaterial(this.m_isWild);
		if (DeckPickerTrayDisplay.Get().IsMissingStandardDeckTrayShown())
		{
			this.m_glowQuad.enabled = false;
			this.m_highlight.ChangeState(ActorStateType.NONE);
		}
		else
		{
			this.m_glowQuad.enabled = true;
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
		this.UpdateMaterialTogglers();
	}

	// Token: 0x06004E79 RID: 20089 RVA: 0x00175504 File Offset: 0x00173704
	public void SetFormat(bool isWild)
	{
		this.m_isWild = isWild;
		this.m_button.GetComponent<Renderer>().material = this.GetCorrectMaterial(this.m_isWild);
		this.UpdateMaterialTogglers();
	}

	// Token: 0x06004E7A RID: 20090 RVA: 0x0017552F File Offset: 0x0017372F
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("collection_manager_hero_mouse_over");
		if (this.m_glowQuad.enabled)
		{
			return;
		}
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
	}

	// Token: 0x06004E7B RID: 20091 RVA: 0x0017555F File Offset: 0x0017375F
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		if (this.m_glowQuad.enabled)
		{
			return;
		}
		this.m_highlight.ChangeState(ActorStateType.NONE);
	}

	// Token: 0x06004E7C RID: 20092 RVA: 0x00175580 File Offset: 0x00173780
	private void UpdateMaterialTogglers()
	{
		foreach (RankedPlayToggleButton.EnabledStateMaterialToggler enabledStateMaterialToggler in this.m_materialTogglers)
		{
			if (base.IsEnabled())
			{
				enabledStateMaterialToggler.m_targetMesh.GetComponent<Renderer>().material = ((!this.m_isWild) ? enabledStateMaterialToggler.m_enabledMaterial : enabledStateMaterialToggler.m_wildEnabledMaterial);
			}
			else
			{
				enabledStateMaterialToggler.m_targetMesh.GetComponent<Renderer>().material = enabledStateMaterialToggler.m_disabledMaterial;
			}
		}
	}

	// Token: 0x06004E7D RID: 20093 RVA: 0x0017560C File Offset: 0x0017380C
	private Material GetCorrectMaterial(bool isWild)
	{
		if (!base.IsEnabled() && this.m_buttonDisabledMaterial != null)
		{
			return this.m_buttonDisabledMaterial;
		}
		if (this.m_isDown)
		{
			return (!isWild || !(this.m_wildButtonDownMaterial != null)) ? this.m_buttonDownMaterial : this.m_wildButtonDownMaterial;
		}
		return (!isWild || !(this.m_wildButtonUpMaterial != null)) ? this.m_buttonUpMaterial : this.m_wildButtonUpMaterial;
	}

	// Token: 0x04003566 RID: 13670
	public GameObject m_button;

	// Token: 0x04003567 RID: 13671
	public Transform m_upBone;

	// Token: 0x04003568 RID: 13672
	public Transform m_downBone;

	// Token: 0x04003569 RID: 13673
	public HighlightState m_highlight;

	// Token: 0x0400356A RID: 13674
	public Material m_buttonUpMaterial;

	// Token: 0x0400356B RID: 13675
	public Material m_buttonDownMaterial;

	// Token: 0x0400356C RID: 13676
	public Material m_wildButtonUpMaterial;

	// Token: 0x0400356D RID: 13677
	public Material m_wildButtonDownMaterial;

	// Token: 0x0400356E RID: 13678
	public Material m_buttonDisabledMaterial;

	// Token: 0x0400356F RID: 13679
	public MeshRenderer m_glowQuad;

	// Token: 0x04003570 RID: 13680
	public RankedPlayToggleButton.EnabledStateMaterialToggler[] m_materialTogglers;

	// Token: 0x04003571 RID: 13681
	private bool m_isDown;

	// Token: 0x04003572 RID: 13682
	private bool m_isWild;

	// Token: 0x04003573 RID: 13683
	private Material m_currentDownMaterial;

	// Token: 0x04003574 RID: 13684
	private Material m_currentUpMaterial;

	// Token: 0x020007ED RID: 2029
	[Serializable]
	public struct EnabledStateMaterialToggler
	{
		// Token: 0x040035C8 RID: 13768
		public MeshRenderer m_targetMesh;

		// Token: 0x040035C9 RID: 13769
		public Material m_enabledMaterial;

		// Token: 0x040035CA RID: 13770
		public Material m_wildEnabledMaterial;

		// Token: 0x040035CB RID: 13771
		public Material m_disabledMaterial;
	}
}
