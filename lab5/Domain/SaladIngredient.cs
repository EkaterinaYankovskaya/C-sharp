namespace lab5.Domain;

public abstract class SaladIngredient : IComparable<SaladIngredient>
{
    protected SaladIngredient(string name, double weightGrams, double calories)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название ингредиента не может быть пустым.", nameof(name));
        if (weightGrams <= 0)
            throw new ArgumentOutOfRangeException(nameof(weightGrams), "Вес должен быть положительным.");
        if (calories < 0)
            throw new ArgumentOutOfRangeException(nameof(calories), "Калорийность не может быть отрицательной.");

        Name = name;
        WeightGrams = weightGrams;
        Calories = calories;
    }

    public string Name { get; }
    public double WeightGrams { get; }
    public double Calories { get; }

    public abstract string GetDescription();

    public int CompareTo(SaladIngredient? other)
    {
        if (other is null) return 1;
        return Calories.CompareTo(other.Calories);
    }

    public override string ToString() =>
        $"{Name} — {WeightGrams:F1} г, {Calories:F1} ккал | {GetDescription()}";
}
