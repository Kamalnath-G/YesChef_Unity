using UnityEngine;

public enum IngredientType
{
    Vegetables,
    Cheese,
    Meat,
    CookedMeat,
    CutVegetables,
    None
}
public class Ingredient : MonoBehaviour
{
    public IngredientType ingredientType;
}
