using Data;
using UnityEngine;
using UnityEngine.Serialization;

public class ShaderScript : MonoBehaviour {
	public ShaderData shader = new();
	public TextureData texture = new() {
		sizeX = 1920, sizeY = 1080, forceAspectRatio = true
	};
	public ScriptProperties properties = new() {
		foregroundColor = Color.white, backgroundColor = Color.black, sizeX = 1024.0f, sizeY = 1024.0f, count = 1, speed = 1.0f, degreeStart = 0.0f, degreeEnd = 360.0f, invert = false
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
		shader.DispatchOverlay(properties, startTime);
		Graphics.Blit(shader.texture, destination);
		
		texture.Update();
	}
}