using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Ngin;

[UnityEditor.AssetImporters.ScriptedImporter(1, "nanim")]
public class AnimAssetImporter : UnityEditor.AssetImporters.ScriptedImporter
{
    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        // Load the lexicon from the file path
        Lexicon lexicon = Lexicon.FromLexiconFilePath(ctx.assetPath);
        AnimationClip clip = new AnimationClip
        {
            name = Path.GetFileNameWithoutExtension(ctx.assetPath),
            frameRate = 60f // Set a default frame rate
        };

        // Retrieve frames from the lexicon
        var frames = lexicon.Get<Lexicon>("frames");
        if (frames == null)
        {
            Debug.LogWarning("No frames found in the lexicon.");
            return;
        }

        // Dictionary to store animation curves for each path and property
        Dictionary<(string path, string propertyName), AnimationCurve> curves = new Dictionary<(string, string), AnimationCurve>();

        foreach (var frame in frames.Objects)
        {
            if (frame.Value is Lexicon frameLexicon)
            {
                var time = frameLexicon.Get<float>("time");
                var properties = frameLexicon.Get<Lexicon>("properties");

                if (properties == null) continue;

                foreach (var property in properties.Objects)
                {
                    if (property.Value is Lexicon propertyLexicon)
                    {
                        var path = propertyLexicon.Get<string>("path");
                        var propertyName = propertyLexicon.Get<string>("propertyName");
                        var value = propertyLexicon.Get<float>("value");

                        if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(propertyName))
                        {
                            Debug.LogWarning("Missing path or propertyName in frame properties.");
                            continue;
                        }

                        // Use tuple as key for the curve dictionary
                        var curveKey = (path, propertyName);

                        // Retrieve or create the animation curve
                        if (!curves.TryGetValue(curveKey, out AnimationCurve curve))
                        {
                            curve = new AnimationCurve();
                            curves[curveKey] = curve;
                        }

                        // Add keyframe to the animation curve
                        curve.AddKey(new Keyframe(time, value));
                    }
                }
            }
        }

        // Add all curves to the animation clip
        foreach (var kvp in curves)
        {
            (string path, string propertyName) = kvp.Key;
            clip.SetCurve(path, typeof(Transform), propertyName, kvp.Value);
        }

        // Add the animation clip to the asset context
        ctx.AddObjectToAsset("animationClip", clip);
        ctx.SetMainObject(clip);
    }
}
