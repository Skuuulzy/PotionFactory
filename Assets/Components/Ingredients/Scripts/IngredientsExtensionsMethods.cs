using System.Collections.Generic;

namespace Components.Ingredients
{
    public static class IngredientsExtensionsMethods
    {
        /// Return a dictionary with ingredients grouped by type and count.
        public static Dictionary<IngredientTemplate, int> GroupedByTypeAndCount(this List<IngredientTemplate> ingredientsToLook)
        {
            var result = new Dictionary<IngredientTemplate, int>();
            
            foreach (var ingredient in ingredientsToLook)
            {
                if (!result.TryAdd(ingredient, 1))
                {
                    result[ingredient]++;
                }
            }
            
            return result;
        }
    }
}