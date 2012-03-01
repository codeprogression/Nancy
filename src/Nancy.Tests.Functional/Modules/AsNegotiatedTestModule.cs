using System;

namespace Nancy.Tests.Functional.Modules
{
    public class AsNegotiatedTestModule : NancyModule
    {
        public AsNegotiatedTestModule() : base("/negotiated")
        {
            Get["/string"] = x => "Normal Response";
            Get["/view"] = p =>
                {
                    var model = new Person("John", "Doe");
                    return Response.AsNegotiated(model, defaultResponse: new Tuple<Func<Response>, string>(()=>View[model],"text/html"));
                };
        }
    }

    public class Person
    {
        public Person()
        {
        }

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}