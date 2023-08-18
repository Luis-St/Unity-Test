using System;
using UnityEngine;

public class ShaderScript : MonoBehaviour {
	public ComputeShader computeShader;
	public RenderTexture renderTexture;
	public int textureSizeX = 256;
	public int textureSizeY = 256;
	public int threadDivisor = 8;
	public int sizeX = 128;
	public int sizeY = 128;
	public bool forceAspectRatio;
	#region Internal
	private int oldTextureSizeX;
	private int oldTextureSizeY;
	private bool oldForceAspectRatio;
	private int kernelId;
	#endregion

	private void OnEnable() 
	{
		kernelId = computeShader.FindKernel("CSMain");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (textureSizeX != oldTextureSizeX || textureSizeY != oldTextureSizeY || forceAspectRatio != oldForceAspectRatio) {
			if (forceAspectRatio) {
				textureSizeY = (textureSizeX / 16) * 9;
			}
			renderTexture = new RenderTexture(Math.Max(1, textureSizeX), Math.Max(1, textureSizeY), 24) {
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
        
		computeShader.Dispatch(kernelId, textureSizeX / threadDivisor, textureSizeY / threadDivisor, 1);
		Graphics.Blit(renderTexture, destination);
		
		oldTextureSizeX = textureSizeX;
		oldTextureSizeY = textureSizeY;
		oldForceAspectRatio = forceAspectRatio;
	}
}