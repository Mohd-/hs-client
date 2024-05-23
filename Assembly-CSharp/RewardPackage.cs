using System;
using UnityEngine;

// Token: 0x020007FB RID: 2043
public class RewardPackage : PegUIElement
{
	// Token: 0x06004F56 RID: 20310 RVA: 0x0017949E File Offset: 0x0017769E
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06004F57 RID: 20311 RVA: 0x001794A6 File Offset: 0x001776A6
	public void OnEnable()
	{
		this.SetEnabled(true);
	}

	// Token: 0x06004F58 RID: 20312 RVA: 0x001794AF File Offset: 0x001776AF
	public void OnDisable()
	{
		this.SetEnabled(false);
	}

	// Token: 0x06004F59 RID: 20313 RVA: 0x001794B8 File Offset: 0x001776B8
	public void Update()
	{
		if (Input.GetKeyDown(49) && this.m_RewardIndex == 0)
		{
			this.OpenReward();
		}
		else if (Input.GetKeyDown(50) && this.m_RewardIndex == 1)
		{
			this.OpenReward();
		}
		else if (Input.GetKeyDown(51) && this.m_RewardIndex == 2)
		{
			this.OpenReward();
		}
		else if (Input.GetKeyDown(52) && this.m_RewardIndex == 3)
		{
			this.OpenReward();
		}
		else if (Input.GetKeyDown(53) && this.m_RewardIndex == 4)
		{
			this.OpenReward();
		}
	}

	// Token: 0x06004F5A RID: 20314 RVA: 0x00179570 File Offset: 0x00177770
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		PlayMakerFSM component = base.GetComponent<PlayMakerFSM>();
		component.SendEvent("Action");
	}

	// Token: 0x06004F5B RID: 20315 RVA: 0x0017958F File Offset: 0x0017778F
	protected override void OnPress()
	{
		this.OpenReward();
	}

	// Token: 0x06004F5C RID: 20316 RVA: 0x00179598 File Offset: 0x00177798
	private void OpenReward()
	{
		PlayMakerFSM component = base.GetComponent<PlayMakerFSM>();
		component.SendEvent("Death");
		RewardBoxesDisplay.Get().OpenReward(this.m_RewardIndex, base.transform.position);
		base.enabled = false;
	}

	// Token: 0x04003637 RID: 13879
	public int m_RewardIndex = -1;
}
