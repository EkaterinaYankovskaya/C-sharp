using lab5.Domain;

namespace lab5.Interfaces;

public interface ISaladAnalyzer
{
    double GetTotalCalories();
    double GetTotalWeightGrams();
    void SortByDefault();
    void SortByCalories();
    IReadOnlyList<Vegetable> FindVegetablesByCalorieRange(double minCalories, double maxCalories);
}