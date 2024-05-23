using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class Notification : MonoBehaviour
{
	// Token: 0x06001605 RID: 5637 RVA: 0x000684B0 File Offset: 0x000666B0
	private void OnDestroy()
	{
		if (this.m_accompaniedAudio && SoundManager.Get() != null)
		{
			SoundManager.Get().Destroy(this.m_accompaniedAudio);
		}
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x000684ED File Offset: 0x000666ED
	public void ChangeText(string newText)
	{
		this.speechUberText.Text = newText;
	}

	// Token: 0x06001607 RID: 5639 RVA: 0x000684FB File Offset: 0x000666FB
	public void ChangeDialogText(string headlineString, string bodyString, string yesOrOKstring, string noString)
	{
		this.speechUberText.Text = bodyString;
		this.headlineUberText.Text = headlineString;
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x00068518 File Offset: 0x00066718
	public void FaceDirection(Notification.SpeechBubbleDirection direction)
	{
		this.bubbleDirection = direction;
		switch (direction)
		{
		case Notification.SpeechBubbleDirection.TopLeft:
			this.upperLeftBubble.GetComponent<Renderer>().enabled = true;
			break;
		case Notification.SpeechBubbleDirection.TopRight:
			this.upperRightBubble.GetComponent<Renderer>().enabled = true;
			break;
		case Notification.SpeechBubbleDirection.BottomLeft:
			this.bottomLeftBubble.GetComponent<Renderer>().enabled = true;
			break;
		case Notification.SpeechBubbleDirection.BottomRight:
			this.bottomRightBubble.GetComponent<Renderer>().enabled = true;
			break;
		}
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000685A3 File Offset: 0x000667A3
	public Notification.SpeechBubbleDirection GetSpeechBubbleDirection()
	{
		return this.bubbleDirection;
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000685AC File Offset: 0x000667AC
	public void ShowPopUpArrow(Notification.PopUpArrowDirection direction)
	{
		switch (direction)
		{
		case Notification.PopUpArrowDirection.Left:
			this.leftPopupArrow.GetComponent<Renderer>().enabled = true;
			break;
		case Notification.PopUpArrowDirection.Right:
			this.rightPopupArrow.GetComponent<Renderer>().enabled = true;
			break;
		case Notification.PopUpArrowDirection.Down:
			this.bottomPopupArrow.GetComponent<Renderer>().enabled = true;
			break;
		case Notification.PopUpArrowDirection.Up:
			this.topPopupArrow.GetComponent<Renderer>().enabled = true;
			break;
		case Notification.PopUpArrowDirection.LeftDown:
			this.bottomLeftPopupArrow.GetComponent<Renderer>().enabled = true;
			break;
		case Notification.PopUpArrowDirection.RightDown:
			this.bottomRightPopupArrow.GetComponent<Renderer>().enabled = true;
			break;
		case Notification.PopUpArrowDirection.RightUp:
			this.topRightPopupArrow.GetComponent<Renderer>().enabled = true;
			break;
		}
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x0006867C File Offset: 0x0006687C
	public void SetPosition(Actor actor, Notification.SpeechBubbleDirection direction)
	{
		if (actor.GetBones() == null)
		{
			Debug.LogError("Notification Error - Tried to set the position of a Speech Bubble, but the target actor has no bones!");
			return;
		}
		GameObject gameObject = SceneUtils.FindChildBySubstring(actor.GetBones(), "SpeechBubbleBones");
		if (gameObject == null)
		{
			Debug.LogError("Notification Error - Tried to set the position of a Speech Bubble, but the target actor has no SpeechBubbleBones!");
			return;
		}
		Vector3 position = Vector3.zero;
		switch (direction)
		{
		case Notification.SpeechBubbleDirection.TopLeft:
			position = SceneUtils.FindChildBySubstring(gameObject, "BottomRight").transform.position;
			break;
		case Notification.SpeechBubbleDirection.TopRight:
			position = SceneUtils.FindChildBySubstring(gameObject, "BottomLeft").transform.position;
			break;
		case Notification.SpeechBubbleDirection.BottomLeft:
			position = SceneUtils.FindChildBySubstring(gameObject, "TopRight").transform.position;
			break;
		case Notification.SpeechBubbleDirection.BottomRight:
			position = SceneUtils.FindChildBySubstring(gameObject, "TopLeft").transform.position;
			break;
		}
		base.transform.position = position;
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x0006876C File Offset: 0x0006696C
	public void SetPositionForSmallBubble(Actor actor)
	{
		if (actor.GetBones() == null)
		{
			Debug.LogError("Notification Error - Tried to set the position of a Speech Bubble, but the target actor has no bones!");
			return;
		}
		GameObject gameObject = SceneUtils.FindChildBySubstring(actor.GetBones(), "SpeechBubbleBones");
		if (gameObject == null)
		{
			Debug.LogError("Notification Error - Tried to set the position of a Speech Bubble, but the target actor has no SpeechBubbleBones!");
			return;
		}
		base.transform.position = SceneUtils.FindChildBySubstring(gameObject, "SmallBubble").transform.position;
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x000687DD File Offset: 0x000669DD
	private void FinishDeath()
	{
		Object.Destroy(base.gameObject);
		if (this.OnFinishDeathState != null)
		{
			this.OnFinishDeathState.Invoke();
		}
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x00068800 File Offset: 0x00066A00
	public void PlayDeath()
	{
		if (this.destroyEvent != null)
		{
			this.destroyEvent.Activate();
		}
		if (this.bounceObject != null || this.fadeArrowObject != null)
		{
			this.FinishDeath();
			return;
		}
		this.isDying = true;
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			Vector3.zero,
			"easetype",
			iTween.EaseType.easeInExpo,
			"time",
			0.5f,
			"oncomplete",
			"FinishDeath",
			"oncompletetarget",
			base.gameObject
		}));
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x000688D0 File Offset: 0x00066AD0
	public void Shrink(float duration = -1f)
	{
		if (duration < 0f)
		{
			duration = 0.5f;
		}
		iTween.Stop(base.gameObject);
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			Vector3.zero,
			"easetype",
			iTween.EaseType.easeInExpo,
			"time",
			duration
		}));
	}

	// Token: 0x06001610 RID: 5648 RVA: 0x00068948 File Offset: 0x00066B48
	public void Unshrink(float duration = -1f)
	{
		if (this.isDying)
		{
			return;
		}
		if (duration < 0f)
		{
			duration = 0.5f;
		}
		iTween.Stop(base.gameObject);
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			this.m_initialScale,
			"easetype",
			iTween.EaseType.easeInExpo,
			"time",
			duration
		}));
	}

	// Token: 0x06001611 RID: 5649 RVA: 0x000689CD File Offset: 0x00066BCD
	public bool IsDying()
	{
		return this.isDying;
	}

	// Token: 0x06001612 RID: 5650 RVA: 0x000689D8 File Offset: 0x00066BD8
	public void PlayBirth()
	{
		if (this.showEvent != null)
		{
			this.showEvent.Activate();
		}
		if (this.bounceObject == null && this.fadeArrowObject == null)
		{
			Vector3 localScale = base.transform.localScale;
			base.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
			{
				"scale",
				localScale,
				"easetype",
				iTween.EaseType.easeOutElastic,
				"time",
				1f
			}));
			this.m_initialScale = localScale;
			return;
		}
		if (this.bounceObject != null)
		{
			this.BounceDown();
		}
		else if (this.fadeArrowObject != null)
		{
			this.FadeOut();
		}
	}

	// Token: 0x06001613 RID: 5651 RVA: 0x00068AD8 File Offset: 0x00066CD8
	public void PlayBirthWithForcedScale(Vector3 targetScale)
	{
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			targetScale,
			"easetype",
			iTween.EaseType.easeOutElastic,
			"time",
			1f
		}));
		this.m_initialScale = base.transform.localScale;
	}

	// Token: 0x06001614 RID: 5652 RVA: 0x00068B44 File Offset: 0x00066D44
	public void PlaySmallBirthForFakeBubble()
	{
		if (this.showEvent != null)
		{
			this.showEvent.Activate();
		}
		if (this.bounceObject == null && this.fadeArrowObject == null)
		{
			float num = 0.25f;
			Vector3 vector;
			vector..ctor(num * base.transform.localScale.x, num * base.transform.localScale.y, num * base.transform.localScale.z);
			base.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
			{
				"scale",
				vector,
				"easetype",
				iTween.EaseType.easeOutElastic,
				"time",
				1f
			}));
			return;
		}
		this.BounceDown();
	}

	// Token: 0x06001615 RID: 5653 RVA: 0x00068C50 File Offset: 0x00066E50
	public void PulseReminderEveryXSeconds(float seconds)
	{
		base.StartCoroutine(this.PulseReminder(seconds));
	}

	// Token: 0x06001616 RID: 5654 RVA: 0x00068C60 File Offset: 0x00066E60
	private IEnumerator PulseReminder(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		if (this.isDying)
		{
			yield break;
		}
		iTween.PunchScale(base.gameObject, iTween.Hash(new object[]
		{
			"amount",
			Vector3.one,
			"time",
			1f
		}));
		base.StartCoroutine(this.PulseReminder(seconds));
		yield break;
	}

	// Token: 0x06001617 RID: 5655 RVA: 0x00068C8C File Offset: 0x00066E8C
	private void BounceUp()
	{
		iTween.MoveTo(this.bounceObject, iTween.Hash(new object[]
		{
			"islocal",
			true,
			"z",
			this.bounceObject.transform.localPosition.z - 0.5f,
			"time",
			0.75f,
			"easetype",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			"BounceDown",
			"oncompletetarget",
			base.gameObject
		}));
	}

	// Token: 0x06001618 RID: 5656 RVA: 0x00068D3C File Offset: 0x00066F3C
	private void BounceDown()
	{
		iTween.MoveTo(this.bounceObject, iTween.Hash(new object[]
		{
			"islocal",
			true,
			"z",
			this.bounceObject.transform.localPosition.z + 0.5f,
			"time",
			0.75f,
			"easetype",
			iTween.EaseType.easeOutCubic,
			"oncomplete",
			"BounceUp",
			"oncompletetarget",
			base.gameObject
		}));
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x00068DEC File Offset: 0x00066FEC
	private void FadeOut()
	{
		iTween.MoveTo(this.fadeArrowObject, iTween.Hash(new object[]
		{
			"islocal",
			true,
			"z",
			this.fadeArrowObject.transform.localPosition.z - 0.5f,
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"FadeComplete",
			"oncompletetarget",
			base.gameObject
		}));
		AnimationUtil.FadeTexture(this.fadeArrowObject.GetComponentInChildren<MeshRenderer>(), 1f, 0f, 0.5f, 0.15f, null);
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x00068EC0 File Offset: 0x000670C0
	private void FadeComplete()
	{
		iTween.MoveTo(this.fadeArrowObject, iTween.Hash(new object[]
		{
			"islocal",
			true,
			"z",
			this.fadeArrowObject.transform.localPosition.z + 0.5f,
			"time",
			0f,
			"delay",
			0.5f,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"FadeOut",
			"oncompletetarget",
			base.gameObject
		}));
		AnimationUtil.FadeTexture(this.fadeArrowObject.GetComponentInChildren<MeshRenderer>(), 0f, 1f, 0f, 0.85f, null);
	}

	// Token: 0x0600161B RID: 5659 RVA: 0x00068FAA File Offset: 0x000671AA
	public void AssignAudio(AudioSource source)
	{
		this.m_accompaniedAudio = source;
	}

	// Token: 0x0600161C RID: 5660 RVA: 0x00068FB3 File Offset: 0x000671B3
	public AudioSource GetAudio()
	{
		return this.m_accompaniedAudio;
	}

	// Token: 0x04000AE7 RID: 2791
	private const float BOUNCE_SPEED = 0.75f;

	// Token: 0x04000AE8 RID: 2792
	private const float FADE_SPEED = 0.5f;

	// Token: 0x04000AE9 RID: 2793
	private const float FADE_PAUSE = 0.85f;

	// Token: 0x04000AEA RID: 2794
	private const int MAX_CHARACTERS = 20;

	// Token: 0x04000AEB RID: 2795
	private const int MAX_CHARACTERS_IN_DIALOG = 28;

	// Token: 0x04000AEC RID: 2796
	public const float DEATH_ANIMATION_DURATION = 0.5f;

	// Token: 0x04000AED RID: 2797
	public UberText speechUberText;

	// Token: 0x04000AEE RID: 2798
	public UberText headlineUberText;

	// Token: 0x04000AEF RID: 2799
	public GameObject upperLeftBubble;

	// Token: 0x04000AF0 RID: 2800
	public GameObject bottomLeftBubble;

	// Token: 0x04000AF1 RID: 2801
	public GameObject upperRightBubble;

	// Token: 0x04000AF2 RID: 2802
	public GameObject bottomRightBubble;

	// Token: 0x04000AF3 RID: 2803
	public GameObject bounceObject;

	// Token: 0x04000AF4 RID: 2804
	public GameObject fadeArrowObject;

	// Token: 0x04000AF5 RID: 2805
	public GameObject leftPopupArrow;

	// Token: 0x04000AF6 RID: 2806
	public GameObject rightPopupArrow;

	// Token: 0x04000AF7 RID: 2807
	public GameObject bottomPopupArrow;

	// Token: 0x04000AF8 RID: 2808
	public GameObject topPopupArrow;

	// Token: 0x04000AF9 RID: 2809
	public GameObject bottomLeftPopupArrow;

	// Token: 0x04000AFA RID: 2810
	public GameObject bottomRightPopupArrow;

	// Token: 0x04000AFB RID: 2811
	public GameObject topRightPopupArrow;

	// Token: 0x04000AFC RID: 2812
	public Spell showEvent;

	// Token: 0x04000AFD RID: 2813
	public Spell destroyEvent;

	// Token: 0x04000AFE RID: 2814
	public PegUIElement clickOff;

	// Token: 0x04000AFF RID: 2815
	public MeshRenderer artOverlay;

	// Token: 0x04000B00 RID: 2816
	public Material swapMaterial;

	// Token: 0x04000B01 RID: 2817
	public Action OnFinishDeathState;

	// Token: 0x04000B02 RID: 2818
	private bool isDying;

	// Token: 0x04000B03 RID: 2819
	private AudioSource m_accompaniedAudio;

	// Token: 0x04000B04 RID: 2820
	private Notification.SpeechBubbleDirection bubbleDirection;

	// Token: 0x04000B05 RID: 2821
	private Vector3 m_initialScale;

	// Token: 0x02000373 RID: 883
	public enum PopUpArrowDirection
	{
		// Token: 0x04001C1C RID: 7196
		Left,
		// Token: 0x04001C1D RID: 7197
		Right,
		// Token: 0x04001C1E RID: 7198
		Down,
		// Token: 0x04001C1F RID: 7199
		Up,
		// Token: 0x04001C20 RID: 7200
		LeftDown,
		// Token: 0x04001C21 RID: 7201
		RightDown,
		// Token: 0x04001C22 RID: 7202
		RightUp
	}

	// Token: 0x020003ED RID: 1005
	public enum SpeechBubbleDirection
	{
		// Token: 0x0400202E RID: 8238
		None,
		// Token: 0x0400202F RID: 8239
		TopLeft,
		// Token: 0x04002030 RID: 8240
		TopRight,
		// Token: 0x04002031 RID: 8241
		BottomLeft,
		// Token: 0x04002032 RID: 8242
		BottomRight
	}
}
