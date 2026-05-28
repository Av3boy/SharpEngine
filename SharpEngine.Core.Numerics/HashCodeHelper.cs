// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// https://github.com/microsoft/referencesource/blob/master/System.Numerics/System/Numerics/HashCodeHelper.cs

namespace SharpEngine.Core.Numerics
{
    /// <summary>
    ///     Provides helper methods for combining hash codes in a deterministic manner.
    ///     This is used internally by numeric types to produce stable hash codes based on multiple fields.
    /// </summary>
    internal static class HashCodeHelper
    {
        /// <summary>
        ///     Combines two hash codes into a single hash code.
        /// </summary>
        /// <param name="h1">The first hash code.</param>
        /// <param name="h2">The second hash code.</param>
        /// <returns>A combined hash code value.</returns>
        internal static int CombineHashCodes(int h1, int h2)
            => (((h1 << 5) + h1) ^ h2);
    }
}