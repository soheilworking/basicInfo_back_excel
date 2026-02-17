namespace ORG.BasicInfo.API.Features.Abstractions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireSessionValidationAttribute : Attribute
    {
    }

}
