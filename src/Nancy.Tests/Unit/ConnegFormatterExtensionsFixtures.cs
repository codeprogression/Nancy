using System.Collections.Generic;

namespace Nancy.Tests.Unit
{
    using System;
    using System.IO;
    using System.Text;
    using FakeItEasy;
    using Nancy.Responses;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class ConnegFormatterExtensionsFixture
    {
        private readonly IResponseFormatter formatter;
        private readonly Person model;
        readonly NancyContext context = new NancyContext();

        public ConnegFormatterExtensionsFixture()
        {
            this.formatter = A.Fake<IResponseFormatter>();
            A.CallTo(() => this.formatter.Context).Returns(context);
            A.CallTo(() => this.formatter.Serializers).Returns(new ISerializer[] { new DefaultJsonSerializer(), new DefaultXmlSerializer() });
            this.model = new Person { FirstName = "Andy", LastName = "Pike" };
        }


        [Fact]
        public void Should_return_a_response_with_status_code_415_UnsupportedMediaType()
        {
            var response = this.formatter.AsConneg(model);
            response.StatusCode.ShouldEqual(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public void Should_return_a_response_with_the_standard_json_content_type()
        {
            context.Request = new FakeRequest("Get", "/conneg", new Dictionary<string, IEnumerable<string>>
                {
                    {"Accept",new[]{"application/json,application/xml;q=0.9"}}
                });
            var response = this.formatter.AsConneg(model);
            response.ContentType.ShouldEqual("application/json");
        }

        [Fact]
        public void Should_return_a_response_with_the_standard_xml_content_type()
        {
            context.Request = new FakeRequest("Get","/conneg",new Dictionary<string, IEnumerable<string>>
                {
                    {"Accept",new[]{"application/xml,application/json;q=0.9"}}
                });
            var response = this.formatter.AsConneg(model);
            response.ContentType.ShouldEqual("application/xml");
        }


        [Fact]
        public void Should_return_a_valid_model_in_json_format()
        {
            context.Request = new FakeRequest("Get", "/conneg", new Dictionary<string, IEnumerable<string>>
                {
                    {"Accept",new[]{"application/json,application/xml;q=0.9"}}
                });
            var response = this.formatter.AsConneg(model);
            using (var stream = new MemoryStream())

            {
                response.Contents(stream);

                Encoding.UTF8.GetString(stream.ToArray()).ShouldEqual("{\"FirstName\":\"Andy\",\"LastName\":\"Pike\"}");
            }
        }
        [Fact]
        public void Should_return_a_valid_model_in_xml_format()
        {
            context.Request = new FakeRequest("Get", "/conneg", new Dictionary<string, IEnumerable<string>>
                {
                    {"Accept",new[]{"application/xml,application/json;q=0.9"}}
                });
            var response = this.formatter.AsConneg(model);
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);

                var expected = @"<?xml version=""1.0""?><Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">  <FirstName>Andy</FirstName>  <LastName>Pike</LastName></Person>";

                Encoding.UTF8.GetString(stream.ToArray()).Replace("\r\n","").ShouldEqual(expected);
            }
        }

        [Fact]
        public void Should_return_null_in_json_format()
        {
            context.Request = new FakeRequest("Get", "/conneg", new Dictionary<string, IEnumerable<string>>
                {
                    {"Accept",new[]{"application/json,application/xml;q=0.9"}}
                });
            var response = this.formatter.AsConneg<Person>(null);
          
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                Encoding.UTF8.GetString(stream.ToArray()).ShouldEqual("null");
            }
        }

        [Fact]
        public void Should_return_null_in_xml_format()
        {
            context.Request = new FakeRequest("Get", "/conneg", new Dictionary<string, IEnumerable<string>>
                {
                    {"Accept",new[]{"application/xml,application/json;q=0.9"}}
                });
            var response = this.formatter.AsConneg<Person>(null);

            var expected = @"<?xml version=""1.0""?><Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xsi:nil=""true"" />";
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                Encoding.UTF8.GetString(stream.ToArray()).Replace("\r\n", "").ShouldEqual(expected);
            }
        }

        [Fact]
        public void Json_formatter_can_deserialize_objects_of_type_Type()
        {
            context.Request = new FakeRequest("Get", "/conneg", new Dictionary<string, IEnumerable<string>>
                {
                    {"Accept",new[]{"application/json,application/xml;q=0.9"}}
                });
            var response = this.formatter.AsConneg(new { type = typeof(string) });
          
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                Encoding.UTF8.GetString(stream.ToArray()).ShouldEqual(@"{""type"":""System.String""}");
            }
        }
        
        [Fact]
        public void Xml_formatter_cannot_deserialize_anonymous_types_so_should_return_json()
        {
            context.Request = new FakeRequest("Get", "/conneg", new Dictionary<string, IEnumerable<string>>
                {
                    {"Accept",new[]{"application/xml,application/json;q=0.9"}}
                });
            var response = this.formatter.AsConneg(new { type = typeof(string) });

            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                Encoding.UTF8.GetString(stream.ToArray()).Replace("\r\n","").ShouldEqual(@"{""type"":""System.String""}");
                response.ContentType.ShouldEqual("application/json");
            }
        }

        [Fact]
        public void Can_set_status_on_json_response()
        {
            context.Request = new FakeRequest("Get", "/conneg", new Dictionary<string, IEnumerable<string>>
                {
                    {"Accept",new[]{"application/json,application/xml;q=0.9"}}
                });
            var response = this.formatter.AsConneg(new { foo = "bar" }, HttpStatusCode.InternalServerError);
            Assert.Equal(response.StatusCode, HttpStatusCode.InternalServerError);
        }
    }
}