using System;
using System.Linq;
using Nancy.Responses;

namespace Nancy.Tests.Fakes
{
    public class FakeNancyModuleWithContentNegotiationPipeline : NancyModule
    {
        public FakeNancyModuleWithContentNegotiationPipeline()
        {
            Get["/negotiated_html_xml"] = x =>
                {
                    var response = new Response();
                    response.Responses.Add(new XmlResponse<object>(new object(), "application/xml",
                                                                   new DefaultXmlSerializer()));
                    return response;
                };

            Get["/negotiated_text_xml"] = x =>
                {
                    var response = new Response
                        {
                            ContentType = "text/plain"
                        };
                    response.Responses.Add(new XmlResponse<object>(new object(), "application/xml",
                                                                   new DefaultXmlSerializer()));
                    return response;
                };

            Get["/negotiated_text_json"] = x =>
                {
                    var response = new Response
                        {
                            ContentType = "text/plain"
                        };
                    response.Responses.Add(new JsonResponse(new object(), new DefaultJsonSerializer()));
                    return response;
                };

            Get["/negotiated_html_png_gif"] = x =>
                {
                    var response = new Response();
                    response.Responses.Add(new Response
                        {
                            ContentType = "image/png"
                        });
                    response.Responses.Add(new Response
                        {
                            ContentType = "image/gif"
                        });
                    return response;
                };
            After += new ContentNegotiationPipelineItem();
        }
    }

    public class ContentNegotiationPipelineItem : PipelineItem<Action<NancyContext>>
    {

        public ContentNegotiationPipelineItem()
            : base("Content Negotiation", action)
        {
        }

        static readonly Action<NancyContext> action = ctx =>
            {

                var request = ctx.Request;
                if (request == null || request.Headers == null || request.Headers.Accept == null)
                    return;

                var responses = ctx.Response.Responses;
                responses.Insert(0, new Response
                    {
                        Contents = ctx.Response.Contents,
                        ContentType = ctx.Response.ContentType,
                        Headers = ctx.Response.Headers,
                        StatusCode = ctx.Response.StatusCode
                    });
                foreach (var cookie in ctx.Response.Cookies)
                    responses[0].AddCookie(cookie);

                foreach (var contentType in request.Headers.Accept.Select(x => x.Item1))
                {
                    var indexOf = contentType.IndexOf("/", StringComparison.Ordinal);
                    if (contentType == "*/*")
                        return;

                    var response = contentType.EndsWith("/*")
                                       ? responses.FirstOrDefault(
                                           x => x.ContentType.StartsWith(contentType.Substring(0, indexOf)))
                                       : responses.FirstOrDefault(x => x.ContentType == contentType);

                    if (response != null)
                    {
                        ctx.Response = response;
                        break;
                    }
                }
            };

    }
}