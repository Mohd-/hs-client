using System;
using UnityEngine;

// Token: 0x02000F3A RID: 3898
[CustomEditClass]
public class SetParenttByName : MonoBehaviour
{
	// Token: 0x060073E5 RID: 29669 RVA: 0x00221DF0 File Offset: 0x0021FFF0
	private void Start()
	{
		if (string.IsNullOrEmpty(this.m_ParentName))
		{
			return;
		}
		GameObject gameObject = this.FindGameObject(this.m_ParentName);
		if (gameObject == null)
		{
			Log.Kyle.Print("SetParenttByName failed to locate parent object: {0}", new object[]
			{
				this.m_ParentName
			});
			return;
		}
		base.transform.parent = gameObject.transform;
	}

	// Token: 0x060073E6 RID: 29670 RVA: 0x00221E58 File Offset: 0x00220058
	private GameObject FindGameObject(string gameObjName)
	{
		if (gameObjName.get_Chars(0) != '/')
		{
			return GameObject.Find(gameObjName);
		}
		string[] array = gameObjName.Split(new char[]
		{
			'/'
		});
		return GameObject.Find(array[array.Length - 1]);
	}

	// Token: 0x04005E6D RID: 24173
	[CustomEditField(T = EditType.SCENE_OBJECT)]
	public string m_ParentName;
}
