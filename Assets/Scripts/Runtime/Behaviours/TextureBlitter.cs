using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	[ExecuteInEditMode]
	public class TextureBlitter : MonoBehaviour
	{
		[SerializeField] private Material screenEffectMaterial = default;

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, screenEffectMaterial);
		}
	}
}