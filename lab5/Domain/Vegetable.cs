using lab5.Domain;

namespace lab5.Domain;

public abstract class Vegetable : SaladIngredient
{
    protected Vegetable(
        string name,
        double weightGrams,
        double calories,
        string cutStyle)
        : base(name, weightGrams, calories)
    {
        CutStyle = string.IsNullOrWhiteSpace(cutStyle) ? "кубиком" : cutStyle;
    }

    public string CutStyle { get; }
}
