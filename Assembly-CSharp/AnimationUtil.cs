using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class AnimationUtil : MonoBehaviour
{
	// Token: 0x0600134C RID: 4940 RVA: 0x00056054 File Offset: 0x00054254
	public static void ShowWithPunch(GameObject go, Vector3 startScale, Vector3 punchScale, Vector3 afterPunchScale, string callbackName = "", bool noFade = false, GameObject callbackGO = null, object callbackData = null, AnimationUtil.DelOnShownWithPunch onShowPunchCallback = null)
	{
		if (!noFade)
		{
			iTween.FadeTo(go, 1f, 0.25f);
		}
		go.transform.localScale = startScale;
		iTween.ScaleTo(go, iTween.Hash(new object[]
		{
			"scale",
			punchScale,
			"time",
			0.25f
		}));
		iTween.MoveTo(go, iTween.Hash(new object[]
		{
			"position",
			go.transform.position + new Vector3(0.02f, 0.02f, 0.02f),
			"time",
			1.5f
		}));
		AnimationUtil.PunchData callbackData2 = new AnimationUtil.PunchData
		{
			m_gameObject = go,
			m_scale = afterPunchScale,
			m_callbackName = callbackName,
			m_callbackGameObject = callbackGO,
			m_callbackData = callbackData,
			m_onShowPunchCallback = onShowPunchCallback
		};
		go.GetComponent<MonoBehaviour>().StartCoroutine(AnimationUtil.ShowPunchRoutine(callbackData2));
	}

	// Token: 0x0600134D RID: 4941 RVA: 0x00056160 File Offset: 0x00054360
	private static IEnumerator ShowPunchRoutine(AnimationUtil.PunchData callbackData)
	{
		yield return new WaitForSeconds(0.25f);
		AnimationUtil.ShowPunch(callbackData.m_gameObject, callbackData.m_scale, callbackData.m_callbackName, callbackData.m_callbackGameObject, callbackData.m_callbackData);
		if (callbackData.m_onShowPunchCallback != null)
		{
			callbackData.m_onShowPunchCallback(callbackData.m_callbackData);
		}
		yield break;
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x00056184 File Offset: 0x00054384
	public static void ShowPunch(GameObject go, Vector3 scale, string callbackName = "", GameObject callbackGO = null, object callbackData = null)
	{
		if (string.IsNullOrEmpty(callbackName))
		{
			iTween.ScaleTo(go, scale, 0.15f);
			return;
		}
		if (callbackGO == null)
		{
			callbackGO = go;
		}
		if (callbackData == null)
		{
			callbackData = new object();
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			scale,
			"time",
			0.15f,
			"oncomplete",
			callbackName,
			"oncompletetarget",
			callbackGO,
			"oncompleteparams",
			callbackData
		});
		iTween.ScaleTo(go, args);
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x00056228 File Offset: 0x00054428
	public static void GrowThenDrift(GameObject go, Vector3 origin, Vector3 driftOffset)
	{
		iTween.ScaleFrom(go, iTween.Hash(new object[]
		{
			"scale",
			Vector3.one * 0.05f,
			"time",
			0.15f,
			"easeType",
			iTween.EaseType.easeOutQuart
		}));
		iTween.MoveFrom(go, iTween.Hash(new object[]
		{
			"position",
			origin,
			"time",
			0.15f,
			"easeType",
			iTween.EaseType.easeOutQuart
		}));
		go.GetComponent<MonoBehaviour>().StartCoroutine(AnimationUtil.DriftAfterTween(go, 0.15f, driftOffset));
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x000562EC File Offset: 0x000544EC
	private static IEnumerator DriftAfterTween(GameObject go, float delayTime, Vector3 driftOffset)
	{
		yield return new WaitForSeconds(delayTime);
		AnimationUtil.DriftObject(go, driftOffset);
		yield break;
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x0005632C File Offset: 0x0005452C
	public static void FloatyPosition(GameObject go, Vector3 startPos, float localRadius, float loopTime)
	{
		Vector3[] array = new Vector3[]
		{
			startPos,
			startPos + new Vector3(localRadius, 0f, localRadius),
			startPos + new Vector3(localRadius * 2f, 0f, 0f),
			startPos + new Vector3(localRadius, 0f, -localRadius),
			startPos + Vector3.zero
		};
		iTween.StopByName("DriftingTween");
		iTween.MoveTo(go, iTween.Hash(new object[]
		{
			"name",
			"DriftingTween",
			"path",
			array,
			"time",
			loopTime,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.linear,
			"looptype",
			iTween.LoopType.loop,
			"movetopath",
			false
		}));
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x0005645C File Offset: 0x0005465C
	public static void FloatyPosition(GameObject go, float radius, float loopTime)
	{
		AnimationUtil.FloatyPosition(go, go.transform.localPosition, radius, loopTime);
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x0005647C File Offset: 0x0005467C
	public static void ScaleFade(GameObject go, Vector3 scale)
	{
		AnimationUtil.ScaleFade(go, scale, null);
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x00056488 File Offset: 0x00054688
	public static void ScaleFade(GameObject go, Vector3 scale, string callbackName)
	{
		iTween.FadeTo(go, 0f, 0.25f);
		Hashtable args;
		if (string.IsNullOrEmpty(callbackName))
		{
			args = iTween.Hash(new object[]
			{
				"scale",
				scale,
				"time",
				0.25f
			});
		}
		else
		{
			args = iTween.Hash(new object[]
			{
				"scale",
				scale,
				"time",
				0.25f,
				"oncomplete",
				callbackName,
				"oncompletetarget",
				go
			});
		}
		iTween.ScaleTo(go, args);
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x00056538 File Offset: 0x00054738
	public static int GetLayerIndexFromName(Animator animator, string layerName)
	{
		if (layerName == null)
		{
			return -1;
		}
		layerName = layerName.Trim();
		for (int i = 0; i < animator.layerCount; i++)
		{
			string text = animator.GetLayerName(i);
			if (text != null)
			{
				text = text.Trim();
				if (text.Equals(layerName, 5))
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x00056598 File Offset: 0x00054798
	public static void DriftObject(GameObject go, Vector3 driftOffset)
	{
		iTween.StopByName(go, "DRIFT_MOVE_OBJECT_ITWEEN");
		iTween.MoveBy(go, iTween.Hash(new object[]
		{
			"amount",
			driftOffset,
			"time",
			10f,
			"name",
			"DRIFT_MOVE_OBJECT_ITWEEN",
			"easeType",
			iTween.EaseType.easeOutQuart
		}));
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x00056608 File Offset: 0x00054808
	public static void FadeTexture(MeshRenderer mesh, float fromAlpha, float toAlpha, float fadeTime, float delay, AnimationUtil.DelOnFade onCompleteCallback = null)
	{
		iTween.StopByName(mesh.gameObject, "FADE_TEXTURE");
		Material logoMaterial = mesh.materials[0];
		Color currentColor = logoMaterial.GetColor("_Color");
		currentColor.a = fromAlpha;
		logoMaterial.SetColor("_Color", currentColor);
		Hashtable hashtable = iTween.Hash(new object[]
		{
			"from",
			fromAlpha,
			"to",
			toAlpha,
			"time",
			fadeTime,
			"onupdate",
			delegate(object val)
			{
				currentColor.a = (float)val;
				logoMaterial.SetColor("_Color", currentColor);
			},
			"name",
			"FADE_TEXTURE"
		});
		if (delay > 0f)
		{
			hashtable.Add("delay", delay);
		}
		if (onCompleteCallback != null)
		{
			hashtable.Add("oncomplete", delegate(object o)
			{
				onCompleteCallback();
			});
		}
		iTween.ValueTo(mesh.gameObject, hashtable);
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x0005672D File Offset: 0x0005492D
	public static void DelayedActivate(GameObject go, float time, bool activate)
	{
		go.GetComponent<MonoBehaviour>().StartCoroutine(AnimationUtil.DelayedActivation(go, time, activate));
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x00056744 File Offset: 0x00054944
	private static IEnumerator DelayedActivation(GameObject go, float time, bool activate)
	{
		yield return new WaitForSeconds(time);
		go.SetActive(activate);
		yield break;
	}

	// Token: 0x02000165 RID: 357
	// (Invoke) Token: 0x0600135B RID: 4955
	public delegate void DelOnShownWithPunch(object callbackData);

	// Token: 0x020003F1 RID: 1009
	// (Invoke) Token: 0x06003421 RID: 13345
	public delegate void DelOnFade();

	// Token: 0x02000433 RID: 1075
	private class PunchData
	{
		// Token: 0x0400219F RID: 8607
		public GameObject m_gameObject;

		// Token: 0x040021A0 RID: 8608
		public Vector3 m_scale;

		// Token: 0x040021A1 RID: 8609
		public string m_callbackName;

		// Token: 0x040021A2 RID: 8610
		public GameObject m_callbackGameObject;

		// Token: 0x040021A3 RID: 8611
		public object m_callbackData;

		// Token: 0x040021A4 RID: 8612
		public AnimationUtil.DelOnShownWithPunch m_onShowPunchCallback;
	}
}
