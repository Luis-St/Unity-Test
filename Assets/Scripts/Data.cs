using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Data {

	[Serializable]
	public class ShaderData {
		public ComputeShader @base;
		public ComputeShader overlay;
		public RenderTexture texture;

		public void CreateOrUpdateTexture(TextureData data) {
			if (texture == null || data.HasChanged()) {
				var y = data.sizeY;
				if (data.forceAspectRatio) {
					y = (data.sizeX / 16) * 9;
				}
				texture = new RenderTexture(data.sizeX, y, 24) {
					enableRandomWrite = true
				};
				texture.Create();
			}
		}
		
		public bool IsValid() => @base != null && texture != null && texture.width > 0 && texture.height > 0;
		
		public void DispatchBase(ScriptProperties properties, float startTime) {
			var kernelId = @base.FindKernel("CSMain");
			@base.SetTexture(kernelId, "Result", texture);
			@base.SetVector("Color", properties.color);
			@base.SetFloat("Time", Time.time);
			@base.SetFloat("StartTime", startTime);
			@base.SetFloat("Width", texture.width);
			@base.SetFloat("Height", texture.height);
			@base.SetFloats("Size", properties.sizeX, properties.sizeY);
			@base.SetInt("Count", properties.count);
			@base.SetFloats("Degree", properties.degreeStart, properties.degreeEnd);
			@base.Dispatch(kernelId, texture.width / 8, texture.height / 8, 1);
		}
	}

	[Serializable]
	public class TextureData {
		[Min(1)] public int sizeX;
		[Min(1)] public int sizeY;
		public bool forceAspectRatio;
		private int oldSizeX;
		private int oldSizeY;
		private bool oldForceAspectRatio;
		
		public bool HasChanged() => sizeX != oldSizeX || sizeY != oldSizeY || forceAspectRatio != oldForceAspectRatio;

		public void Update() {
			oldSizeX = sizeX;
			oldSizeY = sizeY;
			oldForceAspectRatio = forceAspectRatio;
		}
	}

	[Serializable]
	public class ScriptProperties {
		public Color color;
		[Min(1.0f)] public float sizeX;
		[Min(1.0f)] public float sizeY;
		[Range(1, 100)] public int count;
		[Range(0.0f, 360.0f)] public float degreeStart;
		[Range(0.0f, 360.0f)] public float degreeEnd;
	}

}