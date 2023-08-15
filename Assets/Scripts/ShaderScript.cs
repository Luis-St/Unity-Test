using UnityEngine;

public class ShaderScript : MonoBehaviour {
	public ComputeShader computeShader;
	public RenderTexture renderTexture;
	public int textureSize = 256;
	private int oldTextureSize;
	public int textureDepth = 24;
	private int oldTextureDepth;
	public int threadDivisor = 8;
	public int oldThreadDivisor;
	private int kernelId;

	private void OnEnable() {
		kernelId = computeShader.FindKernel("CSMain");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (textureSize != oldTextureSize || textureDepth != oldTextureDepth || threadDivisor != oldThreadDivisor) {
			renderTexture = new RenderTexture(textureSize, textureSize, textureDepth) {
				enableRandomWrite = true
			};
			oldTextureSize = textureSize;
			oldTextureDepth = textureDepth;
			oldThreadDivisor = threadDivisor;
		}
		
		computeShader.SetTexture(kernelId, "Result", renderTexture);
		computeShader.SetFloat("Width", renderTexture.width);
		computeShader.SetFloat("Height", renderTexture.height);

		computeShader.Dispatch(kernelId, textureSize / threadDivisor, textureSize / threadDivisor, 1);
		Graphics.Blit(renderTexture, destination);
        
		{
			oldTextureSize = textureSize;
			oldTextureDepth = textureDepth;
			oldThreadDivisor = threadDivisor;
		}
	}
}