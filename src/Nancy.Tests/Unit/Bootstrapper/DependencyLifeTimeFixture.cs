using Nancy.Bootstrapper;
using Nancy.Json;
using Nancy.ModelBinding.DefaultBodyDeserializers;
using Nancy.Testing;
using Xunit;

namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;
    using Nancy;

    public class DependencyLifeTimeFixture
    {
        readonly DependencyBootstrapper _bootstrapper;
        readonly DependencyResult _body;

        public DependencyLifeTimeFixture()
        {
            _bootstrapper = new DependencyBootstrapper(c =>
            {
                c.EnableAutoRegistration();
                c.Module<MyModule>();
            });

            var browser = new Browser(_bootstrapper);
            var result = browser.Get("/");
            _body = new JavaScriptSerializer().Deserialize<DependencyResult>(result.Body.AsString());

        }

        [Fact]
        public void app_boundary_should_be_null_transaction_boundary()
        {
            _bootstrapper.AppBoundary.ShouldBeOfType<NullTransactionBoundary>();
        }

        [Fact]
        public void request_boundary_should_be_transaction_boundary()
        {
            _bootstrapper.BeforeBoundary.ShouldNotBeOfType<NullTransactionBoundary>();
            _bootstrapper.BeforeBoundary.ShouldBeOfType<TransactionBoundary>();
        }

        [Fact]
        public void other_resolved_boundaries_should_match_request_boundary()
        {
            _body.BoundaryId.ShouldEqual(_bootstrapper.BeforeBoundary.BoundaryId);
            _body.DependencyBoundaryId.ShouldEqual(_bootstrapper.BeforeBoundary.BoundaryId);
        }

    }

    public class DependencyBootstrapper : ConfigurableBootstrapper
    {
        public DependencyBootstrapper()
        {
        }

        public DependencyBootstrapper(Action<ConfigurableBoostrapperConfigurator> configuration) : base(configuration)
        {
        }
        protected override void ConfigureApplicationContainer(TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<ITransactionBoundary, NullTransactionBoundary>();
        }
        protected override void ConfigureRequestContainer(TinyIoc.TinyIoCContainer container, NancyContext context)
        {
            container.Register<ITransactionBoundary, TransactionBoundary>();
            container.Register<IMyDependency, MyDependency>();
        }
        protected override void ApplicationStartup(TinyIoc.TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.BeforeRequest += ctx =>
            {
                AppBoundary = container.Resolve<ITransactionBoundary>();
                return null;
            };
        }


        protected override void RequestStartup(TinyIoc.TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            pipelines.BeforeRequest += ctx =>
            {
                BeforeBoundary = container.Resolve<ITransactionBoundary>();
                return null;
            };
        }

        public ITransactionBoundary BeforeBoundary { get; private set; }
        public ITransactionBoundary AppBoundary { get; private set; }
    }

    public class MyModule : NancyModule
    {
        public MyModule(ITransactionBoundary boundary, IMyDependency dependency)
        {
            Get["/"] = _ => Response.AsJson(new DependencyResult(boundary.BoundaryId, dependency.Boundary.BoundaryId));
        }
    }

    public class DependencyResult
    {
        public DependencyResult()
        {
        }

        public DependencyResult(string boundary, string dependencyBoundary)
        {
            BoundaryId = boundary;
            DependencyBoundaryId = dependencyBoundary;
        }

        public string BoundaryId { get; set; }
        public string DependencyBoundaryId { get; set; }
    }
    
    public interface IMyDependency
    {
        ITransactionBoundary Boundary { get; set; }
    }

    public class MyDependency : IMyDependency
    {
        public ITransactionBoundary Boundary { get; set; }

        public MyDependency(ITransactionBoundary boundary)
        {
            Boundary = boundary;
        }
    }

    public interface ITransactionBoundary
    {
        string BoundaryId { get; set; }
    }

    public class TransactionBoundary : ITransactionBoundary
    {
        public TransactionBoundary()
        {
            BoundaryId = "T" + Guid.NewGuid().ToString().Replace("-", "");
        }

        public string BoundaryId { get; set; }
    }
    public class NullTransactionBoundary : ITransactionBoundary
    {
        public NullTransactionBoundary()
        {
            BoundaryId = "NT" + Guid.NewGuid().ToString().Replace("-", "");
        }

        public string BoundaryId { get; set; }
    }
}