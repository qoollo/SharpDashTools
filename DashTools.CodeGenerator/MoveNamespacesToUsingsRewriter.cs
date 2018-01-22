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
    public class MoveNamespacesToUsingsRewriter : CSharpSyntaxRewriter
    {
        private List<NameSyntax> namespacesToAdd = null;

        public MoveNamespacesToUsingsRewriter(bool visitIntoStructuredTrivia = false) 
            : base(visitIntoStructuredTrivia)
        {
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (namespacesToAdd == null)
            {
                var finder = new UsingDirectivesFinder();
                finder.Visit(node);
                var existingUsingDirectives = finder.ExistingUsingDirectives.ToList();
                namespacesToAdd = finder
                    .NamespacesInQualifiedNames
                    .Where(e => !existingUsingDirectives.Any(ee => ee.ToFullString() == e.ToFullString()))
                    .ToList();
            }

            node = base.Visit(node);

            return node;
        }

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            var usingsToAdd = namespacesToAdd
                .Select(e => e.WithLeadingTrivia(SyntaxFactory.Whitespace(" ")))
                .Select(e => SyntaxFactory.UsingDirective(e).WithLeadingTrivia(SyntaxFactory.Tab))
                .Select(e => e.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed));
            var newUsings = node.Usings.AddRange(usingsToAdd);

            return (base.VisitNamespaceDeclaration(node) as NamespaceDeclarationSyntax)
                .WithUsings(new SyntaxList<UsingDirectiveSyntax>(newUsings));
        }

        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            node = base.VisitAttribute(node) as AttributeSyntax;

            if (node.Name is QualifiedNameSyntax)
            {
                var qualifiedName = node.Name as QualifiedNameSyntax;
                var simpleName = qualifiedName.Right;
                node = node.WithName(simpleName);
            }

            return node;
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            node = base.VisitPropertyDeclaration(node) as PropertyDeclarationSyntax;

            var type = GetTypeWithoutNamespaces(node.Type).WithTriviaFrom(node.Type);
            node = node
                .WithType(type)
                .WithTriviaFrom(node);

            return node;
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            node = base.VisitFieldDeclaration(node) as FieldDeclarationSyntax;

            var type = GetTypeWithoutNamespaces(node.Declaration.Type)
                .WithTriviaFrom(node.Declaration.Type);
            var declaration = node.Declaration
                    .WithType(type)
                    .WithTriviaFrom(node.Declaration);
            node = node.WithDeclaration(declaration);

            return node;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            node = base.VisitMethodDeclaration(node) as MethodDeclarationSyntax;

            node = node.WithReturnType(GetTypeWithoutNamespaces(node.ReturnType))
                .WithLeadingTrivia(node.ReturnType.GetLeadingTrivia())
                .WithTrailingTrivia(node.ReturnType.GetTrailingTrivia());


            var parameters = node.ParameterList.Parameters;
            for (int i = 0; i < parameters.Count; i++)
            {
                var arg = parameters[i];
                parameters = parameters.Replace(
                    arg, 
                    arg.WithType(GetTypeWithoutNamespaces(arg.Type))
                        .WithLeadingTrivia(arg.Type.GetLeadingTrivia())
                        .WithTrailingTrivia(arg.Type.GetTrailingTrivia())
                );
            }
            node = node.WithParameterList(
                node.ParameterList
                    .WithParameters(parameters)
                    .WithLeadingTrivia(node.ParameterList.GetLeadingTrivia())
                    .WithTrailingTrivia(node.ParameterList.GetTrailingTrivia())
            );

            return node;
        }

        private TypeSyntax GetTypeWithoutNamespaces<T>(T type)
            where T : TypeSyntax
        {
            QualifiedNameSyntax qualifiedName = null;
            if (type is QualifiedNameSyntax)
            {
                qualifiedName = type as QualifiedNameSyntax;
            }
            else if (type is ArrayTypeSyntax && (type as ArrayTypeSyntax).ElementType is QualifiedNameSyntax)
            {
                qualifiedName = (type as ArrayTypeSyntax).ElementType as QualifiedNameSyntax;
            }

            return qualifiedName?.Right as TypeSyntax ?? type;
        }
    }
}
