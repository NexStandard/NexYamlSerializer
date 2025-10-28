namespace NexYaml.Parser
{
    public static class YamlScopeExtensions
    {
        public static T As<T>(this Scope scope)
            where T : Scope
        {
            if (scope is T castedScope)
            {
                return castedScope;
            }
            throw new InvalidCastException($"Expected: {typeof(T).Name} but got {scope.Kind}");
        }
    }
}
