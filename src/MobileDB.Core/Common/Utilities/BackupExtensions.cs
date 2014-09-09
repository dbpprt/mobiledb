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

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Reflection;
//using FileBiggy.Storage.Json;
//using Newtonsoft.Json;

//namespace FileBiggy.Common.Utilities
//{
//    /// <summary>
//    ///     This methods are far from being perfect! use it with care..
//    /// </summary>
//    public static class BackupExtensions
//    {
//        public static Stream Export(this DbContext context)
//        {
//            var sets = context.Sets();
//            var serializer = JsonSerializer.CreateDefault();
//            var biggySerializer = new BiggyListSerializer();

//            var memoryStream = new MemoryStream();
//            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
//            {
//                foreach (var setMappings in sets)
//                {
//                    var set = setMappings.Value as EntitySetBase;

//                    if (set == null) continue;

//                    var entities = set.Export();
//                    var fileName = string.Format("{0}.{1}", setMappings.Key.ToFriendlyName(), "json");
//                    var entry = archive.CreateEntry(fileName);

//                    using (var entryStream = entry.Open())
//                    using (var streamWriter = new StreamWriter(entryStream))
//                    {
//                        var writer = new JsonTextWriter(streamWriter);

//                        biggySerializer.WriteJson(writer, entities, serializer);
//                    }
//                }
//            }

//            memoryStream.Seek(0, SeekOrigin.Begin);
//            return memoryStream;
//        }

//        public static void Import(this DbContext context, Stream backup)
//        {
//            var sets = context.Sets();
//            var fileNameSetMappings = sets.ToDictionary(
//                key => string.Format("{0}.{1}", key.Key.ToFriendlyName(), "json"),
//                value => new KeyValuePair<Type, object>(value.Key, value.Value)
//                );

//            using (var archive = new ZipArchive(backup, ZipArchiveMode.Read, true))
//            {
//                foreach (var entry in archive.Entries)
//                {
//                    KeyValuePair<Type, object> set;

//                    if (!fileNameSetMappings.TryGetValue(entry.Name, out set))
//                    {
//                        continue;
//                    }

//                    using (var entryStream = entry.Open())
//                    using (var streamReader = new StreamReader(entryStream))
//                    {
//                        var json = "[" + streamReader.ReadToEnd().Replace(Environment.NewLine, ",") + "]";
//                        var genericListInstance = typeof (List<>).MakeGenericType(set.Key);

//                        var entities = JsonConvert.DeserializeObject(json, genericListInstance);
//                        var entitySet = set.Value;

//                        // this is really ugly... -.- 
//                        var importMethod = entitySet.GetType()
//                            .GetMethod("Import", BindingFlags.NonPublic | BindingFlags.Instance);
//                        importMethod.Invoke(entitySet, new[] {entities});
//                    }
//                }
//            }
//        }
//    }
//}

