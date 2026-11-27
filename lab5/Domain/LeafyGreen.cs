namespace lab5.Domain;

public sealed class LeafyGreen : Vegetable
{
    public LeafyGreen(
        string name,
        double weightGrams,
        double calories,
        string cutStyle,
        bool isBabyLeaf)
        : base(name, weightGrams, calories, cutStyle)
    {
        IsBabyLeaf = isBabyLeaf;
    }

    public bool IsBabyLeaf { get; }

    public override string GetDescription() =>
        $"Листовой овощ, нарезка: {CutStyle}, беби-листья: {(IsBabyLeaf ? "да" : "нет")}";
}
