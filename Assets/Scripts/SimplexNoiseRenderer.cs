using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

internal struct Vec {
	public int x, y, z;
}

public class SimplexNoiseRenderer : MonoBehaviour {
	
	private static readonly Random RNG = new Random(10);
	private static readonly Vec[] GRADIENT = {
		new Vec() {x = 1, y = 1, z = 0}, new Vec() {x = -1, y = 1, z = 0}, new Vec() {x = 1, y = -1, z = 0}, new Vec() {x = -1, y = -1, z = 0}, 
		new Vec() {x = 1, y = 0, z = 1}, new Vec() {x = -1, y = 0, z = 1}, new Vec() {x = 1, y = 0, z = -1}, new Vec() {x = -1, y = 0, z = -1}, 
		new Vec() {x = 0, y = 1, z = 1}, new Vec() {x = 0, y = -1, z = 1}, new Vec() {x = 0, y = 1, z = -1}, new Vec() {x = 0, y = -1, z = -1}, 
		new Vec() {x = 1, y = 1, z = 0}, new Vec() {x = 0, y = -1, z = 1}, new Vec() {x = -1, y = 1, z = 0}, new Vec() {x = 0, y = -1, z = -1}
	};
	
	private readonly int[] permutation = new int[512];
	public ComputeShader computeShader;
	public RenderTexture renderTexture;
	private int shaderId;

	private void OnEnable() {
		renderTexture = new RenderTexture(1920, 1080, 24) {
			enableRandomWrite = true
		};
		renderTexture.Create();
		shaderId = computeShader.FindKernel("CSMain");
		for (var i = 0; i < 256; this.permutation[i] = i++) {}
		for (var i = 0; i < 256; ++i) {
			var offset = RNG.Next(256 - i);
			var point = permutation[i];
			this.permutation[i] = permutation[offset + i];
			this.permutation[offset + i] = point;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		computeShader.SetTexture(shaderId, "Result", renderTexture);
		
		var gradientBuffer = new ComputeBuffer(GRADIENT.Length, sizeof(int) * 3);
		gradientBuffer.SetData(GRADIENT);
		computeShader.SetBuffer(shaderId, "Gradient", gradientBuffer);
		
		var permutationBuffer = new ComputeBuffer(permutation.Length, sizeof(int));
		permutationBuffer.SetData(permutation);
		computeShader.SetBuffer(shaderId, "Permutation", permutationBuffer);
		
		computeShader.SetInts("StartPos", 0, 0);
		computeShader.SetInts("Size", renderTexture.width, renderTexture.height);
		
		computeShader.Dispatch(shaderId, renderTexture.width / 16, renderTexture.height / 16, 1);
		Graphics.Blit(renderTexture, destination);
		gradientBuffer.Release();
		permutationBuffer.Release();
	}
}