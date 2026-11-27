namespace lab5.Domain;

public sealed class RootVegetable : Vegetable
{
    public RootVegetable(
        string name,
        double weightGrams,
        double calories,
        string cutStyle,
        bool isPeeled)
        : base(name, weightGrams, calories, cutStyle)
    {
        IsPeeled = isPeeled;
    }

    public bool IsPeeled { get; }

    public override string GetDescription() =>
        $"Корнеплод, нарезка: {CutStyle}, очищен: {(IsPeeled ? "да" : "нет")}";
}
