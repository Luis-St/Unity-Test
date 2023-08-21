using System;
using UnityEngine;

public class ShaderScript : MonoBehaviour {
	public ComputeShader computeShader;
	public RenderTexture renderTexture;
	[Min(1)] public int textureSizeX = 1920;
	[Min(1)] public int textureSizeY = 1080;
	public bool forceAspectRatio = true;
	[Min(1)] public int threadDivisor = 8;
	[Min(1.0f)] public float sizeX = 1024.0f;
	[Min(1.0f)] public float sizeY = 1024.0f;
	[Range(0.0f, 360.0f)] public float degreeStart = 0.0f;
	[Range(0.0f, 360.0f)] public float degreeEnd = 360.0f;

	#region Internal

	private int oldTextureSizeX;
	private int oldTextureSizeY;
	private bool oldForceAspectRatio;
	private int kernelId;

	#endregion

	private void OnEnable() {
		kernelId = computeShader.FindKernel("CSMain");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (textureSizeX != oldTextureSizeX || textureSizeY != oldTextureSizeY || forceAspectRatio != oldForceAspectRatio) {
			if (forceAspectRatio) {
				textureSizeY = (textureSizeX / 16) * 9;
			}
			renderTexture = new RenderTexture(textureSizeX, textureSizeY, 24) {
				enableRandomWrite = true
			};
			renderTexture.Create();
		}
		if (0 > renderTexture.width || 0 > renderTexture.height) {
			return;
		}
		computeShader.SetTexture(kernelId, "Result", renderTexture);
		computeShader.SetFloat("Time", Time.time);
		computeShader.SetFloat("Width", renderTexture.width);
		computeShader.SetFloat("Height", renderTexture.height);
		computeShader.SetFloats("Size", sizeX, sizeY);
		computeShader.SetFloats("Degree", degreeStart, degreeEnd);

		computeShader.Dispatch(kernelId, textureSizeX / threadDivisor, textureSizeY / threadDivisor, 1);
		Graphics.Blit(renderTexture, destination);

		oldTextureSizeX = textureSizeX;
		oldTextureSizeY = textureSizeY;
		oldForceAspectRatio = forceAspectRatio;
	}
}