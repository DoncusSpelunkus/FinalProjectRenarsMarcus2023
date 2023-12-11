namespace Application.helpers;

public class ObjectSorter<T>
{
    public List<T> SortByProperty(IEnumerable<T> entities, string propertyName, bool ascending = true)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        if (string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        var propertyInfo = typeof(T).GetProperty(propertyName);
        if (propertyInfo == null)
        {
            throw new ArgumentException("entity type not found");
        }

        if (ascending)
        {
            return entities.OrderBy(e => propertyInfo.GetValue(e, null)).ToList();
        }
        else
        {
            return entities.OrderByDescending(e => propertyInfo.GetValue(e, null)).ToList();
        }
    }
}