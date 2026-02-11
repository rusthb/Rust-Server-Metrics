using HarmonyLib;
using RustServerMetrics.HarmonyPatches.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RustServerMetrics.HarmonyPatches.Delayed
{
    /// <summary>
    /// Times all methods in Assembly-CSharp that have an RPC_Server-like attribute.
    /// Much safer reflection than the original implementation:
    ///  - Only scans Assembly-CSharp (BasePlayer assembly)
    ///  - Uses CustomAttributes (metadata) instead of constructing attribute instances
    ///  - Wraps reflection in try/catch
    /// </summary>
    [DelayedHarmonyPatch]
    [HarmonyPatch]
    internal static class RPCServer_Attribute_Method_Patch
    {
        // Methods we must not patch because they run during file/storage init
        private static readonly string[] ForbiddenRpcDeclaringTypes =
        {
    "BaseEntity",          // SV_RequestFile lives here
    "FileStorage",         // Anything that touches server/<identity>/sv.files.*.db
    "ServerMgr",           // Not needed but safe to exclude
    "Bootstrap"            // Avoid Rust bootstrap RPCs
};

        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods(Harmony harmonyInstance)
        {
            Assembly assemblyCSharp;

            try
            {
                assemblyCSharp = typeof(global::BasePlayer).Assembly;
            }
            catch
            {
                yield break;
            }

            Type[] allTypes;
            try
            {
                allTypes = assemblyCSharp.GetTypes();
            }
            catch (ReflectionTypeLoadException rtl)
            {
                allTypes = rtl.Types;
            }

            foreach (var type in allTypes)
            {
                if (type == null) continue;

                // Skip dangerous classes
                if (Array.Exists(ForbiddenRpcDeclaringTypes,
                    forbidden => type.FullName != null && type.FullName.Contains(forbidden)))
                    continue;

                MethodInfo[] methods;
                try
                {
                    methods = type.GetMethods(
                        BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Instance |
                        BindingFlags.Static |
                        BindingFlags.DeclaredOnly);
                }
                catch { continue; }

                foreach (var method in methods)
                {
                    if (method == null) continue;

                    bool hasRpc = false;

                    try
                    {
                        foreach (var cad in method.CustomAttributes)
                        {
                            if (cad.AttributeType.Name.Contains("RPC_Server"))
                            {
                                hasRpc = true;
                                break;
                            }
                        }
                    }
                    catch { continue; }

                    if (!hasRpc)
                        continue;

                    // Also skip explicit problem method: SV_RequestFile
                    if (method.Name.Contains("SV_RequestFile"))
                        continue;

                    yield return method;
                }
            }
        }
    }
}
