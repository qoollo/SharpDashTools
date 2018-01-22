using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashTools.CodeGenerator
{
    public class RemoveAttributeSuffixRewriter : CSharpSyntaxRewriter
    {
        public RemoveAttributeSuffixRewriter(bool visitIntoStructuredTrivia = false) 
            : base(visitIntoStructuredTrivia)
        {
        }

        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            node = base.VisitAttribute(node) as AttributeSyntax;

            if (node.Name is QualifiedNameSyntax)
            {
                var qualifiedName = node.Name as QualifiedNameSyntax;
                var newRight = RemoveTrailingAttributeFromName(qualifiedName.Right);
                var newQualifiedName = qualifiedName.WithRight(newRight);
                node = node.WithName(newQualifiedName);
            }
            else if (node.Name is SimpleNameSyntax)
            {
                var newName = RemoveTrailingAttributeFromName(node.Name as SimpleNameSyntax);
                node = node.WithName(newName);
            }
            else if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            return node;
        }

        private SimpleNameSyntax RemoveTrailingAttributeFromName(SimpleNameSyntax simpleName)
        {
            string attributeName = simpleName.Identifier.ValueText;
            if (attributeName.EndsWith("Attribute"))
            {
                attributeName = attributeName.Substring(0, attributeName.Length - "Attribute".Length);

                var identifier = SyntaxFactory.Identifier(
                    simpleName.Identifier.LeadingTrivia,
                    simpleName.Identifier.Kind(),
                    attributeName,
                    attributeName,
                    simpleName.Identifier.TrailingTrivia
                );
                simpleName = simpleName.WithIdentifier(identifier);
            }

            return simpleName;
        }
    }
}
