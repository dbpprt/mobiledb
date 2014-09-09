using System;
using System.IO;
using System.Linq;
using MobileDB.Common.Factory;
using MobileDB.Tests.Common;
using Xunit;

namespace MobileDB.Tests
{
    public class JsonStoreTests
    {
        [Fact]
        public void Json_Save_Some_Items()
        {
            var context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
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
        public void Json_Save_Item_And_Reload_From_Filesystem()
        {
            var path = "C:\\Development\\database\\" + Guid.NewGuid() + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
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

            context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem(path)
                .Build();

            set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            var queryable = set.AsQueryable();
            var actualCount = queryable.Count();

            Assert.Equal(1, actualCount);
        }

        [Fact]
        public void Json_Save_Alot_Items_And_Query_Them()
        {
            var path = "C:\\Development\\database\\" + Guid.NewGuid() + "\\";
            var count = 100000;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
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

            context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem(path)
                .Build();
            Assert.NotNull(context);

            set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            var queryable = set.AsQueryable();
            var actualCount = queryable.Count();

            Assert.Equal(count, actualCount);
        }

        [Fact]
        public void Json_Save_Alot_Items_And_Reload_From_Filesystem_And_Query_Them()
        {
            var path = "C:\\Development\\database\\" + Guid.NewGuid() + "\\";
            var count = 100000;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
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
            context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithPhysicalFilesystem(path)
                .Build();
            Assert.NotNull(context);

            set = context.Set<SimpleEntityWithIdentity>();
            Assert.NotNull(set);

            var queryable = set.AsQueryable();
            var actualCount = queryable.Count();

            Assert.Equal(count, actualCount);
        }
    }
}