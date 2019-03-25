namespace SeaBattle.Engine.Utils
{
    using Enums;

    internal static class FieldValidator
    {
        internal static bool IsFieldValid(Models.Field.Field field)
        {
            return field.GetCellsCountWithState(CellState.Unit) == 20;
        }
    }
}