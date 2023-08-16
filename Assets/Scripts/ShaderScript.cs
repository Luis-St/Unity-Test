using UnityEngine;
using UnityEngine.Serialization;

public class ShaderScript : MonoBehaviour {
	public ComputeShader computeShader;
	public RenderTexture renderTexture;
	public int textureSize = 256;
	public int threadDivisor = 8;
	public int sizeX = 128;
	public int sizeY = 128;
	#region Internal
	private int oldTextureSize;
	private int oldThreadDivisor;
	private int kernelId;
	#endregion

	private void OnEnable() {
		kernelId = computeShader.FindKernel("CSMain");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (textureSize != oldTextureSize || threadDivisor != oldThreadDivisor) {
			renderTexture = new RenderTexture(textureSize, textureSize, 24) {
				enableRandomWrite = true
			};
		}
		
		computeShader.SetTexture(kernelId, "Result", renderTexture);
		computeShader.SetFloat("Time", Time.time);
		computeShader.SetFloat("Width", renderTexture.width);
		computeShader.SetFloat("Height", renderTexture.height);
		computeShader.SetFloats("Size", sizeX, sizeY);

		computeShader.Dispatch(kernelId, textureSize / threadDivisor, textureSize / threadDivisor, 1);
		Graphics.Blit(renderTexture, destination);
		
		oldTextureSize = textureSize;
		oldThreadDivisor = threadDivisor;
	}
}