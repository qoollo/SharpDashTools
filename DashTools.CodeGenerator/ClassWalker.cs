using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DashTools.CodeGenerator
{
    public class ClassWalker : CSharpSyntaxWalker
    {
        public ClassWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node) 
            : base(depth)
        {
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)

        {
            var token = SyntaxFactory.Identifier(
                node.Identifier.LeadingTrivia, 
                node.Identifier.Kind(), 
                node.Identifier.ValueText.Replace("type", ""), 
                node.Identifier.ValueText.Replace("type", ""),
                node.Identifier.TrailingTrivia);
            var changed = node.WithIdentifier(token);

            base.VisitClassDeclaration(node);
        }
    }
}
