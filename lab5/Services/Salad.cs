using lab5.Domain;
using lab5.Interfaces;

namespace lab5.Services;

public sealed class Salad : ISaladAnalyzer
{
    private readonly List<SaladIngredient> _ingredients;

    public Salad(string dishName, IEnumerable<SaladIngredient> ingredients)
    {
        ArgumentNullException.ThrowIfNull(ingredients);

        DishName = string.IsNullOrWhiteSpace(dishName) ? "Фирменный салат" : dishName;
        _ingredients = ingredients.ToList();
    }

    public string DishName { get; }
    public IReadOnlyList<SaladIngredient> Ingredients => _ingredients;

    public double GetTotalCalories() => _ingredients.Sum(i => i.Calories);

    public double GetTotalWeightGrams() => _ingredients.Sum(i => i.WeightGrams);

    public void SortByDefault() => _ingredients.Sort();

    public void SortByCalories() =>
        _ingredients.Sort((left, right) => left.Calories.CompareTo(right.Calories));

    public IReadOnlyList<Vegetable> FindVegetablesByCalorieRange(double minCalories, double maxCalories)
    {
        if (minCalories > maxCalories)
            throw new ArgumentException("Минимальная калорийность не может превышать максимальную.");

        return _ingredients
            .OfType<Vegetable>()
            .Where(v => v.Calories >= minCalories && v.Calories <= maxCalories)
            .ToList();
    }
}