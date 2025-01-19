using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Ngin;
using TB;

[UnityEditor.AssetImporters.ScriptedImporter(1, "nmsh")]
public class MeshAssetImporter : UnityEditor.AssetImporters.ScriptedImporter {
    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx) {
        Lexicon lexicon = Lexicon.FromLexiconFilePath(ctx.assetPath);
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<BoneWeight> boneWeights = new List<BoneWeight>();
        List<Matrix4x4> bindPoses = new List<Matrix4x4>();

        var faces = lexicon.Get<Lexicon>("faces");
        bool smoothShading = lexicon.Get<bool>("smoothShading", true);
        bool isAnim = lexicon.Get<bool>("isAnim", false);

        foreach (var face in faces.Objects) {
            var faceLexicon = face.Value as Lexicon;
            var faceVertices = faceLexicon.Get<Lexicon>("vertices");

            int baseIndex = vertices.Count;
            int vertexCount = faceVertices.Objects.Count;
            for (int i = 0; i < vertexCount; i++) {
                var vertex = faceVertices.Get<Lexicon>(i.ToString());
                var positionBag = vertex.Get<List<float>>("position");
                var normalBag = vertex.Get<List<float>>("normal");
                var uvBag = vertex.Get<List<float>>("uv");

                vertices.Add(new Vector3(positionBag[0], positionBag[1], positionBag[2]));
                normals.Add(new Vector3(normalBag[0], normalBag[1], normalBag[2]));
                uvs.Add(new Vector2(uvBag[0], uvBag[1]));

                if (isAnim) {
                    var boneWeightsBag = vertex.Get<List<float>>("boneWeights");
                    var boneIndicesBag = vertex.Get<List<int>>("boneIndices");

                    BoneWeight boneWeight = new BoneWeight();
                    boneWeight.weight0 = boneWeightsBag[0];
                    boneWeight.boneIndex0 = boneIndicesBag[0];
                    boneWeights.Add(boneWeight);
                }
            }

            // Triangulate the face
            if (vertexCount == 3) {
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 1);
            } else if (vertexCount == 4) {
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 2);
            }
        }

        Debug.Log("Imported mesh with " + vertices.Count + " vertices and " + faces.Length + " faces.");

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);

        if (isAnim) {
            mesh.boneWeights = boneWeights.ToArray();
            bindPoses.Add(Matrix4x4.identity); // Add a default bind pose
            mesh.bindposes = bindPoses.ToArray();
        }

        float angle = 0.0f;
        if (smoothShading) {
            angle = 60.0f;
        }

        MeshNormalSolver.RecalculateNormals(mesh, 60.0f);

        ctx.AddObjectToAsset("mesh", mesh);
        ctx.SetMainObject(mesh);
    }
}
