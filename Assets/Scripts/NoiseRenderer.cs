using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseRenderer : MonoBehaviour {

	public ComputeShader computeShader;
	public RenderTexture renderTexture;
	private int shaderId;

	private void OnEnable() {
		renderTexture = new RenderTexture(1024, 1024, 24) {
			enableRandomWrite = true
		};
		renderTexture.Create();
		shaderId = computeShader.FindKernel("CSMain");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		computeShader.SetTexture(shaderId, "Result", renderTexture);
		computeShader.SetFloat("XResolution", renderTexture.width);
		computeShader.SetFloat("YResolution", renderTexture.height);
        
		computeShader.Dispatch(shaderId, renderTexture.width / 8, renderTexture.height / 8, 1);
		Graphics.Blit(renderTexture, destination);
	}
}