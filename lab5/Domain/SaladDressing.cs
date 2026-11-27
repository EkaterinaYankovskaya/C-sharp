namespace lab5.Domain;

public sealed class SaladDressing : SaladIngredient
{
    public SaladDressing(
        string name,
        double weightGrams,
        double calories,
        string baseType)
        : base(name, weightGrams, calories)
    {
        BaseType = string.IsNullOrWhiteSpace(baseType) ? "растительное масло" : baseType;
    }

    public string BaseType { get; }

    public override string GetDescription() =>
        $"Заправка, основа: {BaseType}";
}