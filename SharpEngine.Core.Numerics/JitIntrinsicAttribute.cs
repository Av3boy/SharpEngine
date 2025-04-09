// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// https://github.com/microsoft/referencesource/blob/master/System.Numerics/System/Numerics/JITIntrinsicAttribute.cs

using System;

namespace SharpEngine.Core.Numerics
{
    /// <summary>
    /// An attribute that can be attached to JIT Intrinsic methods/properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
    internal class JitIntrinsicAttribute : Attribute
    {
    }
}