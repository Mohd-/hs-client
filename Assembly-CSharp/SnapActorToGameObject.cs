using System;
using UnityEngine;

// Token: 0x02000F40 RID: 3904
public class SnapActorToGameObject : MonoBehaviour
{
	// Token: 0x060073FF RID: 29695 RVA: 0x002225AC File Offset: 0x002207AC
	private void Start()
	{
		Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(base.gameObject);
		if (actor == null)
		{
			Spell spell = SceneUtils.FindComponentInParents<Spell>(base.gameObject);
			if (spell != null)
			{
				actor = spell.GetSourceCard().GetActor();
			}
		}
		if (actor == null)
		{
			Debug.LogError(string.Format("SnapActorToGameObject on {0} failed to find Actor object!", base.gameObject.name));
			base.enabled = false;
			return;
		}
		this.m_actorTransform = actor.transform;
	}

	// Token: 0x06007400 RID: 29696 RVA: 0x00222630 File Offset: 0x00220830
	private void OnEnable()
	{
		if (this.m_actorTransform == null)
		{
			return;
		}
		this.m_OrgPosition = this.m_actorTransform.localPosition;
		this.m_OrgRotation = this.m_actorTransform.localRotation;
		this.m_OrgScale = this.m_actorTransform.localScale;
	}

	// Token: 0x06007401 RID: 29697 RVA: 0x00222684 File Offset: 0x00220884
	private void OnDisable()
	{
		if (this.m_actorTransform == null || !this.m_ResetTransformOnDisable)
		{
			return;
		}
		this.m_actorTransform.localPosition = this.m_OrgPosition;
		this.m_actorTransform.localRotation = this.m_OrgRotation;
		this.m_actorTransform.localScale = this.m_OrgScale;
	}

	// Token: 0x06007402 RID: 29698 RVA: 0x002226E4 File Offset: 0x002208E4
	private void LateUpdate()
	{
		if (this.m_actorTransform == null)
		{
			return;
		}
		if (this.m_SnapPostion)
		{
			this.m_actorTransform.position = base.transform.position;
		}
		if (this.m_SnapRotation)
		{
			this.m_actorTransform.rotation = base.transform.rotation;
		}
		if (this.m_SnapScale)
		{
			TransformUtil.SetWorldScale(this.m_actorTransform, base.transform.lossyScale);
		}
	}

	// Token: 0x04005E8F RID: 24207
	public bool m_SnapPostion = true;

	// Token: 0x04005E90 RID: 24208
	public bool m_SnapRotation = true;

	// Token: 0x04005E91 RID: 24209
	public bool m_SnapScale = true;

	// Token: 0x04005E92 RID: 24210
	public bool m_ResetTransformOnDisable;

	// Token: 0x04005E93 RID: 24211
	private Transform m_actorTransform;

	// Token: 0x04005E94 RID: 24212
	private Vector3 m_OrgPosition;

	// Token: 0x04005E95 RID: 24213
	private Quaternion m_OrgRotation;

	// Token: 0x04005E96 RID: 24214
	private Vector3 m_OrgScale;
}
