using lab5.Domain;
using lab5.Services;

namespace lab5.Interfaces;

public interface ISaladBuilder
{
    void Reset();
    void AddIngredient(SaladIngredient ingredient);
    void AddIngredients(IEnumerable<SaladIngredient> ingredients);
    Salad Build();
}