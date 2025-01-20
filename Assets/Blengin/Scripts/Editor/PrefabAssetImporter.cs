using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using System.Collections.Generic;
using Ngin;

[ScriptedImporter(1, "nobj")]
public class PrefabAssetImporter : ScriptedImporter {
    public override void OnImportAsset(AssetImportContext ctx) {
        // Read the file content as a Lexicon
        Lexicon lexicon = Lexicon.FromLexiconFilePath(ctx.assetPath);
        string name = Path.GetFileNameWithoutExtension(ctx.assetPath);
        nObject obj = new nObject(name, lexicon);
        obj.Setup();
        obj.Build();
        obj.EditorRefresh();

        List<GameObject> allObjects = obj.GetAllGameObjects();
        foreach (var go in allObjects) {
            ctx.AddObjectToAsset(go.name, go);
        }
        ctx.SetMainObject(obj.GameObject);
        /*
        // Create a new GameObject to serve as the base prefab
        GameObject prefab = new GameObject(Path.GetFileNameWithoutExtension(ctx.assetPath));
        // Add the prefab to the asset
        ctx.AddObjectToAsset("prefab", prefab);
        ctx.SetMainObject(prefab);

        Lexicon children = lexicon.Get<Lexicon>("children");
        foreach (var kvp in children) {
            Debug.Log(kvp.Key);
            GameObject child = new GameObject(kvp.Key);
            child.transform.SetParent(prefab.transform);
            ctx.AddObjectToAsset(kvp.Key, child);
        }
        */
    }
}
