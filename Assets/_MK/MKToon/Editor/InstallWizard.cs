//////////////////////////////////////////////////////
// MK Toon Install Wizard           			    //
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
    public class InstallWizard : EditorWindow
    {
        private GUIStyle _flowTextStyle { get { return new GUIStyle(EditorStyles.label) { wordWrap = true }; } }
        private static Configuration _config = null;
        private static readonly int _loadTimeInFrames = 72;
        private static int _waitFrameTillLoading = _loadTimeInFrames;

        private static InstallWizard _window;

        [MenuItem("Window/MK/Toon/Install Wizard")]
        static void Init()
        {
            _window = (InstallWizard)EditorWindow.GetWindow<InstallWizard>(true, "MK Toon Install Wizard", true);
            _window.maxSize = new Vector2(360, 435);
            _window.minSize = new Vector2(360, 435);
            _config = Configuration.TryGetInstance();
            _window.Show();
        }

        [InitializeOnLoadMethod]
        private static void ShowInstallWizardOnStartup()
        {
            _waitFrameTillLoading = _loadTimeInFrames;
            EditorApplication.update += ShowInstallWizard;
        }

        private static void ShowInstallWizard()
        {
            if (_waitFrameTillLoading > 0)
            {
                --_waitFrameTillLoading;
            }
            else
            {
                EditorApplication.update -= ShowInstallWizard;

                _config = Configuration.TryGetInstance();
                if(_config != null && _config.showInstallerOnReload)
                    Init();
            }
        }

        private RenderPipeline _targetRenderPipeline;

        private void OnGUI()
        {
            if(_config)
            {
                EditorGUILayout.LabelField("1. Select your Render Pipeline", UnityEditor.EditorStyles.boldLabel);
                _targetRenderPipeline = (RenderPipeline) EditorGUILayout.EnumPopup("Render Pipeline", _targetRenderPipeline);
                EditorHelper.VerticalSpace();
                EditorHelper.Divider();
                EditorHelper.VerticalSpace();
                EditorGUILayout.LabelField("2. Import Shaders", UnityEditor.EditorStyles.boldLabel);
                if(GUILayout.Button("Import Shaders"))
                {
                    EditorUtility.DisplayProgressBar("MK Toon Install Wizard", "Importing Shaders", 0.5f);
                    _config.ImportShaders(_targetRenderPipeline);
                    EditorUtility.ClearProgressBar();
                }
                EditorHelper.VerticalSpace();
                EditorHelper.Divider();
                EditorHelper.VerticalSpace();
                int readMeNumber = 4;
                if(_targetRenderPipeline == RenderPipeline.Lightweight)
                {
                    readMeNumber = 3;
                    EditorGUILayout.LabelField("3. Examples are only available for Builtin and Universal Render Pipeline.", _flowTextStyle);
                    EditorHelper.VerticalSpace();
                    EditorHelper.Divider();
                }
                else
                {
                    EditorGUILayout.LabelField("3. Import Examples (optional)", UnityEditor.EditorStyles.boldLabel);
                    switch(_targetRenderPipeline)
                    {
                        case RenderPipeline.Built_in:
                        EditorGUILayout.LabelField("Make sure Postprocessing Stack v2 and Text Mesh Pro is installed first!", _flowTextStyle);
                        break;
                        case RenderPipeline.Lightweight:
                        EditorGUILayout.LabelField("Make sure Postprocessing Stack v2 and Text Mesh Pro is installed first!", _flowTextStyle);
                        break;
                        case RenderPipeline.Universal:
                        EditorGUILayout.LabelField("Make sure Text Mesh Pro is installed first!", _flowTextStyle);
                        break;
                    }
                    if(GUILayout.Button("Import Examples"))
                    {
                        EditorUtility.DisplayProgressBar("MK Toon Install Wizard", "Importing Examples", 0.5f);
                        _config.ImportExamples(_targetRenderPipeline);
                        EditorUtility.ClearProgressBar();
                    }
                    EditorHelper.VerticalSpace();
                    EditorHelper.Divider();
                    if(_config.examplesImported)
                    {
                        EditorHelper.VerticalSpace();
                        EditorGUILayout.LabelField("Example Scenes:");
                        EditorGUILayout.BeginHorizontal();
                        _config.DrawOpenSpectateExampleButton();
                        _config.DrawOpenArtisticExampleButton();
                        _config.DrawOpenFlyingIslesExampleButton();
                        _config.DrawOpenOutlinesExampleButton();
                        _config.DrawOpenWitchLabExampleButton();
                        EditorGUILayout.EndHorizontal();
                        EditorHelper.VerticalSpace();
                        EditorHelper.Divider();
                    }
                }
                EditorHelper.VerticalSpace();
                EditorGUILayout.LabelField(readMeNumber.ToString() + ". Read Me (Recommended)", UnityEditor.EditorStyles.boldLabel);
                if(GUILayout.Button("Open Read Me"))
                {
                    _config.OpenReadMe();
                }
            }
            else
            {
                Repaint();
            }
        }
    }
}
#endif