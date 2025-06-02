using CompanyEmployees.Core.Domain.Entities;


namespace CompanyEmployees.Core.Services.Abstractions
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> enitites, string fieldsString);
        ShapedEntity ShapeData(T entity, string fieldsString);
    }
}
