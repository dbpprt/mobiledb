using System;
using System.IO;
using MobileDB.Common.Factory;
using MobileDB.Tests.Common;
using Xunit;

namespace MobileDB.Tests
{
    public class BsonStoreTests
    {
        [Fact]
        public void Bson_Save_Some_Items()
        {
            var context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem("C:\\Development\\database\\")
                .Build();
            Assert.NotNull(context);

            var set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            set.Add(new SimpleEntityWithIdentity
            {
                Id = Guid.NewGuid(),
                Value = "yay"
            });

            context.SaveChanges();
        }

        [Fact]
        public void Bson_Save_Item_And_Reload_From_Filesystem()
        {
            var path = "C:\\Development\\database\\" + Guid.NewGuid() + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem(path)
                .Build();
            Assert.NotNull(context);

            var set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            set.Add(new SimpleEntityWithIdentity
            {
                Id = Guid.NewGuid(),
                Value = "yay"
            });

            context.SaveChanges();

            ReflectionHelper.ClearInternalContextCaches();

            context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem(path)
                .Build();

            set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            var actual = set.Count();

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Bson_Save_Alot_Items_And_Query_Them()
        {
            var path = "C:\\Development\\database\\" + Guid.NewGuid() + "\\";
            var count = 100;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem(path)
                .Build();
            Assert.NotNull(context);

            var set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            for (var i = 0; i < count; i++)
            {
                set.Add(new SimpleEntityWithIdentity
                {
                    Id = Guid.NewGuid(),
                    Value = "yay"
                });
            }

            context.SaveChanges();

            context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem(path)
                .Build();
            Assert.NotNull(context);

            set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            var actualCount = set.Count();

            Assert.Equal(count, actualCount);
        }

        [Fact]
        public void Bson_Save_Alot_Items_And_Reload_From_Filesystem_And_Query_Them()
        {
            var path = "C:\\Development\\database\\" + Guid.NewGuid() + "\\";
            var count = 100;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem(path)
                .Build();
            Assert.NotNull(context);

            var set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            for (var i = 0; i < count; i++)
            {
                set.Add(new SimpleEntityWithIdentity
                {
                    Id = Guid.NewGuid(),
                    Value = "yay"
                });
            }

            context.SaveChanges();

            ReflectionHelper.ClearInternalContextCaches();
            context = ContextFactory.Create<BsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem(path)
                .Build();
            Assert.NotNull(context);

            set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            var actualCount = set.Count();

            Assert.Equal(count, actualCount);
        }
    }
}