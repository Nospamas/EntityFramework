﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public class AppServiceProviderFactoryTest
    {
        [Fact]
        public void Create_works()
        {
            var factory = new TestAppServiceProviderFactory(
                MockAssembly.Create(typeof(Program)),
                typeof(Program));

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
            var services = factory.Create(new[] { "arg1" });

            Assert.NotNull(services.GetRequiredService<TestService>());
        }

        private class Program
        {
            public static TestWebHost BuildWebHost(string[] args)
            {
                Assert.Equal("Development", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
                Assert.Equal(args, new[] { "arg1" });

                return new TestWebHost(
                    new ServiceCollection()
                        .AddScoped<TestService>()
                        .BuildServiceProvider(validateScopes: true));
            }
        }

        private class TestService
        {
        }

        [Fact]
        public void Create_works_when_no_BuildWebHost()
        {
            var factory = new TestAppServiceProviderFactory(
                MockAssembly.Create(typeof(ProgramWithoutBuildWebHost)),
                typeof(ProgramWithoutBuildWebHost));

            var services = factory.Create(Array.Empty<string>());

            Assert.NotNull(services);
        }

        private class ProgramWithoutBuildWebHost
        {
        }
    }
}
