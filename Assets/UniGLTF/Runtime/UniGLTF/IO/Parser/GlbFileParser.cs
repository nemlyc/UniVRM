using System.IO;
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
            byte[] data = null;
            using (var request = new UnityWebRequest(_path, "GET"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    data = request.downloadHandler.data;

                   request.Dispose();
                }

            }
#else
            var data = File.ReadAllBytes(_path);
#endif
            return new GlbLowLevelParser(_path, data).Parse();
        }
    }
}