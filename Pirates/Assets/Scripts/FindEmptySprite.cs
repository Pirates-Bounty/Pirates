using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Somefile
{
    /**
     * REPEATEDLY USE THIS FUNCTION TO FIND ALL EMPTY SPRITES
     * It works in Batches but it saves progress between runs
     */


    [MenuItem("Kingdom/Find Empty Sprites")]
    public static void FindEmptySprites()
    {
        var processedPaths = new HashSet<string>();
        const string PrefName = "processedPaths";
        foreach (var path in EditorPrefs.GetString(PrefName).Split(':'))
        {
            processedPaths.Add(path);
        }
        var guids = AssetDatabase.FindAssets("t:Sprite");
        int count = 0;
        var start = EditorApplication.timeSinceStartup;
        foreach (var guid in guids)
        {
            if (count >= 100)
            {
                break;
            }
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (processedPaths.Add(path))
            {
                var sprites = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var ob in sprites)
                {
                    Sprite sprite = ob as Sprite;
                    if (sprite)
                    {
                        count++;
                        var importer = (TextureImporter)TextureImporter.GetAtPath(path);
                        importer.isReadable = true;
                        AssetDatabase.ImportAsset(path);
                        var rect = sprite.rect;
                        var pixels = sprite.texture.GetPixels();
                        bool spriteHasContent = false;
                        foreach (var pixel in pixels)
                        {
                            if (pixel.a > 0)
                            {
                                spriteHasContent = true;
                                break;
                            }
                        }
                        // Debug.LogFormat("r: {2} tr: {0} o: {1}", sprite.textureRect, sprite.textureRectOffset, sprite.rect);
                        // var rect = sprite.textureRectOffset
                        // sprite.texture
                        if (spriteHasContent == false)
                        {
                            Debug.LogFormat("NO COLOR ON {0} {1}", path, sprite as Sprite);
                        }
                        importer.isReadable = false;
                    }
                }
            }
        }
        EditorPrefs.SetString(PrefName, string.Join(":", processedPaths.ToArray()));
        Debug.LogFormat("Processed {0}/{1} assets in {2}s. Total {3}", count, guids.Length, EditorApplication.timeSinceStartup - start, processedPaths.Count);
    }
}
