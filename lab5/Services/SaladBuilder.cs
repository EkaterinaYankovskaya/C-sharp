using lab5.Domain;
using lab5.Interfaces;

namespace lab5.Services;

public sealed class SaladBuilder : ISaladBuilder
{
    private readonly List<SaladIngredient> _currentIngredients = [];
    private string _dishName = "Фирменный салат";

    public SaladBuilder()
    {
        Reset();
    }

    public void SetDishName(string dishName)
    {
        if (!string.IsNullOrWhiteSpace(dishName))
            _dishName = dishName;
    }

    public void Reset()
    {
        _currentIngredients.Clear();
        _dishName = "Фирменный салат";
    }

    public void AddIngredient(SaladIngredient ingredient)
    {
        ArgumentNullException.ThrowIfNull(ingredient);
        _currentIngredients.Add(ingredient);
    }

    public void AddIngredients(IEnumerable<SaladIngredient> ingredients)
    {
        ArgumentNullException.ThrowIfNull(ingredients);
        foreach (var ingredient in ingredients)
        {
            AddIngredient(ingredient);
        }
    }

    public Salad Build()
    {
        var salad = new Salad(_dishName, _currentIngredients);
        Reset();
        return salad;
    }
}