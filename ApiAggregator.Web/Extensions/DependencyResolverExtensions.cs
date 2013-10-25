using System.Web.Http.Dependencies;

namespace ApiAggregator.Web.Extensions
{
    public static class DependencyResolverExtensions
    {
        public static T GetService<T>(this IDependencyScope scope)
        {
            return (T)scope.GetService(typeof(T));
        }
    }
}