//////////////////////////////////////////////////////
// MK Toon Configuration            			    //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace MK.Toon.Editor
{
    //[CreateAssetMenu(fileName = "MKToonConfiguration", menuName = "MK/Toon/Create Config Asset")]
    public class Configuration : ScriptableObject
    {
        [SerializeField]
        private string _version = "3.0.0";
        [SerializeField]
        private Object _ReadMe = null;
        internal string version { get { return _version; } }

        //[SerializeField]
        //private bool _internalUpdateRequired = false;

        [SerializeField][Space]
        private Object _shadersBuiltin = null;
        [SerializeField]
        private Object _examplesBuiltin = null;
        [SerializeField][Space]
        private Object _ExamplesInc = null;
        [SerializeField][Space]
        private Object _shadersLWRP = null;
        [SerializeField][Space]
        private Object _shadersURP = null;
        [SerializeField]
        private Object _examplesURP = null;

        [Space][SerializeField]
        private Object _exampleSpectate = null;
        [SerializeField]
        private Texture2D _exampleSpectateIcon = null;
        [Space][SerializeField]
        private Object _exampleOutlines = null;
        [SerializeField]
        private Texture2D _exampleOutlinesIcon = null;
        [Space][SerializeField]
        private Object _exampleWitchLab = null;
        [SerializeField]
        private Texture2D _exampleWitchLabIcon = null;
        [Space][SerializeField]
        private Object _exampleFlyingIsles = null;
        [SerializeField]
        private Texture2D _exampleFlyingIslesIcon = null;
        [Space][SerializeField]
        private Object _exampleArtistic = null;
        [SerializeField]
        private Texture2D _exampleArtisticIcon = null;

        [Space][SerializeField]
        internal bool showInstallerOnReload = true;

        internal bool examplesImported { get{ return _exampleArtistic && _exampleSpectate && _exampleOutlines && _exampleWitchLab && _exampleFlyingIsles; } }
        
        private static void LogAssetNotFoundError()
        {
            Debug.LogError("Could not find MK Toon Configuration Asset, please try to import the package again.");
        }

        internal static Configuration TryGetInstance()
        {
            string[] _guids = AssetDatabase.FindAssets("t:MK.Toon.Editor.Configuration", null);
            if(_guids.Length > 0)
            {
                Configuration config = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(_guids[0]), typeof(Configuration)) as Configuration;
                if(config != null)
                    return config;
                else
                {
                    LogAssetNotFoundError();
                    return null;
                }
            }
            else
            {
                LogAssetNotFoundError();
                return null;
            }
        }

        internal void ImportShaders(RenderPipeline renderPipeline)
        {
            switch(renderPipeline)
            {
                case RenderPipeline.Built_in:
                    AssetDatabase.ImportPackage(AssetDatabase.GetAssetPath(_shadersBuiltin), false);
                break;
                case RenderPipeline.Lightweight:
                    AssetDatabase.ImportPackage(AssetDatabase.GetAssetPath(_shadersLWRP), false);
                break;
                case RenderPipeline.Universal:
                    AssetDatabase.ImportPackage(AssetDatabase.GetAssetPath(_shadersURP), false);
                break;
                default:
                //All cases should be handled
                break;
            }
            showInstallerOnReload = false;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        internal void ImportExamples(RenderPipeline renderPipeline)
        {
            AssetDatabase.ImportPackage(AssetDatabase.GetAssetPath(_ExamplesInc), false);
            switch(renderPipeline)
            {
                case RenderPipeline.Built_in:
                    AssetDatabase.ImportPackage(AssetDatabase.GetAssetPath(_examplesBuiltin), false);
                break;
                case RenderPipeline.Lightweight:
                    //Since 2019.4 is the base version LWRP examples can be skipped, but still keep the shaders
                break;
                case RenderPipeline.Universal:
                    AssetDatabase.ImportPackage(AssetDatabase.GetAssetPath(_examplesURP), false);
                break;
                default:
                //All cases should be handled
                break;
            }
        }
        
        internal void OpenReadMe()
        {
            AssetDatabase.OpenAsset(_ReadMe);
        }


        internal void DrawOpenSpectateExampleButton()
        {
            if(GUILayout.Button(_exampleSpectateIcon, GUILayout.Width(64), GUILayout.Height(64)))
                OpenSpectateExampleScene();
        }
        private void OpenSpectateExampleScene()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(_exampleSpectate));
        }
        internal void DrawOpenArtisticExampleButton()
        {
            if(GUILayout.Button(_exampleArtisticIcon, GUILayout.Width(64), GUILayout.Height(64)))
                OpenArtisticExampleScene();
        }
        private void OpenArtisticExampleScene()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(_exampleArtistic));
        }
        internal void DrawOpenWitchLabExampleButton()
        {
            if(GUILayout.Button(_exampleWitchLabIcon, GUILayout.Width(64), GUILayout.Height(64)))
                OpenWitchLabExampleScene();
        }
        private void OpenWitchLabExampleScene()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(_exampleWitchLab));
        }
        internal void DrawOpenOutlinesExampleButton()
        {
            if(GUILayout.Button(_exampleOutlinesIcon, GUILayout.Width(64), GUILayout.Height(64)))
                OpenOutlinesExampleScene();
        }
        private void OpenOutlinesExampleScene()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(_exampleOutlines));
        }
        internal void DrawOpenFlyingIslesExampleButton()
        {
            if(GUILayout.Button(_exampleFlyingIslesIcon, GUILayout.Width(64), GUILayout.Height(64)))
                OpenFlyingIslesExampleScene();
        }
        private void OpenFlyingIslesExampleScene()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(_exampleFlyingIsles));
        }

        [MenuItem("Window/MK/Toon/Open Read Me")]
        internal static void OpenMenuItemReadMe()
        {
            Configuration c = Configuration.TryGetInstance();
            if(c)
                c.OpenReadMe();
        }
    }
}
#endif