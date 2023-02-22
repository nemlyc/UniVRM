using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace UniGLTF
{
    /// <summary>
    /// .GLB file parser.
    /// </summary>
    public sealed class GlbFileParser
    {
        private readonly string _path;

        public GlbFileParser(string glbFilePath)
        {
            _path = glbFilePath;
        }

        public GltfData Parse()
        {
#if UNITY_WEBGL
            var obj = new GameObject("Network");
            var data = obj.AddComponent<GlbFileParserBehaviourExtension>().LoadModel(_path);
#else
            var data = File.ReadAllBytes(_path);
#endif
            return new GlbLowLevelParser(_path, data).Parse();
        }
    }

    public class GlbFileParserBehaviourExtension : MonoBehaviour
    {
        public byte[] data;

        public byte[] LoadModel(string path)
        {
            StartCoroutine(ModelLoadCoroutine(path));
            if (data != null)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        IEnumerator ModelLoadCoroutine(string path)
        {
            using (var request = new UnityWebRequest(path, "GET"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    data = request.downloadHandler.data;

                    request.Dispose();
                }
            }
        }
    }
}