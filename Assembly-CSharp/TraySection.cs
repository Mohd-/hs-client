using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006E9 RID: 1769
public class TraySection : MonoBehaviour
{
	// Token: 0x060048F2 RID: 18674 RVA: 0x0015CC5B File Offset: 0x0015AE5B
	public void ShowDoor(bool show)
	{
		if (!this.m_showDoor)
		{
			show = false;
		}
		this.m_door.gameObject.SetActive(show);
	}

	// Token: 0x060048F3 RID: 18675 RVA: 0x0015CC7C File Offset: 0x0015AE7C
	public bool IsOpen()
	{
		return this.m_isOpen;
	}

	// Token: 0x060048F4 RID: 18676 RVA: 0x0015CC84 File Offset: 0x0015AE84
	public Bounds GetDoorBounds()
	{
		return this.m_door.GetComponent<Renderer>().bounds;
	}

	// Token: 0x060048F5 RID: 18677 RVA: 0x0015CC96 File Offset: 0x0015AE96
	public Vector3? GetIntermediateDeckBoxPos()
	{
		return this.m_intermediateDeckBoxLocalPos;
	}

	// Token: 0x060048F6 RID: 18678 RVA: 0x0015CC9E File Offset: 0x0015AE9E
	public void OpenDoor()
	{
		this.OpenDoor(null);
	}

	// Token: 0x060048F7 RID: 18679 RVA: 0x0015CCA7 File Offset: 0x0015AEA7
	public void OpenDoor(TraySection.DelOnDoorStateChangedCallback callback)
	{
		this.OpenDoor(callback, null);
	}

	// Token: 0x060048F8 RID: 18680 RVA: 0x0015CCB1 File Offset: 0x0015AEB1
	public void OpenDoor(TraySection.DelOnDoorStateChangedCallback callback, object callbackData)
	{
		this.OpenDoor(false, callback, callbackData);
	}

	// Token: 0x060048F9 RID: 18681 RVA: 0x0015CCBC File Offset: 0x0015AEBC
	public void OpenDoorImmediately()
	{
		this.OpenDoorImmediately(null);
	}

	// Token: 0x060048FA RID: 18682 RVA: 0x0015CCC5 File Offset: 0x0015AEC5
	public void OpenDoorImmediately(TraySection.DelOnDoorStateChangedCallback callback)
	{
		this.OpenDoorImmediately(callback, null);
	}

	// Token: 0x060048FB RID: 18683 RVA: 0x0015CCCF File Offset: 0x0015AECF
	public void OpenDoorImmediately(TraySection.DelOnDoorStateChangedCallback callback, object callbackData)
	{
		this.OpenDoor(true, callback, callbackData);
	}

	// Token: 0x060048FC RID: 18684 RVA: 0x0015CCDA File Offset: 0x0015AEDA
	public void CloseDoor()
	{
		this.CloseDoor(null);
	}

	// Token: 0x060048FD RID: 18685 RVA: 0x0015CCE3 File Offset: 0x0015AEE3
	public void CloseDoor(TraySection.DelOnDoorStateChangedCallback callback)
	{
		this.CloseDoor(callback, null);
	}

	// Token: 0x060048FE RID: 18686 RVA: 0x0015CCED File Offset: 0x0015AEED
	public void CloseDoor(TraySection.DelOnDoorStateChangedCallback callback, object callbackData)
	{
		this.CloseDoor(false, callback, callbackData);
	}

	// Token: 0x060048FF RID: 18687 RVA: 0x0015CCF8 File Offset: 0x0015AEF8
	public void CloseDoorImmediately()
	{
		this.CloseDoorImmediately(null);
	}

	// Token: 0x06004900 RID: 18688 RVA: 0x0015CD01 File Offset: 0x0015AF01
	public void CloseDoorImmediately(TraySection.DelOnDoorStateChangedCallback callback)
	{
		this.CloseDoorImmediately(callback, null);
	}

	// Token: 0x06004901 RID: 18689 RVA: 0x0015CD0B File Offset: 0x0015AF0B
	public void CloseDoorImmediately(TraySection.DelOnDoorStateChangedCallback callback, object callbackData)
	{
		this.CloseDoor(true, callback, callbackData);
	}

	// Token: 0x06004902 RID: 18690 RVA: 0x0015CD16 File Offset: 0x0015AF16
	public bool IsDeckBoxShown()
	{
		return this.m_deckBoxShown;
	}

	// Token: 0x06004903 RID: 18691 RVA: 0x0015CD1E File Offset: 0x0015AF1E
	public void EnableDoors(bool show)
	{
		this.m_showDoor = show;
	}

	// Token: 0x06004904 RID: 18692 RVA: 0x0015CD28 File Offset: 0x0015AF28
	public void ShowDeckBox(bool immediate = false, TraySection.DelOnDoorStateChangedCallback callback = null)
	{
		base.gameObject.SetActive(true);
		this.m_deckBoxShown = true;
		if (this.m_showDoor)
		{
			this.m_door.gameObject.SetActive(true);
		}
		this.OpenDoor(immediate, delegate(object _1)
		{
			if (this.m_deckBox != null)
			{
				this.m_deckBox.Show();
				this.m_deckBox.PlayPopUpAnimation(delegate(object _2)
				{
					this.m_door.gameObject.SetActive(false);
					if (callback != null)
					{
						callback(this);
					}
				});
			}
			else
			{
				this.m_door.gameObject.SetActive(false);
				if (callback != null)
				{
					callback(this);
				}
			}
		}, null);
	}

	// Token: 0x06004905 RID: 18693 RVA: 0x0015CD8C File Offset: 0x0015AF8C
	public void HideDeckBox(bool immediate = false, TraySection.DelOnDoorStateChangedCallback callback = null)
	{
		this.m_deckBoxShown = false;
		this.CloseDoor(immediate, delegate(object _1)
		{
			if (this.m_showDoor)
			{
				this.m_door.gameObject.SetActive(true);
			}
			if (this.m_deckBox != null)
			{
				this.m_deckBox.PlayPopDownAnimation(delegate(object _2)
				{
					this.m_deckBox.Hide();
					if (callback != null)
					{
						callback(this);
					}
				});
			}
			else if (callback != null)
			{
				callback(this);
			}
		}, null);
	}

	// Token: 0x06004906 RID: 18694 RVA: 0x0015CDC8 File Offset: 0x0015AFC8
	public void MoveDeckBoxToEditPosition(Vector3 position, float time, TraySection.DelOnDoorStateChangedCallback callback = null)
	{
		if (this.m_deckBox == null)
		{
			return;
		}
		if (this.m_intermediateDeckBoxLocalPos != null)
		{
			Debug.LogWarning("Unable to move deck box to edit position. It has already been moved!", base.gameObject);
			return;
		}
		this.m_deckBox.DisableButtonAnimation();
		this.m_door.gameObject.SetActive(true);
		this.CloseDoor();
		this.m_deckBox.PlayScaleUpAnimation(delegate(object _1)
		{
			this.m_intermediateDeckBoxLocalPos = new Vector3?(this.m_deckBox.transform.localPosition);
			iTween.MoveTo(this.m_deckBox.gameObject, iTween.Hash(new object[]
			{
				"position",
				position,
				"islocal",
				false,
				"time",
				time,
				"easetype",
				iTween.EaseType.linear,
				"oncomplete",
				delegate(object _2)
				{
					if (callback != null)
					{
						callback(this);
					}
				}
			}));
		});
	}

	// Token: 0x06004907 RID: 18695 RVA: 0x0015CE64 File Offset: 0x0015B064
	public void MoveDeckBoxBackToOriginalPosition(float time, TraySection.DelOnDoorStateChangedCallback callback = null)
	{
		if (this.m_deckBox == null)
		{
			return;
		}
		if (this.m_intermediateDeckBoxLocalPos == null)
		{
			Debug.LogWarning("Unable to move deck box to original position. It has not been moved!", base.gameObject);
			return;
		}
		this.OpenDoor(delegate(object _1)
		{
			this.m_door.gameObject.SetActive(false);
		});
		iTween.MoveTo(this.m_deckBox.gameObject, iTween.Hash(new object[]
		{
			"position",
			this.m_intermediateDeckBoxLocalPos.Value,
			"islocal",
			true,
			"time",
			time,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			delegate(object _1)
			{
				this.m_intermediateDeckBoxLocalPos = default(Vector3?);
				this.m_deckBox.PlayScaleDownAnimation(delegate(object _2)
				{
					if (callback != null)
					{
						callback(this);
					}
					this.m_deckBox.EnableButtonAnimation();
					this.m_door.gameObject.SetActive(false);
				});
			}
		}));
	}

	// Token: 0x06004908 RID: 18696 RVA: 0x0015CF50 File Offset: 0x0015B150
	public void FlipDeckBoxHalfOverToShow(float animTime, TraySection.DelOnDoorStateChangedCallback callback = null)
	{
		this.m_deckBox.gameObject.SetActive(true);
		this.m_deckBox.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		SoundManager.Get().LoadAndPlay("collection_manager_new_deck_edge_flips", base.gameObject);
		iTween.StopByName(this.m_deckBox.gameObject, "rotation");
		iTween.RotateTo(this.m_deckBox.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			TraySection.DECKBOX_LOCAL_EULER_ANGLES,
			"isLocal",
			true,
			"time",
			animTime,
			"easeType",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			delegate(object _1)
			{
				if (callback != null)
				{
					callback(this);
				}
			},
			"name",
			"rotation"
		}));
	}

	// Token: 0x06004909 RID: 18697 RVA: 0x0015D05E File Offset: 0x0015B25E
	public void ClearDeckInfo()
	{
		if (this.m_deckBox == null)
		{
			return;
		}
		this.m_deckBox.SetDeckName(string.Empty);
		this.m_deckBox.SetDeckID(-1L);
	}

	// Token: 0x0600490A RID: 18698 RVA: 0x0015D090 File Offset: 0x0015B290
	public bool HideIfNotInBounds(Bounds bounds)
	{
		UIBScrollableItem component = base.GetComponent<UIBScrollableItem>();
		if (component == null)
		{
			Debug.LogWarning("UIBScrollableItem not found on a TraySection! This section may not be hidden properly while entering or exiting Collection Manager!");
			return false;
		}
		Bounds bounds2 = default(Bounds);
		Vector3 vector;
		Vector3 vector2;
		component.GetWorldBounds(out vector, out vector2);
		bounds2.SetMinMax(vector, vector2);
		if (!bounds.Intersects(bounds2))
		{
			base.gameObject.SetActive(false);
			return true;
		}
		return false;
	}

	// Token: 0x0600490B RID: 18699 RVA: 0x0015D0F4 File Offset: 0x0015B2F4
	private void Awake()
	{
		if (this.m_deckBox != null)
		{
			this.m_deckBox.transform.localPosition = CollectionDeckBoxVisual.POPPED_DOWN_LOCAL_POS;
			this.m_deckBox.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
			this.m_deckBox.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		}
		this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
		UIBScrollableItem component = base.GetComponent<UIBScrollableItem>();
		if (component != null)
		{
			component.SetCustomActiveState(new UIBScrollableItem.ActiveStateCallback(this.IsDeckBoxShown));
		}
	}

	// Token: 0x0600490C RID: 18700 RVA: 0x0015D1A4 File Offset: 0x0015B3A4
	private void Update()
	{
		if (this.m_wasTouchModeEnabled != UniversalInputManager.Get().IsTouchMode())
		{
			this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
		}
	}

	// Token: 0x0600490D RID: 18701 RVA: 0x0015D1CC File Offset: 0x0015B3CC
	private void OpenDoor(bool isImmediate, TraySection.DelOnDoorStateChangedCallback callback, object callbackData)
	{
		if (this.m_isOpen)
		{
			if (callback != null)
			{
				callback(callbackData);
			}
			return;
		}
		this.m_isOpen = true;
		this.m_door.GetComponent<Animation>()[this.DOOR_OPEN_ANIM_NAME].time = ((!isImmediate) ? 0f : this.m_door.GetComponent<Animation>()[this.DOOR_OPEN_ANIM_NAME].length);
		this.m_door.GetComponent<Animation>()[this.DOOR_OPEN_ANIM_NAME].speed = 6f;
		this.PlayDoorAnimation(this.DOOR_OPEN_ANIM_NAME, callback, callbackData);
	}

	// Token: 0x0600490E RID: 18702 RVA: 0x0015D270 File Offset: 0x0015B470
	private void CloseDoor(bool isImmediate, TraySection.DelOnDoorStateChangedCallback callback, object callbackData)
	{
		if (!this.m_isOpen)
		{
			if (callback != null)
			{
				callback(callbackData);
			}
			return;
		}
		this.m_isOpen = false;
		this.m_door.GetComponent<Animation>()[this.DOOR_CLOSE_ANIM_NAME].time = ((!isImmediate) ? 0f : this.m_door.GetComponent<Animation>()[this.DOOR_CLOSE_ANIM_NAME].length);
		this.m_door.GetComponent<Animation>()[this.DOOR_CLOSE_ANIM_NAME].speed = 6f;
		this.PlayDoorAnimation(this.DOOR_CLOSE_ANIM_NAME, callback, callbackData);
	}

	// Token: 0x0600490F RID: 18703 RVA: 0x0015D314 File Offset: 0x0015B514
	private void PlayDoorAnimation(string animationName, TraySection.DelOnDoorStateChangedCallback callback, object callbackData)
	{
		this.m_door.GetComponent<Animation>().Play(animationName);
		TraySection.OnDoorStateChangedCallbackData onDoorStateChangedCallbackData = new TraySection.OnDoorStateChangedCallbackData
		{
			m_callback = callback,
			m_callbackData = callbackData,
			m_animationName = animationName
		};
		base.StopCoroutine("WaitThenCallDoorAnimationCallback");
		base.StartCoroutine("WaitThenCallDoorAnimationCallback", onDoorStateChangedCallbackData);
	}

	// Token: 0x06004910 RID: 18704 RVA: 0x0015D368 File Offset: 0x0015B568
	private IEnumerator WaitThenCallDoorAnimationCallback(TraySection.OnDoorStateChangedCallbackData callbackData)
	{
		if (callbackData.m_callback == null)
		{
			yield break;
		}
		yield return new WaitForSeconds(this.m_door.GetComponent<Animation>()[callbackData.m_animationName].length / this.m_door.GetComponent<Animation>()[callbackData.m_animationName].speed);
		callbackData.m_callback(callbackData.m_callbackData);
		yield break;
	}

	// Token: 0x04003023 RID: 12323
	private const float DOOR_ANIM_SPEED = 6f;

	// Token: 0x04003024 RID: 12324
	public GameObject m_door;

	// Token: 0x04003025 RID: 12325
	public CollectionDeckBoxVisual m_deckBox;

	// Token: 0x04003026 RID: 12326
	public Animator m_deckFX;

	// Token: 0x04003027 RID: 12327
	private readonly string DOOR_OPEN_ANIM_NAME = "Deck_DoorOpen";

	// Token: 0x04003028 RID: 12328
	private readonly string DOOR_CLOSE_ANIM_NAME = "Deck_DoorClose";

	// Token: 0x04003029 RID: 12329
	private bool m_isOpen;

	// Token: 0x0400302A RID: 12330
	private Vector3? m_intermediateDeckBoxLocalPos;

	// Token: 0x0400302B RID: 12331
	private bool m_wasTouchModeEnabled;

	// Token: 0x0400302C RID: 12332
	private bool m_deckBoxShown;

	// Token: 0x0400302D RID: 12333
	private bool m_showDoor = true;

	// Token: 0x0400302E RID: 12334
	private static readonly Vector3 DECKBOX_LOCAL_EULER_ANGLES = new Vector3(90f, 180f, 0f);

	// Token: 0x02000752 RID: 1874
	// (Invoke) Token: 0x06004B8C RID: 19340
	public delegate void DelOnDoorStateChangedCallback(object callbackData);

	// Token: 0x02000771 RID: 1905
	private class OnDoorStateChangedCallbackData
	{
		// Token: 0x0400330D RID: 13069
		public TraySection.DelOnDoorStateChangedCallback m_callback;

		// Token: 0x0400330E RID: 13070
		public object m_callbackData;

		// Token: 0x0400330F RID: 13071
		public string m_animationName;
	}
}
