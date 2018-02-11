namespace Delve.Models
{
    public interface IQueryValidator
    {
        void Validate(IResourceParameter parameters);
    }

    public interface IQueryValidator<T> : IQueryValidator { }
}
