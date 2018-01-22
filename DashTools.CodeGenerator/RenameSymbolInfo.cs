using Microsoft.CodeAnalysis;

namespace DashTools.CodeGenerator
{
    public class RenameSymbolInfo
    {
        public ISymbol Symbol { get; set; }

        public string NewName { get; set; }

        public string Comment { get; set; }

        public override string ToString()
        {
            return $"{Comment}.\n\tOld name: {Symbol.Name}.\n\tNew name: {NewName}.";
        }
    }
}