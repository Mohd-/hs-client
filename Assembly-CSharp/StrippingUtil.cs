using System;
using UnityEngine;

// Token: 0x020006B3 RID: 1715
public class StrippingUtil : MonoBehaviour
{
	// Token: 0x060047AA RID: 18346 RVA: 0x00157E3C File Offset: 0x0015603C
	public void SeedMonotouchAheadOfTimeCompile()
	{
		ParticleSystem particleSystem = null;
		particleSystem.playbackSpeed += 1f;
		particleSystem.startLifetime += 1f;
		particleSystem.startSpeed += 1f;
		particleSystem.startDelay += 1f;
		particleSystem.startRotation += 1f;
		particleSystem.emissionRate += 1f;
		RenderToTexture renderToTexture = null;
		renderToTexture.m_ObjectToRender = null;
		renderToTexture.m_RealtimeRender &= true;
		renderToTexture.enabled &= true;
		FullScreenEffects fullScreenEffects = null;
		fullScreenEffects.BlendToColorAmount += 1f;
		fullScreenEffects.VignettingEnable &= true;
		fullScreenEffects.VignettingIntensity += 1f;
		MeshFilter meshFilter = null;
		meshFilter.mesh = null;
		RotateOverTime rotateOverTime = null;
		rotateOverTime.RotateSpeedX += 1f;
		rotateOverTime.RotateSpeedY += 1f;
		rotateOverTime.RotateSpeedZ += 1f;
	}

	// Token: 0x04002EEF RID: 12015
	private MeshRenderer dummy_2;

	// Token: 0x04002EF0 RID: 12016
	private Avatar dummy_3;

	// Token: 0x04002EF1 RID: 12017
	private AudioReverbZone dummy_4;

	// Token: 0x04002EF2 RID: 12018
	private GameObject dummy_5;

	// Token: 0x04002EF3 RID: 12019
	private Component dummy_6;

	// Token: 0x04002EF4 RID: 12020
	private Transform dummy_8;

	// Token: 0x04002EF5 RID: 12021
	private Behaviour dummy_11;

	// Token: 0x04002EF6 RID: 12022
	private ParticleAnimator dummy_14;

	// Token: 0x04002EF7 RID: 12023
	private Camera dummy_19;

	// Token: 0x04002EF8 RID: 12024
	private Material dummy_20;

	// Token: 0x04002EF9 RID: 12025
	private MeshRenderer dummy_21;

	// Token: 0x04002EFA RID: 12026
	private Renderer dummy_22;

	// Token: 0x04002EFB RID: 12027
	private ParticleRenderer dummy_23;

	// Token: 0x04002EFC RID: 12028
	private Texture dummy_24;

	// Token: 0x04002EFD RID: 12029
	private Texture2D dummy_25;

	// Token: 0x04002EFE RID: 12030
	private MeshFilter dummy_28;

	// Token: 0x04002EFF RID: 12031
	private OcclusionPortal dummy_29;

	// Token: 0x04002F00 RID: 12032
	private Mesh dummy_30;

	// Token: 0x04002F01 RID: 12033
	private Skybox dummy_31;

	// Token: 0x04002F02 RID: 12034
	private QualitySettings dummy_32;

	// Token: 0x04002F03 RID: 12035
	private Shader dummy_33;

	// Token: 0x04002F04 RID: 12036
	private TextAsset dummy_34;

	// Token: 0x04002F05 RID: 12037
	private Rigidbody dummy_36;

	// Token: 0x04002F06 RID: 12038
	private Collider dummy_38;

	// Token: 0x04002F07 RID: 12039
	private Joint dummy_39;

	// Token: 0x04002F08 RID: 12040
	private HingeJoint dummy_40;

	// Token: 0x04002F09 RID: 12041
	private MeshCollider dummy_41;

	// Token: 0x04002F0A RID: 12042
	private BoxCollider dummy_42;

	// Token: 0x04002F0B RID: 12043
	private AnimationClip dummy_44;

	// Token: 0x04002F0C RID: 12044
	private ConstantForce dummy_45;

	// Token: 0x04002F0D RID: 12045
	private AudioListener dummy_48;

	// Token: 0x04002F0E RID: 12046
	private AudioSource dummy_49;

	// Token: 0x04002F0F RID: 12047
	private AudioClip dummy_50;

	// Token: 0x04002F10 RID: 12048
	private RenderTexture dummy_51;

	// Token: 0x04002F11 RID: 12049
	private ParticleEmitter dummy_53;

	// Token: 0x04002F12 RID: 12050
	private Animator dummy_54;

	// Token: 0x04002F13 RID: 12051
	private TrailRenderer dummy_55;

	// Token: 0x04002F14 RID: 12052
	private ParticleSystem dummy_56;

	// Token: 0x04002F15 RID: 12053
	private ConfigurableJoint dummy_57;

	// Token: 0x04002F16 RID: 12054
	private FixedJoint dummy_58;

	// Token: 0x04002F17 RID: 12055
	private WindZone dummy_59;
}
