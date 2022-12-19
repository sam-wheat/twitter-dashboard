using Autofac;
using Dashboard.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Tests;

public class BaseTest
{
    protected IContainer Container { get; private set; }
    protected readonly BearerToken TwitterToken;

    public BaseTest()
    {
        var appConfig = new ConfigurationBuilder()
          .AddJsonFile("appsettings.global.json", optional: false).Build();
        TwitterToken = AuthHelper.GetTwitterBearerToken(appConfig["TwitterTokenFile"]);
    }

    protected virtual void BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule(new Dashboard.Components.AutofacModule(TwitterToken));
        RegisterMocks(builder);
        Container = builder.Build();
    }

    [SetUp]
    protected virtual void Setup()
    {
        BuildContainer();
    }

    protected virtual void RegisterMocks(ContainerBuilder builder)
    {

    }
}
