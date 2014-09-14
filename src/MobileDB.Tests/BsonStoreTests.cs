//#region Copyright (C) 2014 Dennis Bappert
//// The MIT License (MIT)

//// Copyright (c) 2014 Dennis Bappert

//// Permission is hereby granted, free of charge, to any person obtaining a copy
//// of this software and associated documentation files (the "Software"), to deal
//// in the Software without restriction, including without limitation the rights
//// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//// copies of the Software, and to permit persons to whom the Software is
//// furnished to do so, subject to the following conditions:

//// The above copyright notice and this permission notice shall be included in all
//// copies or substantial portions of the Software.

//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//// SOFTWARE.
//#endregion

//using System;
//using System.IO;
//using System.Threading.Tasks;
//using MobileDB.Common.Factory;
//using MobileDB.Tests.Common;
//using Xunit;

//namespace MobileDB.Tests
//{
//    public class BsonStoreTests
//    {
//        [Fact]
//        public async Task Bson_Save_Some_Items()
//        {
//            var context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
//                .WithPhysicalFilesystem("C:\\Development\\database\\")
//                .Build();
//            Assert.NotNull(context);

//            var set = context.Set<SimpleEntityWithIdentity>();
//            Assert.NotNull(set);

//            set.Add(new SimpleEntityWithIdentity
//            {
//                Id = Guid.NewGuid(),
//                Value = "yay"
//            });

//            await context.SaveChangesAsync();
//        }

//        [Fact]
//        public async Task Bson_Save_Item_And_Reload_From_Filesystem()
//        {
//            var path = "C:\\Development\\database\\" + Guid.NewGuid() + "\\";
//            if (!Directory.Exists(path))
//            {
//                Directory.CreateDirectory(path);
//            }

//            var context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
//                .WithPhysicalFilesystem(path)
//                .Build();
//            Assert.NotNull(context);

//            var set = context.Set<SimpleEntityWithIdentity>();
//            Assert.NotNull(set);

//            set.Add(new SimpleEntityWithIdentity
//            {
//                Id = Guid.NewGuid(),
//                Value = "yay"
//            });

//            await context.SaveChangesAsync();

//            ReflectionHelper.ClearInternalContextCaches();

//            context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
//                .WithPhysicalFilesystem(path)
//                .Build();

//            set = context.Set<SimpleEntityWithIdentity>();
//            Assert.NotNull(set);

//            var actual = set.Count();

//            //Assert.Equal(1, actual);
//        }

//        [Fact]
//        public async Task Bson_Save_Alot_Items_And_Query_Them()
//        {
//            var path = "C:\\Development\\database\\" + Guid.NewGuid() + "\\";
//            var count = 100;

//            if (!Directory.Exists(path))
//            {
//                Directory.CreateDirectory(path);
//            }

//            var context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
//                .WithPhysicalFilesystem(path)
//                .Build();
//            Assert.NotNull(context);

//            var set = context.Set<SimpleEntityWithIdentity>();
//            Assert.NotNull(set);

//            for (var i = 0; i < count; i++)
//            {
//                set.Add(new SimpleEntityWithIdentity
//                {
//                    Id = Guid.NewGuid(),
//                    Value = "yay"
//                });
//            }

//            await context.SaveChangesAsync();

//            context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
//                .WithPhysicalFilesystem(path)
//                .Build();
//            Assert.NotNull(context);

//            set = context.Set<SimpleEntityWithIdentity>();
//            Assert.NotNull(set);

//            var actualCount = await set.Count();

//            Assert.Equal(count, actualCount);
//        }

//        [Fact]
//        public async Task Bson_Save_Alot_Items_And_Reload_From_Filesystem_And_Query_Them()
//        {
//            var path = "C:\\Development\\database\\" + Guid.NewGuid() + "\\";
//            var count = 100;

//            if (!Directory.Exists(path))
//            {
//                Directory.CreateDirectory(path);
//            }

//            var context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
//                .WithPhysicalFilesystem(path)
//                .Build();
//            Assert.NotNull(context);

//            var set = context.Set<SimpleEntityWithIdentity>();
//            Assert.NotNull(set);

//            for (var i = 0; i < count; i++)
//            {
//                set.Add(new SimpleEntityWithIdentity
//                {
//                    Id = Guid.NewGuid(),
//                    Value = "yay"
//                });
//            }

//            await context.SaveChangesAsync();

//            ReflectionHelper.ClearInternalContextCaches();
//            context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
//                .WithPhysicalFilesystem(path)
//                .Build();
//            Assert.NotNull(context);

//            set = context.Set<SimpleEntityWithIdentity>();
//            Assert.NotNull(set);

//            var actualCount = await set.Count();

//            Assert.Equal(count, actualCount);
//        }
//    }
//}
