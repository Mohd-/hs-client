using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000787 RID: 1927
[CustomEditClass]
public class UIBObjectSpacing : MonoBehaviour
{
	// Token: 0x17000553 RID: 1363
	// (get) Token: 0x06004C80 RID: 19584 RVA: 0x0016C19D File Offset: 0x0016A39D
	// (set) Token: 0x06004C81 RID: 19585 RVA: 0x0016C1A5 File Offset: 0x0016A3A5
	public Vector3 LocalOffset
	{
		get
		{
			return this.m_LocalOffset;
		}
		set
		{
			this.m_LocalOffset = value;
			this.UpdatePositions();
		}
	}

	// Token: 0x17000554 RID: 1364
	// (get) Token: 0x06004C82 RID: 19586 RVA: 0x0016C1B4 File Offset: 0x0016A3B4
	// (set) Token: 0x06004C83 RID: 19587 RVA: 0x0016C1BC File Offset: 0x0016A3BC
	public Vector3 LocalSpacing
	{
		get
		{
			return this.m_LocalSpacing;
		}
		set
		{
			this.m_LocalSpacing = value;
			this.UpdatePositions();
		}
	}

	// Token: 0x17000555 RID: 1365
	// (get) Token: 0x06004C84 RID: 19588 RVA: 0x0016C1CB File Offset: 0x0016A3CB
	// (set) Token: 0x06004C85 RID: 19589 RVA: 0x0016C1D4 File Offset: 0x0016A3D4
	[CustomEditField(Range = "0 - 1")]
	public Vector3 Alignment
	{
		get
		{
			return this.m_Alignment;
		}
		set
		{
			this.m_Alignment = value;
			this.m_Alignment.x = Mathf.Clamp01(this.m_Alignment.x);
			this.m_Alignment.y = Mathf.Clamp01(this.m_Alignment.y);
			this.m_Alignment.z = Mathf.Clamp01(this.m_Alignment.z);
			this.UpdatePositions();
		}
	}

	// Token: 0x06004C86 RID: 19590 RVA: 0x0016C240 File Offset: 0x0016A440
	public void AddSpace(int index)
	{
		this.m_Objects.Insert(index, new UIBObjectSpacing.SpacedObject
		{
			m_CountIfNull = true
		});
	}

	// Token: 0x06004C87 RID: 19591 RVA: 0x0016C267 File Offset: 0x0016A467
	public void AddObject(GameObject obj, bool countIfNull = true)
	{
		this.AddObject(obj, Vector3.zero, countIfNull);
	}

	// Token: 0x06004C88 RID: 19592 RVA: 0x0016C276 File Offset: 0x0016A476
	public void AddObject(Component comp, bool countIfNull = true)
	{
		this.AddObject(comp, Vector3.zero, countIfNull);
	}

	// Token: 0x06004C89 RID: 19593 RVA: 0x0016C285 File Offset: 0x0016A485
	public void AddObject(Component comp, Vector3 offset, bool countIfNull = true)
	{
		this.AddObject(comp.gameObject, offset, countIfNull);
	}

	// Token: 0x06004C8A RID: 19594 RVA: 0x0016C298 File Offset: 0x0016A498
	public void AddObject(GameObject obj, Vector3 offset, bool countIfNull = true)
	{
		this.m_Objects.Add(new UIBObjectSpacing.SpacedObject
		{
			m_Object = obj,
			m_Offset = offset,
			m_CountIfNull = countIfNull
		});
	}

	// Token: 0x06004C8B RID: 19595 RVA: 0x0016C2CC File Offset: 0x0016A4CC
	public void ClearObjects()
	{
		this.m_Objects.Clear();
	}

	// Token: 0x06004C8C RID: 19596 RVA: 0x0016C2DC File Offset: 0x0016A4DC
	public void AnimateUpdatePositions(float animTime, iTween.EaseType tweenType = iTween.EaseType.easeInOutQuad)
	{
		List<UIBObjectSpacing.AnimationPosition> list = new List<UIBObjectSpacing.AnimationPosition>();
		List<UIBObjectSpacing.SpacedObject> list2 = this.m_Objects.FindAll((UIBObjectSpacing.SpacedObject o) => o.m_CountIfNull || (o.m_Object != null && o.m_Object.activeInHierarchy));
		if (this.m_reverse)
		{
			list2.Reverse();
		}
		Vector3 vector = this.m_LocalOffset;
		Vector3 localSpacing = this.m_LocalSpacing;
		Vector3 vector2 = Vector3.zero;
		for (int i = 0; i < list2.Count; i++)
		{
			UIBObjectSpacing.SpacedObject spacedObject = list2[i];
			GameObject @object = spacedObject.m_Object;
			if (@object != null)
			{
				list.Add(new UIBObjectSpacing.AnimationPosition
				{
					m_targetPos = vector + spacedObject.m_Offset,
					m_object = @object
				});
			}
			Vector3 vector3 = spacedObject.m_Offset;
			if (i < list2.Count - 1)
			{
				vector3 += localSpacing;
			}
			vector += vector3;
			vector2 += vector3;
		}
		vector2.x *= this.m_Alignment.x;
		vector2.y *= this.m_Alignment.y;
		vector2.z *= this.m_Alignment.z;
		for (int j = 0; j < list.Count; j++)
		{
			UIBObjectSpacing.AnimationPosition animationPosition = list[j];
			iTween.MoveTo(animationPosition.m_object, iTween.Hash(new object[]
			{
				"position",
				animationPosition.m_targetPos - vector2,
				"islocal",
				true,
				"easetype",
				tweenType,
				"time",
				animTime
			}));
		}
	}

	// Token: 0x06004C8D RID: 19597 RVA: 0x0016C4B4 File Offset: 0x0016A6B4
	public void UpdatePositions()
	{
		List<UIBObjectSpacing.SpacedObject> list = this.m_Objects.FindAll((UIBObjectSpacing.SpacedObject o) => o.m_CountIfNull || (o.m_Object != null && o.m_Object.activeInHierarchy));
		if (this.m_reverse)
		{
			list.Reverse();
		}
		Vector3 vector = this.m_LocalOffset;
		Vector3 localSpacing = this.m_LocalSpacing;
		Vector3 vector2 = Vector3.zero;
		for (int i = 0; i < list.Count; i++)
		{
			UIBObjectSpacing.SpacedObject spacedObject = list[i];
			GameObject @object = spacedObject.m_Object;
			if (@object != null)
			{
				@object.transform.localPosition = vector + spacedObject.m_Offset;
			}
			Vector3 vector3 = spacedObject.m_Offset;
			if (i < list.Count - 1)
			{
				vector3 += localSpacing;
			}
			vector += vector3;
			vector2 += vector3;
		}
		vector2.x *= this.m_Alignment.x;
		vector2.y *= this.m_Alignment.y;
		vector2.z *= this.m_Alignment.z;
		for (int j = 0; j < list.Count; j++)
		{
			GameObject object2 = list[j].m_Object;
			if (object2 != null)
			{
				object2.transform.localPosition -= vector2;
			}
		}
	}

	// Token: 0x04003357 RID: 13143
	public List<UIBObjectSpacing.SpacedObject> m_Objects = new List<UIBObjectSpacing.SpacedObject>();

	// Token: 0x04003358 RID: 13144
	[SerializeField]
	private Vector3 m_LocalOffset;

	// Token: 0x04003359 RID: 13145
	[SerializeField]
	private Vector3 m_LocalSpacing;

	// Token: 0x0400335A RID: 13146
	[SerializeField]
	private Vector3 m_Alignment = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x0400335B RID: 13147
	public bool m_reverse;

	// Token: 0x020007C3 RID: 1987
	[Serializable]
	public class SpacedObject
	{
		// Token: 0x040034EC RID: 13548
		public GameObject m_Object;

		// Token: 0x040034ED RID: 13549
		public Vector3 m_Offset;

		// Token: 0x040034EE RID: 13550
		public bool m_CountIfNull;
	}

	// Token: 0x020007C4 RID: 1988
	private class AnimationPosition
	{
		// Token: 0x040034EF RID: 13551
		public Vector3 m_targetPos;

		// Token: 0x040034F0 RID: 13552
		public GameObject m_object;
	}
}
