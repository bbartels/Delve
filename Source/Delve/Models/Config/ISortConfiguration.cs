using System.Linq;

namespace Delve.Models
{
    public interface ISortConfiguration<T>
    {
        IOrderedQueryable<T> ApplySort(IOrderedQueryable<T> source);
        IOrderedQueryable<T> ApplySort(IQueryable<T> source);
    }
}
