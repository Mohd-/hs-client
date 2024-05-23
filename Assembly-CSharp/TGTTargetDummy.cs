using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200065E RID: 1630
[CustomEditClass]
public class TGTTargetDummy : MonoBehaviour
{
	// Token: 0x060045D2 RID: 17874 RVA: 0x0014FF96 File Offset: 0x0014E196
	private void Awake()
	{
		TGTTargetDummy.s_instance = this;
	}

	// Token: 0x060045D3 RID: 17875 RVA: 0x0014FFA0 File Offset: 0x0014E1A0
	private void Start()
	{
		base.StartCoroutine(this.RegisterBoardEventLargeShake());
		this.m_BodyRotX.GetComponent<Rigidbody>().maxAngularVelocity = this.m_BodyHitIntensity;
		this.m_BodyRotY.GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Max(this.m_SwordHitIntensity, this.m_ShieldHitIntensity);
	}

	// Token: 0x060045D4 RID: 17876 RVA: 0x0014FFF1 File Offset: 0x0014E1F1
	private void Update()
	{
		this.HandleHits();
	}

	// Token: 0x060045D5 RID: 17877 RVA: 0x0014FFF9 File Offset: 0x0014E1F9
	public static TGTTargetDummy Get()
	{
		return TGTTargetDummy.s_instance;
	}

	// Token: 0x060045D6 RID: 17878 RVA: 0x00150000 File Offset: 0x0014E200
	public void ArrowHit()
	{
		this.m_BodyRotX.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(this.m_BodyHitIntensity * 0.25f, this.m_BodyHitIntensity * 0.5f), 0f, 0f);
	}

	// Token: 0x060045D7 RID: 17879 RVA: 0x00150040 File Offset: 0x0014E240
	public void BodyHit()
	{
		this.PlaySqueakSound();
		if (!string.IsNullOrEmpty(this.m_HitBodySoundPrefab))
		{
			string text = FileUtils.GameAssetPathToName(this.m_HitBodySoundPrefab);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, this.m_Body);
			}
		}
		this.m_BodyRotX.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(this.m_BodyHitIntensity * 0.75f, this.m_BodyHitIntensity), 0f, 0f);
		this.m_BodyRotY.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, Random.Range(-5f, 5f), 0f);
	}

	// Token: 0x060045D8 RID: 17880 RVA: 0x001500F0 File Offset: 0x0014E2F0
	public void ShieldHit()
	{
		this.PlaySqueakSound();
		if (Random.Range(0, 100) < 5)
		{
			this.Spin(false);
			return;
		}
		if (!string.IsNullOrEmpty(this.m_HitShieldSoundPrefab))
		{
			string text = FileUtils.GameAssetPathToName(this.m_HitShieldSoundPrefab);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, this.m_Body);
			}
		}
		this.m_BodyRotY.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, Random.Range(this.m_ShieldHitIntensity * 0.7f, this.m_ShieldHitIntensity), 0f);
		this.m_BodyRotX.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-5f, -10f), 0f, 0f);
	}

	// Token: 0x060045D9 RID: 17881 RVA: 0x001501B8 File Offset: 0x0014E3B8
	public void SwordHit()
	{
		this.PlaySqueakSound();
		if (Random.Range(0, 100) < 5)
		{
			this.Spin(true);
			return;
		}
		if (!string.IsNullOrEmpty(this.m_HitSwordSoundPrefab))
		{
			string text = FileUtils.GameAssetPathToName(this.m_HitSwordSoundPrefab);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, this.m_Body);
			}
		}
		this.m_BodyRotY.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, Random.Range(this.m_SwordHitIntensity * 0.7f, this.m_SwordHitIntensity), 0f);
		this.m_BodyRotX.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-5f, -10f), 0f, 0f);
	}

	// Token: 0x060045DA RID: 17882 RVA: 0x00150280 File Offset: 0x0014E480
	private IEnumerator RegisterBoardEventLargeShake()
	{
		while (BoardEvents.Get() == null)
		{
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		BoardEvents.Get().RegisterLargeShakeEvent(new BoardEvents.LargeShakeEventDelegate(this.BodyHit));
		yield break;
	}

	// Token: 0x060045DB RID: 17883 RVA: 0x0015029C File Offset: 0x0014E49C
	private void HandleHits()
	{
		if (UniversalInputManager.Get().GetMouseButtonDown(0) && this.IsOver(this.m_Body))
		{
			this.BodyHit();
		}
		if (UniversalInputManager.Get().GetMouseButtonDown(0) && this.IsOver(this.m_Shield))
		{
			this.ShieldHit();
		}
		if (UniversalInputManager.Get().GetMouseButtonDown(0) && this.IsOver(this.m_Sword))
		{
			this.SwordHit();
		}
	}

	// Token: 0x060045DC RID: 17884 RVA: 0x00150320 File Offset: 0x0014E520
	private void Spin(bool reverse)
	{
		float num = 1080f;
		if (reverse)
		{
			num = -1080f;
		}
		if (!string.IsNullOrEmpty(this.m_HitSpinSoundPrefab))
		{
			string text = FileUtils.GameAssetPathToName(this.m_HitSpinSoundPrefab);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, this.m_Body);
			}
		}
		this.m_BodyMesh.transform.localEulerAngles = Vector3.zero;
		Vector3 vector;
		vector..ctor(0f, this.m_BodyMesh.transform.localEulerAngles.y + num, 0f);
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			vector,
			"isLocal",
			true,
			"time",
			3f,
			"easetype",
			iTween.EaseType.easeOutElastic
		});
		iTween.RotateTo(this.m_BodyMesh, args);
	}

	// Token: 0x060045DD RID: 17885 RVA: 0x0015041C File Offset: 0x0014E61C
	private void PlaySqueakSound()
	{
		base.StopCoroutine("SqueakSound");
		this.m_lastSqueakSoundVol = 0f;
		base.StartCoroutine("SqueakSound");
	}

	// Token: 0x060045DE RID: 17886 RVA: 0x0015044C File Offset: 0x0014E64C
	private IEnumerator SqueakSound()
	{
		if (string.IsNullOrEmpty(this.m_SqueakSoundPrefab))
		{
			yield break;
		}
		if (this.m_squeakSound != null && this.m_squeakSound.isPlaying)
		{
			SoundManager.Get().Stop(this.m_squeakSound);
		}
		if (this.m_squeakSound == null)
		{
			GameObject squeakSoundGO = AssetLoader.Get().LoadSound(FileUtils.GameAssetPathToName(this.m_SqueakSoundPrefab), true, false);
			squeakSoundGO.transform.position = this.m_Body.transform.position;
			this.m_squeakSound = squeakSoundGO.GetComponent<AudioSource>();
		}
		if (this.m_squeakSound == null)
		{
			yield break;
		}
		SoundManager.Get().PlayPreloaded(this.m_squeakSound, 0f);
		while (this.m_squeakSound != null && this.m_squeakSound.isPlaying)
		{
			float difAngle = Mathf.Clamp01(Quaternion.Angle(this.m_Body.transform.rotation, this.m_lastFrameSqueakAngle) * 0.1f);
			this.m_lastFrameSqueakAngle = this.m_Body.transform.rotation;
			difAngle = Mathf.SmoothDamp(difAngle, this.m_lastSqueakSoundVol, ref this.m_squeakSoundVelocity, 0.5f);
			this.m_lastSqueakSoundVol = difAngle;
			SoundManager.Get().SetVolume(this.m_squeakSound, Mathf.Clamp01(difAngle));
			yield return null;
		}
		yield break;
	}

	// Token: 0x060045DF RID: 17887 RVA: 0x00150468 File Offset: 0x0014E668
	private bool IsOver(GameObject go)
	{
		return go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
	}

	// Token: 0x04002CD5 RID: 11477
	private const int SPIN_PERCENT = 5;

	// Token: 0x04002CD6 RID: 11478
	public GameObject m_Body;

	// Token: 0x04002CD7 RID: 11479
	public GameObject m_Shield;

	// Token: 0x04002CD8 RID: 11480
	public GameObject m_Sword;

	// Token: 0x04002CD9 RID: 11481
	public GameObject m_BodyRotX;

	// Token: 0x04002CDA RID: 11482
	public GameObject m_BodyRotY;

	// Token: 0x04002CDB RID: 11483
	public GameObject m_BodyMesh;

	// Token: 0x04002CDC RID: 11484
	public float m_BodyHitIntensity = 25f;

	// Token: 0x04002CDD RID: 11485
	public float m_ShieldHitIntensity = 25f;

	// Token: 0x04002CDE RID: 11486
	public float m_SwordHitIntensity = -25f;

	// Token: 0x04002CDF RID: 11487
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_HitBodySoundPrefab;

	// Token: 0x04002CE0 RID: 11488
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_HitShieldSoundPrefab;

	// Token: 0x04002CE1 RID: 11489
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_HitSwordSoundPrefab;

	// Token: 0x04002CE2 RID: 11490
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_HitSpinSoundPrefab;

	// Token: 0x04002CE3 RID: 11491
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_SqueakSoundPrefab;

	// Token: 0x04002CE4 RID: 11492
	private static TGTTargetDummy s_instance;

	// Token: 0x04002CE5 RID: 11493
	private float m_squeakSoundVelocity;

	// Token: 0x04002CE6 RID: 11494
	private float m_lastSqueakSoundVol;

	// Token: 0x04002CE7 RID: 11495
	private Quaternion m_lastFrameSqueakAngle;

	// Token: 0x04002CE8 RID: 11496
	private AudioSource m_squeakSound;
}
