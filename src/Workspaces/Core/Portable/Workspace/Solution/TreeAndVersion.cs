// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// A class that represents both a tree and its top level signature version
    /// </summary>
    internal sealed class TreeAndVersion
    {
        /// <summary>
        /// The syntax tree
        /// </summary>
        public SyntaxTree Tree { get; }

        /// <summary>
        /// The version of the top level signature of the tree
        /// </summary>
        public VersionStamp Version { get; }

        private TreeAndVersion(SyntaxTree tree, VersionStamp version)
        {
            this.Tree = tree;
            this.Version = version;
        }

        public static TreeAndVersion Create(SyntaxTree tree, VersionStamp version)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            return new TreeAndVersion(tree, version);
        }
    }
}
