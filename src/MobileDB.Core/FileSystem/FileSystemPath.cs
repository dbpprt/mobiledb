#region Copyright (C) 2014 Dennis Bappert

// The MIT License (MIT)

// Copyright (c) 2014 Dennis Bappert

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MobileDB.Exceptions;

namespace MobileDB.FileSystem
{
    public struct FileSystemPath : IEquatable<FileSystemPath>, IComparable<FileSystemPath>
    {
        public const char DirectorySeparator = '/';

        private readonly string _path;

        static FileSystemPath()
        {
            Root = new FileSystemPath(DirectorySeparator.ToString());
        }

        private FileSystemPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException();

            _path = path;
        }

        public static FileSystemPath Root { get; private set; }

        public bool IsDirectory
        {
            get { return _path[_path.Length - 1] == DirectorySeparator; }
        }

        public bool IsFile
        {
            get { return !IsDirectory; }
        }

        public bool IsRoot
        {
            get { return _path.Length == 1; }
        }

        public string EntityName
        {
            get
            {
                var name = _path;

                if (IsRoot)
                    return null;

                var endOfName = name.Length;

                if (IsDirectory)
                    endOfName--;

                var startOfName = name.LastIndexOf(DirectorySeparator, endOfName - 1, endOfName) + 1;
                return name.Substring(startOfName, endOfName - startOfName);
            }
        }

        public FileSystemPath ParentPath
        {
            get
            {
                var parentPath = _path;

                if (IsRoot)
                    throw new InvalidOperationException("There is no parent of root.");

                var lookaheadCount = parentPath.Length;

                if (IsDirectory)
                    lookaheadCount--;

                var index = parentPath.LastIndexOf(DirectorySeparator, lookaheadCount - 1, lookaheadCount);

                Debug.Assert(index >= 0);

                parentPath = parentPath.Remove(index + 1);
                return new FileSystemPath(parentPath);
            }
        }

        public int CompareTo(FileSystemPath other)
        {
            return String.Compare(_path, other._path, StringComparison.Ordinal);
        }

        public bool Equals(FileSystemPath other)
        {
            return other._path.Equals(_path);
        }

        public static bool IsRooted(string s)
        {
            if (s.Length == 0)
                return false;

            return s[0] == DirectorySeparator;
        }

        public static FileSystemPath Parse(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            if (!IsRooted(s))
                throw new ParseException(s, "Path is not rooted");

            if (s.Contains(string.Concat(DirectorySeparator, DirectorySeparator)))
                throw new ParseException(s, "Path contains double directory-separators.");

            return new FileSystemPath(s);
        }

        public FileSystemPath AppendPath(string relativePath)
        {
            if (IsRooted(relativePath))
                throw new ArgumentException("The specified path should be relative.", "relativePath");

            if (!IsDirectory)
                throw new InvalidOperationException("This FileSystemPath is not a directory.");

            return new FileSystemPath(_path + relativePath);
        }

        public FileSystemPath AppendPath(FileSystemPath path)
        {
            if (!IsDirectory)
                throw new InvalidOperationException("This FileSystemPath is not a directory.");

            return new FileSystemPath(_path + path._path.Substring(1));
        }

        public FileSystemPath AppendDirectory(string directoryName)
        {
            if (directoryName.Contains(DirectorySeparator.ToString()))
                throw new ArgumentException("The specified name includes directory-separator(s).", "directoryName");

            if (!IsDirectory)
                throw new InvalidOperationException("The specified FileSystemPath is not a directory.");

            return new FileSystemPath(_path + directoryName + DirectorySeparator);
        }

        public FileSystemPath AppendFile(string fileName)
        {
            if (fileName.Contains(DirectorySeparator.ToString()))
                throw new ArgumentException("The specified name includes directory-separator(s).", "fileName");

            if (!IsDirectory)
                throw new InvalidOperationException("The specified FileSystemPath is not a directory.");

            return new FileSystemPath(_path + fileName);
        }

        public bool IsParentOf(FileSystemPath path)
        {
            return IsDirectory && _path.Length != path._path.Length && path._path.StartsWith(_path);
        }

        public bool IsChildOf(FileSystemPath path)
        {
            return path.IsParentOf(this);
        }

        public FileSystemPath RemoveParent(FileSystemPath parent)
        {
            if (Equals(parent))
                return Root;

            if (!parent.IsParentOf(this))
                throw new ArgumentException("The specified path is not a parent of this path.");

            return new FileSystemPath(_path.Remove(0, parent._path.Length - 1));
        }

        public FileSystemPath RemoveChild(FileSystemPath child)
        {
            if (Equals(child))
                return Root;

            if (!child.IsChildOf(this))
                throw new ArgumentException("The specified path is not a child of this path.");

            return new FileSystemPath(_path.Substring(0, _path.Length - child._path.Length + 1));
        }

        public string GetExtension()
        {
            if (!IsFile)
                throw new ArgumentException("The specified FileSystemPath is not a file.");

            var name = EntityName;

            var extensionIndex = name.LastIndexOf('.');

            return extensionIndex < 0
                ? null
                : name.Substring(extensionIndex);
        }

        public FileSystemPath ChangeExtension(string extension)
        {
            if (!IsFile)
                throw new ArgumentException("The specified FileSystemPath is not a file.");

            var name = EntityName;
            var extensionIndex = name.LastIndexOf('.');

            return extensionIndex >= 0
                ? ParentPath.AppendFile(name.Substring(0, extensionIndex) + extension)
                : Parse(_path + extension);
        }

        public string[] GetDirectorySegments()
        {
            var path = this;

            if (IsFile)
                path = path.ParentPath;

            var segments = new LinkedList<string>();

            while (!path.IsRoot)
            {
                segments.AddFirst(path.EntityName);
                path = path.ParentPath;
            }

            return segments.ToArray();
        }

        public override string ToString()
        {
            return _path;
        }

        public override bool Equals(object obj)
        {
            if (obj is FileSystemPath)
                return Equals((FileSystemPath) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }

        public static bool operator ==(FileSystemPath pathA, FileSystemPath pathB)
        {
            return pathA.Equals(pathB);
        }

        public static bool operator !=(FileSystemPath pathA, FileSystemPath pathB)
        {
            return !(pathA == pathB);
        }
    }
}