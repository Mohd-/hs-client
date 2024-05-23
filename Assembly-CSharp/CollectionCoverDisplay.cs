using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006D6 RID: 1750
public class CollectionCoverDisplay : PegUIElement
{
	// Token: 0x0600487F RID: 18559 RVA: 0x0015AC60 File Offset: 0x00158E60
	protected override void Awake()
	{
		base.Awake();
		this.m_boxCollider = base.transform.GetComponent<BoxCollider>();
	}

	// Token: 0x06004880 RID: 18560 RVA: 0x0015AC79 File Offset: 0x00158E79
	private void Start()
	{
	}

	// Token: 0x06004881 RID: 18561 RVA: 0x0015AC7B File Offset: 0x00158E7B
	private void Update()
	{
	}

	// Token: 0x06004882 RID: 18562 RVA: 0x0015AC7D File Offset: 0x00158E7D
	public bool IsAnimating()
	{
		return this.m_isAnimating;
	}

	// Token: 0x06004883 RID: 18563 RVA: 0x0015AC88 File Offset: 0x00158E88
	public void Open(CollectionCoverDisplay.DelOnOpened callback)
	{
		if (this.m_bookCover.transform.localEulerAngles.z == this.BOOK_COVER_FULLY_OPEN_Z_ROTATION)
		{
			return;
		}
		this.EnableCollider(false);
		this.SetIsAnimating(true);
		this.AnimateLatchOpening();
		this.AnimateCoverOpening(callback);
		SoundManager.Get().LoadAndPlay("collection_manager_book_open");
	}

	// Token: 0x06004884 RID: 18564 RVA: 0x0015ACE4 File Offset: 0x00158EE4
	public void SetOpenState()
	{
		if (!this.m_bookCover.activeSelf)
		{
			return;
		}
		this.EnableCollider(false);
		this.SetIsAnimating(false);
		this.m_bookCover.SetActive(false);
		this.m_bookCoverLatchJoint.GetComponent<Renderer>().enabled = false;
	}

	// Token: 0x06004885 RID: 18565 RVA: 0x0015AD30 File Offset: 0x00158F30
	public void Close()
	{
		this.m_bookCover.SetActive(true);
		if (this.m_bookCover.transform.localEulerAngles.z == this.BOOK_COVER_FULLY_CLOSED_Z_ROTATION)
		{
			return;
		}
		this.SetIsAnimating(true);
		this.AnimateCoverClosing();
		SoundManager.Get().LoadAndPlay("collection_manager_book_close");
	}

	// Token: 0x06004886 RID: 18566 RVA: 0x0015AD89 File Offset: 0x00158F89
	private void SetIsAnimating(bool animating)
	{
		this.m_isAnimating = animating;
	}

	// Token: 0x06004887 RID: 18567 RVA: 0x0015AD92 File Offset: 0x00158F92
	private void EnableCollider(bool enabled)
	{
		this.SetEnabled(enabled);
		this.m_boxCollider.enabled = enabled;
	}

	// Token: 0x06004888 RID: 18568 RVA: 0x0015ADA8 File Offset: 0x00158FA8
	private void AnimateLatchOpening()
	{
		this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].speed = this.LATCH_OPEN_ANIM_SPEED;
		if (this.m_bookCoverLatch.GetComponent<Animation>().IsPlaying(this.LATCH_OPEN_ANIM_NAME))
		{
			base.StopCoroutine(this.CRACK_LATCH_OPEN_ANIM_COROUTINE);
		}
		else
		{
			this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].time = 0f;
			this.m_bookCoverLatch.GetComponent<Animation>().Play(this.LATCH_OPEN_ANIM_NAME);
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			0,
			"delay",
			this.LATCH_FADE_DELAY,
			"time",
			this.LATCH_FADE_TIME,
			"easeType",
			iTween.EaseType.linear,
			"oncomplete",
			"OnLatchOpened",
			"oncompletetarget",
			base.gameObject
		});
		iTween.FadeTo(this.m_bookCoverLatchJoint, args);
	}

	// Token: 0x06004889 RID: 18569 RVA: 0x0015AEC8 File Offset: 0x001590C8
	private void AnimateCoverOpening(CollectionCoverDisplay.DelOnOpened callback)
	{
		this.m_bookCoverLatchJoint.GetComponent<Renderer>().material = this.m_latchFadeMaterial;
		Vector3 localEulerAngles = this.m_bookCover.transform.localEulerAngles;
		localEulerAngles.z = this.BOOK_COVER_FULLY_OPEN_Z_ROTATION;
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			localEulerAngles,
			"isLocal",
			true,
			"time",
			this.BOOK_COVER_FULL_ANIM_TIME,
			"easeType",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			"OnCoverOpened",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			callback,
			"name",
			"rotation"
		});
		iTween.StopByName(this.m_bookCover.gameObject, "rotation");
		iTween.RotateTo(this.m_bookCover.gameObject, args);
	}

	// Token: 0x0600488A RID: 18570 RVA: 0x0015AFCC File Offset: 0x001591CC
	private void AnimateCoverClosing()
	{
		Vector3 localEulerAngles = this.m_bookCover.transform.localEulerAngles;
		localEulerAngles.z = this.BOOK_COVER_FULLY_CLOSED_Z_ROTATION;
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			localEulerAngles,
			"isLocal",
			true,
			"time",
			this.BOOK_COVER_FULL_ANIM_TIME,
			"easeType",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			"AnimateLatchClosing",
			"oncompletetarget",
			base.gameObject,
			"name",
			"rotation"
		});
		iTween.StopByName(this.m_bookCover.gameObject, "rotation");
		iTween.RotateTo(this.m_bookCover.gameObject, args);
	}

	// Token: 0x0600488B RID: 18571 RVA: 0x0015B0AC File Offset: 0x001592AC
	private void AnimateLatchClosing()
	{
		this.m_bookCoverLatchJoint.GetComponent<Renderer>().enabled = true;
		this.m_bookCoverLatchJoint.GetComponent<Renderer>().material = this.m_latchFadeMaterial;
		this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].time = this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].length;
		this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].speed = -this.LATCH_OPEN_ANIM_SPEED * 2f;
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			1,
			"time",
			this.LATCH_FADE_TIME,
			"easeType",
			iTween.EaseType.linear,
			"oncomplete",
			"OnLatchClosed",
			"oncompletetarget",
			base.gameObject
		});
		this.m_bookCoverLatch.GetComponent<Animation>().Play(this.LATCH_OPEN_ANIM_NAME);
		iTween.FadeTo(this.m_bookCoverLatchJoint, args);
	}

	// Token: 0x0600488C RID: 18572 RVA: 0x0015B1CC File Offset: 0x001593CC
	private void OnCoverOpened(CollectionCoverDisplay.DelOnOpened callback)
	{
		this.m_bookCover.SetActive(false);
		this.SetIsAnimating(false);
		if (callback == null)
		{
			return;
		}
		callback();
	}

	// Token: 0x0600488D RID: 18573 RVA: 0x0015B1F9 File Offset: 0x001593F9
	private void OnLatchOpened()
	{
		this.m_bookCoverLatchJoint.GetComponent<Renderer>().enabled = false;
	}

	// Token: 0x0600488E RID: 18574 RVA: 0x0015B20C File Offset: 0x0015940C
	private void OnLatchClosed()
	{
		this.EnableCollider(true);
		this.SetIsAnimating(false);
	}

	// Token: 0x0600488F RID: 18575 RVA: 0x0015B21C File Offset: 0x0015941C
	private void CrackOpen()
	{
		if (this.IsAnimating())
		{
			return;
		}
		base.StopCoroutine(this.CRACK_LATCH_OPEN_ANIM_COROUTINE);
		base.StartCoroutine(this.CRACK_LATCH_OPEN_ANIM_COROUTINE);
	}

	// Token: 0x06004890 RID: 18576 RVA: 0x0015B244 File Offset: 0x00159444
	private IEnumerator AnimateLatchCrackOpen()
	{
		this.m_bookCoverLatchJoint.GetComponent<Renderer>().material = this.m_latchOpaqueMaterial;
		this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].time = 0f;
		this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].speed = this.LATCH_OPEN_ANIM_SPEED;
		SoundManager.Get().LoadAndPlay("collection_manager_book_latch_jiggle");
		this.m_bookCoverLatch.GetComponent<Animation>().Play(this.LATCH_OPEN_ANIM_NAME);
		while (this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].time < 0.75f)
		{
			yield return null;
		}
		this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].speed = 0f;
		yield break;
	}

	// Token: 0x06004891 RID: 18577 RVA: 0x0015B260 File Offset: 0x00159460
	private void CrackClose()
	{
		if (this.IsAnimating())
		{
			return;
		}
		if (!this.m_bookCoverLatch.GetComponent<Animation>().IsPlaying(this.LATCH_OPEN_ANIM_NAME))
		{
			return;
		}
		base.StopCoroutine(this.CRACK_LATCH_OPEN_ANIM_COROUTINE);
		this.m_bookCoverLatch.GetComponent<Animation>()[this.LATCH_OPEN_ANIM_NAME].speed = -this.LATCH_OPEN_ANIM_SPEED;
	}

	// Token: 0x04002FAC RID: 12204
	public GameObject m_bookCoverLatch;

	// Token: 0x04002FAD RID: 12205
	public GameObject m_bookCoverLatchJoint;

	// Token: 0x04002FAE RID: 12206
	public GameObject m_bookCover;

	// Token: 0x04002FAF RID: 12207
	public Material m_latchFadeMaterial;

	// Token: 0x04002FB0 RID: 12208
	public Material m_latchOpaqueMaterial;

	// Token: 0x04002FB1 RID: 12209
	private readonly string CRACK_LATCH_OPEN_ANIM_COROUTINE = "AnimateLatchCrackOpen";

	// Token: 0x04002FB2 RID: 12210
	private readonly string LATCH_OPEN_ANIM_NAME = "CollectionManagerCoverV2_Lock_edit";

	// Token: 0x04002FB3 RID: 12211
	private readonly float LATCH_OPEN_ANIM_SPEED = 4f;

	// Token: 0x04002FB4 RID: 12212
	private readonly float LATCH_FADE_TIME = 0.1f;

	// Token: 0x04002FB5 RID: 12213
	private readonly float LATCH_FADE_DELAY = 0.15f;

	// Token: 0x04002FB6 RID: 12214
	private readonly float BOOK_COVER_FULLY_CLOSED_Z_ROTATION;

	// Token: 0x04002FB7 RID: 12215
	private readonly float BOOK_COVER_FULLY_OPEN_Z_ROTATION = 280f;

	// Token: 0x04002FB8 RID: 12216
	private readonly float BOOK_COVER_FULL_ANIM_TIME = 0.75f;

	// Token: 0x04002FB9 RID: 12217
	private bool m_isAnimating;

	// Token: 0x04002FBA RID: 12218
	private BoxCollider m_boxCollider;

	// Token: 0x020006D7 RID: 1751
	// (Invoke) Token: 0x06004893 RID: 18579
	public delegate void DelOnOpened();
}
