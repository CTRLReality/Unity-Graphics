using System;
using UnityEngine;
using UnityEditor;
using ShaderPathID = UnityEngine.Rendering.Universal.ShaderPathID;
using UnityEditor.ShaderGraph;
using UnityEditor.Rendering.Universal.ShaderGraph;

namespace Unity.Rendering.Universal
{
    public static class ShaderUtils
    {
        internal enum ShaderID
        {
            Unknown = -1,

            Lit = ShaderPathID.Lit,
            SimpleLit = ShaderPathID.SimpleLit,
            Unlit = ShaderPathID.Unlit,
            TerrainLit = ShaderPathID.TerrainLit,
            ParticlesLit = ShaderPathID.ParticlesLit,
            ParticlesSimpleLit = ShaderPathID.ParticlesSimpleLit,
            ParticlesUnlit = ShaderPathID.ParticlesUnlit,
            BakedLit = ShaderPathID.BakedLit,
            SpeedTree7 = ShaderPathID.SpeedTree7,
            SpeedTree7Billboard = ShaderPathID.SpeedTree7Billboard,
            SpeedTree8 = ShaderPathID.SpeedTree8,

            // ShaderGraph IDs start at 1000, correspond to subtargets
            SG_Unlit = 1000,        // UniversalUnlitSubTarget
            SG_Lit,                 // UniversalLitSubTarget
        }

        internal static bool IsShaderGraph(this ShaderID id)
        {
            return ((int) id >= 1000);
        }

        // NOTE: this won't work for non-Asset shaders... (i.e. shadergraph preview shaders)
        internal static ShaderID GetShaderID(Shader shader)
        {
            if (shader.IsShaderGraphAsset())
            {
                UniversalMetadata meta;
                if (!shader.TryGetMetadataOfType<UniversalMetadata>(out meta))
                    return ShaderID.Unknown;
                return meta.shaderID;
            }
            else
            {
                ShaderPathID pathID = UnityEngine.Rendering.Universal.ShaderUtils.GetEnumFromPath(shader.name);
                return (ShaderID)pathID;
            }
        }

        // this is used to reset a material's keywords, based on the ShaderID it is using
        internal static void ResetMaterialKeywords(Material material, ShaderID shaderID = ShaderID.Unknown)
        {
            // if unknown, look it up from the material's shader
            // NOTE: this will only work for asset-based shaders..
            if (shaderID == ShaderID.Unknown)
                shaderID = GetShaderID(material.shader);

            switch (shaderID)
            {
                case ShaderID.SG_Lit:
                    URPLitGUI.UpdateMaterial(material);
                    break;
                case ShaderID.SG_Unlit:
                    URPUnlitGUI.UpdateMaterial(material);
                    break;
                // TODO: handle other shaders that need keyword resets here
                default:
                    break;
            }
        }
    }
}
