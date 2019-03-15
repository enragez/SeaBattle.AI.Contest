namespace SeaWarsEngine
{
    using System.Collections.Generic;

    public class ColumnNamesHelper
    {
        public static readonly List<string> ColumnNames = new List<string>
                                                     {
                                                         "А","Б","В","Г","Д","Е","Ж","З","И","К"
                                                     };
        
        public static readonly Dictionary<int, string> ColumnNameByNumber = new Dictionary<int, string>
                                                                     {
                                                                         {0, "А"},
                                                                         {1, "Б"},
                                                                         {2, "В"},
                                                                         {3, "Г"},
                                                                         {4, "Д"},
                                                                         {5, "Е"},
                                                                         {6, "Ж"},
                                                                         {7, "З"},
                                                                         {8, "И"},
                                                                         {9, "К"}
                                                                     };
    }
}