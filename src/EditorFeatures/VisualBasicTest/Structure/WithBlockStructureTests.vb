' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.CodeAnalysis.Structure
Imports Microsoft.CodeAnalysis.VisualBasic.Structure
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.Outlining
    Public Class WithBlockStructureProviderTests
        Inherits AbstractVisualBasicSyntaxNodeStructureProviderTests(Of WithBlockSyntax)

        Friend Overrides Function CreateProvider() As AbstractSyntaxStructureProvider
            Return New WithBlockStructureProvider()
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.Outlining)>
        Public Async Function TestWithBlock1() As Task
            Const code = "
Class C
    Sub M()
        {|span:With (goo) $$
        End With|}
    End Sub
End Class
"

            Await VerifyBlockSpansAsync(code,
                Region("span", "With (goo) ...", autoCollapse:=False))
        End Function
    End Class
End Namespace
