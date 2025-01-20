using UnityEngine;
namespace Ngin {
    public static class Ngin {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init() {
            _env = Lexicon.FromResourcesLexicon(EnvLoad);
        }

        private static void OnRuntimeMethodLoad() {
            Init();
        }

        public static T GetEnv<T>(string name, T defaultVal = default(T)) {
            return _env.Get<T>(name, defaultVal);
        }
        public static void SetEnv<T>(string name, T value) {
            _env.Set(name, value);
        }

        public static void SaveEnv() {
            _env.WriteSave(EnvSave);
        }
        public static string Game;




        private static Lexicon _env;
        public const string SceneStart = "Scene/Start";
        public const string EnvSave = "Env";
        public const string EnvLoad = "Env";

        public const string ResourcesMesh = "Mesh/";
        public const string ResourcesMaterial = "Material/";
        public const string ResourcesTexture = "Texture/";
        
        public const string Armature = "Armature";
        
    }
}