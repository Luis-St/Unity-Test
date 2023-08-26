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
			if (texture != null && !data.HasChanged()) {
				return;
			}
			var y = data.sizeY;
			if (data.forceAspectRatio) {
				y = (data.sizeX / 16) * 9;
			}
			texture = new RenderTexture(data.sizeX, y, 24) {
				enableRandomWrite = true
			};
			texture.Create();
		}
		
		public bool IsValid() => @base != null && texture != null && texture.width > 0 && texture.height > 0;
		
		public void DispatchBase(ScriptProperties properties, float startTime) => Dispatch(@base, @base.FindKernel("CSMain"), properties, false, startTime);
		
		public void DispatchOverlay(ScriptProperties properties, float startTime) {
			if (overlay == null) {
				return;
			}
			Dispatch(overlay, overlay.FindKernel("CSMain"), properties, true, startTime);
		}

		private void Dispatch(ComputeShader shader, int kernelId, ScriptProperties properties, bool isOverlay, float startTime) {
			shader.SetTexture(kernelId, "Result", texture);
			shader.SetFloat("Width", texture.width);
			shader.SetFloat("Height", texture.height);
			shader.SetFloat("Time", Time.time);
			shader.SetFloat("StartTime", startTime);
			shader.SetVector("ForegroundColor", properties.foregroundColor);
			shader.SetVector("BackgroundColor", properties.backgroundColor);
			shader.SetBool("Overlay", isOverlay);
			shader.SetFloats("Size", properties.sizeX, properties.sizeY);
			shader.SetInt("Count", properties.count);
			shader.SetFloats("Degree", properties.degreeStart, properties.degreeEnd);
			shader.SetBool("Invert", properties.invert);
			shader.Dispatch(kernelId, texture.width / 8, texture.height / 8, 1);
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
		public Color foregroundColor;
		public Color backgroundColor;
		[Min(1.0f)] public float sizeX;
		[Min(1.0f)] public float sizeY;
		[Range(1, 100)] public int count;
		[Range(0.0f, 360.0f)] public float degreeStart;
		[Range(0.0f, 360.0f)] public float degreeEnd;
		public bool invert;
	}

}