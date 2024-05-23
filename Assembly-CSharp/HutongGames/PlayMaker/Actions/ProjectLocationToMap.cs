using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C72 RID: 3186
	[ActionCategory(33)]
	[Tooltip("Projects the location found with Get Location Info to a 2d map using common projections.")]
	public class ProjectLocationToMap : FsmStateAction
	{
		// Token: 0x06006741 RID: 26433 RVA: 0x001E736C File Offset: 0x001E556C
		public override void Reset()
		{
			this.GPSLocation = new FsmVector3
			{
				UseVariable = true
			};
			this.mapProjection = ProjectLocationToMap.MapProjection.EquidistantCylindrical;
			this.minLongitude = -180f;
			this.maxLongitude = 180f;
			this.minLatitude = -90f;
			this.maxLatitude = 90f;
			this.minX = 0f;
			this.minY = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
			this.projectedX = null;
			this.projectedY = null;
			this.everyFrame = false;
		}

		// Token: 0x06006742 RID: 26434 RVA: 0x001E7435 File Offset: 0x001E5635
		public override void OnEnter()
		{
			if (this.GPSLocation.IsNone)
			{
				base.Finish();
				return;
			}
			this.DoProjectGPSLocation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006743 RID: 26435 RVA: 0x001E7465 File Offset: 0x001E5665
		public override void OnUpdate()
		{
			this.DoProjectGPSLocation();
		}

		// Token: 0x06006744 RID: 26436 RVA: 0x001E7470 File Offset: 0x001E5670
		private void DoProjectGPSLocation()
		{
			this.x = Mathf.Clamp(this.GPSLocation.Value.x, this.minLongitude.Value, this.maxLongitude.Value);
			this.y = Mathf.Clamp(this.GPSLocation.Value.y, this.minLatitude.Value, this.maxLatitude.Value);
			ProjectLocationToMap.MapProjection mapProjection = this.mapProjection;
			if (mapProjection != ProjectLocationToMap.MapProjection.EquidistantCylindrical)
			{
				if (mapProjection == ProjectLocationToMap.MapProjection.Mercator)
				{
					this.DoMercatorProjection();
				}
			}
			else
			{
				this.DoEquidistantCylindrical();
			}
			this.x *= this.width.Value;
			this.y *= this.height.Value;
			this.projectedX.Value = ((!this.normalized.Value) ? (this.minX.Value + this.x * (float)Screen.width) : (this.minX.Value + this.x));
			this.projectedY.Value = ((!this.normalized.Value) ? (this.minY.Value + this.y * (float)Screen.height) : (this.minY.Value + this.y));
		}

		// Token: 0x06006745 RID: 26437 RVA: 0x001E75DC File Offset: 0x001E57DC
		private void DoEquidistantCylindrical()
		{
			this.x = (this.x - this.minLongitude.Value) / (this.maxLongitude.Value - this.minLongitude.Value);
			this.y = (this.y - this.minLatitude.Value) / (this.maxLatitude.Value - this.minLatitude.Value);
		}

		// Token: 0x06006746 RID: 26438 RVA: 0x001E764C File Offset: 0x001E584C
		private void DoMercatorProjection()
		{
			this.x = (this.x - this.minLongitude.Value) / (this.maxLongitude.Value - this.minLongitude.Value);
			float num = ProjectLocationToMap.LatitudeToMercator(this.minLatitude.Value);
			float num2 = ProjectLocationToMap.LatitudeToMercator(this.maxLatitude.Value);
			this.y = (ProjectLocationToMap.LatitudeToMercator(this.GPSLocation.Value.y) - num) / (num2 - num);
		}

		// Token: 0x06006747 RID: 26439 RVA: 0x001E76D0 File Offset: 0x001E58D0
		private static float LatitudeToMercator(float latitudeInDegrees)
		{
			float num = Mathf.Clamp(latitudeInDegrees, -85f, 85f);
			num = 0.017453292f * num;
			return Mathf.Log(Mathf.Tan(num / 2f + 0.7853982f));
		}

		// Token: 0x04004F05 RID: 20229
		[Tooltip("Location vector in degrees longitude and latitude. Typically returned by the Get Location Info action.")]
		public FsmVector3 GPSLocation;

		// Token: 0x04004F06 RID: 20230
		[Tooltip("The projection used by the map.")]
		public ProjectLocationToMap.MapProjection mapProjection;

		// Token: 0x04004F07 RID: 20231
		[HasFloatSlider(-180f, 180f)]
		[ActionSection("Map Region")]
		public FsmFloat minLongitude;

		// Token: 0x04004F08 RID: 20232
		[HasFloatSlider(-180f, 180f)]
		public FsmFloat maxLongitude;

		// Token: 0x04004F09 RID: 20233
		[HasFloatSlider(-90f, 90f)]
		public FsmFloat minLatitude;

		// Token: 0x04004F0A RID: 20234
		[HasFloatSlider(-90f, 90f)]
		public FsmFloat maxLatitude;

		// Token: 0x04004F0B RID: 20235
		[ActionSection("Screen Region")]
		public FsmFloat minX;

		// Token: 0x04004F0C RID: 20236
		public FsmFloat minY;

		// Token: 0x04004F0D RID: 20237
		public FsmFloat width;

		// Token: 0x04004F0E RID: 20238
		public FsmFloat height;

		// Token: 0x04004F0F RID: 20239
		[UIHint(10)]
		[ActionSection("Projection")]
		[Tooltip("Store the projected X coordinate in a Float Variable. Use this to display a marker on the map.")]
		public FsmFloat projectedX;

		// Token: 0x04004F10 RID: 20240
		[UIHint(10)]
		[Tooltip("Store the projected Y coordinate in a Float Variable. Use this to display a marker on the map.")]
		public FsmFloat projectedY;

		// Token: 0x04004F11 RID: 20241
		[Tooltip("If true all coordinates in this action are normalized (0-1); otherwise coordinates are in pixels.")]
		public FsmBool normalized;

		// Token: 0x04004F12 RID: 20242
		public bool everyFrame;

		// Token: 0x04004F13 RID: 20243
		private float x;

		// Token: 0x04004F14 RID: 20244
		private float y;

		// Token: 0x02000C73 RID: 3187
		public enum MapProjection
		{
			// Token: 0x04004F16 RID: 20246
			EquidistantCylindrical,
			// Token: 0x04004F17 RID: 20247
			Mercator
		}
	}
}
