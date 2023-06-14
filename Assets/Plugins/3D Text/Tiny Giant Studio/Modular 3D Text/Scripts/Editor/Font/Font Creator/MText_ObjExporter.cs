using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MText.FontCreation
{
    public class MText_ObjExporter
    {
        private int StartIndex = 0;

        public string MeshToString(MeshFilter mf, Transform t)
        {
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            Quaternion r = t.localRotation;

            int numVertices = 0;
            Mesh m = mf.sharedMesh;
            if (!m)
            {
                return "####Error####";
            }
            Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

            StringBuilder sb = new StringBuilder();

            sb.Append("g ").Append(mf.name).Append("\n"); //new
            foreach (Vector3 vv in m.vertices)
            {
                Vector3 v = t.TransformPoint(vv);
                numVertices++;
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
            }
            sb.Append("\n");
            foreach (Vector3 nn in m.normals)
            {
                Vector3 v = r * nn;
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, -v.y, -v.z));
                //sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z)); original I had
            }
            sb.Append("\n");
            foreach (Vector3 v in m.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }

            for (int material = 0; material < m.subMeshCount; material++)
            {
                sb.Append("\n");
                //Debug.Log(mats[material]);
                if (mats[material]) //This is only needed in newer? unity versions. idk why. //TODO //more research
                {
                    sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                    sb.Append("usemap ").Append(mats[material].name).Append("\n");
                }

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                                            triangles[i] + 1 + StartIndex, triangles[i + 1] + 1 + StartIndex, triangles[i + 2] + 1 + StartIndex));
                }
            }
            StartIndex += numVertices;
            return sb.ToString();
        }


        public string DoExport(GameObject targetObj, bool makeSubmeshes)
        {
            string meshName = targetObj.name;
            string fileName = EditorUtility.SaveFilePanel("Save mesh location", "Assets/", meshName, "obj");
            if (string.IsNullOrEmpty(fileName)) return null;

            StartIndex = 0;
            StringBuilder meshString = new StringBuilder();

            meshString.Append("#" + meshName + ".obj"
                              + "\n#" + System.DateTime.Now.ToLongDateString()
                              + "\n#" + System.DateTime.Now.ToLongTimeString()
                              + "\n#-------"
                              + "\n\n");

            Transform t = targetObj.transform;

            Vector3 originalPosition = t.position;
            t.position = Vector3.zero;

            if (!makeSubmeshes)
            {
                meshString.Append("g ").Append(t.name).Append("\n");
            }
            meshString.Append(ProcessTransform(t, makeSubmeshes));

            WriteToFile(meshString.ToString(), fileName);

            t.position = originalPosition;

            StartIndex = 0;
            fileName = FileUtil.GetProjectRelativePath(fileName);
            AssetDatabase.ImportAsset(fileName);
            //ModelImporter importer = AssetImporter as ModelImporter;

            Debug.Log("Exported Mesh: " + fileName);

            return fileName;
        }



        string ProcessTransform(Transform t, bool makeSubmeshes)
        {
            StringBuilder meshString = new StringBuilder();

            meshString.Append("#" + t.name
                              + "\n#-------"
                              + "\n");

            if (makeSubmeshes)
            {
                meshString.Append("g ").Append(t.name).Append("\n");
            }

            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf != null)
            {
                meshString.Append(MeshToString(mf, t));
            }

            for (int i = 0; i < t.childCount; i++)
            {
                meshString.Append(ProcessTransform(t.GetChild(i), makeSubmeshes));
            }

            return meshString.ToString();
        }

        void WriteToFile(string s, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(s);
            }
        }
    }
}
