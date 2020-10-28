﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using VSCommanding = Microsoft.VisualStudio.Commanding;

namespace Microsoft.CodeAnalysis.Editor.Implementation.InlineRename
{
    internal partial class RenameCommandHandler : IChainedCommandHandler<BackspaceKeyCommandArgs>, IChainedCommandHandler<DeleteKeyCommandArgs>
    {
        public VSCommanding.CommandState GetCommandState(BackspaceKeyCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            return GetCommandState(nextHandler);
        }

        public VSCommanding.CommandState GetCommandState(DeleteKeyCommandArgs args, Func<VSCommanding.CommandState> nextHandler)
        {
            return GetCommandState(nextHandler);
        }

        public void ExecuteCommand(BackspaceKeyCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            HandlePossibleTypingCommand(args, nextHandler, span =>
                {
                    var caretPoint = args.TextView.GetCaretPoint(args.SubjectBuffer);
                    if (!args.TextView.Selection.IsEmpty || caretPoint.Value != span.Start)
                    {
                        nextHandler();
                    }
                });
        }

        public void ExecuteCommand(DeleteKeyCommandArgs args, Action nextHandler, CommandExecutionContext context)
        {
            HandlePossibleTypingCommand(args, nextHandler, span =>
                {
                    var caretPoint = args.TextView.GetCaretPoint(args.SubjectBuffer);
                    if (!args.TextView.Selection.IsEmpty || caretPoint.Value != span.End)
                    {
                        nextHandler();
                    }
                });
        }
    }
}