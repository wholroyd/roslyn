// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.DiaSymReader;
using Roslyn.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.ExpressionEvaluator
{
    internal sealed class MockSymUnmanagedReader : ISymUnmanagedReader, ISymUnmanagedReader2, ISymUnmanagedReader3
    {
        private readonly ImmutableDictionary<int, MethodDebugInfoBytes> _methodDebugInfoMap;

        public MockSymUnmanagedReader(ImmutableDictionary<int, MethodDebugInfoBytes> methodDebugInfoMap)
        {
            _methodDebugInfoMap = methodDebugInfoMap;
        }

        public int GetMethodByVersion(int methodToken, int version, out ISymUnmanagedMethod retVal)
        {
            Assert.Equal(1, version);

            MethodDebugInfoBytes info;
            if (!_methodDebugInfoMap.TryGetValue(methodToken, out info))
            {
                retVal = null;
                return SymUnmanagedReaderExtensions.E_FAIL;
            }

            Assert.NotNull(info);
            retVal = info.Method;
            return SymUnmanagedReaderExtensions.S_OK;
        }

        public int GetSymAttribute(int methodToken, string name, int bufferLength, out int count, byte[] customDebugInformation)
        {
            // The EE should never be calling ISymUnmanagedReader.GetSymAttribute.  
            // In order to account for EnC updates, it should always be calling 
            // ISymUnmanagedReader3.GetSymAttributeByVersion instead.
            // TODO (DevDiv #1145183): throw ExceptionUtilities.Unreachable;

            return GetSymAttributeByVersion(methodToken, 1, name, bufferLength, out count, customDebugInformation);
        }

        public int GetSymAttributeByVersion(int methodToken, int version, string name, int bufferLength, out int count, byte[] customDebugInformation)
        {
            Assert.Equal(1, version);

            Assert.Equal("MD2", name);

            MethodDebugInfoBytes info;
            if (!_methodDebugInfoMap.TryGetValue(methodToken, out info))
            {
                count = 0;
                return SymUnmanagedReaderExtensions.S_FALSE; // This is a guess.  We're not consuming it, so it doesn't really matter.
            }

            Assert.NotNull(info);
            info.Bytes.TwoPhaseCopy(bufferLength, out count, customDebugInformation);
            return SymUnmanagedReaderExtensions.S_OK;
        }

        public int GetDocument(string url, Guid language, Guid languageVendor, Guid documentType, out ISymUnmanagedDocument document)
        {
            throw new NotImplementedException();
        }

        public int GetDocuments(int bufferLength, out int count, ISymUnmanagedDocument[] documents)
        {
            throw new NotImplementedException();
        }

        public int GetUserEntryPoint(out int methodToken)
        {
            throw new NotImplementedException();
        }

        public int GetMethod(int methodToken, out ISymUnmanagedMethod method)
        {
            throw new NotImplementedException();
        }

        public int GetVariables(int methodToken, int bufferLength, out int count, ISymUnmanagedVariable[] variables)
        {
            throw new NotImplementedException();
        }

        public int GetGlobalVariables(int bufferLength, out int count, ISymUnmanagedVariable[] variables)
        {
            throw new NotImplementedException();
        }

        public int GetMethodFromDocumentPosition(ISymUnmanagedDocument document, int line, int column, out ISymUnmanagedMethod method)
        {
            throw new NotImplementedException();
        }

        public int GetNamespaces(int bufferLength, out int count, ISymUnmanagedNamespace[] namespaces)
        {
            throw new NotImplementedException();
        }

        public int Initialize(object metadataImporter, string fileName, string searchPath, IStream stream)
        {
            throw new NotImplementedException();
        }

        public int UpdateSymbolStore(string fileName, IStream stream)
        {
            throw new NotImplementedException();
        }

        public int ReplaceSymbolStore(string fileName, IStream stream)
        {
            throw new NotImplementedException();
        }

        public int GetSymbolStoreFileName(int bufferLength, out int count, char[] name)
        {
            throw new NotImplementedException();
        }

        public int GetMethodsFromDocumentPosition(ISymUnmanagedDocument document, int line, int column, int bufferLength, out int count, ISymUnmanagedMethod[] methods)
        {
            throw new NotImplementedException();
        }

        public int GetDocumentVersion(ISymUnmanagedDocument document, out int version, out bool isCurrent)
        {
            throw new NotImplementedException();
        }

        public int GetMethodVersion(ISymUnmanagedMethod method, out int version)
        {
            throw new NotImplementedException();
        }

        public int GetMethodByVersionPreRemap(int methodToken, int version, out ISymUnmanagedMethod method)
        {
            throw new NotImplementedException();
        }

        public int GetSymAttributePreRemap(int methodToken, string name, int bufferLength, out int count, byte[] customDebugInformation)
        {
            throw new NotImplementedException();
        }

        public int GetMethodsInDocument(ISymUnmanagedDocument document, int bufferLength, out int count, ISymUnmanagedMethod[] methods)
        {
            throw new NotImplementedException();
        }

        public int GetSymAttributeByVersionPreRemap(int methodToken, int version, string name, int bufferLength, out int count, byte[] customDebugInformation)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class MockSymUnmanagedMethod : ISymUnmanagedMethod
    {
        private readonly ISymUnmanagedScope _rootScope;

        public MockSymUnmanagedMethod(ISymUnmanagedScope rootScope)
        {
            _rootScope = rootScope;
        }

        int ISymUnmanagedMethod.GetRootScope(out ISymUnmanagedScope retVal)
        {
            retVal = _rootScope;
            return SymUnmanagedReaderExtensions.S_OK;
        }

        int ISymUnmanagedMethod.GetSequencePointCount(out int retVal)
        {
            retVal = 1;
            return SymUnmanagedReaderExtensions.S_OK;
        }

        int ISymUnmanagedMethod.GetSequencePoints(int cPoints, out int pcPoints, int[] offsets, ISymUnmanagedDocument[] documents, int[] lines, int[] columns, int[] endLines, int[] endColumns)
        {
            pcPoints = 1;
            offsets[0] = 0;
            documents[0] = null;
            lines[0] = 0;
            columns[0] = 0;
            endLines[0] = 0;
            endColumns[0] = 0;
            return SymUnmanagedReaderExtensions.S_OK;
        }

        int ISymUnmanagedMethod.GetNamespace(out ISymUnmanagedNamespace retVal)
        {
            throw new NotImplementedException();
        }

        int ISymUnmanagedMethod.GetOffset(ISymUnmanagedDocument document, int line, int column, out int retVal)
        {
            throw new NotImplementedException();
        }

        int ISymUnmanagedMethod.GetParameters(int cParams, out int pcParams, ISymUnmanagedVariable[] parms)
        {
            throw new NotImplementedException();
        }

        int ISymUnmanagedMethod.GetRanges(ISymUnmanagedDocument document, int line, int column, int cRanges, out int pcRanges, int[] ranges)
        {
            throw new NotImplementedException();
        }

        int ISymUnmanagedMethod.GetScopeFromOffset(int offset, out ISymUnmanagedScope retVal)
        {
            throw new NotImplementedException();
        }

        int ISymUnmanagedMethod.GetSourceStartEnd(ISymUnmanagedDocument[] docs, int[] lines, int[] columns, out bool retVal)
        {
            throw new NotImplementedException();
        }

        int ISymUnmanagedMethod.GetToken(out int pToken)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class MockSymUnmanagedScope : ISymUnmanagedScope, ISymUnmanagedScope2
    {
        private readonly ImmutableArray<ISymUnmanagedScope> _children;
        private readonly ImmutableArray<ISymUnmanagedNamespace> _namespaces;
        private readonly int _startOffset;
        private readonly int _endOffset;

        public MockSymUnmanagedScope(ImmutableArray<ISymUnmanagedScope> children, ImmutableArray<ISymUnmanagedNamespace> namespaces, int startOffset = 0, int endOffset = 1)
        {
            _children = children;
            _namespaces = namespaces;
            _startOffset = startOffset;
            _endOffset = endOffset;
        }

        public int GetChildren(int numDesired, out int numRead, ISymUnmanagedScope[] buffer)
        {
            _children.TwoPhaseCopy(numDesired, out numRead, buffer);
            return SymUnmanagedReaderExtensions.S_OK;
        }

        public int GetNamespaces(int numDesired, out int numRead, ISymUnmanagedNamespace[] buffer)
        {
            _namespaces.TwoPhaseCopy(numDesired, out numRead, buffer);
            return SymUnmanagedReaderExtensions.S_OK;
        }

        public int GetStartOffset(out int pRetVal)
        {
            pRetVal = _startOffset;
            return SymUnmanagedReaderExtensions.S_OK;
        }

        public int GetEndOffset(out int pRetVal)
        {
            pRetVal = _endOffset;
            return SymUnmanagedReaderExtensions.S_OK;
        }

        public int GetLocalCount(out int pRetVal)
        {
            pRetVal = 0;
            return SymUnmanagedReaderExtensions.S_OK;
        }

        public int GetLocals(int cLocals, out int pcLocals, ISymUnmanagedVariable[] locals)
        {
            pcLocals = 0;
            return SymUnmanagedReaderExtensions.S_OK;
        }

        public int GetMethod(out ISymUnmanagedMethod pRetVal)
        {
            throw new NotImplementedException();
        }

        public int GetParent(out ISymUnmanagedScope pRetVal)
        {
            throw new NotImplementedException();
        }

        public int GetConstantCount(out int pRetVal)
        {
            pRetVal = 0;
            return SymUnmanagedReaderExtensions.S_OK;
        }

        public int GetConstants(int cConstants, out int pcConstants, ISymUnmanagedConstant[] constants)
        {
            pcConstants = 0;
            return SymUnmanagedReaderExtensions.S_OK;
        }
    }

    internal sealed class MockSymUnmanagedNamespace : ISymUnmanagedNamespace
    {
        private readonly ImmutableArray<char> _nameChars;

        public MockSymUnmanagedNamespace(string name)
        {
            if (name != null)
            {
                var builder = ArrayBuilder<char>.GetInstance();
                builder.AddRange(name);
                builder.AddRange('\0');
                _nameChars = builder.ToImmutableAndFree();
            }
        }

        int ISymUnmanagedNamespace.GetName(int numDesired, out int numRead, char[] buffer)
        {
            _nameChars.TwoPhaseCopy(numDesired, out numRead, buffer);
            return 0;
        }

        int ISymUnmanagedNamespace.GetNamespaces(int cNameSpaces, out int pcNameSpaces, ISymUnmanagedNamespace[] namespaces)
        {
            throw new NotImplementedException();
        }

        int ISymUnmanagedNamespace.GetVariables(int cVars, out int pcVars, ISymUnmanagedVariable[] pVars)
        {
            throw new NotImplementedException();
        }
    }

    internal static class MockSymUnmanagedHelpers
    {
        public static void TwoPhaseCopy<T>(this ImmutableArray<T> source, int numDesired, out int numRead, T[] destination)
        {
            if (destination == null)
            {
                Assert.Equal(0, numDesired);
                numRead = source.IsDefault ? 0 : source.Length;
            }
            else
            {
                Assert.False(source.IsDefault);
                Assert.Equal(source.Length, numDesired);
                source.CopyTo(0, destination, 0, numDesired);
                numRead = numDesired;
            }
        }

        public static void Add2(this ArrayBuilder<byte> bytes, short s)
        {
            var shortBytes = BitConverter.GetBytes(s);
            Assert.Equal(2, shortBytes.Length);
            bytes.AddRange(shortBytes);
        }

        public static void Add4(this ArrayBuilder<byte> bytes, int i)
        {
            var intBytes = BitConverter.GetBytes(i);
            Assert.Equal(4, intBytes.Length);
            bytes.AddRange(intBytes);
        }
    }
}