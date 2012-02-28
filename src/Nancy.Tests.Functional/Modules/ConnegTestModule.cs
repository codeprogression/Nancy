namespace Nancy.Tests.Functional.Modules
{
    public class ConnegTestModule : NancyModule
    {
        public ConnegTestModule() : base("/conneg")
        {
            Get["/string"] = x => "Normal Response";
            Get["/view"] = p =>
                {
                    var model = new Person("John", "Doe");
                    return Response.AsConneg(model, defaultResponse: View[model]);
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