#if UNITY_EDITOR
using System.Reflection;
using UPM_PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace K5F.InspectorComment
{
    using static Comment;

    public static class GizmoIcons
    {
        private const string AssetFolderName = "InspectorComment";
        private const string IconSubfolder = "/Resources/";

        public static string GetFilePath(EKind kind)
        {
            string filename = GetIconNameForKind(kind) + ".png";

            /// determining the right project-relative path is not so easy
            string gizmoIconBasePath;
            /// depending on if running in Unity Package or imported in Assets folder,
            /// the Gizmo Icons are stored in a different folder
            var assembly = Assembly.GetExecutingAssembly();
            var packageInfo = UPM_PackageInfo.FindForAssembly(assembly);
            bool isRunningAsUnityPackage = packageInfo != null;
            if (isRunningAsUnityPackage)
            {
                var packagePath = packageInfo.assetPath;
                gizmoIconBasePath = packagePath + IconSubfolder;
            }
            else
            {
                gizmoIconBasePath = "Assets/" + AssetFolderName + IconSubfolder;
            }

            return gizmoIconBasePath + filename;
        }

        public static string GetIconNameForKind(EKind kind)
        {
            return kind switch
            {
                EKind.Info => "IC_Information",
                EKind.Warning => "IC_Warning",
                EKind.ToDo => "IC_Todo",
                _ => "IC_Warning"
            };
        }
    }
}
#endif
