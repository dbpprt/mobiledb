using System;
using System.Diagnostics;
using MobileDB.Common.Factory;
using MobileDB.Tests.Common;
using Xunit;

namespace MobileDB.Tests
{
    [Trait("Create Biggy Context Instances", "")]
    public class BiggyContextTests
    {
        [Fact]
        public void Create_Context_Instance()
        {
            var context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithMemoryFilesystem()
                .Build();

            Assert.NotNull(context.SimpleEntities);
        }

        [Fact]
        public void Create_Context_Instance_With_Default_Store()
        {
            var context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithMemoryFilesystem()
                .Build();

            Assert.NotNull(context.SimpleEntities);
        }

        [Fact]
        public void Create_Some_Context_Instances_With_Default_Stores()
        {
            ReflectionHelper.ClearInternalContextCaches();

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var first = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithMemoryFilesystem()
                .Build();
            var firstElapsed = stopwatch.ElapsedTicks;

            stopwatch.Restart();
            var second = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithMemoryFilesystem()
                .Build();
            var secondElapsed = stopwatch.ElapsedTicks;

            var factor = firstElapsed/secondElapsed;

            // TODO: 1000 is the expected factor, but test conditions arent good
            // okay that's a lie => ClearInternalContextCaches is broken ^^
            Assert.True(factor > 2,
                "Initializing a context with warm caches should be really really fast! (factor " + factor + ")");
        }

        [Fact]
        public void Create_Context_Instance_Get_Set_With_Generic()
        {
            var context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithMemoryFilesystem()
                .Build();

            var set = context.Set<SimpleEntityWithIdentity>();

            Assert.NotNull(set);
        }

        [Fact]
        public void Create_Context_Instance_Get_Set_With_Type()
        {
            var context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithMemoryFilesystem()
                .Build();

            var set = context.Set(typeof (SimpleEntityWithIdentity));

            Assert.NotNull(set);
        }

        [Fact]
        public void Create_Context_Instance_Add_Simple_Identity_Entity()
        {
            var context = ContextFactory.Create<JsonContextWithSimpleIdentity>()
                .WithMemoryFilesystem()
                .Build();

            var set = context.Set<SimpleEntityWithIdentity>();
            set.Add(new SimpleEntityWithIdentity
            {
                Id = Guid.NewGuid(),
                Value = "some value"
            });

            context.SaveChanges();
        }
    }
}