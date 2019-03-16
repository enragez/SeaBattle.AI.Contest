namespace SeaWarsEngine.Models
{
    internal static class FieldValidator
    {
        internal static bool IsFieldValid(Field field)
        {
            return field.GetCellsCountWithState(CellState.Unit) == 20;
        }
    }
}