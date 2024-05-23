using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class iTween
{
	// Token: 0x060014BB RID: 5307 RVA: 0x0005B9A1 File Offset: 0x00059BA1
	public iTween(GameObject obj, Hashtable args)
	{
		this.gameObject = obj;
		this.tweenArguments = args;
	}

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x060014BD RID: 5309 RVA: 0x0005BA54 File Offset: 0x00059C54
	private Transform transform
	{
		get
		{
			return this.gameObject.transform;
		}
	}

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x060014BE RID: 5310 RVA: 0x0005BA61 File Offset: 0x00059C61
	private Renderer renderer
	{
		get
		{
			return this.gameObject.GetComponent<Renderer>();
		}
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x060014BF RID: 5311 RVA: 0x0005BA6E File Offset: 0x00059C6E
	private Light light
	{
		get
		{
			return this.gameObject.GetComponent<Light>();
		}
	}

	// Token: 0x17000313 RID: 787
	// (get) Token: 0x060014C0 RID: 5312 RVA: 0x0005BA7B File Offset: 0x00059C7B
	private AudioSource audio
	{
		get
		{
			return this.gameObject.GetComponent<AudioSource>();
		}
	}

	// Token: 0x17000314 RID: 788
	// (get) Token: 0x060014C1 RID: 5313 RVA: 0x0005BA88 File Offset: 0x00059C88
	private Rigidbody rigidbody
	{
		get
		{
			return this.gameObject.GetComponent<Rigidbody>();
		}
	}

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x060014C2 RID: 5314 RVA: 0x0005BA95 File Offset: 0x00059C95
	private GUITexture guiTexture
	{
		get
		{
			return this.gameObject.GetComponent<GUITexture>();
		}
	}

	// Token: 0x17000316 RID: 790
	// (get) Token: 0x060014C3 RID: 5315 RVA: 0x0005BAA2 File Offset: 0x00059CA2
	private GUIText guiText
	{
		get
		{
			return this.gameObject.GetComponent<GUIText>();
		}
	}

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x060014C4 RID: 5316 RVA: 0x0005BAAF File Offset: 0x00059CAF
	private bool activeInHierarchy
	{
		get
		{
			return this.enabled && !this.destroyed && this.gameObject != null && this.gameObject.activeInHierarchy;
		}
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x0005BAE6 File Offset: 0x00059CE6
	private Component GetComponent(Type t)
	{
		return this.gameObject.GetComponent(t);
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x0005BAF4 File Offset: 0x00059CF4
	public static void Init(GameObject target)
	{
		iTween.MoveBy(target, Vector3.zero, 0f);
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x0005BB08 File Offset: 0x00059D08
	public static void CameraFadeFrom(float amount, float time)
	{
		if (iTween.cameraFade)
		{
			iTween.CameraFadeFrom(iTween.Hash(new object[]
			{
				"amount",
				amount,
				"time",
				time
			}));
		}
		else
		{
			Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x0005BB65 File Offset: 0x00059D65
	public static void CameraFadeFrom(Hashtable args)
	{
		if (iTween.cameraFade)
		{
			iTween.ColorFrom(iTween.cameraFade, args);
		}
		else
		{
			Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
	}

	// Token: 0x060014C9 RID: 5321 RVA: 0x0005BB90 File Offset: 0x00059D90
	public static void CameraFadeTo(float amount, float time)
	{
		if (iTween.cameraFade)
		{
			iTween.CameraFadeTo(iTween.Hash(new object[]
			{
				"amount",
				amount,
				"time",
				time
			}));
		}
		else
		{
			Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x0005BBED File Offset: 0x00059DED
	public static void CameraFadeTo(Hashtable args)
	{
		if (iTween.cameraFade)
		{
			iTween.ColorTo(iTween.cameraFade, args);
		}
		else
		{
			Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
		}
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x0005BC18 File Offset: 0x00059E18
	public static void ValueTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (!args.Contains("onupdate") || !args.Contains("from") || !args.Contains("to"))
		{
			Debug.LogError("iTween Error: ValueTo() requires an 'onupdate' callback function and a 'from' and 'to' property.  The supplied 'onupdate' callback must accept a single argument that is the same type as the supplied 'from' and 'to' properties!");
			return;
		}
		args["type"] = "value";
		if (args["from"].GetType() == typeof(Vector2))
		{
			args["method"] = "vector2";
		}
		else if (args["from"].GetType() == typeof(Vector3))
		{
			args["method"] = "vector3";
		}
		else if (args["from"].GetType() == typeof(Rect))
		{
			args["method"] = "rect";
		}
		else if (args["from"].GetType() == typeof(float))
		{
			args["method"] = "float";
		}
		else
		{
			if (args["from"].GetType() != typeof(Color))
			{
				Debug.LogError("iTween Error: ValueTo() only works with interpolating Vector3s, Vector2s, floats, ints, Rects and Colors!");
				return;
			}
			args["method"] = "color";
		}
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		iTween.Launch(target, args);
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x0005BDB0 File Offset: 0x00059FB0
	public static void FadeFrom(GameObject target, float alpha, float time)
	{
		iTween.FadeFrom(target, iTween.Hash(new object[]
		{
			"alpha",
			alpha,
			"time",
			time
		}));
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x0005BDE5 File Offset: 0x00059FE5
	public static void FadeFrom(GameObject target, Hashtable args)
	{
		iTween.ColorFrom(target, args);
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x0005BDEE File Offset: 0x00059FEE
	public static void FadeTo(GameObject target, float alpha, float time)
	{
		iTween.FadeTo(target, iTween.Hash(new object[]
		{
			"alpha",
			alpha,
			"time",
			time
		}));
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x0005BE23 File Offset: 0x0005A023
	public static void FadeTo(GameObject target, Hashtable args)
	{
		iTween.ColorTo(target, args);
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x0005BE2C File Offset: 0x0005A02C
	public static void ColorFrom(GameObject target, Color color, float time)
	{
		iTween.ColorFrom(target, iTween.Hash(new object[]
		{
			"color",
			color,
			"time",
			time
		}));
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x0005BE64 File Offset: 0x0005A064
	public static void ColorFrom(GameObject target, Hashtable args)
	{
		Color color = default(Color);
		Color color2 = default(Color);
		args = iTween.CleanArgs(args);
		if (!args.Contains("includechildren") || (bool)args["includechildren"])
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				Hashtable hashtable = (Hashtable)args.Clone();
				hashtable["ischild"] = true;
				iTween.ColorFrom(transform.gameObject, hashtable);
			}
		}
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		if (target.GetComponent(typeof(GUITexture)))
		{
			color = (color2 = target.GetComponent<GUITexture>().color);
		}
		else if (target.GetComponent(typeof(GUIText)))
		{
			color = (color2 = target.GetComponent<GUIText>().material.color);
		}
		else if (target.GetComponent<Renderer>())
		{
			color = (color2 = target.GetComponent<Renderer>().material.color);
		}
		else if (target.GetComponent<Light>())
		{
			color = (color2 = target.GetComponent<Light>().color);
		}
		if (args.Contains("color"))
		{
			color = (Color)args["color"];
		}
		else
		{
			if (args.Contains("r"))
			{
				color.r = (float)args["r"];
			}
			if (args.Contains("g"))
			{
				color.g = (float)args["g"];
			}
			if (args.Contains("b"))
			{
				color.b = (float)args["b"];
			}
			if (args.Contains("a"))
			{
				color.a = (float)args["a"];
			}
		}
		if (args.Contains("amount"))
		{
			color.a = (float)args["amount"];
			args.Remove("amount");
		}
		else if (args.Contains("alpha"))
		{
			color.a = (float)args["alpha"];
			args.Remove("alpha");
		}
		if (target.GetComponent(typeof(GUITexture)))
		{
			target.GetComponent<GUITexture>().color = color;
		}
		else if (target.GetComponent(typeof(GUIText)))
		{
			target.GetComponent<GUIText>().material.color = color;
		}
		else if (target.GetComponent<Renderer>())
		{
			target.GetComponent<Renderer>().material.color = color;
		}
		else if (target.GetComponent<Light>())
		{
			target.GetComponent<Light>().color = color;
		}
		args["color"] = color2;
		args["type"] = "color";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x0005C1F4 File Offset: 0x0005A3F4
	public static void ColorTo(GameObject target, Color color, float time)
	{
		iTween.ColorTo(target, iTween.Hash(new object[]
		{
			"color",
			color,
			"time",
			time
		}));
	}

	// Token: 0x060014D3 RID: 5331 RVA: 0x0005C22C File Offset: 0x0005A42C
	public static void ColorTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (!args.Contains("includechildren") || (bool)args["includechildren"])
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				Hashtable hashtable = (Hashtable)args.Clone();
				hashtable["ischild"] = true;
				iTween.ColorTo(transform.gameObject, hashtable);
			}
		}
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		args["type"] = "color";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014D4 RID: 5332 RVA: 0x0005C328 File Offset: 0x0005A528
	public static void AudioFrom(GameObject target, float volume, float pitch, float time)
	{
		iTween.AudioFrom(target, iTween.Hash(new object[]
		{
			"volume",
			volume,
			"pitch",
			pitch,
			"time",
			time
		}));
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x0005C37C File Offset: 0x0005A57C
	public static void AudioFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		AudioSource audioSource;
		if (args.Contains("audiosource"))
		{
			audioSource = (AudioSource)args["audiosource"];
		}
		else
		{
			if (!target.GetComponent(typeof(AudioSource)))
			{
				Debug.LogError("iTween Error: AudioFrom requires an AudioSource.");
				return;
			}
			audioSource = target.GetComponent<AudioSource>();
		}
		Vector2 vector;
		Vector2 vector2;
		vector.x = (vector2.x = audioSource.volume);
		vector.y = (vector2.y = audioSource.pitch);
		if (args.Contains("volume"))
		{
			vector2.x = (float)args["volume"];
		}
		if (args.Contains("pitch"))
		{
			vector2.y = (float)args["pitch"];
		}
		audioSource.volume = vector2.x;
		audioSource.pitch = vector2.y;
		args["volume"] = vector.x;
		args["pitch"] = vector.y;
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		args["type"] = "audio";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x0005C4F8 File Offset: 0x0005A6F8
	public static void AudioTo(GameObject target, float volume, float pitch, float time)
	{
		iTween.AudioTo(target, iTween.Hash(new object[]
		{
			"volume",
			volume,
			"pitch",
			pitch,
			"time",
			time
		}));
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x0005C54C File Offset: 0x0005A74C
	public static void AudioTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (!args.Contains("easetype"))
		{
			args.Add("easetype", iTween.EaseType.linear);
		}
		args["type"] = "audio";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x0005C5AC File Offset: 0x0005A7AC
	public static void Stab(GameObject target, AudioClip audioclip, float delay)
	{
		iTween.Stab(target, iTween.Hash(new object[]
		{
			"audioclip",
			audioclip,
			"delay",
			delay
		}));
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x0005C5E7 File Offset: 0x0005A7E7
	public static void Stab(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "stab";
		iTween.Launch(target, args);
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x0005C608 File Offset: 0x0005A808
	public static void LookFrom(GameObject target, Vector3 looktarget, float time)
	{
		iTween.LookFrom(target, iTween.Hash(new object[]
		{
			"looktarget",
			looktarget,
			"time",
			time
		}));
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x0005C640 File Offset: 0x0005A840
	public static void LookFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		Vector3 eulerAngles = target.transform.eulerAngles;
		if (args["looktarget"].GetType() == typeof(Transform))
		{
			Transform transform = target.transform;
			Transform transform2 = (Transform)args["looktarget"];
			Vector3? vector = (Vector3?)args["up"];
			transform.LookAt(transform2, (vector == null) ? iTween.Defaults.up : vector.Value);
		}
		else if (args["looktarget"].GetType() == typeof(Vector3))
		{
			Transform transform3 = target.transform;
			Vector3 vector2 = (Vector3)args["looktarget"];
			Vector3? vector3 = (Vector3?)args["up"];
			transform3.LookAt(vector2, (vector3 == null) ? iTween.Defaults.up : vector3.Value);
		}
		if (args.Contains("axis"))
		{
			Vector3 eulerAngles2 = target.transform.eulerAngles;
			string text = (string)args["axis"];
			if (text != null)
			{
				if (iTween.<>f__switch$map92 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
					dictionary.Add("x", 0);
					dictionary.Add("y", 1);
					dictionary.Add("z", 2);
					iTween.<>f__switch$map92 = dictionary;
				}
				int num;
				if (iTween.<>f__switch$map92.TryGetValue(text, ref num))
				{
					switch (num)
					{
					case 0:
						eulerAngles2.y = eulerAngles.y;
						eulerAngles2.z = eulerAngles.z;
						break;
					case 1:
						eulerAngles2.x = eulerAngles.x;
						eulerAngles2.z = eulerAngles.z;
						break;
					case 2:
						eulerAngles2.x = eulerAngles.x;
						eulerAngles2.y = eulerAngles.y;
						break;
					}
				}
			}
			target.transform.eulerAngles = eulerAngles2;
		}
		args["rotation"] = eulerAngles;
		args["type"] = "rotate";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x0005C87D File Offset: 0x0005AA7D
	public static void LookTo(GameObject target, Vector3 looktarget, float time)
	{
		iTween.LookTo(target, iTween.Hash(new object[]
		{
			"looktarget",
			looktarget,
			"time",
			time
		}));
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x0005C8B4 File Offset: 0x0005AAB4
	public static void LookTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("looktarget") && args["looktarget"].GetType() == typeof(Transform))
		{
			Transform transform = (Transform)args["looktarget"];
			args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
		}
		args["type"] = "look";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x0005C9B1 File Offset: 0x0005ABB1
	public static void MoveTo(GameObject target, Vector3 position, float time)
	{
		iTween.MoveTo(target, iTween.Hash(new object[]
		{
			"position",
			position,
			"time",
			time
		}));
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x0005C9E8 File Offset: 0x0005ABE8
	public static void MoveTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("position") && args["position"].GetType() == typeof(Transform))
		{
			Transform transform = (Transform)args["position"];
			args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
			args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		args["type"] = "move";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x0005CB27 File Offset: 0x0005AD27
	public static void MoveFrom(GameObject target, Vector3 position, float time)
	{
		iTween.MoveFrom(target, iTween.Hash(new object[]
		{
			"position",
			position,
			"time",
			time
		}));
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x0005CB5C File Offset: 0x0005AD5C
	public static void MoveFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		bool flag;
		if (args.Contains("islocal"))
		{
			flag = (bool)args["islocal"];
		}
		else
		{
			flag = iTween.Defaults.isLocal;
		}
		if (args.Contains("path"))
		{
			Vector3[] array2;
			if (args["path"].GetType() == typeof(Vector3[]))
			{
				Vector3[] array = (Vector3[])args["path"];
				array2 = new Vector3[array.Length];
				Array.Copy(array, array2, array.Length);
			}
			else
			{
				Transform[] array3 = (Transform[])args["path"];
				array2 = new Vector3[array3.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					array2[i] = array3[i].position;
				}
			}
			if (array2[array2.Length - 1] != target.transform.position)
			{
				Vector3[] array4 = new Vector3[array2.Length + 1];
				Array.Copy(array2, array4, array2.Length);
				if (flag)
				{
					array4[array4.Length - 1] = target.transform.localPosition;
					target.transform.localPosition = array4[0];
				}
				else
				{
					array4[array4.Length - 1] = target.transform.position;
					target.transform.position = array4[0];
				}
				args["path"] = array4;
			}
			else
			{
				if (flag)
				{
					target.transform.localPosition = array2[0];
				}
				else
				{
					target.transform.position = array2[0];
				}
				args["path"] = array2;
			}
		}
		else
		{
			Vector3 vector2;
			Vector3 vector;
			if (flag)
			{
				vector = (vector2 = target.transform.localPosition);
			}
			else
			{
				vector = (vector2 = target.transform.position);
			}
			if (args.Contains("position"))
			{
				if (args["position"].GetType() == typeof(Transform))
				{
					Transform transform = (Transform)args["position"];
					vector = transform.position;
				}
				else if (args["position"].GetType() == typeof(Vector3))
				{
					vector = (Vector3)args["position"];
				}
			}
			else
			{
				if (args.Contains("x"))
				{
					vector.x = (float)args["x"];
				}
				if (args.Contains("y"))
				{
					vector.y = (float)args["y"];
				}
				if (args.Contains("z"))
				{
					vector.z = (float)args["z"];
				}
			}
			if (flag)
			{
				target.transform.localPosition = vector;
			}
			else
			{
				target.transform.position = vector;
			}
			args["position"] = vector2;
		}
		args["type"] = "move";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x0005CEC8 File Offset: 0x0005B0C8
	public static void MoveAdd(GameObject target, Vector3 amount, float time)
	{
		iTween.MoveAdd(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x0005CEFD File Offset: 0x0005B0FD
	public static void MoveAdd(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "move";
		args["method"] = "add";
		iTween.Launch(target, args);
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x0005CF2E File Offset: 0x0005B12E
	public static void MoveBy(GameObject target, Vector3 amount, float time)
	{
		iTween.MoveBy(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x0005CF63 File Offset: 0x0005B163
	public static void MoveBy(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "move";
		args["method"] = "by";
		iTween.Launch(target, args);
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x0005CF94 File Offset: 0x0005B194
	public static void ScaleTo(GameObject target, Vector3 scale, float time)
	{
		iTween.ScaleTo(target, iTween.Hash(new object[]
		{
			"scale",
			scale,
			"time",
			time
		}));
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x0005CFCC File Offset: 0x0005B1CC
	public static void ScaleTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("scale") && args["scale"].GetType() == typeof(Transform))
		{
			Transform transform = (Transform)args["scale"];
			args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
			args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		args["type"] = "scale";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x0005D10B File Offset: 0x0005B30B
	public static void ScaleFrom(GameObject target, Vector3 scale, float time)
	{
		iTween.ScaleFrom(target, iTween.Hash(new object[]
		{
			"scale",
			scale,
			"time",
			time
		}));
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x0005D140 File Offset: 0x0005B340
	public static void ScaleFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		Vector3 localScale2;
		Vector3 localScale = localScale2 = target.transform.localScale;
		if (args.Contains("scale"))
		{
			if (args["scale"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["scale"];
				localScale = transform.localScale;
			}
			else if (args["scale"].GetType() == typeof(Vector3))
			{
				localScale = (Vector3)args["scale"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				localScale.x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				localScale.y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				localScale.z = (float)args["z"];
			}
		}
		target.transform.localScale = localScale;
		args["scale"] = localScale2;
		args["type"] = "scale";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x0005D29D File Offset: 0x0005B49D
	public static void ScaleAdd(GameObject target, Vector3 amount, float time)
	{
		iTween.ScaleAdd(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x0005D2D2 File Offset: 0x0005B4D2
	public static void ScaleAdd(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "scale";
		args["method"] = "add";
		iTween.Launch(target, args);
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x0005D303 File Offset: 0x0005B503
	public static void ScaleBy(GameObject target, Vector3 amount, float time)
	{
		iTween.ScaleBy(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x0005D338 File Offset: 0x0005B538
	public static void ScaleBy(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "scale";
		args["method"] = "by";
		iTween.Launch(target, args);
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x0005D369 File Offset: 0x0005B569
	public static void RotateTo(GameObject target, Vector3 rotation, float time)
	{
		iTween.RotateTo(target, iTween.Hash(new object[]
		{
			"rotation",
			rotation,
			"time",
			time
		}));
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x0005D3A0 File Offset: 0x0005B5A0
	public static void RotateTo(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		if (args.Contains("rotation") && args["rotation"].GetType() == typeof(Transform))
		{
			Transform transform = (Transform)args["rotation"];
			args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
			args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		args["type"] = "rotate";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x0005D4DF File Offset: 0x0005B6DF
	public static void RotateFrom(GameObject target, Vector3 rotation, float time)
	{
		iTween.RotateFrom(target, iTween.Hash(new object[]
		{
			"rotation",
			rotation,
			"time",
			time
		}));
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x0005D514 File Offset: 0x0005B714
	public static void RotateFrom(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		bool flag;
		if (args.Contains("islocal"))
		{
			flag = (bool)args["islocal"];
		}
		else
		{
			flag = iTween.Defaults.isLocal;
		}
		Vector3 vector2;
		Vector3 vector;
		if (flag)
		{
			vector = (vector2 = target.transform.localEulerAngles);
		}
		else
		{
			vector = (vector2 = target.transform.eulerAngles);
		}
		if (args.Contains("rotation"))
		{
			if (args["rotation"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["rotation"];
				vector = transform.eulerAngles;
			}
			else if (args["rotation"].GetType() == typeof(Vector3))
			{
				vector = (Vector3)args["rotation"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				vector.x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				vector.y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				vector.z = (float)args["z"];
			}
		}
		if (flag)
		{
			target.transform.localEulerAngles = vector;
		}
		else
		{
			target.transform.eulerAngles = vector;
		}
		args["rotation"] = vector2;
		args["type"] = "rotate";
		args["method"] = "to";
		iTween.Launch(target, args);
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x0005D6CD File Offset: 0x0005B8CD
	public static void RotateAdd(GameObject target, Vector3 amount, float time)
	{
		iTween.RotateAdd(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x0005D702 File Offset: 0x0005B902
	public static void RotateAdd(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "rotate";
		args["method"] = "add";
		iTween.Launch(target, args);
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x0005D733 File Offset: 0x0005B933
	public static void RotateBy(GameObject target, Vector3 amount, float time)
	{
		iTween.RotateBy(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x0005D768 File Offset: 0x0005B968
	public static void RotateBy(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "rotate";
		args["method"] = "by";
		iTween.Launch(target, args);
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x0005D799 File Offset: 0x0005B999
	public static void ShakePosition(GameObject target, Vector3 amount, float time)
	{
		iTween.ShakePosition(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x0005D7CE File Offset: 0x0005B9CE
	public static void ShakePosition(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "shake";
		args["method"] = "position";
		iTween.Launch(target, args);
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x0005D7FF File Offset: 0x0005B9FF
	public static void ShakeScale(GameObject target, Vector3 amount, float time)
	{
		iTween.ShakeScale(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x0005D834 File Offset: 0x0005BA34
	public static void ShakeScale(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "shake";
		args["method"] = "scale";
		iTween.Launch(target, args);
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x0005D865 File Offset: 0x0005BA65
	public static void ShakeRotation(GameObject target, Vector3 amount, float time)
	{
		iTween.ShakeRotation(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x0005D89A File Offset: 0x0005BA9A
	public static void ShakeRotation(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "shake";
		args["method"] = "rotation";
		iTween.Launch(target, args);
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x0005D8CB File Offset: 0x0005BACB
	public static void PunchPosition(GameObject target, Vector3 amount, float time)
	{
		iTween.PunchPosition(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x0005D900 File Offset: 0x0005BB00
	public static void PunchPosition(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "punch";
		args["method"] = "position";
		args["easetype"] = iTween.EaseType.punch;
		iTween.Launch(target, args);
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x0005D94E File Offset: 0x0005BB4E
	public static void PunchRotation(GameObject target, Vector3 amount, float time)
	{
		iTween.PunchRotation(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x0005D984 File Offset: 0x0005BB84
	public static void PunchRotation(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "punch";
		args["method"] = "rotation";
		args["easetype"] = iTween.EaseType.punch;
		iTween.Launch(target, args);
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x0005D9D2 File Offset: 0x0005BBD2
	public static void PunchScale(GameObject target, Vector3 amount, float time)
	{
		iTween.PunchScale(target, iTween.Hash(new object[]
		{
			"amount",
			amount,
			"time",
			time
		}));
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x0005DA08 File Offset: 0x0005BC08
	public static void PunchScale(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "punch";
		args["method"] = "scale";
		args["easetype"] = iTween.EaseType.punch;
		iTween.Launch(target, args);
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x0005DA56 File Offset: 0x0005BC56
	public static void Timer(GameObject target, Hashtable args)
	{
		args = iTween.CleanArgs(args);
		args["type"] = "timer";
		iTween.Launch(target, args);
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x0005DA78 File Offset: 0x0005BC78
	private void GenerateTargets()
	{
		string text = this.type;
		if (text != null)
		{
			if (iTween.<>f__switch$map9C == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(10);
				dictionary.Add("value", 0);
				dictionary.Add("color", 1);
				dictionary.Add("audio", 2);
				dictionary.Add("move", 3);
				dictionary.Add("scale", 4);
				dictionary.Add("rotate", 5);
				dictionary.Add("shake", 6);
				dictionary.Add("punch", 7);
				dictionary.Add("look", 8);
				dictionary.Add("stab", 9);
				iTween.<>f__switch$map9C = dictionary;
			}
			int num;
			if (iTween.<>f__switch$map9C.TryGetValue(text, ref num))
			{
				switch (num)
				{
				case 0:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map93 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
							dictionary.Add("float", 0);
							dictionary.Add("vector2", 1);
							dictionary.Add("vector3", 2);
							dictionary.Add("color", 3);
							dictionary.Add("rect", 4);
							iTween.<>f__switch$map93 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map93.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GenerateFloatTargets();
								this.apply = new iTween.ApplyTween(this.ApplyFloatTargets);
								break;
							case 1:
								this.GenerateVector2Targets();
								this.apply = new iTween.ApplyTween(this.ApplyVector2Targets);
								break;
							case 2:
								this.GenerateVector3Targets();
								this.apply = new iTween.ApplyTween(this.ApplyVector3Targets);
								break;
							case 3:
								this.GenerateColorTargets();
								this.apply = new iTween.ApplyTween(this.ApplyColorTargets);
								break;
							case 4:
								this.GenerateRectTargets();
								this.apply = new iTween.ApplyTween(this.ApplyRectTargets);
								break;
							}
						}
					}
					break;
				}
				case 1:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map94 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
							dictionary.Add("to", 0);
							iTween.<>f__switch$map94 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map94.TryGetValue(text2, ref num2))
						{
							if (num2 == 0)
							{
								this.GenerateColorToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyColorToTargets);
							}
						}
					}
					break;
				}
				case 2:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map95 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
							dictionary.Add("to", 0);
							iTween.<>f__switch$map95 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map95.TryGetValue(text2, ref num2))
						{
							if (num2 == 0)
							{
								this.GenerateAudioToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyAudioToTargets);
							}
						}
					}
					break;
				}
				case 3:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map96 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("to", 0);
							dictionary.Add("by", 1);
							dictionary.Add("add", 1);
							iTween.<>f__switch$map96 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map96.TryGetValue(text2, ref num2))
						{
							if (num2 != 0)
							{
								if (num2 == 1)
								{
									this.GenerateMoveByTargets();
									this.apply = new iTween.ApplyTween(this.ApplyMoveByTargets);
								}
							}
							else if (this.tweenArguments.Contains("path"))
							{
								this.GenerateMoveToPathTargets();
								this.apply = new iTween.ApplyTween(this.ApplyMoveToPathTargets);
							}
							else
							{
								this.GenerateMoveToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyMoveToTargets);
							}
						}
					}
					break;
				}
				case 4:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map97 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("to", 0);
							dictionary.Add("by", 1);
							dictionary.Add("add", 2);
							iTween.<>f__switch$map97 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map97.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GenerateScaleToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyScaleToTargets);
								break;
							case 1:
								this.GenerateScaleByTargets();
								this.apply = new iTween.ApplyTween(this.ApplyScaleToTargets);
								break;
							case 2:
								this.GenerateScaleAddTargets();
								this.apply = new iTween.ApplyTween(this.ApplyScaleToTargets);
								break;
							}
						}
					}
					break;
				}
				case 5:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map98 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("to", 0);
							dictionary.Add("add", 1);
							dictionary.Add("by", 2);
							iTween.<>f__switch$map98 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map98.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GenerateRotateToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyRotateToTargets);
								break;
							case 1:
								this.GenerateRotateAddTargets();
								this.apply = new iTween.ApplyTween(this.ApplyRotateAddTargets);
								break;
							case 2:
								this.GenerateRotateByTargets();
								this.apply = new iTween.ApplyTween(this.ApplyRotateAddTargets);
								break;
							}
						}
					}
					break;
				}
				case 6:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map99 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("position", 0);
							dictionary.Add("scale", 1);
							dictionary.Add("rotation", 2);
							iTween.<>f__switch$map99 = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map99.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GenerateShakePositionTargets();
								this.apply = new iTween.ApplyTween(this.ApplyShakePositionTargets);
								break;
							case 1:
								this.GenerateShakeScaleTargets();
								this.apply = new iTween.ApplyTween(this.ApplyShakeScaleTargets);
								break;
							case 2:
								this.GenerateShakeRotationTargets();
								this.apply = new iTween.ApplyTween(this.ApplyShakeRotationTargets);
								break;
							}
						}
					}
					break;
				}
				case 7:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map9A == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
							dictionary.Add("position", 0);
							dictionary.Add("rotation", 1);
							dictionary.Add("scale", 2);
							iTween.<>f__switch$map9A = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map9A.TryGetValue(text2, ref num2))
						{
							switch (num2)
							{
							case 0:
								this.GeneratePunchPositionTargets();
								this.apply = new iTween.ApplyTween(this.ApplyPunchPositionTargets);
								break;
							case 1:
								this.GeneratePunchRotationTargets();
								this.apply = new iTween.ApplyTween(this.ApplyPunchRotationTargets);
								break;
							case 2:
								this.GeneratePunchScaleTargets();
								this.apply = new iTween.ApplyTween(this.ApplyPunchScaleTargets);
								break;
							}
						}
					}
					break;
				}
				case 8:
				{
					string text2 = this.method;
					if (text2 != null)
					{
						if (iTween.<>f__switch$map9B == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
							dictionary.Add("to", 0);
							iTween.<>f__switch$map9B = dictionary;
						}
						int num2;
						if (iTween.<>f__switch$map9B.TryGetValue(text2, ref num2))
						{
							if (num2 == 0)
							{
								this.GenerateLookToTargets();
								this.apply = new iTween.ApplyTween(this.ApplyLookToTargets);
							}
						}
					}
					break;
				}
				case 9:
					this.GenerateStabTargets();
					this.apply = new iTween.ApplyTween(this.ApplyStabTargets);
					break;
				}
			}
		}
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x0005E214 File Offset: 0x0005C414
	private void GenerateRectTargets()
	{
		this.rects = new Rect[3];
		this.rects[0] = (Rect)this.tweenArguments["from"];
		this.rects[1] = (Rect)this.tweenArguments["to"];
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x0005E27C File Offset: 0x0005C47C
	private void GenerateColorTargets()
	{
		this.colors = new Color[1, 3];
		this.colors[0, 0] = (Color)this.tweenArguments["from"];
		this.colors[0, 1] = (Color)this.tweenArguments["to"];
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x0005E2DC File Offset: 0x0005C4DC
	private void GenerateVector3Targets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = (Vector3)this.tweenArguments["from"];
		this.vector3s[1] = (Vector3)this.tweenArguments["to"];
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x0005E3A0 File Offset: 0x0005C5A0
	private void GenerateVector2Targets()
	{
		this.vector2s = new Vector2[3];
		this.vector2s[0] = (Vector2)this.tweenArguments["from"];
		this.vector2s[1] = (Vector2)this.tweenArguments["to"];
		if (this.tweenArguments.Contains("speed"))
		{
			Vector3 vector;
			vector..ctor(this.vector2s[0].x, this.vector2s[0].y, 0f);
			Vector3 vector2;
			vector2..ctor(this.vector2s[1].x, this.vector2s[1].y, 0f);
			float num = Math.Abs(Vector3.Distance(vector, vector2));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x0005E4A0 File Offset: 0x0005C6A0
	private void GenerateFloatTargets()
	{
		this.floats = new float[3];
		this.floats[0] = (float)this.tweenArguments["from"];
		this.floats[1] = (float)this.tweenArguments["to"];
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(this.floats[0] - this.floats[1]);
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x0005E53C File Offset: 0x0005C73C
	private void GenerateColorToTargets()
	{
		if (this.GetComponent(typeof(GUITexture)))
		{
			this.colors = new Color[1, 3];
			this.colors[0, 0] = (this.colors[0, 1] = this.guiTexture.color);
		}
		else if (this.GetComponent(typeof(GUIText)))
		{
			this.colors = new Color[1, 3];
			this.colors[0, 0] = (this.colors[0, 1] = this.guiText.material.color);
		}
		else if (this.renderer)
		{
			int num = 0;
			this.colors = new Color[this.renderer.materials.Length, 3];
			for (int i = 0; i < this.renderer.materials.Length; i++)
			{
				if (this.renderer.materials[i].HasProperty(this.namedColorValueString))
				{
					this.colors[i, 0] = this.renderer.materials[i].GetColor(this.namedColorValueString);
					this.colors[i, 1] = this.renderer.materials[i].GetColor(this.namedColorValueString);
					num++;
				}
			}
		}
		else if (this.light)
		{
			this.colors = new Color[1, 3];
			this.colors[0, 0] = (this.colors[0, 1] = this.light.color);
		}
		else
		{
			this.colors = new Color[1, 3];
		}
		if (this.tweenArguments.Contains("color"))
		{
			for (int j = 0; j < this.colors.GetLength(0); j++)
			{
				this.colors[j, 1] = (Color)this.tweenArguments["color"];
			}
		}
		else
		{
			if (this.tweenArguments.Contains("r"))
			{
				for (int k = 0; k < this.colors.GetLength(0); k++)
				{
					this.colors[k, 1].r = (float)this.tweenArguments["r"];
				}
			}
			if (this.tweenArguments.Contains("g"))
			{
				for (int l = 0; l < this.colors.GetLength(0); l++)
				{
					this.colors[l, 1].g = (float)this.tweenArguments["g"];
				}
			}
			if (this.tweenArguments.Contains("b"))
			{
				for (int m = 0; m < this.colors.GetLength(0); m++)
				{
					this.colors[m, 1].b = (float)this.tweenArguments["b"];
				}
			}
			if (this.tweenArguments.Contains("a"))
			{
				for (int n = 0; n < this.colors.GetLength(0); n++)
				{
					this.colors[n, 1].a = (float)this.tweenArguments["a"];
				}
			}
		}
		if (this.tweenArguments.Contains("amount"))
		{
			for (int num2 = 0; num2 < this.colors.GetLength(0); num2++)
			{
				this.colors[num2, 1].a = (float)this.tweenArguments["amount"];
			}
		}
		else if (this.tweenArguments.Contains("alpha"))
		{
			for (int num3 = 0; num3 < this.colors.GetLength(0); num3++)
			{
				this.colors[num3, 1].a = (float)this.tweenArguments["alpha"];
			}
		}
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x0005E9A0 File Offset: 0x0005CBA0
	private void GenerateAudioToTargets()
	{
		this.vector2s = new Vector2[3];
		if (this.tweenArguments.Contains("audiosource"))
		{
			this.audioSource = (AudioSource)this.tweenArguments["audiosource"];
		}
		else if (this.GetComponent(typeof(AudioSource)))
		{
			this.audioSource = this.audio;
		}
		else
		{
			Debug.LogError("iTween Error: AudioTo requires an AudioSource.");
			this.Dispose();
		}
		this.vector2s[0] = (this.vector2s[1] = new Vector2(this.audioSource.volume, this.audioSource.pitch));
		if (this.tweenArguments.Contains("volume"))
		{
			this.vector2s[1].x = (float)this.tweenArguments["volume"];
		}
		if (this.tweenArguments.Contains("pitch"))
		{
			this.vector2s[1].y = (float)this.tweenArguments["pitch"];
		}
	}

	// Token: 0x0600150B RID: 5387 RVA: 0x0005EAE0 File Offset: 0x0005CCE0
	private void GenerateStabTargets()
	{
		if (this.tweenArguments.Contains("audiosource"))
		{
			this.audioSource = (AudioSource)this.tweenArguments["audiosource"];
		}
		else if (this.GetComponent(typeof(AudioSource)))
		{
			this.audioSource = this.audio;
		}
		else
		{
			this.gameObject.AddComponent(typeof(AudioSource));
			this.audioSource = this.audio;
			this.audioSource.playOnAwake = false;
		}
		this.audioSource.clip = (AudioClip)this.tweenArguments["audioclip"];
		if (this.tweenArguments.Contains("pitch"))
		{
			this.audioSource.pitch = (float)this.tweenArguments["pitch"];
		}
		if (this.tweenArguments.Contains("volume"))
		{
			this.audioSource.volume = (float)this.tweenArguments["volume"];
		}
		this.time = this.audioSource.clip.length / this.audioSource.pitch;
	}

	// Token: 0x0600150C RID: 5388 RVA: 0x0005EC28 File Offset: 0x0005CE28
	private void GenerateLookToTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = this.transform.eulerAngles;
		if (this.tweenArguments.Contains("looktarget"))
		{
			if (this.tweenArguments["looktarget"].GetType() == typeof(Transform))
			{
				Transform transform = this.transform;
				Transform transform2 = (Transform)this.tweenArguments["looktarget"];
				Vector3? vector = (Vector3?)this.tweenArguments["up"];
				transform.LookAt(transform2, (vector == null) ? iTween.Defaults.up : vector.Value);
			}
			else if (this.tweenArguments["looktarget"].GetType() == typeof(Vector3))
			{
				Transform transform3 = this.transform;
				Vector3 vector2 = (Vector3)this.tweenArguments["looktarget"];
				Vector3? vector3 = (Vector3?)this.tweenArguments["up"];
				transform3.LookAt(vector2, (vector3 == null) ? iTween.Defaults.up : vector3.Value);
			}
		}
		else
		{
			Debug.LogError("iTween Error: LookTo needs a 'looktarget' property!");
			this.Dispose();
		}
		this.vector3s[1] = this.transform.eulerAngles;
		this.transform.eulerAngles = this.vector3s[0];
		if (this.tweenArguments.Contains("axis"))
		{
			string text = (string)this.tweenArguments["axis"];
			if (text != null)
			{
				if (iTween.<>f__switch$map9D == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
					dictionary.Add("x", 0);
					dictionary.Add("y", 1);
					dictionary.Add("z", 2);
					iTween.<>f__switch$map9D = dictionary;
				}
				int num;
				if (iTween.<>f__switch$map9D.TryGetValue(text, ref num))
				{
					switch (num)
					{
					case 0:
						this.vector3s[1].y = this.vector3s[0].y;
						this.vector3s[1].z = this.vector3s[0].z;
						break;
					case 1:
						this.vector3s[1].x = this.vector3s[0].x;
						this.vector3s[1].z = this.vector3s[0].z;
						break;
					case 2:
						this.vector3s[1].x = this.vector3s[0].x;
						this.vector3s[1].y = this.vector3s[0].y;
						break;
					}
				}
			}
		}
		this.vector3s[1] = new Vector3(this.clerp(this.vector3s[0].x, this.vector3s[1].x, 1f), this.clerp(this.vector3s[0].y, this.vector3s[1].y, 1f), this.clerp(this.vector3s[0].z, this.vector3s[1].z, 1f));
		if (this.tweenArguments.Contains("speed"))
		{
			float num2 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num2 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x0600150D RID: 5389 RVA: 0x0005F024 File Offset: 0x0005D224
	private void GenerateMoveToPathTargets()
	{
		Vector3[] array2;
		if (this.tweenArguments["path"].GetType() == typeof(Vector3[]))
		{
			Vector3[] array = (Vector3[])this.tweenArguments["path"];
			if (array.Length == 1)
			{
				Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				this.Dispose();
			}
			array2 = new Vector3[array.Length];
			Array.Copy(array, array2, array.Length);
		}
		else
		{
			Transform[] array3 = (Transform[])this.tweenArguments["path"];
			if (array3.Length == 1)
			{
				Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
				this.Dispose();
			}
			array2 = new Vector3[array3.Length];
			for (int i = 0; i < array3.Length; i++)
			{
				array2[i] = array3[i].position;
			}
		}
		bool flag;
		int num;
		if (this.transform.position != array2[0])
		{
			if (!this.tweenArguments.Contains("movetopath") || (bool)this.tweenArguments["movetopath"])
			{
				flag = true;
				num = 3;
			}
			else
			{
				flag = false;
				num = 2;
			}
		}
		else
		{
			flag = false;
			num = 2;
		}
		this.vector3s = new Vector3[array2.Length + num];
		if (flag)
		{
			this.vector3s[1] = this.transform.position;
			num = 2;
		}
		else
		{
			num = 1;
		}
		Array.Copy(array2, 0, this.vector3s, num, array2.Length);
		this.vector3s[0] = this.vector3s[1] + (this.vector3s[1] - this.vector3s[2]);
		this.vector3s[this.vector3s.Length - 1] = this.vector3s[this.vector3s.Length - 2] + (this.vector3s[this.vector3s.Length - 2] - this.vector3s[this.vector3s.Length - 3]);
		if (this.vector3s[1] == this.vector3s[this.vector3s.Length - 2])
		{
			Vector3[] array4 = new Vector3[this.vector3s.Length];
			Array.Copy(this.vector3s, array4, this.vector3s.Length);
			array4[0] = array4[array4.Length - 3];
			array4[array4.Length - 1] = array4[2];
			this.vector3s = new Vector3[array4.Length];
			Array.Copy(array4, this.vector3s, array4.Length);
		}
		this.path = new iTween.CRSpline(this.vector3s);
		if (this.tweenArguments.Contains("speed"))
		{
			float num2 = iTween.PathLength(this.vector3s);
			this.time = num2 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x0005F384 File Offset: 0x0005D584
	private void GenerateMoveToTargets()
	{
		this.vector3s = new Vector3[3];
		if (this.isLocal)
		{
			this.vector3s[0] = (this.vector3s[1] = this.transform.localPosition);
		}
		else
		{
			this.vector3s[0] = (this.vector3s[1] = this.transform.position);
		}
		if (this.tweenArguments.Contains("position"))
		{
			if (this.tweenArguments["position"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)this.tweenArguments["position"];
				this.vector3s[1] = transform.position;
			}
			else if (this.tweenArguments["position"].GetType() == typeof(Vector3))
			{
				this.vector3s[1] = (Vector3)this.tweenArguments["position"];
			}
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("orienttopath") && (bool)this.tweenArguments["orienttopath"])
		{
			this.tweenArguments["looktarget"] = this.vector3s[1];
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x0005F62C File Offset: 0x0005D82C
	private void GenerateMoveByTargets()
	{
		this.vector3s = new Vector3[6];
		this.vector3s[4] = this.transform.eulerAngles;
		this.vector3s[0] = (this.vector3s[1] = (this.vector3s[3] = this.transform.position));
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = this.vector3s[0] + (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = this.vector3s[0].x + (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = this.vector3s[0].y + (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = this.vector3s[0].z + (float)this.tweenArguments["z"];
			}
		}
		this.transform.Translate(this.vector3s[1], this.space);
		this.vector3s[5] = this.transform.position;
		this.transform.position = this.vector3s[0];
		if (this.tweenArguments.Contains("orienttopath") && (bool)this.tweenArguments["orienttopath"])
		{
			this.tweenArguments["looktarget"] = this.vector3s[1];
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x0005F8F0 File Offset: 0x0005DAF0
	private void GenerateScaleToTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = (this.vector3s[1] = this.transform.localScale);
		if (this.tweenArguments.Contains("scale"))
		{
			if (this.tweenArguments["scale"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)this.tweenArguments["scale"];
				this.vector3s[1] = transform.localScale;
			}
			else if (this.tweenArguments["scale"].GetType() == typeof(Vector3))
			{
				this.vector3s[1] = (Vector3)this.tweenArguments["scale"];
			}
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x0005FB04 File Offset: 0x0005DD04
	private void GenerateScaleByTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = (this.vector3s[1] = this.transform.localScale);
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = Vector3.Scale(this.vector3s[1], (Vector3)this.tweenArguments["amount"]);
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				Vector3[] array = this.vector3s;
				int num = 1;
				array[num].x = array[num].x * (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				Vector3[] array2 = this.vector3s;
				int num2 = 1;
				array2[num2].y = array2[num2].y * (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				Vector3[] array3 = this.vector3s;
				int num3 = 1;
				array3[num3].z = array3[num3].z * (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num4 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num4 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001512 RID: 5394 RVA: 0x0005FCC8 File Offset: 0x0005DEC8
	private void GenerateScaleAddTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = (this.vector3s[1] = this.transform.localScale);
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] += (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				Vector3[] array = this.vector3s;
				int num = 1;
				array[num].x = array[num].x + (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				Vector3[] array2 = this.vector3s;
				int num2 = 1;
				array2[num2].y = array2[num2].y + (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				Vector3[] array3 = this.vector3s;
				int num3 = 1;
				array3[num3].z = array3[num3].z + (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num4 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num4 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x0005FE84 File Offset: 0x0005E084
	private void GenerateRotateToTargets()
	{
		this.vector3s = new Vector3[3];
		if (this.isLocal)
		{
			this.vector3s[0] = (this.vector3s[1] = this.transform.localEulerAngles);
		}
		else
		{
			this.vector3s[0] = (this.vector3s[1] = this.transform.eulerAngles);
		}
		if (this.tweenArguments.Contains("rotation"))
		{
			if (this.tweenArguments["rotation"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)this.tweenArguments["rotation"];
				this.vector3s[1] = transform.eulerAngles;
			}
			else if (this.tweenArguments["rotation"].GetType() == typeof(Vector3))
			{
				this.vector3s[1] = (Vector3)this.tweenArguments["rotation"];
			}
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
		this.vector3s[1] = new Vector3(this.clerp(this.vector3s[0].x, this.vector3s[1].x, 1f), this.clerp(this.vector3s[0].y, this.vector3s[1].y, 1f), this.clerp(this.vector3s[0].z, this.vector3s[1].z, 1f));
		if (this.tweenArguments.Contains("speed"))
		{
			float num = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x00060174 File Offset: 0x0005E374
	private void GenerateRotateAddTargets()
	{
		this.vector3s = new Vector3[5];
		this.vector3s[0] = (this.vector3s[1] = (this.vector3s[3] = this.transform.eulerAngles));
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] += (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				Vector3[] array = this.vector3s;
				int num = 1;
				array[num].x = array[num].x + (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				Vector3[] array2 = this.vector3s;
				int num2 = 1;
				array2[num2].y = array2[num2].y + (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				Vector3[] array3 = this.vector3s;
				int num3 = 1;
				array3[num3].z = array3[num3].z + (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num4 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num4 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x00060344 File Offset: 0x0005E544
	private void GenerateRotateByTargets()
	{
		this.vector3s = new Vector3[4];
		this.vector3s[0] = (this.vector3s[1] = (this.vector3s[3] = this.transform.eulerAngles));
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] += Vector3.Scale((Vector3)this.tweenArguments["amount"], new Vector3(360f, 360f, 360f));
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				Vector3[] array = this.vector3s;
				int num = 1;
				array[num].x = array[num].x + 360f * (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				Vector3[] array2 = this.vector3s;
				int num2 = 1;
				array2[num2].y = array2[num2].y + 360f * (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				Vector3[] array3 = this.vector3s;
				int num3 = 1;
				array3[num3].z = array3[num3].z + 360f * (float)this.tweenArguments["z"];
			}
		}
		if (this.tweenArguments.Contains("speed"))
		{
			float num4 = Math.Abs(Vector3.Distance(this.vector3s[0], this.vector3s[1]));
			this.time = num4 / (float)this.tweenArguments["speed"];
		}
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x0006053C File Offset: 0x0005E73C
	private void GenerateShakePositionTargets()
	{
		this.vector3s = new Vector3[4];
		this.vector3s[3] = this.transform.eulerAngles;
		if (this.isLocal)
		{
			this.vector3s[0] = this.transform.localPosition;
		}
		else
		{
			this.vector3s[0] = this.transform.position;
		}
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x000606AC File Offset: 0x0005E8AC
	private void GenerateShakeScaleTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = this.transform.localScale;
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x000607D4 File Offset: 0x0005E9D4
	private void GenerateShakeRotationTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = this.transform.eulerAngles;
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x000608FC File Offset: 0x0005EAFC
	private void GeneratePunchPositionTargets()
	{
		this.vector3s = new Vector3[5];
		this.vector3s[4] = this.transform.eulerAngles;
		this.vector3s[0] = this.transform.position;
		this.vector3s[1] = (this.vector3s[3] = Vector3.zero);
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x00060A68 File Offset: 0x0005EC68
	private void GeneratePunchRotationTargets()
	{
		this.vector3s = new Vector3[4];
		this.vector3s[0] = this.transform.eulerAngles;
		this.vector3s[1] = (this.vector3s[3] = Vector3.zero);
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x00060BB8 File Offset: 0x0005EDB8
	private void GeneratePunchScaleTargets()
	{
		this.vector3s = new Vector3[3];
		this.vector3s[0] = this.transform.localScale;
		this.vector3s[1] = Vector3.zero;
		if (this.tweenArguments.Contains("amount"))
		{
			this.vector3s[1] = (Vector3)this.tweenArguments["amount"];
		}
		else
		{
			if (this.tweenArguments.Contains("x"))
			{
				this.vector3s[1].x = (float)this.tweenArguments["x"];
			}
			if (this.tweenArguments.Contains("y"))
			{
				this.vector3s[1].y = (float)this.tweenArguments["y"];
			}
			if (this.tweenArguments.Contains("z"))
			{
				this.vector3s[1].z = (float)this.tweenArguments["z"];
			}
		}
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x00060CF4 File Offset: 0x0005EEF4
	private void ApplyRectTargets()
	{
		this.rects[2].x = this.ease(this.rects[0].x, this.rects[1].x, this.percentage);
		this.rects[2].y = this.ease(this.rects[0].y, this.rects[1].y, this.percentage);
		this.rects[2].width = this.ease(this.rects[0].width, this.rects[1].width, this.percentage);
		this.rects[2].height = this.ease(this.rects[0].height, this.rects[1].height, this.percentage);
		this.tweenArguments["onupdateparams"] = this.rects[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.rects[1];
		}
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x00060E70 File Offset: 0x0005F070
	private void ApplyColorTargets()
	{
		this.colors[0, 2].r = this.ease(this.colors[0, 0].r, this.colors[0, 1].r, this.percentage);
		this.colors[0, 2].g = this.ease(this.colors[0, 0].g, this.colors[0, 1].g, this.percentage);
		this.colors[0, 2].b = this.ease(this.colors[0, 0].b, this.colors[0, 1].b, this.percentage);
		this.colors[0, 2].a = this.ease(this.colors[0, 0].a, this.colors[0, 1].a, this.percentage);
		this.tweenArguments["onupdateparams"] = this.colors[0, 2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.colors[0, 1];
		}
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x00060FF0 File Offset: 0x0005F1F0
	private void ApplyVector3Targets()
	{
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		this.tweenArguments["onupdateparams"] = this.vector3s[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.vector3s[1];
		}
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x00061128 File Offset: 0x0005F328
	private void ApplyVector2Targets()
	{
		this.vector2s[2].x = this.ease(this.vector2s[0].x, this.vector2s[1].x, this.percentage);
		this.vector2s[2].y = this.ease(this.vector2s[0].y, this.vector2s[1].y, this.percentage);
		this.tweenArguments["onupdateparams"] = this.vector2s[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.vector2s[1];
		}
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x0006121C File Offset: 0x0005F41C
	private void ApplyFloatTargets()
	{
		this.floats[2] = this.ease(this.floats[0], this.floats[1], this.percentage);
		this.tweenArguments["onupdateparams"] = this.floats[2];
		if (this.percentage == 1f)
		{
			this.tweenArguments["onupdateparams"] = this.floats[1];
		}
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x0006129C File Offset: 0x0005F49C
	private void ApplyColorToTargets()
	{
		for (int i = 0; i < this.colors.GetLength(0); i++)
		{
			this.colors[i, 2].r = this.ease(this.colors[i, 0].r, this.colors[i, 1].r, this.percentage);
			this.colors[i, 2].g = this.ease(this.colors[i, 0].g, this.colors[i, 1].g, this.percentage);
			this.colors[i, 2].b = this.ease(this.colors[i, 0].b, this.colors[i, 1].b, this.percentage);
			this.colors[i, 2].a = this.ease(this.colors[i, 0].a, this.colors[i, 1].a, this.percentage);
		}
		if (this.GetComponent(typeof(GUITexture)))
		{
			this.guiTexture.color = this.colors[0, 2];
		}
		else if (this.GetComponent(typeof(GUIText)))
		{
			this.guiText.material.color = this.colors[0, 2];
		}
		else if (this.renderer)
		{
			for (int j = 0; j < this.colors.GetLength(0); j++)
			{
				this.renderer.materials[j].SetColor(this.namedColorValueString, this.colors[j, 2]);
			}
		}
		else if (this.light)
		{
			this.light.color = this.colors[0, 2];
		}
		if (this.percentage == 1f)
		{
			if (this.GetComponent(typeof(GUITexture)))
			{
				this.guiTexture.color = this.colors[0, 1];
			}
			else if (this.GetComponent(typeof(GUIText)))
			{
				this.guiText.material.color = this.colors[0, 1];
			}
			else if (this.renderer)
			{
				for (int k = 0; k < this.colors.GetLength(0); k++)
				{
					this.renderer.materials[k].SetColor(this.namedColorValueString, this.colors[k, 1]);
				}
			}
			else if (this.light)
			{
				this.light.color = this.colors[0, 1];
			}
		}
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x000615D8 File Offset: 0x0005F7D8
	private void ApplyAudioToTargets()
	{
		this.vector2s[2].x = this.ease(this.vector2s[0].x, this.vector2s[1].x, this.percentage);
		this.vector2s[2].y = this.ease(this.vector2s[0].y, this.vector2s[1].y, this.percentage);
		this.audioSource.volume = this.vector2s[2].x;
		this.audioSource.pitch = this.vector2s[2].y;
		if (this.percentage == 1f)
		{
			this.audioSource.volume = this.vector2s[1].x;
			this.audioSource.pitch = this.vector2s[1].y;
		}
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x000616ED File Offset: 0x0005F8ED
	private void ApplyStabTargets()
	{
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x000616F0 File Offset: 0x0005F8F0
	private void ApplyMoveToPathTargets()
	{
		this.preUpdate = this.transform.position;
		float num = this.ease(0f, 1f, this.percentage);
		if (this.isLocal)
		{
			this.transform.localPosition = this.path.Interp(Mathf.Clamp(num, 0f, 1f));
		}
		else
		{
			this.transform.position = this.path.Interp(Mathf.Clamp(num, 0f, 1f));
		}
		if (this.tweenArguments.Contains("orienttopath") && (bool)this.tweenArguments["orienttopath"])
		{
			float num2;
			if (this.tweenArguments.Contains("lookahead"))
			{
				num2 = (float)this.tweenArguments["lookahead"];
			}
			else
			{
				num2 = iTween.Defaults.lookAhead;
			}
			float num3 = this.ease(0f, 1f, Mathf.Min(1f, this.percentage + num2));
			this.tweenArguments["looktarget"] = this.path.Interp(Mathf.Clamp(num3, 0f, 1f));
		}
		this.postUpdate = this.transform.position;
		if (this.physics)
		{
			this.transform.position = this.preUpdate;
			this.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x00061884 File Offset: 0x0005FA84
	private void ApplyMoveToTargets()
	{
		this.preUpdate = this.transform.position;
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		if (this.isLocal)
		{
			this.transform.localPosition = this.vector3s[2];
		}
		else
		{
			this.transform.position = this.vector3s[2];
		}
		if (this.percentage == 1f)
		{
			if (this.isLocal)
			{
				this.transform.localPosition = this.vector3s[1];
			}
			else
			{
				this.transform.position = this.vector3s[1];
			}
		}
		this.postUpdate = this.transform.position;
		if (this.physics)
		{
			this.transform.position = this.preUpdate;
			this.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x00061A4C File Offset: 0x0005FC4C
	private void ApplyMoveByTargets()
	{
		this.preUpdate = this.transform.position;
		Vector3 eulerAngles = default(Vector3);
		if (this.tweenArguments.Contains("looktarget"))
		{
			eulerAngles = this.transform.eulerAngles;
			this.transform.eulerAngles = this.vector3s[4];
		}
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		this.transform.Translate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		if (this.tweenArguments.Contains("looktarget"))
		{
			this.transform.eulerAngles = eulerAngles;
		}
		this.postUpdate = this.transform.position;
		if (this.physics)
		{
			this.transform.position = this.preUpdate;
			this.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x00061C34 File Offset: 0x0005FE34
	private void ApplyScaleToTargets()
	{
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		this.transform.localScale = this.vector3s[2];
		if (this.percentage == 1f)
		{
			this.transform.localScale = this.vector3s[1];
		}
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x00061D58 File Offset: 0x0005FF58
	private void ApplyLookToTargets()
	{
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		if (this.isLocal)
		{
			this.transform.localRotation = Quaternion.Euler(this.vector3s[2]);
		}
		else
		{
			this.transform.rotation = Quaternion.Euler(this.vector3s[2]);
		}
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x00061E84 File Offset: 0x00060084
	private void ApplyRotateToTargets()
	{
		this.preUpdate = this.transform.eulerAngles;
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		if (this.isLocal)
		{
			this.transform.localRotation = Quaternion.Euler(this.vector3s[2]);
		}
		else
		{
			this.transform.rotation = Quaternion.Euler(this.vector3s[2]);
		}
		if (this.percentage == 1f)
		{
			if (this.isLocal)
			{
				this.transform.localRotation = Quaternion.Euler(this.vector3s[1]);
			}
			else
			{
				this.transform.rotation = Quaternion.Euler(this.vector3s[1]);
			}
		}
		this.postUpdate = this.transform.eulerAngles;
		if (this.physics)
		{
			this.transform.eulerAngles = this.preUpdate;
			this.rigidbody.MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x00062068 File Offset: 0x00060268
	private void ApplyRotateAddTargets()
	{
		this.preUpdate = this.transform.eulerAngles;
		this.vector3s[2].x = this.ease(this.vector3s[0].x, this.vector3s[1].x, this.percentage);
		this.vector3s[2].y = this.ease(this.vector3s[0].y, this.vector3s[1].y, this.percentage);
		this.vector3s[2].z = this.ease(this.vector3s[0].z, this.vector3s[1].z, this.percentage);
		this.transform.Rotate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		this.postUpdate = this.transform.eulerAngles;
		if (this.physics)
		{
			this.transform.eulerAngles = this.preUpdate;
			this.rigidbody.MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x000621F0 File Offset: 0x000603F0
	private void ApplyShakePositionTargets()
	{
		if (this.isLocal)
		{
			this.preUpdate = this.transform.localPosition;
		}
		else
		{
			this.preUpdate = this.transform.position;
		}
		Vector3 eulerAngles = default(Vector3);
		if (this.tweenArguments.Contains("looktarget"))
		{
			eulerAngles = this.transform.eulerAngles;
			this.transform.eulerAngles = this.vector3s[3];
		}
		float num = 1f - this.percentage;
		this.vector3s[2].x = Random.Range(-this.vector3s[1].x * num, this.vector3s[1].x * num);
		this.vector3s[2].y = Random.Range(-this.vector3s[1].y * num, this.vector3s[1].y * num);
		this.vector3s[2].z = Random.Range(-this.vector3s[1].z * num, this.vector3s[1].z * num);
		if (this.isLocal)
		{
			this.transform.localPosition = this.vector3s[0] + this.vector3s[2];
		}
		else
		{
			this.transform.position = this.vector3s[0] + this.vector3s[2];
		}
		if (this.tweenArguments.Contains("looktarget"))
		{
			this.transform.eulerAngles = eulerAngles;
		}
		this.postUpdate = this.transform.position;
		if (this.physics)
		{
			this.transform.position = this.preUpdate;
			this.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x0600152C RID: 5420 RVA: 0x0006240C File Offset: 0x0006060C
	private void ApplyShakeScaleTargets()
	{
		if (this.percentage == 0f)
		{
			this.transform.localScale = this.vector3s[1];
		}
		this.transform.localScale = this.vector3s[0];
		float num = 1f - this.percentage;
		this.vector3s[2].x = Random.Range(-this.vector3s[1].x * num, this.vector3s[1].x * num);
		this.vector3s[2].y = Random.Range(-this.vector3s[1].y * num, this.vector3s[1].y * num);
		this.vector3s[2].z = Random.Range(-this.vector3s[1].z * num, this.vector3s[1].z * num);
		this.transform.localScale += this.vector3s[2];
	}

	// Token: 0x0600152D RID: 5421 RVA: 0x0006254C File Offset: 0x0006074C
	private void ApplyShakeRotationTargets()
	{
		this.preUpdate = this.transform.eulerAngles;
		if (this.percentage == 0f)
		{
			this.transform.Rotate(this.vector3s[1], this.space);
		}
		this.transform.eulerAngles = this.vector3s[0];
		float num = 1f - this.percentage;
		this.vector3s[2].x = Random.Range(-this.vector3s[1].x * num, this.vector3s[1].x * num);
		this.vector3s[2].y = Random.Range(-this.vector3s[1].y * num, this.vector3s[1].y * num);
		this.vector3s[2].z = Random.Range(-this.vector3s[1].z * num, this.vector3s[1].z * num);
		this.transform.Rotate(this.vector3s[2], this.space);
		this.postUpdate = this.transform.eulerAngles;
		if (this.physics)
		{
			this.transform.eulerAngles = this.preUpdate;
			this.rigidbody.MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x000626E4 File Offset: 0x000608E4
	private void ApplyPunchPositionTargets()
	{
		this.preUpdate = this.transform.position;
		Vector3 eulerAngles = default(Vector3);
		if (this.tweenArguments.Contains("looktarget"))
		{
			eulerAngles = this.transform.eulerAngles;
			this.transform.eulerAngles = this.vector3s[4];
		}
		if (this.vector3s[1].x > 0f)
		{
			this.vector3s[2].x = this.punch(this.vector3s[1].x, this.percentage);
		}
		else if (this.vector3s[1].x < 0f)
		{
			this.vector3s[2].x = -this.punch(Mathf.Abs(this.vector3s[1].x), this.percentage);
		}
		if (this.vector3s[1].y > 0f)
		{
			this.vector3s[2].y = this.punch(this.vector3s[1].y, this.percentage);
		}
		else if (this.vector3s[1].y < 0f)
		{
			this.vector3s[2].y = -this.punch(Mathf.Abs(this.vector3s[1].y), this.percentage);
		}
		if (this.vector3s[1].z > 0f)
		{
			this.vector3s[2].z = this.punch(this.vector3s[1].z, this.percentage);
		}
		else if (this.vector3s[1].z < 0f)
		{
			this.vector3s[2].z = -this.punch(Mathf.Abs(this.vector3s[1].z), this.percentage);
		}
		this.transform.Translate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		if (this.tweenArguments.Contains("looktarget"))
		{
			this.transform.eulerAngles = eulerAngles;
		}
		this.postUpdate = this.transform.position;
		if (this.physics)
		{
			this.transform.position = this.preUpdate;
			this.rigidbody.MovePosition(this.postUpdate);
		}
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x000629D8 File Offset: 0x00060BD8
	private void ApplyPunchRotationTargets()
	{
		this.preUpdate = this.transform.eulerAngles;
		if (this.vector3s[1].x > 0f)
		{
			this.vector3s[2].x = this.punch(this.vector3s[1].x, this.percentage);
		}
		else if (this.vector3s[1].x < 0f)
		{
			this.vector3s[2].x = -this.punch(Mathf.Abs(this.vector3s[1].x), this.percentage);
		}
		if (this.vector3s[1].y > 0f)
		{
			this.vector3s[2].y = this.punch(this.vector3s[1].y, this.percentage);
		}
		else if (this.vector3s[1].y < 0f)
		{
			this.vector3s[2].y = -this.punch(Mathf.Abs(this.vector3s[1].y), this.percentage);
		}
		if (this.vector3s[1].z > 0f)
		{
			this.vector3s[2].z = this.punch(this.vector3s[1].z, this.percentage);
		}
		else if (this.vector3s[1].z < 0f)
		{
			this.vector3s[2].z = -this.punch(Mathf.Abs(this.vector3s[1].z), this.percentage);
		}
		this.transform.Rotate(this.vector3s[2] - this.vector3s[3], this.space);
		this.vector3s[3] = this.vector3s[2];
		this.postUpdate = this.transform.eulerAngles;
		if (this.physics)
		{
			this.transform.eulerAngles = this.preUpdate;
			this.rigidbody.MoveRotation(Quaternion.Euler(this.postUpdate));
		}
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x00062C6C File Offset: 0x00060E6C
	private void ApplyPunchScaleTargets()
	{
		if (this.vector3s[1].x > 0f)
		{
			this.vector3s[2].x = this.punch(this.vector3s[1].x, this.percentage);
		}
		else if (this.vector3s[1].x < 0f)
		{
			this.vector3s[2].x = -this.punch(Mathf.Abs(this.vector3s[1].x), this.percentage);
		}
		if (this.vector3s[1].y > 0f)
		{
			this.vector3s[2].y = this.punch(this.vector3s[1].y, this.percentage);
		}
		else if (this.vector3s[1].y < 0f)
		{
			this.vector3s[2].y = -this.punch(Mathf.Abs(this.vector3s[1].y), this.percentage);
		}
		if (this.vector3s[1].z > 0f)
		{
			this.vector3s[2].z = this.punch(this.vector3s[1].z, this.percentage);
		}
		else if (this.vector3s[1].z < 0f)
		{
			this.vector3s[2].z = -this.punch(Mathf.Abs(this.vector3s[1].z), this.percentage);
		}
		this.transform.localScale = this.vector3s[0] + this.vector3s[2];
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x00062E82 File Offset: 0x00061082
	private void ResetDelay()
	{
		this.delayStarted = Time.time;
		this.waitForDelay = true;
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x00062E98 File Offset: 0x00061098
	private void TweenStart()
	{
		if (this.tweenArguments == null)
		{
			return;
		}
		this.CallBack(iTween.CallbackType.OnStart);
		if (!this.loop)
		{
			this.ConflictCheck();
			this.GenerateTargets();
		}
		this.loop = true;
		if (this.type == "stab")
		{
			this.audioSource.PlayOneShot(this.audioSource.clip);
		}
		if (this.type == "move" || this.type == "scale" || this.type == "rotate" || this.type == "punch" || this.type == "shake" || this.type == "curve" || this.type == "look")
		{
			this.EnableKinematic();
		}
		this.isRunning = true;
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x00062FA1 File Offset: 0x000611A1
	private void TweenRestart()
	{
		this.ResetDelay();
		this.loop = true;
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x00062FB0 File Offset: 0x000611B0
	private void TweenUpdate()
	{
		if (this.type != "timer")
		{
			if (this.apply == null)
			{
				return;
			}
			this.apply();
		}
		this.CallBack(iTween.CallbackType.OnUpdate);
		this.UpdatePercentage();
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x00062FEC File Offset: 0x000611EC
	private void TweenComplete()
	{
		this.isRunning = false;
		if (this.percentage > 0.5f)
		{
			this.percentage = 1f;
		}
		else
		{
			this.percentage = 0f;
		}
		if (this.type != "timer")
		{
			this.apply();
		}
		if (this.type == "value" || this.type == "timer")
		{
			this.CallBack(iTween.CallbackType.OnUpdate);
		}
		if (this.loopType == iTween.LoopType.none)
		{
			this.Dispose();
		}
		else
		{
			this.TweenLoop();
		}
		this.CallBack(iTween.CallbackType.OnComplete);
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x000630A0 File Offset: 0x000612A0
	private void TweenLoop()
	{
		this.DisableKinematic();
		iTween.LoopType loopType = this.loopType;
		if (loopType != iTween.LoopType.loop)
		{
			if (loopType == iTween.LoopType.pingPong)
			{
				this.reverse = !this.reverse;
				this.runningTime = 0f;
				this.TweenRestart();
			}
		}
		else
		{
			this.percentage = 0f;
			this.runningTime = 0f;
			if (this.apply != null)
			{
				this.apply();
			}
			this.TweenRestart();
		}
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x0006312C File Offset: 0x0006132C
	public static Rect RectUpdate(Rect currentValue, Rect targetValue, float speed)
	{
		Rect result;
		result..ctor(iTween.FloatUpdate(currentValue.x, targetValue.x, speed), iTween.FloatUpdate(currentValue.y, targetValue.y, speed), iTween.FloatUpdate(currentValue.width, targetValue.width, speed), iTween.FloatUpdate(currentValue.height, targetValue.height, speed));
		return result;
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x00063194 File Offset: 0x00061394
	public static Vector3 Vector3Update(Vector3 currentValue, Vector3 targetValue, float speed)
	{
		Vector3 vector = targetValue - currentValue;
		currentValue += vector * speed * Time.deltaTime;
		return currentValue;
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x000631C4 File Offset: 0x000613C4
	public static Vector2 Vector2Update(Vector2 currentValue, Vector2 targetValue, float speed)
	{
		Vector2 vector = targetValue - currentValue;
		currentValue += vector * speed * Time.deltaTime;
		return currentValue;
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000631F4 File Offset: 0x000613F4
	public static float FloatUpdate(float currentValue, float targetValue, float speed)
	{
		float num = targetValue - currentValue;
		currentValue += num * speed * Time.deltaTime;
		return currentValue;
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x00063213 File Offset: 0x00061413
	public static void FadeUpdate(GameObject target, Hashtable args)
	{
		args["a"] = args["alpha"];
		iTween.ColorUpdate(target, args);
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x00063232 File Offset: 0x00061432
	public static void FadeUpdate(GameObject target, float alpha, float time)
	{
		iTween.FadeUpdate(target, iTween.Hash(new object[]
		{
			"alpha",
			alpha,
			"time",
			time
		}));
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x00063268 File Offset: 0x00061468
	public static void ColorUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Color[] array = new Color[4];
		if (!args.Contains("includechildren") || (bool)args["includechildren"])
		{
			foreach (object obj in target.transform)
			{
				Transform transform = (Transform)obj;
				iTween.ColorUpdate(transform.gameObject, args);
			}
		}
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		if (target.GetComponent(typeof(GUITexture)))
		{
			array[0] = (array[1] = target.GetComponent<GUITexture>().color);
		}
		else if (target.GetComponent(typeof(GUIText)))
		{
			array[0] = (array[1] = target.GetComponent<GUIText>().material.color);
		}
		else if (target.GetComponent<Renderer>())
		{
			array[0] = (array[1] = target.GetComponent<Renderer>().material.color);
		}
		else if (target.GetComponent<Light>())
		{
			array[0] = (array[1] = target.GetComponent<Light>().color);
		}
		else if (target.GetComponent<UberText>())
		{
			UberText component = target.GetComponent<UberText>();
			array[0] = (array[1] = component.TextColor);
		}
		if (args.Contains("color"))
		{
			array[1] = (Color)args["color"];
		}
		else
		{
			if (args.Contains("r"))
			{
				array[1].r = (float)args["r"];
			}
			if (args.Contains("g"))
			{
				array[1].g = (float)args["g"];
			}
			if (args.Contains("b"))
			{
				array[1].b = (float)args["b"];
			}
			if (args.Contains("a"))
			{
				array[1].a = (float)args["a"];
			}
		}
		array[3].r = Mathf.SmoothDamp(array[0].r, array[1].r, ref array[2].r, num);
		array[3].g = Mathf.SmoothDamp(array[0].g, array[1].g, ref array[2].g, num);
		array[3].b = Mathf.SmoothDamp(array[0].b, array[1].b, ref array[2].b, num);
		array[3].a = Mathf.SmoothDamp(array[0].a, array[1].a, ref array[2].a, num);
		if (target.GetComponent(typeof(GUITexture)))
		{
			target.GetComponent<GUITexture>().color = array[3];
		}
		else if (target.GetComponent(typeof(GUIText)))
		{
			target.GetComponent<GUIText>().material.color = array[3];
		}
		else if (target.GetComponent<Renderer>())
		{
			target.GetComponent<Renderer>().material.color = array[3];
		}
		else if (target.GetComponent<Light>())
		{
			target.GetComponent<Light>().color = array[3];
		}
		else if (target.GetComponent<UberText>())
		{
			UberText component2 = target.GetComponent<UberText>();
			component2.TextAlpha = array[3].a;
			component2.OutlineAlpha = array[3].a;
			component2.ShadowAlpha = array[3].a;
		}
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x00063760 File Offset: 0x00061960
	public static void ColorUpdate(GameObject target, Color color, float time)
	{
		iTween.ColorUpdate(target, iTween.Hash(new object[]
		{
			"color",
			color,
			"time",
			time
		}));
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x00063798 File Offset: 0x00061998
	public static void AudioUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector2[] array = new Vector2[4];
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		AudioSource audioSource;
		if (args.Contains("audiosource"))
		{
			audioSource = (AudioSource)args["audiosource"];
		}
		else
		{
			if (!target.GetComponent(typeof(AudioSource)))
			{
				Debug.LogError("iTween Error: AudioUpdate requires an AudioSource.");
				return;
			}
			audioSource = target.GetComponent<AudioSource>();
		}
		array[0] = (array[1] = new Vector2(audioSource.volume, audioSource.pitch));
		if (args.Contains("volume"))
		{
			array[1].x = (float)args["volume"];
		}
		if (args.Contains("pitch"))
		{
			array[1].y = (float)args["pitch"];
		}
		array[3].x = Mathf.SmoothDampAngle(array[0].x, array[1].x, ref array[2].x, num);
		array[3].y = Mathf.SmoothDampAngle(array[0].y, array[1].y, ref array[2].y, num);
		audioSource.volume = array[3].x;
		audioSource.pitch = array[3].y;
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x00063954 File Offset: 0x00061B54
	public static void AudioUpdate(GameObject target, float volume, float pitch, float time)
	{
		iTween.AudioUpdate(target, iTween.Hash(new object[]
		{
			"volume",
			volume,
			"pitch",
			pitch,
			"time",
			time
		}));
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x000639A8 File Offset: 0x00061BA8
	public static void RotateUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector3[] array = new Vector3[4];
		Vector3 eulerAngles = target.transform.eulerAngles;
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		bool flag;
		if (args.Contains("islocal"))
		{
			flag = (bool)args["islocal"];
		}
		else
		{
			flag = iTween.Defaults.isLocal;
		}
		if (flag)
		{
			array[0] = target.transform.localEulerAngles;
		}
		else
		{
			array[0] = target.transform.eulerAngles;
		}
		if (args.Contains("rotation"))
		{
			if (args["rotation"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["rotation"];
				array[1] = transform.eulerAngles;
			}
			else if (args["rotation"].GetType() == typeof(Vector3))
			{
				array[1] = (Vector3)args["rotation"];
			}
		}
		array[3].x = Mathf.SmoothDampAngle(array[0].x, array[1].x, ref array[2].x, num);
		array[3].y = Mathf.SmoothDampAngle(array[0].y, array[1].y, ref array[2].y, num);
		array[3].z = Mathf.SmoothDampAngle(array[0].z, array[1].z, ref array[2].z, num);
		if (flag)
		{
			target.transform.localEulerAngles = array[3];
		}
		else
		{
			target.transform.eulerAngles = array[3];
		}
		if (target.GetComponent<Rigidbody>() != null)
		{
			Vector3 eulerAngles2 = target.transform.eulerAngles;
			target.transform.eulerAngles = eulerAngles;
			target.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(eulerAngles2));
		}
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x00063C13 File Offset: 0x00061E13
	public static void RotateUpdate(GameObject target, Vector3 rotation, float time)
	{
		iTween.RotateUpdate(target, iTween.Hash(new object[]
		{
			"rotation",
			rotation,
			"time",
			time
		}));
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x00063C48 File Offset: 0x00061E48
	public static void ScaleUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector3[] array = new Vector3[4];
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		array[0] = (array[1] = target.transform.localScale);
		if (args.Contains("scale"))
		{
			if (args["scale"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["scale"];
				array[1] = transform.localScale;
			}
			else if (args["scale"].GetType() == typeof(Vector3))
			{
				array[1] = (Vector3)args["scale"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				array[1].x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				array[1].y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				array[1].z = (float)args["z"];
			}
		}
		array[3].x = Mathf.SmoothDamp(array[0].x, array[1].x, ref array[2].x, num);
		array[3].y = Mathf.SmoothDamp(array[0].y, array[1].y, ref array[2].y, num);
		array[3].z = Mathf.SmoothDamp(array[0].z, array[1].z, ref array[2].z, num);
		target.transform.localScale = array[3];
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x00063E91 File Offset: 0x00062091
	public static void ScaleUpdate(GameObject target, Vector3 scale, float time)
	{
		iTween.ScaleUpdate(target, iTween.Hash(new object[]
		{
			"scale",
			scale,
			"time",
			time
		}));
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x00063EC8 File Offset: 0x000620C8
	public static void MoveUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector3[] array = new Vector3[4];
		Vector3 position = target.transform.position;
		float num;
		if (args.Contains("time"))
		{
			num = (float)args["time"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		bool flag;
		if (args.Contains("islocal"))
		{
			flag = (bool)args["islocal"];
		}
		else
		{
			flag = iTween.Defaults.isLocal;
		}
		if (flag)
		{
			array[0] = (array[1] = target.transform.localPosition);
		}
		else
		{
			array[0] = (array[1] = target.transform.position);
		}
		if (args.Contains("position"))
		{
			if (args["position"].GetType() == typeof(Transform))
			{
				Transform transform = (Transform)args["position"];
				array[1] = transform.position;
			}
			else if (args["position"].GetType() == typeof(Vector3))
			{
				array[1] = (Vector3)args["position"];
			}
		}
		else
		{
			if (args.Contains("x"))
			{
				array[1].x = (float)args["x"];
			}
			if (args.Contains("y"))
			{
				array[1].y = (float)args["y"];
			}
			if (args.Contains("z"))
			{
				array[1].z = (float)args["z"];
			}
		}
		array[3].x = Mathf.SmoothDamp(array[0].x, array[1].x, ref array[2].x, num);
		array[3].y = Mathf.SmoothDamp(array[0].y, array[1].y, ref array[2].y, num);
		array[3].z = Mathf.SmoothDamp(array[0].z, array[1].z, ref array[2].z, num);
		if (args.Contains("orienttopath") && (bool)args["orienttopath"])
		{
			args["looktarget"] = array[3];
		}
		if (args.Contains("looktarget"))
		{
			iTween.LookUpdate(target, args);
		}
		if (flag)
		{
			target.transform.localPosition = array[3];
		}
		else
		{
			target.transform.position = array[3];
		}
		if (target.GetComponent<Rigidbody>() != null)
		{
			Vector3 position2 = target.transform.position;
			target.transform.position = position;
			target.GetComponent<Rigidbody>().MovePosition(position2);
		}
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x00064231 File Offset: 0x00062431
	public static void MoveUpdate(GameObject target, Vector3 position, float time)
	{
		iTween.MoveUpdate(target, iTween.Hash(new object[]
		{
			"position",
			position,
			"time",
			time
		}));
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x00064268 File Offset: 0x00062468
	public static void LookUpdate(GameObject target, Hashtable args)
	{
		iTween.CleanArgs(args);
		Vector3[] array = new Vector3[5];
		float num;
		if (args.Contains("looktime"))
		{
			num = (float)args["looktime"];
			num *= iTween.Defaults.updateTimePercentage;
		}
		else if (args.Contains("time"))
		{
			num = (float)args["time"] * 0.15f;
			num *= iTween.Defaults.updateTimePercentage;
		}
		else
		{
			num = iTween.Defaults.updateTime;
		}
		array[0] = target.transform.eulerAngles;
		if (args.Contains("looktarget"))
		{
			if (args["looktarget"].GetType() == typeof(Transform))
			{
				Transform transform = target.transform;
				Transform transform2 = (Transform)args["looktarget"];
				Vector3? vector = (Vector3?)args["up"];
				transform.LookAt(transform2, (vector == null) ? iTween.Defaults.up : vector.Value);
			}
			else if (args["looktarget"].GetType() == typeof(Vector3))
			{
				Transform transform3 = target.transform;
				Vector3 vector2 = (Vector3)args["looktarget"];
				Vector3? vector3 = (Vector3?)args["up"];
				transform3.LookAt(vector2, (vector3 == null) ? iTween.Defaults.up : vector3.Value);
			}
			array[1] = target.transform.eulerAngles;
			target.transform.eulerAngles = array[0];
			array[3].x = Mathf.SmoothDampAngle(array[0].x, array[1].x, ref array[2].x, num);
			array[3].y = Mathf.SmoothDampAngle(array[0].y, array[1].y, ref array[2].y, num);
			array[3].z = Mathf.SmoothDampAngle(array[0].z, array[1].z, ref array[2].z, num);
			target.transform.eulerAngles = array[3];
			if (args.Contains("axis"))
			{
				array[4] = target.transform.eulerAngles;
				string text = (string)args["axis"];
				if (text != null)
				{
					if (iTween.<>f__switch$map9E == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
						dictionary.Add("x", 0);
						dictionary.Add("y", 1);
						dictionary.Add("z", 2);
						iTween.<>f__switch$map9E = dictionary;
					}
					int num2;
					if (iTween.<>f__switch$map9E.TryGetValue(text, ref num2))
					{
						switch (num2)
						{
						case 0:
							array[4].y = array[0].y;
							array[4].z = array[0].z;
							break;
						case 1:
							array[4].x = array[0].x;
							array[4].z = array[0].z;
							break;
						case 2:
							array[4].x = array[0].x;
							array[4].y = array[0].y;
							break;
						}
					}
				}
				target.transform.eulerAngles = array[4];
			}
			return;
		}
		Debug.LogError("iTween Error: LookUpdate needs a 'looktarget' property!");
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x0006463F File Offset: 0x0006283F
	public static void LookUpdate(GameObject target, Vector3 looktarget, float time)
	{
		iTween.LookUpdate(target, iTween.Hash(new object[]
		{
			"looktarget",
			looktarget,
			"time",
			time
		}));
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x00064674 File Offset: 0x00062874
	public static float PathLength(Transform[] path)
	{
		Vector3[] array = new Vector3[path.Length];
		float num = 0f;
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = path[i].position;
		}
		Vector3[] pts = iTween.PathControlPointGenerator(array);
		Vector3 vector = iTween.Interp(pts, 0f);
		int num2 = path.Length * 20;
		for (int j = 1; j <= num2; j++)
		{
			float t = (float)j / (float)num2;
			Vector3 vector2 = iTween.Interp(pts, t);
			num += Vector3.Distance(vector, vector2);
			vector = vector2;
		}
		return num;
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x00064710 File Offset: 0x00062910
	public static float PathLength(Vector3[] path)
	{
		float num = 0f;
		Vector3[] pts = iTween.PathControlPointGenerator(path);
		Vector3 vector = iTween.Interp(pts, 0f);
		int num2 = path.Length * 20;
		for (int i = 1; i <= num2; i++)
		{
			float t = (float)i / (float)num2;
			Vector3 vector2 = iTween.Interp(pts, t);
			num += Vector3.Distance(vector, vector2);
			vector = vector2;
		}
		return num;
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x00064774 File Offset: 0x00062974
	public static Texture2D CameraTexture(Color color)
	{
		Texture2D texture2D = new Texture2D(Screen.width, Screen.height, 5, false);
		Color[] array = new Color[Screen.width * Screen.height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x000647D3 File Offset: 0x000629D3
	public static void PutOnPath(GameObject target, Vector3[] path, float percent)
	{
		target.transform.position = iTween.Interp(iTween.PathControlPointGenerator(path), percent);
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x000647EC File Offset: 0x000629EC
	public static void PutOnPath(Transform target, Vector3[] path, float percent)
	{
		target.position = iTween.Interp(iTween.PathControlPointGenerator(path), percent);
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x00064800 File Offset: 0x00062A00
	public static void PutOnPath(GameObject target, Transform[] path, float percent)
	{
		Vector3[] array = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = path[i].position;
		}
		target.transform.position = iTween.Interp(iTween.PathControlPointGenerator(array), percent);
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x00064858 File Offset: 0x00062A58
	public static void PutOnPath(Transform target, Transform[] path, float percent)
	{
		Vector3[] array = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = path[i].position;
		}
		target.position = iTween.Interp(iTween.PathControlPointGenerator(array), percent);
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x000648A8 File Offset: 0x00062AA8
	public static Vector3 PointOnPath(Transform[] path, float percent)
	{
		Vector3[] array = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = path[i].position;
		}
		return iTween.Interp(iTween.PathControlPointGenerator(array), percent);
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x000648F2 File Offset: 0x00062AF2
	public static void DrawLine(Vector3[] line)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x0006490D File Offset: 0x00062B0D
	public static void DrawLine(Vector3[] line, Color color)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, color, "gizmos");
		}
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x00064924 File Offset: 0x00062B24
	public static void DrawLine(Transform[] line)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x0006497C File Offset: 0x00062B7C
	public static void DrawLine(Transform[] line, Color color)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, color, "gizmos");
		}
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x000649CF File Offset: 0x00062BCF
	public static void DrawLineGizmos(Vector3[] line)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x000649EA File Offset: 0x00062BEA
	public static void DrawLineGizmos(Vector3[] line, Color color)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, color, "gizmos");
		}
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x00064A04 File Offset: 0x00062C04
	public static void DrawLineGizmos(Transform[] line)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06001558 RID: 5464 RVA: 0x00064A5C File Offset: 0x00062C5C
	public static void DrawLineGizmos(Transform[] line, Color color)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, color, "gizmos");
		}
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x00064AAF File Offset: 0x00062CAF
	public static void DrawLineHandles(Vector3[] line)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, iTween.Defaults.color, "handles");
		}
	}

	// Token: 0x0600155A RID: 5466 RVA: 0x00064ACA File Offset: 0x00062CCA
	public static void DrawLineHandles(Vector3[] line, Color color)
	{
		if (line.Length > 0)
		{
			iTween.DrawLineHelper(line, color, "handles");
		}
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x00064AE4 File Offset: 0x00062CE4
	public static void DrawLineHandles(Transform[] line)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, iTween.Defaults.color, "handles");
		}
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x00064B3C File Offset: 0x00062D3C
	public static void DrawLineHandles(Transform[] line, Color color)
	{
		if (line.Length > 0)
		{
			Vector3[] array = new Vector3[line.Length];
			for (int i = 0; i < line.Length; i++)
			{
				array[i] = line[i].position;
			}
			iTween.DrawLineHelper(array, color, "handles");
		}
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x00064B8F File Offset: 0x00062D8F
	public static Vector3 PointOnPath(Vector3[] path, float percent)
	{
		return iTween.Interp(iTween.PathControlPointGenerator(path), percent);
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x00064B9D File Offset: 0x00062D9D
	public static void DrawPath(Vector3[] path)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x0600155F RID: 5471 RVA: 0x00064BB8 File Offset: 0x00062DB8
	public static void DrawPath(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, color, "gizmos");
		}
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x00064BD0 File Offset: 0x00062DD0
	public static void DrawPath(Transform[] path)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x00064C28 File Offset: 0x00062E28
	public static void DrawPath(Transform[] path, Color color)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, color, "gizmos");
		}
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x00064C7B File Offset: 0x00062E7B
	public static void DrawPathGizmos(Vector3[] path)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x00064C96 File Offset: 0x00062E96
	public static void DrawPathGizmos(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, color, "gizmos");
		}
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x00064CB0 File Offset: 0x00062EB0
	public static void DrawPathGizmos(Transform[] path)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, iTween.Defaults.color, "gizmos");
		}
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x00064D08 File Offset: 0x00062F08
	public static void DrawPathGizmos(Transform[] path, Color color)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, color, "gizmos");
		}
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x00064D5B File Offset: 0x00062F5B
	public static void DrawPathHandles(Vector3[] path)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, iTween.Defaults.color, "handles");
		}
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x00064D76 File Offset: 0x00062F76
	public static void DrawPathHandles(Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			iTween.DrawPathHelper(path, color, "handles");
		}
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x00064D90 File Offset: 0x00062F90
	public static void DrawPathHandles(Transform[] path)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, iTween.Defaults.color, "handles");
		}
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x00064DE8 File Offset: 0x00062FE8
	public static void DrawPathHandles(Transform[] path, Color color)
	{
		if (path.Length > 0)
		{
			Vector3[] array = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = path[i].position;
			}
			iTween.DrawPathHelper(array, color, "handles");
		}
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x00064E3C File Offset: 0x0006303C
	public static void CameraFadeDepth(int depth)
	{
		if (iTween.cameraFade)
		{
			iTween.cameraFade.transform.position = new Vector3(iTween.cameraFade.transform.position.x, iTween.cameraFade.transform.position.y, (float)depth);
		}
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x00064E9C File Offset: 0x0006309C
	public static void CameraFadeDestroy()
	{
		if (iTween.cameraFade)
		{
			Object.Destroy(iTween.cameraFade);
		}
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x00064EB7 File Offset: 0x000630B7
	public static void CameraFadeSwap(Texture2D texture)
	{
		if (iTween.cameraFade)
		{
			iTween.cameraFade.GetComponent<GUITexture>().texture = texture;
		}
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x00064ED8 File Offset: 0x000630D8
	public static GameObject CameraFadeAdd(Texture2D texture, int depth)
	{
		if (iTween.cameraFade)
		{
			return null;
		}
		iTween.cameraFade = new GameObject("iTween Camera Fade");
		iTween.cameraFade.transform.position = new Vector3(0.5f, 0.5f, (float)depth);
		iTween.cameraFade.AddComponent<GUITexture>();
		iTween.cameraFade.GetComponent<GUITexture>().texture = texture;
		iTween.cameraFade.GetComponent<GUITexture>().color = new Color(0.5f, 0.5f, 0.5f, 0f);
		return iTween.cameraFade;
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x00064F70 File Offset: 0x00063170
	public static GameObject CameraFadeAdd(Texture2D texture)
	{
		if (iTween.cameraFade)
		{
			return null;
		}
		iTween.cameraFade = new GameObject("iTween Camera Fade");
		iTween.cameraFade.transform.position = new Vector3(0.5f, 0.5f, (float)iTween.Defaults.cameraFadeDepth);
		iTween.cameraFade.AddComponent<GUITexture>();
		iTween.cameraFade.GetComponent<GUITexture>().texture = texture;
		iTween.cameraFade.GetComponent<GUITexture>().color = new Color(0.5f, 0.5f, 0.5f, 0f);
		return iTween.cameraFade;
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x0006500C File Offset: 0x0006320C
	public static GameObject CameraFadeAdd()
	{
		if (iTween.cameraFade)
		{
			return null;
		}
		iTween.cameraFade = new GameObject("iTween Camera Fade");
		iTween.cameraFade.transform.position = new Vector3(0.5f, 0.5f, (float)iTween.Defaults.cameraFadeDepth);
		iTween.cameraFade.AddComponent<GUITexture>();
		iTween.cameraFade.GetComponent<GUITexture>().texture = iTween.CameraTexture(Color.black);
		iTween.cameraFade.GetComponent<GUITexture>().color = new Color(0.5f, 0.5f, 0.5f, 0f);
		return iTween.cameraFade;
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x000650AE File Offset: 0x000632AE
	public static void EnableTween(iTween tween)
	{
		tween.enabled = true;
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x000650B7 File Offset: 0x000632B7
	public static void Resume(GameObject target)
	{
		iTweenManager.ForEachByGameObject(new iTweenManager.TweenOperation(iTween.EnableTween), target);
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x000650CB File Offset: 0x000632CB
	public static void Resume(GameObject target, bool includechildren)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.EnableTween), target, null, null, includechildren);
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x000650E2 File Offset: 0x000632E2
	public static void Resume(GameObject target, string type)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.EnableTween), target, null, type, false);
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x000650F9 File Offset: 0x000632F9
	public static void Resume(GameObject target, string type, bool includechildren)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.EnableTween), target, null, type, includechildren);
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x00065110 File Offset: 0x00063310
	public static void Resume()
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.EnableTween), null, null, null, false);
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x00065127 File Offset: 0x00063327
	public static void Resume(string type)
	{
		iTweenManager.ForEachByType(new iTweenManager.TweenOperation(iTween.EnableTween), type);
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x0006513B File Offset: 0x0006333B
	public static void PauseTween(iTween tween)
	{
		tween.isPaused = true;
		tween.enabled = false;
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x0006514B File Offset: 0x0006334B
	public static void Pause(GameObject target)
	{
		iTweenManager.ForEachByGameObject(new iTweenManager.TweenOperation(iTween.PauseTween), target);
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x0006515F File Offset: 0x0006335F
	public static void Pause(GameObject target, bool includechildren)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.PauseTween), target, null, null, includechildren);
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x00065176 File Offset: 0x00063376
	public static void Pause(GameObject target, string type)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.PauseTween), target, null, type, false);
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x0006518D File Offset: 0x0006338D
	public static void Pause(GameObject target, string type, bool includechildren)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.PauseTween), target, null, type, includechildren);
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x000651A4 File Offset: 0x000633A4
	public static void Pause()
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.PauseTween), null, null, null, false);
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x000651BB File Offset: 0x000633BB
	public static void Pause(string type)
	{
		iTweenManager.ForEachByType(new iTweenManager.TweenOperation(iTween.PauseTween), type);
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x000651CF File Offset: 0x000633CF
	public static int Count()
	{
		return iTweenManager.GetTweenCount();
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x000651D8 File Offset: 0x000633D8
	public static int Count(string type)
	{
		int num = 0;
		iTween next;
		while ((next = iTweenManager.GetIterator().GetNext()) != null)
		{
			string text = next.type + next.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower().Equals(type.ToLower()))
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x0006523C File Offset: 0x0006343C
	public static int Count(GameObject target)
	{
		int num = 0;
		iTween next;
		while ((next = iTweenManager.GetIterator().GetNext()) != null)
		{
			if (next.gameObject == target)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x0006527C File Offset: 0x0006347C
	public static int Count(GameObject target, string type)
	{
		int num = 0;
		iTween[] tweensForObject = iTweenManager.GetTweensForObject(target);
		foreach (iTween iTween in tweensForObject)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() == type.ToLower())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001582 RID: 5506 RVA: 0x000652F0 File Offset: 0x000634F0
	public static int CountByName(GameObject target, string name)
	{
		int num = 0;
		iTween[] tweensForObject = iTweenManager.GetTweensForObject(target);
		foreach (iTween iTween in tweensForObject)
		{
			if (iTween._name == name)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x0006533C File Offset: 0x0006353C
	public static int CountOtherTypes(GameObject target, string type)
	{
		int num = 0;
		iTween[] tweensForObject = iTweenManager.GetTweensForObject(target);
		foreach (iTween iTween in tweensForObject)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() != type.ToLower())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x000653B0 File Offset: 0x000635B0
	public static int CountOtherNames(GameObject target, string name)
	{
		int num = 0;
		iTween[] tweensForObject = iTweenManager.GetTweensForObject(target);
		foreach (iTween iTween in tweensForObject)
		{
			if (iTween._name != name)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x000653FB File Offset: 0x000635FB
	public static bool HasTween(GameObject target)
	{
		return iTween.Count(target) > 0;
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x00065406 File Offset: 0x00063606
	public static bool HasType(GameObject target, string type)
	{
		return iTween.Count(target, type) > 0;
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x00065412 File Offset: 0x00063612
	public static bool HasName(GameObject target, string name)
	{
		return iTween.CountByName(target, name) > 0;
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x00065420 File Offset: 0x00063620
	public static bool HasOtherType(GameObject target, string type)
	{
		iTween[] tweensForObject = iTweenManager.GetTweensForObject(target);
		foreach (iTween iTween in tweensForObject)
		{
			string text = iTween.type + iTween.method;
			text = text.Substring(0, type.Length);
			if (text.ToLower() != type.ToLower())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001589 RID: 5513 RVA: 0x0006548C File Offset: 0x0006368C
	public static bool HasOtherName(GameObject target, string name)
	{
		iTween[] tweensForObject = iTweenManager.GetTweensForObject(target);
		foreach (iTween iTween in tweensForObject)
		{
			if (iTween._name != name)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x000654D0 File Offset: 0x000636D0
	public static bool HasNameNotInList(GameObject target, params string[] names)
	{
		iTween[] tweensForObject = iTweenManager.GetTweensForObject(target);
		foreach (iTween iTween in tweensForObject)
		{
			bool flag = false;
			for (int j = 0; j < names.Length; j++)
			{
				if (iTween._name == names[j])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x0006553F File Offset: 0x0006373F
	public static void StopTween(iTween tween)
	{
		tween.Dispose();
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x00065547 File Offset: 0x00063747
	public static void Stop()
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.StopTween), null, null, null, false);
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x0006555E File Offset: 0x0006375E
	public static void Stop(string type)
	{
		iTweenManager.ForEachByType(new iTweenManager.TweenOperation(iTween.StopTween), type);
	}

	// Token: 0x0600158E RID: 5518 RVA: 0x00065572 File Offset: 0x00063772
	public static void StopByName(string name)
	{
		iTweenManager.ForEachByName(new iTweenManager.TweenOperation(iTween.StopTween), name);
	}

	// Token: 0x0600158F RID: 5519 RVA: 0x00065586 File Offset: 0x00063786
	public static void Stop(GameObject target)
	{
		iTweenManager.ForEachByGameObject(new iTweenManager.TweenOperation(iTween.StopTween), target);
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x0006559A File Offset: 0x0006379A
	public static void Stop(GameObject target, bool includechildren)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.StopTween), target, null, null, includechildren);
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x000655B1 File Offset: 0x000637B1
	public static void Stop(GameObject target, string type)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.StopTween), target, null, type, false);
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x000655C8 File Offset: 0x000637C8
	public static void StopByName(GameObject target, string name)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.StopTween), target, name, null, false);
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x000655DF File Offset: 0x000637DF
	public static void Stop(GameObject target, string type, bool includechildren)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.StopTween), target, null, type, includechildren);
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x000655F6 File Offset: 0x000637F6
	public static void StopByName(GameObject target, string name, bool includechildren)
	{
		iTweenManager.ForEach(new iTweenManager.TweenOperation(iTween.StopTween), target, name, null, includechildren);
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x0006560D File Offset: 0x0006380D
	public static void StopOthers(GameObject target, string type, bool includechildren = false)
	{
		iTweenManager.ForEachInverted(new iTweenManager.TweenOperation(iTween.StopTween), target, null, type, includechildren);
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x00065624 File Offset: 0x00063824
	public static void StopOthersByName(GameObject target, string name, bool includechildren = false)
	{
		iTweenManager.ForEachInverted(new iTweenManager.TweenOperation(iTween.StopTween), target, name, null, includechildren);
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x0006563C File Offset: 0x0006383C
	public static Hashtable Hash(params object[] args)
	{
		Hashtable hashtable = new Hashtable(args.Length / 2);
		if (args.Length % 2 != 0)
		{
			Debug.LogError("Tween Error: Hash requires an even number of arguments!");
			return null;
		}
		for (int i = 0; i < args.Length - 1; i += 2)
		{
			hashtable.Add(args[i], args[i + 1]);
		}
		return hashtable;
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x0006568F File Offset: 0x0006388F
	public void Awake()
	{
		this.RetrieveArgs();
		this.lastRealTime = Time.realtimeSinceStartup;
		this.ResetDelay();
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x000656A8 File Offset: 0x000638A8
	public void Update()
	{
		if (!this.activeInHierarchy)
		{
			return;
		}
		if (this.waitForDelay)
		{
			if (this.delay > 0f && this.delay > Time.time - this.delayStarted)
			{
				return;
			}
			this.TweenStart();
			this.waitForDelay = false;
		}
		if (this.isRunning && !this.physics)
		{
			if (!this.reverse)
			{
				if (this.percentage < 1f)
				{
					this.TweenUpdate();
				}
				else
				{
					this.TweenComplete();
				}
			}
			else if (this.percentage > 0f)
			{
				this.TweenUpdate();
			}
			else
			{
				this.TweenComplete();
			}
		}
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x0006576C File Offset: 0x0006396C
	public void FixedUpdate()
	{
		if (!this.activeInHierarchy)
		{
			return;
		}
		if (this.isRunning && this.physics)
		{
			if (!this.reverse)
			{
				if (this.percentage < 1f)
				{
					this.TweenUpdate();
				}
				else
				{
					this.TweenComplete();
				}
			}
			else if (this.percentage > 0f)
			{
				this.TweenUpdate();
			}
			else
			{
				this.TweenComplete();
			}
		}
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x000657F0 File Offset: 0x000639F0
	public void LateUpdate()
	{
		if (!this.activeInHierarchy)
		{
			return;
		}
		if (this.waitForDelay)
		{
			return;
		}
		if (this.tweenArguments == null)
		{
			return;
		}
		if (this.tweenArguments.Contains("looktarget") && this.isRunning && (this.type == "move" || this.type == "shake" || this.type == "punch"))
		{
			iTween.LookUpdate(this.gameObject, this.tweenArguments);
		}
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x00065891 File Offset: 0x00063A91
	public void OnEnable()
	{
		if (this.isRunning)
		{
			this.EnableKinematic();
		}
		if (this.isPaused)
		{
			this.isPaused = false;
			if (this.delay > 0f)
			{
				this.ResumeDelay();
			}
		}
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x000658CC File Offset: 0x00063ACC
	public void OnDisable()
	{
		this.DisableKinematic();
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x000658D4 File Offset: 0x00063AD4
	public void Upkeep()
	{
		if (this.destroyed)
		{
			return;
		}
		if (this.gameObject == null)
		{
			iTweenManager.Remove(this);
			return;
		}
		if (!this.gameObject.activeInHierarchy || !this.enabled)
		{
			if (this.activeLastTick)
			{
				this.OnDisable();
			}
			this.activeLastTick = false;
		}
		else
		{
			if (!this.activeLastTick)
			{
				this.OnEnable();
			}
			this.activeLastTick = true;
		}
	}

	// Token: 0x0600159F RID: 5535 RVA: 0x00065958 File Offset: 0x00063B58
	private static void DrawLineHelper(Vector3[] line, Color color, string method)
	{
		Gizmos.color = color;
		for (int i = 0; i < line.Length - 1; i++)
		{
			if (method == "gizmos")
			{
				Gizmos.DrawLine(line[i], line[i + 1]);
			}
			else if (method == "handles")
			{
				Debug.LogError("iTween Error: Drawing a line with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
			}
		}
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x000659D0 File Offset: 0x00063BD0
	private static void DrawPathHelper(Vector3[] path, Color color, string method)
	{
		Vector3[] pts = iTween.PathControlPointGenerator(path);
		Vector3 vector = iTween.Interp(pts, 0f);
		Gizmos.color = color;
		int num = path.Length * 20;
		for (int i = 1; i <= num; i++)
		{
			float t = (float)i / (float)num;
			Vector3 vector2 = iTween.Interp(pts, t);
			if (method == "gizmos")
			{
				Gizmos.DrawLine(vector2, vector);
			}
			else if (method == "handles")
			{
				Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
			}
			vector = vector2;
		}
	}

	// Token: 0x060015A1 RID: 5537 RVA: 0x00065A5C File Offset: 0x00063C5C
	private static Vector3[] PathControlPointGenerator(Vector3[] path)
	{
		int num = 2;
		Vector3[] array = new Vector3[path.Length + num];
		Array.Copy(path, 0, array, 1, path.Length);
		array[0] = array[1] + (array[1] - array[2]);
		array[array.Length - 1] = array[array.Length - 2] + (array[array.Length - 2] - array[array.Length - 3]);
		if (array[1] == array[array.Length - 2])
		{
			Vector3[] array2 = new Vector3[array.Length];
			Array.Copy(array, array2, array.Length);
			array2[0] = array2[array2.Length - 3];
			array2[array2.Length - 1] = array2[2];
			array = new Vector3[array2.Length];
			Array.Copy(array2, array, array2.Length);
		}
		return array;
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x00065B90 File Offset: 0x00063D90
	private static Vector3 Interp(Vector3[] pts, float t)
	{
		int num = pts.Length - 3;
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float num3 = t * (float)num - (float)num2;
		Vector3 vector = pts[num2];
		Vector3 vector2 = pts[num2 + 1];
		Vector3 vector3 = pts[num2 + 2];
		Vector3 vector4 = pts[num2 + 3];
		return 0.5f * ((-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num3 * num3) + (-vector + vector3) * num3 + 2f * vector2);
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x00065CA8 File Offset: 0x00063EA8
	private static void Launch(GameObject target, Hashtable args)
	{
		if (!args.Contains("id"))
		{
			args["id"] = iTween.GenerateID();
		}
		if (!args.Contains("target"))
		{
			args["target"] = target;
		}
		if (args.Contains("oncomplete") && !args.Contains("onconflict"))
		{
			args["onconflict"] = args["oncomplete"];
			if (args.Contains("oncompletetarget") && !args.Contains("onconflicttarget"))
			{
				args["onconflicttarget"] = args["oncompletetarget"];
			}
			if (args.Contains("oncompleteparams") && !args.Contains("onconflictparams"))
			{
				args["onconflictparams"] = args["oncompleteparams"];
			}
		}
		iTween tween = new iTween(target, args);
		iTweenManager.Add(tween);
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x00065DA8 File Offset: 0x00063FA8
	private static Hashtable CleanArgs(Hashtable args)
	{
		Hashtable hashtable = new Hashtable(args.Count);
		Hashtable hashtable2 = new Hashtable(args.Count);
		foreach (object obj in args)
		{
			DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
			hashtable.Add(dictionaryEntry.Key, dictionaryEntry.Value);
		}
		foreach (object obj2 in hashtable)
		{
			DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj2;
			if (!((string)dictionaryEntry2.Key == "id"))
			{
				if (dictionaryEntry2.Value.GetType() == typeof(int))
				{
					int num = (int)dictionaryEntry2.Value;
					float num2 = (float)num;
					args[dictionaryEntry2.Key] = num2;
				}
				if (dictionaryEntry2.Value.GetType() == typeof(double))
				{
					double num3 = (double)dictionaryEntry2.Value;
					float num4 = (float)num3;
					args[dictionaryEntry2.Key] = num4;
				}
			}
		}
		foreach (object obj3 in args)
		{
			DictionaryEntry dictionaryEntry3 = (DictionaryEntry)obj3;
			hashtable2.Add(dictionaryEntry3.Key.ToString().ToLower(), dictionaryEntry3.Value);
		}
		args = hashtable2;
		return args;
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x00065F90 File Offset: 0x00064190
	private static int GenerateID()
	{
		int result = iTween.nextId;
		iTween.nextId = ((iTween.nextId != int.MaxValue) ? (iTween.nextId + 1) : 1);
		return result;
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x00065FC8 File Offset: 0x000641C8
	public Vector3 GetTargetPosition()
	{
		if (this.vector3s.Length >= 2)
		{
			return this.vector3s[1];
		}
		return new Vector3(0f, 0f, 0f);
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x0006600C File Offset: 0x0006420C
	private void RetrieveArgs()
	{
		if (this.tweenArguments == null)
		{
			return;
		}
		this.id = (int)this.tweenArguments["id"];
		this.type = (string)this.tweenArguments["type"];
		this._name = (string)this.tweenArguments["name"];
		this.method = (string)this.tweenArguments["method"];
		if (this.tweenArguments.Contains("time"))
		{
			this.time = (float)this.tweenArguments["time"];
		}
		else
		{
			this.time = iTween.Defaults.time;
		}
		if (this.rigidbody != null)
		{
			this.physics = true;
		}
		if (this.tweenArguments.Contains("delay"))
		{
			this.delay = (float)this.tweenArguments["delay"];
		}
		else
		{
			this.delay = iTween.Defaults.delay;
		}
		if (this.tweenArguments.Contains("namedcolorvalue"))
		{
			if (this.tweenArguments["namedcolorvalue"].GetType() == typeof(iTween.NamedValueColor))
			{
				this.namedColorValueString = ((iTween.NamedValueColor)((int)this.tweenArguments["namedcolorvalue"])).ToString();
			}
			else if (this.tweenArguments["namedcolorvalue"].GetType() == typeof(string))
			{
				this.namedColorValueString = (string)this.tweenArguments["namedcolorvalue"];
			}
			else
			{
				try
				{
					this.namedColorValueString = ((iTween.NamedValueColor)((int)Enum.Parse(typeof(iTween.NamedValueColor), (string)this.tweenArguments["namedcolorvalue"], true))).ToString();
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported namedcolorvalue supplied! Default will be used.");
					this.namedcolorvalue = iTween.NamedValueColor._Color;
					this.namedColorValueString = this.namedcolorvalue.ToString();
				}
			}
		}
		else
		{
			this.namedcolorvalue = iTween.Defaults.namedColorValue;
			this.namedColorValueString = "_Color";
		}
		if (this.tweenArguments.Contains("looptype"))
		{
			if (this.tweenArguments["looptype"].GetType() == typeof(iTween.LoopType))
			{
				this.loopType = (iTween.LoopType)((int)this.tweenArguments["looptype"]);
			}
			else
			{
				try
				{
					this.loopType = (iTween.LoopType)((int)Enum.Parse(typeof(iTween.LoopType), (string)this.tweenArguments["looptype"], true));
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported loopType supplied! Default will be used.");
					this.loopType = iTween.LoopType.none;
				}
			}
		}
		else
		{
			this.loopType = iTween.LoopType.none;
		}
		if (this.tweenArguments.Contains("easetype"))
		{
			if (this.tweenArguments["easetype"].GetType() == typeof(iTween.EaseType))
			{
				this.easeType = (iTween.EaseType)((int)this.tweenArguments["easetype"]);
			}
			else
			{
				try
				{
					this.easeType = (iTween.EaseType)((int)Enum.Parse(typeof(iTween.EaseType), (string)this.tweenArguments["easetype"], true));
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported easeType supplied! Default will be used.");
					this.easeType = iTween.Defaults.easeType;
				}
			}
		}
		else
		{
			this.easeType = iTween.Defaults.easeType;
		}
		if (this.tweenArguments.Contains("space"))
		{
			if (this.tweenArguments["space"].GetType() == typeof(Space))
			{
				this.space = (int)this.tweenArguments["space"];
			}
			else
			{
				try
				{
					this.space = (int)Enum.Parse(typeof(Space), (string)this.tweenArguments["space"], true);
				}
				catch
				{
					Debug.LogWarning("iTween: Unsupported space supplied! Default will be used.");
					this.space = iTween.Defaults.space;
				}
			}
		}
		else
		{
			this.space = iTween.Defaults.space;
		}
		if (this.tweenArguments.Contains("islocal"))
		{
			this.isLocal = (bool)this.tweenArguments["islocal"];
		}
		else
		{
			this.isLocal = iTween.Defaults.isLocal;
		}
		if (this.tweenArguments.Contains("ignoretimescale"))
		{
			this.useRealTime = (bool)this.tweenArguments["ignoretimescale"];
		}
		else
		{
			this.useRealTime = iTween.Defaults.useRealTime;
		}
		this.GetEasingFunction();
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x00066534 File Offset: 0x00064734
	private void GetEasingFunction()
	{
		switch (this.easeType)
		{
		case iTween.EaseType.easeInQuad:
			this.ease = new iTween.EasingFunction(this.easeInQuad);
			break;
		case iTween.EaseType.easeOutQuad:
			this.ease = new iTween.EasingFunction(this.easeOutQuad);
			break;
		case iTween.EaseType.easeInOutQuad:
			this.ease = new iTween.EasingFunction(this.easeInOutQuad);
			break;
		case iTween.EaseType.easeInCubic:
			this.ease = new iTween.EasingFunction(this.easeInCubic);
			break;
		case iTween.EaseType.easeOutCubic:
			this.ease = new iTween.EasingFunction(this.easeOutCubic);
			break;
		case iTween.EaseType.easeInOutCubic:
			this.ease = new iTween.EasingFunction(this.easeInOutCubic);
			break;
		case iTween.EaseType.easeInQuart:
			this.ease = new iTween.EasingFunction(this.easeInQuart);
			break;
		case iTween.EaseType.easeOutQuart:
			this.ease = new iTween.EasingFunction(this.easeOutQuart);
			break;
		case iTween.EaseType.easeInOutQuart:
			this.ease = new iTween.EasingFunction(this.easeInOutQuart);
			break;
		case iTween.EaseType.easeInQuint:
			this.ease = new iTween.EasingFunction(this.easeInQuint);
			break;
		case iTween.EaseType.easeOutQuint:
			this.ease = new iTween.EasingFunction(this.easeOutQuint);
			break;
		case iTween.EaseType.easeInOutQuint:
			this.ease = new iTween.EasingFunction(this.easeInOutQuint);
			break;
		case iTween.EaseType.easeInSine:
			this.ease = new iTween.EasingFunction(this.easeInSine);
			break;
		case iTween.EaseType.easeOutSine:
			this.ease = new iTween.EasingFunction(this.easeOutSine);
			break;
		case iTween.EaseType.easeInOutSine:
			this.ease = new iTween.EasingFunction(this.easeInOutSine);
			break;
		case iTween.EaseType.easeInExpo:
			this.ease = new iTween.EasingFunction(this.easeInExpo);
			break;
		case iTween.EaseType.easeOutExpo:
			this.ease = new iTween.EasingFunction(this.easeOutExpo);
			break;
		case iTween.EaseType.easeInOutExpo:
			this.ease = new iTween.EasingFunction(this.easeInOutExpo);
			break;
		case iTween.EaseType.easeInCirc:
			this.ease = new iTween.EasingFunction(this.easeInCirc);
			break;
		case iTween.EaseType.easeOutCirc:
			this.ease = new iTween.EasingFunction(this.easeOutCirc);
			break;
		case iTween.EaseType.easeInOutCirc:
			this.ease = new iTween.EasingFunction(this.easeInOutCirc);
			break;
		case iTween.EaseType.linear:
			this.ease = new iTween.EasingFunction(this.linear);
			break;
		case iTween.EaseType.spring:
			this.ease = new iTween.EasingFunction(this.spring);
			break;
		case iTween.EaseType.easeInBounce:
			this.ease = new iTween.EasingFunction(this.easeInBounce);
			break;
		case iTween.EaseType.easeOutBounce:
			this.ease = new iTween.EasingFunction(this.easeOutBounce);
			break;
		case iTween.EaseType.easeInOutBounce:
			this.ease = new iTween.EasingFunction(this.easeInOutBounce);
			break;
		case iTween.EaseType.easeInBack:
			this.ease = new iTween.EasingFunction(this.easeInBack);
			break;
		case iTween.EaseType.easeOutBack:
			this.ease = new iTween.EasingFunction(this.easeOutBack);
			break;
		case iTween.EaseType.easeInOutBack:
			this.ease = new iTween.EasingFunction(this.easeInOutBack);
			break;
		case iTween.EaseType.easeInElastic:
			this.ease = new iTween.EasingFunction(this.easeInElastic);
			break;
		case iTween.EaseType.easeOutElastic:
			this.ease = new iTween.EasingFunction(this.easeOutElastic);
			break;
		case iTween.EaseType.easeInOutElastic:
			this.ease = new iTween.EasingFunction(this.easeInOutElastic);
			break;
		case iTween.EaseType.easeInSineOutExpo:
			this.ease = new iTween.EasingFunction(this.easeInSineOutExpo);
			break;
		case iTween.EaseType.easeOutElasticLight:
			this.ease = new iTween.EasingFunction(this.easeOutElasticLight);
			break;
		}
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x000668F0 File Offset: 0x00064AF0
	private void UpdatePercentage()
	{
		if (this.useRealTime)
		{
			this.runningTime += Time.realtimeSinceStartup - this.lastRealTime;
		}
		else
		{
			this.runningTime += Time.deltaTime;
		}
		if (this.reverse)
		{
			this.percentage = 1f - this.runningTime / this.time;
		}
		else
		{
			this.percentage = this.runningTime / this.time;
		}
		this.lastRealTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x00066980 File Offset: 0x00064B80
	private void CallBack(iTween.CallbackType callbackType)
	{
		string text = iTween.CALLBACK_NAMES[(int)callbackType];
		string text2 = iTween.CALLBACK_PARAMS_NAMES[(int)callbackType];
		string text3 = iTween.CALLBACK_TARGET_NAMES[(int)callbackType];
		if (this.tweenArguments.Contains(text) && !this.tweenArguments.Contains("ischild"))
		{
			GameObject gameObject;
			if (this.tweenArguments.Contains(text3))
			{
				gameObject = (GameObject)this.tweenArguments[text3];
			}
			else
			{
				gameObject = this.gameObject;
			}
			if (gameObject == null)
			{
				Debug.LogError(string.Format("iTween Error: target is null! callbackType={0} tween={1}", text, this));
				return;
			}
			object obj = this.tweenArguments[text];
			if (obj is Action<object>)
			{
				((Action<object>)obj).Invoke(this.tweenArguments[text2]);
			}
			else if (obj is string)
			{
				gameObject.SendMessage((string)obj, this.tweenArguments[text2], 1);
			}
			else
			{
				Debug.LogError("iTween Error: Callback method references must be passed as a delegate or string!");
				iTweenManager.Remove(this);
			}
		}
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x00066A8A File Offset: 0x00064C8A
	private void Dispose()
	{
		iTweenManager.Remove(this);
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x00066A94 File Offset: 0x00064C94
	private void ConflictCheck()
	{
		iTween next;
		while ((next = iTweenManager.GetIterator().GetNext()) != null)
		{
			if (!next.destroyed && !(next.gameObject != this.gameObject) && next != this)
			{
				if (next.type == "value")
				{
					return;
				}
				if (next.type == "timer")
				{
					return;
				}
				if (next.isRunning && next.type == this.type)
				{
					if (next.method != this.method)
					{
						return;
					}
					if (next.tweenArguments == null)
					{
						return;
					}
					if (next.tweenArguments.Count != this.tweenArguments.Count)
					{
						next.Dispose();
						next.CallBack(iTween.CallbackType.OnConflict);
						return;
					}
					foreach (object obj in this.tweenArguments)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						if (!next.tweenArguments.Contains(dictionaryEntry.Key))
						{
							next.Dispose();
							next.CallBack(iTween.CallbackType.OnConflict);
							return;
						}
						if (!next.tweenArguments[dictionaryEntry.Key].Equals(this.tweenArguments[dictionaryEntry.Key]) && (string)dictionaryEntry.Key != "id")
						{
							next.Dispose();
							next.CallBack(iTween.CallbackType.OnConflict);
							return;
						}
					}
					this.Dispose();
					this.CallBack(iTween.CallbackType.OnConflict);
				}
			}
		}
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x00066C64 File Offset: 0x00064E64
	private void EnableKinematic()
	{
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x00066C66 File Offset: 0x00064E66
	private void DisableKinematic()
	{
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x00066C68 File Offset: 0x00064E68
	private void ResumeDelay()
	{
		this.waitForDelay = true;
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x00066C71 File Offset: 0x00064E71
	private float linear(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value);
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x00066C7C File Offset: 0x00064E7C
	private float clerp(float start, float end, float value)
	{
		float num = 0f;
		float num2 = 360f;
		float num3 = Mathf.Abs((num2 - num) / 2f);
		float result;
		if (end - start < -num3)
		{
			float num4 = (num2 - start + end) * value;
			result = start + num4;
		}
		else if (end - start > num3)
		{
			float num4 = -(num2 - end + start) * value;
			result = start + num4;
		}
		else
		{
			result = start + (end - start) * value;
		}
		return result;
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x00066CF4 File Offset: 0x00064EF4
	private float spring(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * 3.1415927f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
		return start + (end - start) * value;
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x00066D58 File Offset: 0x00064F58
	private float easeInQuad(float start, float end, float value)
	{
		end -= start;
		return end * value * value + start;
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x00066D66 File Offset: 0x00064F66
	private float easeOutQuad(float start, float end, float value)
	{
		end -= start;
		return -end * value * (value - 2f) + start;
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x00066D7C File Offset: 0x00064F7C
	private float easeInOutQuad(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * value * value + start;
		}
		value -= 1f;
		return -end / 2f * (value * (value - 2f) - 1f) + start;
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x00066DD3 File Offset: 0x00064FD3
	private float easeInCubic(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value + start;
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x00066DE3 File Offset: 0x00064FE3
	private float easeOutCubic(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return end * (value * value * value + 1f) + start;
	}

	// Token: 0x060015B8 RID: 5560 RVA: 0x00066E04 File Offset: 0x00065004
	private float easeInOutCubic(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * value * value * value + start;
		}
		value -= 2f;
		return end / 2f * (value * value * value + 2f) + start;
	}

	// Token: 0x060015B9 RID: 5561 RVA: 0x00066E58 File Offset: 0x00065058
	private float easeInQuart(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value + start;
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x00066E6C File Offset: 0x0006506C
	private float easeOutQuart(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return -end * (value * value * value * value - 1f) + start;
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x00066E9C File Offset: 0x0006509C
	private float easeInOutQuart(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * value * value * value * value + start;
		}
		value -= 2f;
		return -end / 2f * (value * value * value * value - 2f) + start;
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x00066EF5 File Offset: 0x000650F5
	private float easeInQuint(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value * value * value + start;
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x00066F09 File Offset: 0x00065109
	private float easeOutQuint(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return end * (value * value * value * value * value + 1f) + start;
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x00066F2C File Offset: 0x0006512C
	private float easeInOutQuint(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * value * value * value * value * value + start;
		}
		value -= 2f;
		return end / 2f * (value * value * value * value * value + 2f) + start;
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x00066F88 File Offset: 0x00065188
	private float easeInSine(float start, float end, float value)
	{
		end -= start;
		return -end * Mathf.Cos(value / 1f * 1.5707964f) + end + start;
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x00066FA8 File Offset: 0x000651A8
	private float easeOutSine(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Sin(value / 1f * 1.5707964f) + start;
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x00066FC8 File Offset: 0x000651C8
	private float easeInOutSine(float start, float end, float value)
	{
		end -= start;
		return -end / 2f * (Mathf.Cos(3.1415927f * value / 1f) - 1f) + start;
	}

	// Token: 0x060015C2 RID: 5570 RVA: 0x00067000 File Offset: 0x00065200
	private float easeInExpo(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x00067033 File Offset: 0x00065233
	private float easeOutExpo(float start, float end, float value)
	{
		end -= start;
		return end * (-Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x0006705C File Offset: 0x0006525C
	private float easeInOutExpo(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
		}
		value -= 1f;
		return end / 2f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x000670CF File Offset: 0x000652CF
	private float easeInCirc(float start, float end, float value)
	{
		end -= start;
		return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x000670F0 File Offset: 0x000652F0
	private float easeOutCirc(float start, float end, float value)
	{
		value -= 1f;
		end -= start;
		return end * Mathf.Sqrt(1f - value * value) + start;
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x00067120 File Offset: 0x00065320
	private float easeInOutCirc(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return -end / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}
		value -= 2f;
		return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x00067190 File Offset: 0x00065390
	private float easeInBounce(float start, float end, float value)
	{
		end -= start;
		float num = 1f;
		return end - this.easeOutBounce(0f, end, num - value) + start;
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x000671BC File Offset: 0x000653BC
	private float easeOutBounce(float start, float end, float value)
	{
		value /= 1f;
		end -= start;
		if (value < 0.36363637f)
		{
			return end * (7.5625f * value * value) + start;
		}
		if (value < 0.72727275f)
		{
			value -= 0.54545456f;
			return end * (7.5625f * value * value + 0.75f) + start;
		}
		if ((double)value < 0.9090909090909091)
		{
			value -= 0.8181818f;
			return end * (7.5625f * value * value + 0.9375f) + start;
		}
		value -= 0.95454544f;
		return end * (7.5625f * value * value + 0.984375f) + start;
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x00067264 File Offset: 0x00065464
	private float easeInOutBounce(float start, float end, float value)
	{
		end -= start;
		float num = 1f;
		if (value < num / 2f)
		{
			return this.easeInBounce(0f, end, value * 2f) * 0.5f + start;
		}
		return this.easeOutBounce(0f, end, value * 2f - num) * 0.5f + end * 0.5f + start;
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x000672CC File Offset: 0x000654CC
	private float easeInBack(float start, float end, float value)
	{
		end -= start;
		value /= 1f;
		float num = 1.70158f;
		return end * value * value * ((num + 1f) * value - num) + start;
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x00067300 File Offset: 0x00065500
	private float easeOutBack(float start, float end, float value)
	{
		float num = 1.70158f;
		end -= start;
		value = value / 1f - 1f;
		return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x00067340 File Offset: 0x00065540
	private float easeInOutBack(float start, float end, float value)
	{
		float num = 1.70158f;
		end -= start;
		value /= 0.5f;
		if (value < 1f)
		{
			num *= 1.525f;
			return end / 2f * (value * value * ((num + 1f) * value - num)) + start;
		}
		value -= 2f;
		num *= 1.525f;
		return end / 2f * (value * value * ((num + 1f) * value + num) + 2f) + start;
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x000673C0 File Offset: 0x000655C0
	private float punch(float amplitude, float value)
	{
		if (value == 0f)
		{
			return 0f;
		}
		if (value == 1f)
		{
			return 0f;
		}
		float num = 0.3f;
		float num2 = num / 6.2831855f * Mathf.Asin(0f);
		return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num2) * 6.2831855f / num);
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x00067438 File Offset: 0x00065638
	private float easeInElastic(float start, float end, float value)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		if (value == 0f)
		{
			return start;
		}
		if ((value /= num) == 1f)
		{
			return start + end;
		}
		float num4;
		if (num3 == 0f || num3 < Mathf.Abs(end))
		{
			num3 = end;
			num4 = num2 / 4f;
		}
		else
		{
			num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
		}
		return -(num3 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2)) + start;
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x000674F0 File Offset: 0x000656F0
	private float easeOutElastic(float start, float end, float value)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		if (value == 0f)
		{
			return start;
		}
		if ((value /= num) == 1f)
		{
			return start + end;
		}
		float num4;
		if (num3 == 0f || num3 < Mathf.Abs(end))
		{
			num3 = end;
			num4 = num2 / 4f;
		}
		else
		{
			num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
		}
		return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) + end + start;
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x000675A0 File Offset: 0x000657A0
	private float easeOutElasticLight(float start, float end, float value)
	{
		if (value == 0f)
		{
			return start;
		}
		if (value == 1f)
		{
			return end;
		}
		end -= start;
		float num = 0.6f;
		float num2 = num / 4f;
		float num3 = end;
		return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value - num2) * 6.2831855f / num) + end + start;
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x00067604 File Offset: 0x00065804
	private float easeInOutElastic(float start, float end, float value)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		if (value == 0f)
		{
			return start;
		}
		if ((value /= num / 2f) == 2f)
		{
			return start + end;
		}
		float num4;
		if (num3 == 0f || num3 < Mathf.Abs(end))
		{
			num3 = end;
			num4 = num2 / 4f;
		}
		else
		{
			num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
		}
		if (value < 1f)
		{
			return -0.5f * (num3 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2)) + start;
		}
		return num3 * Mathf.Pow(2f, -10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) * 0.5f + end + start;
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x0006770C File Offset: 0x0006590C
	private float easeInSineOutExpo(float start, float end, float value)
	{
		if (value > start / 2f)
		{
			return this.easeOutExpo(start, end, value);
		}
		return this.easeInSine(start, end, value);
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x0006773C File Offset: 0x0006593C
	private float easeInExpoFirstHalf(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x00067770 File Offset: 0x00065970
	private float easeInExpoSecondHalf(float start, float end, float value)
	{
		end -= start;
		return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x000677A4 File Offset: 0x000659A4
	public override string ToString()
	{
		string name = this.gameObject.name;
		if (this.tweenArguments == null)
		{
			return string.Format("[iTween - gameObject={0}", name);
		}
		object obj = this.tweenArguments["type"];
		object obj2 = this.tweenArguments["method"];
		object obj3 = this.tweenArguments["name"];
		object obj4 = this.tweenArguments["id"];
		return string.Format("[iTween - gameObject={0} type={1} method={2} name={3} id={4}]", new object[]
		{
			name,
			obj,
			obj2,
			obj3,
			obj4
		});
	}

	// Token: 0x04000A77 RID: 2679
	private static GameObject cameraFade;

	// Token: 0x04000A78 RID: 2680
	private static int nextId = 1;

	// Token: 0x04000A79 RID: 2681
	public int id;

	// Token: 0x04000A7A RID: 2682
	public string type;

	// Token: 0x04000A7B RID: 2683
	public string method;

	// Token: 0x04000A7C RID: 2684
	public iTween.EaseType easeType;

	// Token: 0x04000A7D RID: 2685
	public float time;

	// Token: 0x04000A7E RID: 2686
	public float delay;

	// Token: 0x04000A7F RID: 2687
	public iTween.LoopType loopType;

	// Token: 0x04000A80 RID: 2688
	public bool isRunning;

	// Token: 0x04000A81 RID: 2689
	public bool isPaused;

	// Token: 0x04000A82 RID: 2690
	public string _name;

	// Token: 0x04000A83 RID: 2691
	private bool waitForDelay;

	// Token: 0x04000A84 RID: 2692
	private float runningTime;

	// Token: 0x04000A85 RID: 2693
	private float percentage;

	// Token: 0x04000A86 RID: 2694
	private float delayStarted;

	// Token: 0x04000A87 RID: 2695
	private bool kinematic;

	// Token: 0x04000A88 RID: 2696
	private bool isLocal;

	// Token: 0x04000A89 RID: 2697
	private bool loop;

	// Token: 0x04000A8A RID: 2698
	private bool reverse;

	// Token: 0x04000A8B RID: 2699
	private bool physics;

	// Token: 0x04000A8C RID: 2700
	private Hashtable tweenArguments;

	// Token: 0x04000A8D RID: 2701
	private Space space;

	// Token: 0x04000A8E RID: 2702
	private iTween.EasingFunction ease;

	// Token: 0x04000A8F RID: 2703
	private iTween.ApplyTween apply;

	// Token: 0x04000A90 RID: 2704
	private AudioSource audioSource;

	// Token: 0x04000A91 RID: 2705
	private Vector3[] vector3s;

	// Token: 0x04000A92 RID: 2706
	private Vector2[] vector2s;

	// Token: 0x04000A93 RID: 2707
	private Color[,] colors;

	// Token: 0x04000A94 RID: 2708
	private float[] floats;

	// Token: 0x04000A95 RID: 2709
	private Rect[] rects;

	// Token: 0x04000A96 RID: 2710
	private iTween.CRSpline path;

	// Token: 0x04000A97 RID: 2711
	private Vector3 preUpdate;

	// Token: 0x04000A98 RID: 2712
	private Vector3 postUpdate;

	// Token: 0x04000A99 RID: 2713
	private iTween.NamedValueColor namedcolorvalue;

	// Token: 0x04000A9A RID: 2714
	private string namedColorValueString;

	// Token: 0x04000A9B RID: 2715
	private float lastRealTime;

	// Token: 0x04000A9C RID: 2716
	private bool useRealTime;

	// Token: 0x04000A9D RID: 2717
	public GameObject gameObject;

	// Token: 0x04000A9E RID: 2718
	public bool enabled = true;

	// Token: 0x04000A9F RID: 2719
	public bool activeLastTick;

	// Token: 0x04000AA0 RID: 2720
	public bool destroyed;

	// Token: 0x04000AA1 RID: 2721
	private static string[] CALLBACK_NAMES = new string[]
	{
		"onstart",
		"onupdate",
		"oncomplete",
		"onconflict"
	};

	// Token: 0x04000AA2 RID: 2722
	private static string[] CALLBACK_TARGET_NAMES = new string[]
	{
		"onstarttarget",
		"onupdatetarget",
		"oncompletetarget",
		"onconflicttarget"
	};

	// Token: 0x04000AA3 RID: 2723
	private static string[] CALLBACK_PARAMS_NAMES = new string[]
	{
		"onstartparams",
		"onupdateparams",
		"oncompleteparams",
		"onconflictparams"
	};

	// Token: 0x020001D3 RID: 467
	public enum EaseType
	{
		// Token: 0x0400100F RID: 4111
		easeInQuad,
		// Token: 0x04001010 RID: 4112
		easeOutQuad,
		// Token: 0x04001011 RID: 4113
		easeInOutQuad,
		// Token: 0x04001012 RID: 4114
		easeInCubic,
		// Token: 0x04001013 RID: 4115
		easeOutCubic,
		// Token: 0x04001014 RID: 4116
		easeInOutCubic,
		// Token: 0x04001015 RID: 4117
		easeInQuart,
		// Token: 0x04001016 RID: 4118
		easeOutQuart,
		// Token: 0x04001017 RID: 4119
		easeInOutQuart,
		// Token: 0x04001018 RID: 4120
		easeInQuint,
		// Token: 0x04001019 RID: 4121
		easeOutQuint,
		// Token: 0x0400101A RID: 4122
		easeInOutQuint,
		// Token: 0x0400101B RID: 4123
		easeInSine,
		// Token: 0x0400101C RID: 4124
		easeOutSine,
		// Token: 0x0400101D RID: 4125
		easeInOutSine,
		// Token: 0x0400101E RID: 4126
		easeInExpo,
		// Token: 0x0400101F RID: 4127
		easeOutExpo,
		// Token: 0x04001020 RID: 4128
		easeInOutExpo,
		// Token: 0x04001021 RID: 4129
		easeInCirc,
		// Token: 0x04001022 RID: 4130
		easeOutCirc,
		// Token: 0x04001023 RID: 4131
		easeInOutCirc,
		// Token: 0x04001024 RID: 4132
		linear,
		// Token: 0x04001025 RID: 4133
		spring,
		// Token: 0x04001026 RID: 4134
		easeInBounce,
		// Token: 0x04001027 RID: 4135
		easeOutBounce,
		// Token: 0x04001028 RID: 4136
		easeInOutBounce,
		// Token: 0x04001029 RID: 4137
		easeInBack,
		// Token: 0x0400102A RID: 4138
		easeOutBack,
		// Token: 0x0400102B RID: 4139
		easeInOutBack,
		// Token: 0x0400102C RID: 4140
		easeInElastic,
		// Token: 0x0400102D RID: 4141
		easeOutElastic,
		// Token: 0x0400102E RID: 4142
		easeInOutElastic,
		// Token: 0x0400102F RID: 4143
		punch,
		// Token: 0x04001030 RID: 4144
		easeInSineOutExpo,
		// Token: 0x04001031 RID: 4145
		easeOutElasticLight
	}

	// Token: 0x020001D7 RID: 471
	public enum LoopType
	{
		// Token: 0x04001040 RID: 4160
		none,
		// Token: 0x04001041 RID: 4161
		loop,
		// Token: 0x04001042 RID: 4162
		pingPong
	}

	// Token: 0x020001D8 RID: 472
	public static class Defaults
	{
		// Token: 0x04001043 RID: 4163
		public static float time = 1f;

		// Token: 0x04001044 RID: 4164
		public static float delay = 0f;

		// Token: 0x04001045 RID: 4165
		public static iTween.NamedValueColor namedColorValue = iTween.NamedValueColor._Color;

		// Token: 0x04001046 RID: 4166
		public static iTween.LoopType loopType = iTween.LoopType.none;

		// Token: 0x04001047 RID: 4167
		public static iTween.EaseType easeType = iTween.EaseType.easeOutExpo;

		// Token: 0x04001048 RID: 4168
		public static float lookSpeed = 3f;

		// Token: 0x04001049 RID: 4169
		public static bool isLocal = false;

		// Token: 0x0400104A RID: 4170
		public static Space space = 1;

		// Token: 0x0400104B RID: 4171
		public static bool orientToPath = false;

		// Token: 0x0400104C RID: 4172
		public static Color color = Color.white;

		// Token: 0x0400104D RID: 4173
		public static float updateTimePercentage = 0.05f;

		// Token: 0x0400104E RID: 4174
		public static float updateTime = 1f * iTween.Defaults.updateTimePercentage;

		// Token: 0x0400104F RID: 4175
		public static int cameraFadeDepth = 999999;

		// Token: 0x04001050 RID: 4176
		public static float lookAhead = 0.05f;

		// Token: 0x04001051 RID: 4177
		public static bool useRealTime = false;

		// Token: 0x04001052 RID: 4178
		public static Vector3 up = Vector3.up;
	}

	// Token: 0x020001D9 RID: 473
	public enum NamedValueColor
	{
		// Token: 0x04001054 RID: 4180
		_Color,
		// Token: 0x04001055 RID: 4181
		_SpecColor,
		// Token: 0x04001056 RID: 4182
		_Emission,
		// Token: 0x04001057 RID: 4183
		_ReflectColor
	}

	// Token: 0x020001DA RID: 474
	private enum CallbackType
	{
		// Token: 0x04001059 RID: 4185
		OnStart,
		// Token: 0x0400105A RID: 4186
		OnUpdate,
		// Token: 0x0400105B RID: 4187
		OnComplete,
		// Token: 0x0400105C RID: 4188
		OnConflict
	}

	// Token: 0x020001DB RID: 475
	private class CRSpline
	{
		// Token: 0x06001D85 RID: 7557 RVA: 0x0008A068 File Offset: 0x00088268
		public CRSpline(params Vector3[] pts)
		{
			this.pts = new Vector3[pts.Length];
			Array.Copy(pts, this.pts, pts.Length);
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x0008A090 File Offset: 0x00088290
		public Vector3 Interp(float t)
		{
			int num = this.pts.Length - 3;
			int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
			float num3 = t * (float)num - (float)num2;
			Vector3 vector = this.pts[num2];
			Vector3 vector2 = this.pts[num2 + 1];
			Vector3 vector3 = this.pts[num2 + 2];
			Vector3 vector4 = this.pts[num2 + 3];
			return 0.5f * ((-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num3 * num3) + (-vector + vector3) * num3 + 2f * vector2);
		}

		// Token: 0x0400105D RID: 4189
		public Vector3[] pts;
	}

	// Token: 0x020001DC RID: 476
	// (Invoke) Token: 0x06001D88 RID: 7560
	private delegate float EasingFunction(float start, float end, float value);

	// Token: 0x020001DD RID: 477
	// (Invoke) Token: 0x06001D8C RID: 7564
	private delegate void ApplyTween();
}
