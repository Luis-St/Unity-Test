using Data;
using UnityEngine;
using UnityEngine.Serialization;

public class ShaderScript : MonoBehaviour {
	public ShaderData shader = new();
	public TextureData texture = new() {
		sizeX = 1920, sizeY = 1080, forceAspectRatio = true
	};
	public ScriptProperties properties = new() {
		color = Color.white, sizeX = 1024.0f, sizeY = 1024.0f, count = 1, degreeStart = 0.0f, degreeEnd = 360.0f
	};
	private float startTime;

	private void OnEnable() {
		startTime = Time.time;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		shader.CreateOrUpdateTexture(texture);
		if (!shader.IsValid()) {
			return;
		}
		
		shader.DispatchBase(properties, startTime);
		Graphics.Blit(shader.texture, destination);
		
		texture.Update();
	}
}