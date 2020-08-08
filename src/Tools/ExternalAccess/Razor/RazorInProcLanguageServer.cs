using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis.Common;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editor.UnitTests;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.LanguageServer;
using Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.ExternalAccess.Razor
{
    internal class RazorInProcLanguageServer
    {
        private InProcLanguageServer _server;

        public RazorInProcLanguageServer(Stream inputStream, Stream outputStream, CodeAnalysis.Workspace workspace)
        {
            var composition = EditorTestCompositions.LanguageServerProtocolWpf.AddParts(typeof(RazorLSPSolutionProvider));
            var exportProvider = composition.ExportProviderFactory.CreateExportProvider();
            var provider = (RazorLSPSolutionProvider)exportProvider.GetExportedValue<ILspSolutionProvider>();
            provider.UpdateSolution(workspace.CurrentSolution);

            var protocol = exportProvider.GetExportedValue<LanguageServerProtocol>();
            _server = new InProcLanguageServer(inputStream, outputStream, protocol, workspace, new MockDiagnosticService(), clientName: null);
        }

        private class MockDiagnosticService : IDiagnosticService
        {
            public event EventHandler<DiagnosticsUpdatedArgs> DiagnosticsUpdated;

            public IEnumerable<DiagnosticData> GetDiagnostics(Workspace workspace, ProjectId projectId, DocumentId documentId, object id, bool includeSuppressedDiagnostics, CancellationToken cancellationToken)
            {
                return Array.Empty<DiagnosticData>();
            }

            public IEnumerable<UpdatedEventArgs> GetDiagnosticsUpdatedEventArgs(Workspace workspace, ProjectId projectId, DocumentId documentId, CancellationToken cancellationToken)
            {
                return Array.Empty<UpdatedEventArgs>();
            }
        }

        [Export(typeof(ILspSolutionProvider)), PartNotDiscoverable]
        internal class RazorLSPSolutionProvider : ILspSolutionProvider
        {
            [DisallowNull]
            private Solution? _currentSolution;

            [ImportingConstructor]
            [Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
            public RazorLSPSolutionProvider()
            {
            }

            public void UpdateSolution(Solution solution)
            {
                _currentSolution = solution;
            }

            public Solution GetCurrentSolutionForMainWorkspace()
            {
                Contract.ThrowIfNull(_currentSolution);
                return _currentSolution;
            }

            public ImmutableArray<Document> GetDocuments(Uri documentUri)
            {
                Contract.ThrowIfNull(_currentSolution);
                return _currentSolution.GetDocuments(documentUri);
            }

            public ImmutableArray<TextDocument> GetTextDocuments(Uri documentUri)
            {
                Contract.ThrowIfNull(_currentSolution);
                return _currentSolution.GetTextDocuments(documentUri);
            }
        }
    }
}
