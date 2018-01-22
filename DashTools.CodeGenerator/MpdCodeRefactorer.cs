using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Rename;

namespace DashTools.CodeGenerator
{
    public class MpdCodeRefactorer : CSharpSyntaxRewriter
    {
        private readonly SemanticModel semanticModel;

        public RenameSymbolInfo Refactoring { get; private set; }

        public MpdCodeRefactorer(SemanticModel semanticModel, bool visitIntoStructuredTrivia = false) 
            : base(visitIntoStructuredTrivia)
        {
            this.semanticModel = semanticModel;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            string className = node.Identifier.ValueText;
            if (className.EndsWith("type"))
            {
                className = className.Replace("type", "");

                Refactoring = new RenameSymbolInfo
                {
                    Symbol = semanticModel.GetDeclaredSymbol(node),
                    NewName = className,
                    Comment = "Remove \"type\" suffix from class name"
                };
            }

            return Refactoring == null
                ? base.VisitClassDeclaration(node)
                : node;
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            string name = node.Identifier.ValueText;
            if (name.Substring(0, 1) != name.Substring(0, 1).ToUpper())
            {
                name = name.Substring(0, 1).ToUpper() + name.Substring(1);

                Refactoring = new RenameSymbolInfo
                {
                    Symbol = semanticModel.GetDeclaredSymbol(node),
                    NewName = name,
                    Comment = "Capitalize property name's 1st letter"
                };
            }

            return base.VisitPropertyDeclaration(node);
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            foreach (var variable in node.Declaration.Variables)
            {
                string fieldName = variable.Identifier.ValueText;
                if (fieldName.EndsWith("Field"))
                {
                    fieldName = fieldName.Substring(0, fieldName.Length - "Field".Length);

                    Refactoring = new RenameSymbolInfo
                    {
                        Symbol = semanticModel.GetDeclaredSymbol(variable),
                        NewName = fieldName,
                        Comment = "Remove \"Field\" suffix from field name"
                    };
                }
            }

            return Refactoring == null
                ? base.VisitFieldDeclaration(node)
                : node;
        }
    }
}
