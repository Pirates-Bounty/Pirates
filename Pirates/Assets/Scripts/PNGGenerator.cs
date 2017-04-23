using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PNGGenerator : MonoBehaviour {
	public int size;
	// Use this for initialization
	void Start () {
		GeneratePNGs ();
	}

	private void GeneratePNGs(){
		for (int i = 0; i < 256; ++i) {
			Texture2D tex = new Texture2D (3, 3);
			tex.SetPixel (1, 1, Color.green);
			int copy = i;
			// find the closest power of two that is smaller than copy and subtract it from copy until copy is 0
			while (copy != 0) {
				// pow is the closest power of two that is small than copy
				int pow = Mathf.ClosestPowerOfTwo (copy);
				if (pow > copy) {
					pow /= 2;
				}
				copy -= pow;
				// index is the what power of two pow is
				int index = (int) (Mathf.Log (pow) / Mathf.Log (2));
				if (index > 3) {
					index++;
				}
				tex.SetPixel (index % 3, 2 - Mathf.FloorToInt (index / 3), Color.green);
			}
			Texture2D scaledTex = ScaleTexture (tex, 3 * size, 3 * size);
			byte[] bytes = scaledTex.EncodeToPNG();
			Object.Destroy (tex);
			Object.Destroy (scaledTex);
			File.WriteAllBytes (Application.dataPath + "/Resources/Art/Sprites/Tiles/Bitmasked Tiles/" + i + ".png", bytes);
		}
	}
	private Texture2D ScaleTexture(Texture2D source,int targetWidth,int targetHeight) {
		Texture2D result=new Texture2D(targetWidth,targetHeight,source.format,false);
		float incX=(1.0f / (float)targetWidth);
		float incY=(1.0f / (float)targetHeight);
		for (int i = 0; i < result.height; ++i) {
			for (int j = 0; j < result.width; ++j) {
				Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
				result.SetPixel(j, i, newColor);
			}
		}
		result.Apply();
		return result;
	}
}
