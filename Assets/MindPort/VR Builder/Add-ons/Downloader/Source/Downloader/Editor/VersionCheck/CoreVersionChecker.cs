using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace VRBuilder.Editor.Downloader
{
    /// <summary>
    /// Checks if the VR Builder package is installed, and if it is a supported version.
    /// </summary>
    [InitializeOnLoad]
    internal static class CoreVersionChecker
    {
        private const string corePackageName = "co.mindport.builder.core";
        private const string requiredVersionFilePath = "Assets/MindPort/VR Builder/Add-ons/Downloader/Source/Downloader/Editor/required-core-version.txt";

        /// <summary>
        /// True if the core package is found.
        /// </summary>
        public static bool IsCorePackageInstalled { get; private set; }

        /// <summary>
        /// True if the installed core package version is equal or higher than the required one.
        /// </summary>
        public static bool IsCoreVersionSupported { get; private set; }

        /// <summary>
        /// Installed version of the core package.
        /// </summary>
        public static string CurrentVersion
        { 
            get
            {
                if(string.IsNullOrEmpty(currentVersion))
                {
                    currentVersion = GetCurrentVersion();
                }

                return currentVersion;
            }
        }

        /// <summary>
        /// Required version of the core package.
        /// </summary>
        public static string RequiredVersion
        {
            get
            {
                if(string.IsNullOrEmpty(requiredVersion))
                {
                    requiredVersion = GetRequiredVersion();
                }

                return requiredVersion;
            }
        }

        private static PackageCollection packages;
        private static string requiredVersion;
        private static string currentVersion;

        static CoreVersionChecker()
        {
            FetchPackageList();
        }

        private static async void FetchPackageList()
        {
            ListRequest listRequest = Client.List();

            while (listRequest.IsCompleted == false)
            {
                await Task.Delay(100);
            }

            if (listRequest.Status == StatusCode.Failure)
            {
                Debug.LogError($"There was an error trying to retrieve a package list from the Package Manager - Error Code: [{listRequest.Error.errorCode}] .\n{listRequest.Error.message}");
            }
            else
            {
                packages = listRequest.Result;

                CheckCorePackageCompatibility();
            }
        }

        private static void CheckCorePackageCompatibility()
        {
            IsCorePackageInstalled = packages.Any(packageInfo => packageInfo.name == corePackageName);

            if(IsCorePackageInstalled == false)
            {
                CoreDownloadWindow.ShowWindow();
                return;
            }

            IsCoreVersionSupported = IsInstalledVersionSupported();

            if(IsCoreVersionSupported == false)
            {
                CoreDownloadWindow.ShowWindow();
                return;
            }
        }

        public static bool IsInstalledVersionSupported()
        {
            Version installed = new Version(CurrentVersion);
            Version required = new Version(RequiredVersion);

            return installed >= required;
        }

        private static string GetCurrentVersion()
        {
            return packages.First(packageInfo => corePackageName.Contains(packageInfo.name))?.version;
        }

        private static string GetRequiredVersion()
        {
            if (File.Exists(requiredVersionFilePath))
            {
                return File.ReadAllText(requiredVersionFilePath);
            }

             return "unknown";
        }
    }
}