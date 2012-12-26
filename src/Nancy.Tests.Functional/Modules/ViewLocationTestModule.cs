namespace Nancy.Tests.Functional.Modules
{
    public class ViewLocationTestModule : NancyModule
    {
        public ViewLocationTestModule()
            : base(@"/(?<name>[\w]+)")
        {
            Get["/test"] = _ => View["LocationTest"];
        }
    }
}