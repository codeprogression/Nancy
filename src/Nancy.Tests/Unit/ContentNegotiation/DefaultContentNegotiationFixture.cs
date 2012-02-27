namespace Nancy.Tests.Unit.ContentNegotiation
{
    using System.Collections.Generic;
    using Nancy.Tests.Fakes;
    using Nancy.Tests.Extensions;
    using Xunit;

    public class DefaultContentNegotiationFixture
    {
        readonly INancyEngine engine;

        public DefaultContentNegotiationFixture()
        {
            var bootstrapper = new DefaultNancyBootstrapper();
            bootstrapper.Initialise();
            engine = bootstrapper.GetEngine();
        }

        [Fact]
        public void Should_match_text_html_response()
        {
            // Given
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                {"Accept",new[]{"application/xml,application/xhtml+xml,text/html;q=0.9,*/*;q=0.8"}}
            };

            var request = new FakeRequest("GET", "/negotiated_html_xml", headers);

            // When 
            
            var response = engine.HandleRequest(request).Response;

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            response.ContentType.ShouldEqual("application/xml");
            response.GetStringContentsFromResponse().ShouldStartWith("<?xml version=\"1.0\"?>");
        }
        
        [Fact]
        public void Should_match_application_xml_response()
        {
            // Given
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                {"Accept",new[]{"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"}}
            };
            var request = new FakeRequest("GET", "/negotiated_text_xml", headers);

            // When 
            var response = engine.HandleRequest(request).Response;

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            response.ContentType.ShouldEqual("application/xml");
            response.GetStringContentsFromResponse().ShouldStartWith("<?xml version=\"1.0\"?>");
        }

        [Fact]
        public void Should_return_default_response()
        {
            // Given

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                {"Accept",new[]{"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"}}
            };

            var request = new FakeRequest("GET", "/negotiated_text_json", headers);

            // When 
            var response = engine.HandleRequest(request).Response;

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            response.ContentType.ShouldEqual("text/plain");
            response.GetStringContentsFromResponse().ShouldEqual("");
        }

        [Fact]
        public void Should_return_generic_image_match()
        {
            // Given

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                {"Accept",new[]{"image/jpeg,image/*;q=0.9,*/*;q=0.8"}}
            };
            var request = new FakeRequest("GET", "/negotiated_html_png_gif", headers);

            // When 
            var response = engine.HandleRequest(request).Response;

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            response.ContentType.ShouldEqual("image/png");
            response.GetStringContentsFromResponse().ShouldEqual("");
        }
    }
}