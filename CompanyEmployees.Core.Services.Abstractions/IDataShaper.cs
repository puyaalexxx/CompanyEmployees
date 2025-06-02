using System.Dynamic;

namespace CompanyEmployees.Core.Services.Abstractions
{
    public interface IDataShaper<T>
    {
        IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> enitites, string fieldsString);
        ExpandoObject ShapeData(T entity, string fieldsString);
    }
}
