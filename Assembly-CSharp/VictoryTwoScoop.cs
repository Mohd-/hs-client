using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000880 RID: 2176
public class VictoryTwoScoop : EndGameTwoScoop
{
	// Token: 0x06005317 RID: 21271 RVA: 0x0018C064 File Offset: 0x0018A264
	protected override void ShowImpl()
	{
		this.m_heroActor.SetEntityDef(GameState.Get().GetFriendlySidePlayer().GetHero().GetEntityDef());
		this.m_heroActor.SetCardDef(GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetCardDef());
		this.m_heroActor.UpdateAllComponents();
		this.m_heroActor.TurnOffCollider();
		base.SaveBannerText(GameStrings.Get("GAMEPLAY_END_OF_GAME_VICTORY"));
		base.SetBannerLabel(GameStrings.Get("GAMEPLAY_END_OF_GAME_VICTORY"));
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
			base.gameObject
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
		this.AnimateGodraysTo();
		this.AnimateCrownTo();
		base.StartCoroutine(this.AnimateAll());
	}

	// Token: 0x06005318 RID: 21272 RVA: 0x0018C248 File Offset: 0x0018A448
	protected override void ResetPositions()
	{
		base.gameObject.transform.position = EndGameTwoScoop.START_POSITION;
		base.gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		this.m_rightTrumpet.transform.localPosition = new Vector3(0.23f, -0.6f, 0.16f);
		this.m_rightTrumpet.transform.localScale = new Vector3(1f, 1f, 1f);
		this.m_leftTrumpet.transform.localPosition = new Vector3(-0.23f, -0.6f, 0.16f);
		this.m_leftTrumpet.transform.localScale = new Vector3(-1f, 1f, 1f);
		this.m_rightBanner.transform.localScale = new Vector3(1f, 1f, 0.08f);
		this.m_leftBanner.transform.localScale = new Vector3(1f, 1f, 0.08f);
		this.m_rightCloud.transform.localPosition = new Vector3(-0.2f, -0.8f, 0.26f);
		this.m_leftCloud.transform.localPosition = new Vector3(0.16f, -0.8f, 0.2f);
		this.m_godRays.transform.localEulerAngles = new Vector3(0f, 29f, 0f);
		this.m_godRays2.transform.localEulerAngles = new Vector3(0f, -29f, 0f);
		this.m_crown.transform.localPosition = new Vector3(-0.041f, -0.06f, -0.834f);
		this.m_rightLaurel.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
		this.m_rightLaurel.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		this.m_leftLaurel.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
		this.m_leftLaurel.transform.localScale = new Vector3(-0.7f, 0.7f, 0.7f);
	}

	// Token: 0x06005319 RID: 21273 RVA: 0x0018C4AC File Offset: 0x0018A6AC
	public override void StopAnimating()
	{
		base.StopCoroutine("AnimateAll");
		iTween.Stop(base.gameObject, true);
		base.StartCoroutine(this.ResetPositionsForGoldEvent());
	}

	// Token: 0x0600531A RID: 21274 RVA: 0x0018C4E0 File Offset: 0x0018A6E0
	private IEnumerator AnimateAll()
	{
		yield return new WaitForSeconds(0.25f);
		float TRUMPET_OUT_TIME = 0.4f;
		Hashtable trumpetMoveArgsRight = iTween.Hash(new object[]
		{
			"position",
			new Vector3(-0.52f, -0.6f, -0.23f),
			"time",
			TRUMPET_OUT_TIME,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		});
		iTween.MoveTo(this.m_rightTrumpet, trumpetMoveArgsRight);
		Hashtable trumpetMoveArgsLeft = iTween.Hash(new object[]
		{
			"position",
			new Vector3(0.44f, -0.6f, -0.23f),
			"time",
			TRUMPET_OUT_TIME,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		});
		iTween.MoveTo(this.m_leftTrumpet, trumpetMoveArgsLeft);
		Hashtable trumpetScaleArgsRight = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(1.1f, 1.1f, 1.1f),
			"time",
			0.25f,
			"delay",
			0.3f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.ScaleTo(this.m_rightTrumpet, trumpetScaleArgsRight);
		Hashtable trumpetScaleArgsLeft = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(-1.1f, 1.1f, 1.1f),
			"time",
			0.25f,
			"delay",
			0.3f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.ScaleTo(this.m_leftTrumpet, trumpetScaleArgsLeft);
		Hashtable bannerScaleArgsRight = iTween.Hash(new object[]
		{
			"z",
			1,
			"delay",
			0.24f,
			"time",
			1f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		});
		iTween.ScaleTo(this.m_rightBanner, bannerScaleArgsRight);
		Hashtable bannerScaleArgsLeft = iTween.Hash(new object[]
		{
			"z",
			1,
			"delay",
			0.24f,
			"time",
			1f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		});
		iTween.ScaleTo(this.m_leftBanner, bannerScaleArgsLeft);
		Hashtable cloudMoveArgsRight = iTween.Hash(new object[]
		{
			"x",
			-1.227438,
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
			1.053244,
			"time",
			5,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutCubic
		});
		iTween.MoveTo(this.m_leftCloud, cloudMoveArgsLeft);
		Hashtable laurelRotateArgsRight = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 2f, 0f),
			"time",
			0.5f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic,
			"oncomplete",
			"LaurelWaveTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_rightLaurel, laurelRotateArgsRight);
		Hashtable laurelScaleArgsRight = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(1f, 1f, 1f),
			"time",
			0.25f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.ScaleTo(this.m_rightLaurel, laurelScaleArgsRight);
		Hashtable laurelRotateArgsLeft = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, -2f, 0f),
			"time",
			0.5f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		});
		iTween.RotateTo(this.m_leftLaurel, laurelRotateArgsLeft);
		Hashtable laurelScaleArgsLeft = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(-1f, 1f, 1f),
			"time",
			0.25f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.ScaleTo(this.m_leftLaurel, laurelScaleArgsLeft);
		yield break;
	}

	// Token: 0x0600531B RID: 21275 RVA: 0x0018C4FC File Offset: 0x0018A6FC
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

	// Token: 0x0600531C RID: 21276 RVA: 0x0018C5AC File Offset: 0x0018A7AC
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

	// Token: 0x0600531D RID: 21277 RVA: 0x0018C640 File Offset: 0x0018A840
	private void CloudTo()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"x",
			-0.92f,
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
			0.82f,
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_leftCloud, args2);
	}

	// Token: 0x0600531E RID: 21278 RVA: 0x0018C738 File Offset: 0x0018A938
	private void CloudFro()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"x",
			-1.227438f,
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
			1.053244f,
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_leftCloud, args2);
	}

	// Token: 0x0600531F RID: 21279 RVA: 0x0018C830 File Offset: 0x0018AA30
	private void LaurelWaveTo()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, 0f),
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"LaurelWaveFro",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_rightLaurel, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, 0f),
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.RotateTo(this.m_leftLaurel, args2);
	}

	// Token: 0x06005320 RID: 21280 RVA: 0x0018C944 File Offset: 0x0018AB44
	private void LaurelWaveFro()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 2f, 0f),
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"LaurelWaveTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_rightLaurel, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, -2f, 0f),
			"time",
			10,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.RotateTo(this.m_leftLaurel, args2);
	}

	// Token: 0x06005321 RID: 21281 RVA: 0x0018CA58 File Offset: 0x0018AC58
	private void AnimateCrownTo()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"z",
			-0.78f,
			"time",
			5,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutBack,
			"oncomplete",
			"AnimateCrownFro",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(this.m_crown, args);
	}

	// Token: 0x06005322 RID: 21282 RVA: 0x0018CAEC File Offset: 0x0018ACEC
	private void AnimateCrownFro()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"z",
			-0.834f,
			"time",
			5,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutBack,
			"oncomplete",
			"AnimateCrownTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(this.m_crown, args);
	}

	// Token: 0x06005323 RID: 21283 RVA: 0x0018CB80 File Offset: 0x0018AD80
	private void AnimateGodraysTo()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, -20f, 0f),
			"time",
			20f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"AnimateGodraysFro",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_godRays, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 20f, 0f),
			"time",
			20f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.RotateTo(this.m_godRays2, args2);
	}

	// Token: 0x06005324 RID: 21284 RVA: 0x0018CC9C File Offset: 0x0018AE9C
	private void AnimateGodraysFro()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 20f, 0f),
			"time",
			20f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"AnimateGodraysTo",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateTo(this.m_godRays, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, -20f, 0f),
			"time",
			20f,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.RotateTo(this.m_godRays2, args2);
	}

	// Token: 0x06005325 RID: 21285 RVA: 0x0018CDB8 File Offset: 0x0018AFB8
	private IEnumerator ResetPositionsForGoldEvent()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		float resetTime = 0.25f;
		Hashtable cloudMoveArgsRight = iTween.Hash(new object[]
		{
			"position",
			new Vector3(-1.211758f, -0.8f, -0.2575677f),
			"time",
			resetTime,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_rightCloud, cloudMoveArgsRight);
		Hashtable cloudMoveArgsLeft = iTween.Hash(new object[]
		{
			"position",
			new Vector3(1.068925f, -0.8f, -0.197469f),
			"time",
			resetTime,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_leftCloud, cloudMoveArgsLeft);
		this.m_rightLaurel.transform.localRotation = Quaternion.Euler(Vector3.zero);
		Hashtable laurelMoveArgsRight = iTween.Hash(new object[]
		{
			"position",
			new Vector3(0.1723f, -0.206f, 0.753f),
			"time",
			resetTime,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_rightLaurel, laurelMoveArgsRight);
		this.m_leftLaurel.transform.localRotation = Quaternion.Euler(Vector3.zero);
		Hashtable laurelMoveArgsLeft = iTween.Hash(new object[]
		{
			"position",
			new Vector3(-0.2201783f, -0.318f, 0.753f),
			"time",
			resetTime,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_leftLaurel, laurelMoveArgsLeft);
		Hashtable crownMoveArgs = iTween.Hash(new object[]
		{
			"z",
			-0.9677765f,
			"time",
			resetTime,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutBack
		});
		iTween.MoveTo(this.m_crown, crownMoveArgs);
		Hashtable godrayRotationArgs = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 20f, 0f),
			"time",
			resetTime,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.RotateTo(this.m_godRays, godrayRotationArgs);
		Hashtable godray2RotationArgs = iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, -20f, 0f),
			"time",
			resetTime,
			"isLocal",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.RotateTo(this.m_godRays2, godray2RotationArgs);
		yield break;
	}

	// Token: 0x04003941 RID: 14657
	private const float GOD_RAY_ANGLE = 20f;

	// Token: 0x04003942 RID: 14658
	private const float GOD_RAY_DURATION = 20f;

	// Token: 0x04003943 RID: 14659
	private const float LAUREL_ROTATION = 2f;

	// Token: 0x04003944 RID: 14660
	public GameObject m_godRays;

	// Token: 0x04003945 RID: 14661
	public GameObject m_godRays2;

	// Token: 0x04003946 RID: 14662
	public GameObject m_rightTrumpet;

	// Token: 0x04003947 RID: 14663
	public GameObject m_rightBanner;

	// Token: 0x04003948 RID: 14664
	public GameObject m_rightCloud;

	// Token: 0x04003949 RID: 14665
	public GameObject m_rightLaurel;

	// Token: 0x0400394A RID: 14666
	public GameObject m_leftTrumpet;

	// Token: 0x0400394B RID: 14667
	public GameObject m_leftBanner;

	// Token: 0x0400394C RID: 14668
	public GameObject m_leftCloud;

	// Token: 0x0400394D RID: 14669
	public GameObject m_leftLaurel;

	// Token: 0x0400394E RID: 14670
	public GameObject m_crown;
}
