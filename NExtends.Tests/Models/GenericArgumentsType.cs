using Newtonsoft.Json; 

namespace NExtends.Tests.Models
{
    public abstract class GenericArgumentsType<T, U>
    {
    }

    public class ConcreteClass : GenericArgumentsType<string, int>
    {
    }

    [JsonObject]
    public class ConcreteClassWithAttribute : GenericArgumentsType<string, int>
    {
    }
}
