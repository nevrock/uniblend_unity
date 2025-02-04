using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

public class FbxPostProcessor : AssetPostprocessor {
    void OnPreprocessModel() {
        if (assetPath.EndsWith(".fbx") && assetPath.Contains("/Resources/")) {
            ModelImporter modelImporter = (ModelImporter)assetImporter;
            // Set import settings here
            modelImporter.globalScale = 1.0f;
            modelImporter.importBlendShapes = true;
            modelImporter.importVisibility = false;
            modelImporter.importCameras = false;
            modelImporter.importLights = false;
            modelImporter.meshCompression = ModelImporterMeshCompression.Off;
            modelImporter.isReadable = true;
            modelImporter.optimizeMesh = true;

            if (assetPath.Contains("Rig")) {
                modelImporter.animationType = ModelImporterAnimationType.Generic;
                modelImporter.importAnimation = true;
                modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
            }

            Debug.Log("FBX model preprocessed: " + assetPath);
        }
    }

    void OnPostprocessModel(GameObject g) {
        if (assetPath.EndsWith(".fbx") && assetPath.Contains("/Resources/")) {
            if (assetPath.Contains("Rig")) {
                AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(g);
                foreach (AnimationClip clip in animationClips) {
                    if (clip.name.Contains("Looping")) {
                        SerializedObject serializedClip = new SerializedObject(clip);
                        SerializedProperty loopTimeProperty = serializedClip.FindProperty("m_AnimationClipSettings.m_LoopTime");
                        if (loopTimeProperty != null) {
                            loopTimeProperty.boolValue = true;
                            serializedClip.ApplyModifiedProperties();
                        }
                    }
                }
            }

            Debug.Log("FBX model imported: " + assetPath);
        }
    }
}
