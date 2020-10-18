//////////////////////////////////////////////////////
// MK Toon Editor Common             			    //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////

#if UNITY_EDITOR
namespace MK.Toon.Editor
{
    internal enum RenderPipeline
    {
        Built_in,
        Lightweight,
        Universal
    }
    internal enum RenderPipelineUpgrade
    {
        Lightweight,
        Universal
    }
    internal enum ShaderTemplate
    {
        Unlit,
        Simple,
        PhysicallyBased
    }
    internal enum BlendOpaque
    {
        Default = 0,
        Custom = 4
    };
}
#endif
