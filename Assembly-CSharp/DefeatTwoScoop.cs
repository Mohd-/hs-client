using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000825 RID: 2085
public class DefeatTwoScoop : EndGameTwoScoop
{
	// Token: 0x0600504E RID: 20558 RVA: 0x0017D4F4 File Offset: 0x0017B6F4
	protected override void ShowImpl()
	{
		this.m_heroActor.SetEntityDef(GameState.Get().GetFriendlySidePlayer().GetHero().GetEntityDef());
		this.m_heroActor.SetCardDef(GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetCardDef());
		this.m_heroActor.UpdateAllComponents();
		this.m_heroActor.TurnOffCollider();
		base.SaveBannerText(GameStrings.Get("GAMEPLAY_END_OF_GAME_DEFEAT"));
		base.SetBannerLabel(GameStrings.Get("GAMEPLAY_END_OF_GAME_DEFEAT"));
		base.GetComponent<PlayMakerFSM>().SendEvent("Action");
		iTween.FadeTo(base.gameObject, 1f, 0.25f);
		base.gameObject.transform.localScale = new Vector3(EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL);
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(EndGameTwoScoop.END_SCALE_VAL, EndGameTwoScoop.END_SCALE_VAL, EndGameTwoScoop.END_SCALE_VAL),
			"time",
			0.5f,
			"oncomplete",
			"PunchEndGameTwoScoop",
			"oncompletetarget",
			base.gameObject,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.ScaleTo(base.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"position",
			base.gameObject.transform.position + new Vector3(0.005f, 0.005f, 0.005f),
			"time",
			1.5f,
			"oncomplete",
			"TokyoDriftTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(base.gameObject, args2);
		this.AnimateCrownTo();
		this.AnimateLeftTrumpetTo();
		this.AnimateRightTrumpetTo();
		base.StartCoroutine(this.AnimateAll());
	}

	// Token: 0x0600504F RID: 20559 RVA: 0x0017D6F0 File Offset: 0x0017B8F0
	protected override void ResetPositions()
	{
		base.gameObject.transform.position = EndGameTwoScoop.START_POSITION;
		base.gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		this.m_rightTrumpet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		this.m_rightTrumpet.transform.localEulerAngles = new Vector3(0f, -180f, 0f);
		this.m_leftTrumpet.transform.localEulerAngles = new Vector3(0f, -180f, 0f);
		this.m_rightBanner.transform.localScale = new Vector3(1f, 1f, -0.0375f);
		this.m_rightBannerShred.transform.localScale = new Vector3(1f, 1f, 0.05f);
		this.m_rightCloud.transform.localPosition = new Vector3(-0.036f, -0.28f, 0.46f);
		this.m_leftCloud.transform.localPosition = new Vector3(-0.047f, -0.3f, 0.41f);
		this.m_crown.transform.localEulerAngles = new Vector3(-0.026f, 17f, 0.2f);
		this.m_defeatBanner.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
	}

	// Token: 0x06005050 RID: 20560 RVA: 0x0017D87C File Offset: 0x0017BA7C
	private IEnumerator AnimateAll()
	{
		yield return new WaitForSeconds(0.25f);
		Hashtable trumpetScaleArgsRight = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(1f, 1f, 1.1f),
			"time",
			0.25f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.ScaleTo(this.m_rightTrumpet, trumpetScaleArgsRight);
		Hashtable bannerScaleArgsRight = iTween.Hash(new object[]
		{
			"z",
			1,
			"delay",
			0.5f,
			"time",
			1f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		});
		iTween.ScaleTo(this.m_rightBanner, bannerScaleArgsRight);
		Hashtable bannerShredScaleArgsRight = iTween.Hash(new object[]
		{
			"z",
			1,
			"delay",
			0.5f,
			"time",
			1f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		});
		iTween.ScaleTo(this.m_rightBannerShred, bannerShredScaleArgsRight);
		Hashtable cloudMoveArgsRight = iTween.Hash(new object[]
		{
			"x",
			-0.81f,
			"time",
			5,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutCubic,
			"oncomplete",
			"CloudTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(this.m_rightCloud, cloudMoveArgsRight);
		Hashtable cloudMoveArgsLeft = iTween.Hash(new object[]
		{
			"x",
			0.824f,
			"time",
			5,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutCubic
		});
		iTween.MoveTo(this.m_leftCloud, cloudMoveArgsLeft);
		Hashtable woodBannerRotateArgs = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 183f, 0f),
			"time",
			0.5f,
			"delay",
			0.75f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.RotateTo(this.m_defeatBanner, woodBannerRotateArgs);
		yield break;
	}

	// Token: 0x06005051 RID: 20561 RVA: 0x0017D898 File Offset: 0x0017BA98
	private void AnimateLeftTrumpetTo()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, -184f, 0f),
			"time",
			5f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutCirc,
			"oncomplete",
			"AnimateLeftTrumpetFro",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_leftTrumpet, args);
	}

	// Token: 0x06005052 RID: 20562 RVA: 0x0017D940 File Offset: 0x0017BB40
	private void AnimateLeftTrumpetFro()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, -180f, 0f),
			"time",
			5f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutCirc,
			"oncomplete",
			"AnimateLeftTrumpetTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_leftTrumpet, args);
	}

	// Token: 0x06005053 RID: 20563 RVA: 0x0017D9E8 File Offset: 0x0017BBE8
	private void AnimateRightTrumpetTo()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, -172f, 0f),
			"time",
			8f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutCirc,
			"oncomplete",
			"AnimateRightTrumpetFro",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_rightTrumpet, args);
	}

	// Token: 0x06005054 RID: 20564 RVA: 0x0017DA90 File Offset: 0x0017BC90
	private void AnimateRightTrumpetFro()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, -180f, 0f),
			"time",
			8f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutCirc,
			"oncomplete",
			"AnimateRightTrumpetTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_rightTrumpet, args);
	}

	// Token: 0x06005055 RID: 20565 RVA: 0x0017DB38 File Offset: 0x0017BD38
	private void TokyoDriftTo()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			EndGameTwoScoop.START_POSITION + new Vector3(0.2f, 0.2f, 0.2f),
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"TokyoDriftFro",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(base.gameObject, args);
	}

	// Token: 0x06005056 RID: 20566 RVA: 0x0017DBE8 File Offset: 0x0017BDE8
	private void TokyoDriftFro()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			EndGameTwoScoop.START_POSITION,
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"TokyoDriftTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(base.gameObject, args);
	}

	// Token: 0x06005057 RID: 20567 RVA: 0x0017DC7C File Offset: 0x0017BE7C
	private void CloudTo()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"x",
			-0.38f,
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"CloudFro",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(this.m_rightCloud, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"x",
			0.443f,
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_leftCloud, args2);
	}

	// Token: 0x06005058 RID: 20568 RVA: 0x0017DD74 File Offset: 0x0017BF74
	private void CloudFro()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"x",
			-0.81f,
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"CloudTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(this.m_rightCloud, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"x",
			0.824f,
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_leftCloud, args2);
	}

	// Token: 0x06005059 RID: 20569 RVA: 0x0017DE6C File Offset: 0x0017C06C
	private void AnimateCrownTo()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 1.8f, 0f),
			"time",
			0.75f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"AnimateCrownFro",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_crown, args);
	}

	// Token: 0x0600505A RID: 20570 RVA: 0x0017DF14 File Offset: 0x0017C114
	private void AnimateCrownFro()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 17f, 0f),
			"time",
			0.75f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"AnimateCrownTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_crown, args);
	}

	// Token: 0x040036FE RID: 14078
	public GameObject m_rightTrumpet;

	// Token: 0x040036FF RID: 14079
	public GameObject m_rightBanner;

	// Token: 0x04003700 RID: 14080
	public GameObject m_rightBannerShred;

	// Token: 0x04003701 RID: 14081
	public GameObject m_rightCloud;

	// Token: 0x04003702 RID: 14082
	public GameObject m_leftTrumpet;

	// Token: 0x04003703 RID: 14083
	public GameObject m_leftBanner;

	// Token: 0x04003704 RID: 14084
	public GameObject m_leftBannerFront;

	// Token: 0x04003705 RID: 14085
	public GameObject m_leftCloud;

	// Token: 0x04003706 RID: 14086
	public GameObject m_crown;

	// Token: 0x04003707 RID: 14087
	public GameObject m_defeatBanner;
}
