using lab5.Domain;
using lab5.Services;

var chefIngredients = new SaladIngredient[]
{
    new LeafyGreen("Руккола", 40.0, 10.0, "рваными листьями", true),
    new RootVegetable("Морковь", 80.0, 28.0, "тонкой соломкой", true),
    new RootVegetable("Редис", 60.0, 11.4, "тонкими кружочками", false),
    new LeafyGreen("Салат Романо", 70.0, 11.9, "крупно", false),
    new SaladDressing("Оливково-лимонная заправка", 25.0, 145.0, "оливковое масло extra virgin")
};

var builder = new SaladBuilder();
builder.SetDishName("Свежий витаминный салат от шефа");
builder.AddIngredients(chefIngredients);

Salad salad = builder.Build();

Console.WriteLine($"=== {salad.DishName} ===");
Console.WriteLine($"Общий вес: {salad.GetTotalWeightGrams():F1} г | Общая калорийность: {salad.GetTotalCalories():F1} ккал\n");

Console.WriteLine("--- Состав салата (исходный порядок) ---");
PrintIngredients(salad.Ingredients);

salad.SortByCalories();
Console.WriteLine("\n--- Сортировка по калорийности (по возрастанию) ---");
PrintIngredients(salad.Ingredients);

const double minCalories = 10.0;
const double maxCalories = 20.0;
var matchingVegetables = salad.FindVegetablesByCalorieRange(minCalories, maxCalories);

Console.WriteLine($"\n--- Овощи с калорийностью от {minCalories:F1} до {maxCalories:F1} ккал ---");
if (matchingVegetables.Count == 0)
{
    Console.WriteLine("Подходящих овощей не найдено.");
}
else
{
    PrintIngredients(matchingVegetables);
}

static void PrintIngredients(IEnumerable<SaladIngredient> ingredients)
{
    foreach (var ingredient in ingredients)
    {
        Console.WriteLine($"• {ingredient}");
    }
}