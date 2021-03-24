﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis.InheritanceMargin;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Editor.InheritanceMargin
{
    internal class InheritanceMarginTag : IGlyphTag
    {
        /// <summary>
        /// Margin moniker.
        /// </summary>
        public readonly ImageMoniker Moniker;

        /// <summary>
        /// Members needs to be shown on this line. There might be multiple members.
        /// For example:
        /// interface IBar { void Foo1(); void Foo2(); }
        /// class Bar : IBar { void Foo1() { } void Foo2() { } }
        /// </summary>
        public readonly ImmutableArray<InheritanceMarginItem> MembersOnLine;

        public InheritanceMarginTag(ImmutableArray<InheritanceMarginItem> membersOnLine)
        {
            Contract.ThrowIfTrue(membersOnLine.IsEmpty);

            MembersOnLine = membersOnLine;
            // The common case, one line has one member, avoid to use select & aggregate
            if (membersOnLine.Length == 1)
            {
                var member = membersOnLine[0];
                var targets = member.TargetItems;
                var relationship = targets[0].RelationToMember;
                foreach (var target in targets.Skip(1))
                {
                    relationship |= target.RelationToMember;
                }

                Moniker = GetMoniker(relationship);
            }
            else
            {
                // Multiple members on same line.
                var aggregateRelationship = membersOnLine
                    .SelectMany(member => member.TargetItems.Select(target => target.RelationToMember))
                    .Aggregate((r1, r2) => r1 | r2);
                Moniker = GetMoniker(aggregateRelationship);
            }
        }

        /// <summary>
        /// Decide which moniker should be shown.
        /// </summary>
        private static ImageMoniker GetMoniker(InheritanceRelationship inheritanceRelationship)
        {
            //  If there are multiple targets and we have the corresponding compound image, use it
            if (inheritanceRelationship.HasFlag(InheritanceRelationship.ImplementingOverriding))
            {
                return KnownMonikers.ImplementingOverriding;
            }

            if (inheritanceRelationship.HasFlag(InheritanceRelationship.ImplementingOverridden))
            {
                return KnownMonikers.ImplementingOverridden;
            }

            // Otherwise, show the image based on this preference
            if (inheritanceRelationship.HasFlag(InheritanceRelationship.Implemented))
            {
                return KnownMonikers.Implemented;
            }

            if (inheritanceRelationship.HasFlag(InheritanceRelationship.Implementing))
            {
                return KnownMonikers.Implementing;
            }

            if (inheritanceRelationship.HasFlag(InheritanceRelationship.Overridden))
            {
                return KnownMonikers.Overridden;
            }

            if (inheritanceRelationship.HasFlag(InheritanceRelationship.Overriding))
            {
                return KnownMonikers.Overriding;
            }

            // The relationship is None. Don't know what image should be shown, throws
            throw ExceptionUtilities.UnexpectedValue(inheritanceRelationship);
        }
    }
}