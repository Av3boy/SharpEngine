namespace SharpEngine.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FilterAttribute : Attribute
{
    public bool IsFilterable { get; set; }

    public FilterAttribute(bool isFilterable = true)
    {
        IsFilterable = isFilterable;
    }
}
