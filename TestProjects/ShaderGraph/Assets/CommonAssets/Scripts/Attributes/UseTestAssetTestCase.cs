using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools.Graphics;
//using UnityEngine.XR.Management;
using Attribute = System.Attribute;

public class UseTestAssetTestCaseAttribute : UnityEngine.TestTools.UnityTestAttribute, ITestBuilder
{

    NUnitTestCaseBuilder m_builder = new NUnitTestCaseBuilder();

    IEnumerable<ShaderGraphTestAsset> ShaderGraphTests
    {
        get
        {
            var strings = UnityEditor.AssetDatabase.FindAssets("t:ShaderGraphTestAsset");
            foreach(string path in strings)
            {
                yield return UnityEditor.AssetDatabase.LoadAssetAtPath<ShaderGraphTestAsset>(UnityEditor.AssetDatabase.GUIDToAssetPath(path));
            }
        }
    }

    IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
    {
        List<TestMethod> results = new List<TestMethod>();

        //foreach material we will be testing on the asset 
        foreach(var shaderGraphTest in ShaderGraphTests)
        {
            foreach(var materialTest in shaderGraphTest.testMaterial)
            {
                if(materialTest.enabled == false || materialTest.material == null)
                {
                    continue;
                }

                TestCaseData data = new TestCaseData(new object[] {materialTest.material, shaderGraphTest.isCameraPerspective, new Texture2D(32,32), new ImageComparisonSettings(), null});
                data.SetName(materialTest.material.name);
                data.ExpectedResult = new UnityEngine.Object();
                data.HasExpectedResult = true;
                data.SetCategory(shaderGraphTest.name);

                TestMethod test = this.m_builder.BuildTestMethod(method, suite, data);
                if (test.parms != null)
                    test.parms.HasExpectedResult = false;

                test.Name = $"{shaderGraphTest.name} {materialTest.material.name}";
                results.Add(test);
            }
        }

        return results;
    }


}

