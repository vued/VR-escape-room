using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace VRBuilder.Editor.Downloader
{
    /// <summary>
    /// Offers the possibility to download the VPG Core package.
    /// </summary>
    internal class CoreDownloadWindow : EditorWindow
    {
        private static CoreDownloadWindow window;

        private const string menuPath = "Tools/VR Builder/Developer/Download Core Package";
        private const string logoDarkPath = "VRBuilder2_transparent_darkmode";
        private const string logoLightPath = "VRBuilder2_transparent_whitemode";
        private const string corePackageName = "co.mindport.builder.core";
        private const string manualDownloadUrl = @"https://github.com/MindPort-GmbH/VR-Builder-Core/releases";
        private const string registryName = "MindPort VR Builder (package.openupm.com)";
        private const string registryUrl = @"https://package.openupm.com";
        private const string registryScope = "co.mindport.builder";

        private bool isDownloading;
        private bool isSearchingCore;
        private bool isSearchingRequired;
        private bool isCoreAvailable;
        private bool isRequiredVersionAvailable;
        private bool showInstructions;
        private string latestVersion;

        private AddRequest addRequest;
        SearchRequest coreSearch;
        SearchRequest requiredSearch;


        [MenuItem(menuPath, false, 80)]
        public static void ShowWindow()
        {
            if (IsWindowOpened<CoreDownloadWindow>())
            {
                window = Resources.FindObjectsOfTypeAll<CoreDownloadWindow>().First();
            }
            else
            {
                window = GetWindow<CoreDownloadWindow>();
            }

            window.ShowUtility();
            window.Focus();
        }

        /// <summary>
        /// Returns true if there is a window of type <typeparamref name="T"/> opened.
        /// </summary>
        private static bool IsWindowOpened<T>() where T : EditorWindow
        {
            // https://answers.unity.com/questions/523839/find-out-if-an-editor-window-is-open.html
            T[] windows = Resources.FindObjectsOfTypeAll<T>();
            return windows != null && windows.Length > 0;
        }

        private void Awake()
        {
            SearchCorePackage();
            SearchRequiredVersion();
        }

        private void Update()
        {
            if (isSearchingCore && coreSearch.IsCompleted)
            {
                isSearchingCore = false;

                if (coreSearch.Result != null && coreSearch.Result.Length > 0)
                {
                    isCoreAvailable = true;
                    latestVersion = coreSearch.Result.First().version;
                }

                Repaint();
            }

            if(isSearchingRequired && requiredSearch.IsCompleted)
            {
                isSearchingRequired = false;

                if(requiredSearch.Result != null && requiredSearch.Result.Length > 0)
                {
                    isRequiredVersionAvailable = true;
                }

                Repaint();
            }

            if (isDownloading && addRequest.IsCompleted)
            {
                isDownloading = false;
                Repaint();
            }
        }

        private void OnGUI()
        {
            minSize = new Vector2(420f, 520f);
            titleContent = new GUIContent("Core Package Check");

            GUILayout.Space(20f);

            Texture2D logo = Resources.Load<Texture2D>(GetLogoPath());
            Rect rect = GUILayoutUtility.GetRect(position.width, 150, GUI.skin.box);
            GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);

            GUILayout.Space(20f);

            if(coreSearch.IsCompleted == false)
            {
                EditorGUILayout.HelpBox(string.Format("Searching for VR Builder Core Package in package registries..."), MessageType.Info);
            }
            else if (coreSearch.Result == null || coreSearch.Result.Length == 0)
            {
                EditorGUILayout.HelpBox(string.Format("The VR Builder package ({0}) could not be found in a package registry. Has the MindPort scoped registry been added to the project?", corePackageName), MessageType.Warning);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add registry"))
                {
                    Type client = typeof(Client); 
                    MethodInfo addRegistry = client.GetMethod("AddScopedRegistry", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string[]) }, new ParameterModifier[0]);
                    object[] args = new object[] { registryName, registryUrl, new string[] { registryScope } };
                    addRegistry.Invoke(null, args);
                    if (EditorUtility.DisplayDialog("Registry added", string.Format("Attempted to add registry:\n'{0}'", registryName), "Ok")) 
                    {
                        SearchCorePackage();
                        SearchRequiredVersion();
                    }
                }

                if(GUILayout.Button("Instructions"))
                {
                    showInstructions = true;
                }

                EditorGUI.BeginDisabledGroup(isSearchingCore);

                if (GUILayout.Button("Search again"))
                {
                    SearchCorePackage();
                    SearchRequiredVersion();
                }

                EditorGUI.EndDisabledGroup();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (showInstructions)
                {
                    EditorGUILayout.HelpBox("To add the registry manually, open 'Project Settings -> Package Manager.\nThen add the following registry, and press 'Search again'.", MessageType.Info);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name: ");
                    GUILayout.TextField(registryName);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("URL: ");
                    GUILayout.TextField(registryUrl);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Scope: ");
                    GUILayout.TextField(registryScope);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(20f);           

            if (CoreVersionChecker.IsCorePackageInstalled == false)
            {
                EditorGUILayout.HelpBox(string.Format("The VR Builder package is not present in the project.\nInstalled add-ons require version {0} or newer.\nSelect an option below to download the package.", CoreVersionChecker.RequiredVersion), MessageType.Error);
            }
            else if (CoreVersionChecker.IsCoreVersionSupported == false)
            {
                EditorGUILayout.HelpBox(string.Format("The VR Builder package is present, but the installed version ({0}) does not support all installed add.ons.\nThe minimum version required is {1}.\nPlease download and install the required version or a later one.\nSelect an option below to download the package.", CoreVersionChecker.CurrentVersion, CoreVersionChecker.RequiredVersion), MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox(string.Format("A supported version of the VR Builder package is installed ({0}).\nNo further action is required.", CoreVersionChecker.CurrentVersion), MessageType.Info);
            }

            if (CoreVersionChecker.IsCorePackageInstalled == false || CoreVersionChecker.IsCoreVersionSupported == false)
            {               
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUI.BeginDisabledGroup(isDownloading || isCoreAvailable == false);

                if (string.IsNullOrEmpty(CoreVersionChecker.RequiredVersion) == false && CoreVersionChecker.RequiredVersion != "unknown" && isRequiredVersionAvailable)
                {
                    if (GUILayout.Button(string.Format("Install version {0} {1}", CoreVersionChecker.RequiredVersion, CoreVersionChecker.RequiredVersion == latestVersion ? " (latest)" : "(required)"))) 
                    {
                        addRequest = Client.Add(string.Format("{0}@{1}", corePackageName, CoreVersionChecker.RequiredVersion));
                        isDownloading = true;
                    }
                }

                if(CoreVersionChecker.RequiredVersion != latestVersion)
                {
                    if (string.IsNullOrEmpty(latestVersion) == false && latestVersion != "unknown")
                    {
                        if (GUILayout.Button(string.Format("Install version {0} (latest)", latestVersion)))
                        {
                            addRequest = Client.Add(string.Format("{0}", corePackageName));
                            isDownloading = true;
                        }
                    }
                }

                EditorGUI.EndDisabledGroup();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Download Manually"))
                {
                    StartDownload();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (isDownloading)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    GUILayout.Label("Please wait...");

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else if (addRequest != null && addRequest.Status >= StatusCode.Failure && addRequest.Error != null)
                {
                    EditorGUILayout.HelpBox(string.Format("Error {0}: {1}", addRequest.Error.errorCode, addRequest.Error.message), MessageType.Error);
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Close"))
                {
                    this.Close();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        private string GetLogoPath()
        {
            if (EditorGUIUtility.isProSkin)
            {
                return logoDarkPath;
            }
            else
            {
                return logoLightPath;
            }
        }

        private void StartDownload()
        {
            Application.OpenURL(manualDownloadUrl);
        }

        private void SearchRequiredVersion()
        {
            requiredSearch = Client.Search(string.Format("{0}@{1}", corePackageName, CoreVersionChecker.RequiredVersion));
            isSearchingRequired = true;
        }

        private void SearchCorePackage()
        {
            coreSearch = Client.Search(corePackageName);
            isSearchingCore = true;
        }
    }
}