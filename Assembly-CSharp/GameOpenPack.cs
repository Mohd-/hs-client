using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000855 RID: 2133
public class GameOpenPack : MonoBehaviour
{
	// Token: 0x06005227 RID: 21031 RVA: 0x00188949 File Offset: 0x00186B49
	public void Finish()
	{
		if (GameState.Get() == null)
		{
			return;
		}
		GameState.Get().GetGameEntity().NotifyOfCustomIntroFinished();
	}

	// Token: 0x06005228 RID: 21032 RVA: 0x00188965 File Offset: 0x00186B65
	public void PlayJainaLine()
	{
		GameState.Get().GetGameEntity().SendCustomEvent(66);
	}

	// Token: 0x06005229 RID: 21033 RVA: 0x00188978 File Offset: 0x00186B78
	public void PlayHoggerLine()
	{
		if (MulliganManager.Get() == null)
		{
			return;
		}
	}

	// Token: 0x0600522A RID: 21034 RVA: 0x0018898C File Offset: 0x00186B8C
	private IEnumerator PlayHoggerAfterVersus()
	{
		yield return new WaitForSeconds(1f);
		Card hoggerCard = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
		SoundManager.Get().Play(hoggerCard.GetAnnouncerLine());
		yield break;
	}

	// Token: 0x0600522B RID: 21035 RVA: 0x001889A0 File Offset: 0x00186BA0
	public void RaiseBoardLights()
	{
		Board.Get().RaiseTheLights();
	}

	// Token: 0x0600522C RID: 21036 RVA: 0x001889AC File Offset: 0x00186BAC
	public void Begin()
	{
		if (GameState.Get() == null)
		{
			return;
		}
		GameState.Get().GetGameEntity().NotifyOfGamePackOpened();
	}

	// Token: 0x0600522D RID: 21037 RVA: 0x001889C8 File Offset: 0x00186BC8
	public void NotifyOfFullyLoaded()
	{
		this.fullyLoaded = true;
	}

	// Token: 0x0600522E RID: 21038 RVA: 0x001889D4 File Offset: 0x00186BD4
	public void NotifyOfMouseOver()
	{
		if (!this.fullyLoaded)
		{
			return;
		}
		if (this.clickedOnPack)
		{
			return;
		}
		this.m_playMakerFSM.SendEvent("Birth");
	}

	// Token: 0x0600522F RID: 21039 RVA: 0x00188A0C File Offset: 0x00186C0C
	public void NotifyOfMouseOff()
	{
		if (!this.fullyLoaded)
		{
			return;
		}
		if (this.clickedOnPack)
		{
			return;
		}
		this.m_playMakerFSM.SendEvent("Cancel");
	}

	// Token: 0x06005230 RID: 21040 RVA: 0x00188A44 File Offset: 0x00186C44
	public void HandleClick()
	{
		if (!this.fullyLoaded)
		{
			return;
		}
		if (this.clickedOnPack)
		{
			return;
		}
		if (!SceneMgr.Get().IsSceneLoaded())
		{
			return;
		}
		if (LoadingScreen.Get() != null && LoadingScreen.Get().IsTransitioning())
		{
			return;
		}
		MusicManager.Get().StartPlaylist(MusicPlaylistType.Misc_Tutorial01PackOpen);
		this.clickedOnPack = true;
		this.m_playMakerFSM.SendEvent("Action");
	}

	// Token: 0x04003871 RID: 14449
	public PlayMakerFSM m_playMakerFSM;

	// Token: 0x04003872 RID: 14450
	private bool clickedOnPack;

	// Token: 0x04003873 RID: 14451
	private bool fullyLoaded;
}
