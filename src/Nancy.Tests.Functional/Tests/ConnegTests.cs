using System;
using System.IO;
using Nancy.Bootstrapper;
using Nancy.Testing;
using Nancy.Testing.Fakes;
using Nancy.Tests.Functional.Modules;
using Xunit;

namespace Nancy.Tests.Functional.Tests
{
    public class ConnegTests
    {
          private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public ConnegTests()
        {
            Func<IRootPathProvider> rootPathProvider = () =>
                {
                    var assemblyPath = Path.GetDirectoryName(GetType().Assembly.CodeBase).Replace(@"file:\", string.Empty);
                    var rootPath = PathHelper.GetParent(assemblyPath, 2);
                    FakeRootPathProvider.RootPath = rootPath;
                    return new FakeRootPathProvider();

                };

            this.bootstrapper = new ConfigurableBootstrapper(
                configuration =>
                    {
                        configuration.Modules(new[] { typeof(ConnegTestModule) });
                        configuration.RootPathProvider(rootPathProvider.Invoke());
                    });
            
            browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Ensure_that_conneg_hook_does_not_affect_normal_responses()
        {
            var result = browser.Get("/conneg/string", c =>
            {
                c.HttpRequest();
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Normal Response", result.Body.AsString());
        }

        [Fact]
        public void Ensure_that_accept_header_causes_view_to_return()
        {
            var result = browser.Get("/conneg/view", c =>
            {
                c.HttpRequest();
                c.Header("Accept","text/html,application/json;0.9,application/xml;q=0.8");
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("text/html", result.Context.Response.ContentType);
            Assert.Equal("John Doe", result.Body.AsString());
        }
    }
}