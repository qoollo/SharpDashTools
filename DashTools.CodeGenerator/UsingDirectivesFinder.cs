using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DashTools.CodeGenerator
{
    public class UsingDirectivesFinder : CSharpSyntaxWalker
    {
        public IEnumerable<NameSyntax> ExistingUsingDirectives
        {
            get { return existingUsingDirectives; }
        }
        private readonly List<NameSyntax> existingUsingDirectives = new List<NameSyntax>();

        public IEnumerable<NameSyntax> NamespacesInQualifiedNames
        {
            get { return namespacesInQualifiedNames; }
        }
        private readonly List<NameSyntax> namespacesInQualifiedNames = new List<NameSyntax>();

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            base.VisitUsingDirective(node);

            if (!existingUsingDirectives.Any(e => e.ToFullString() == node.Name.ToFullString()))
                existingUsingDirectives.Add(node.Name);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            base.VisitAttribute(node);

            if (node.Name is QualifiedNameSyntax)
            {
                var qualifiedName = node.Name as QualifiedNameSyntax;
                if (!namespacesInQualifiedNames.Any(e => e.ToFullString() == qualifiedName.Left.ToFullString()))
                {
                    namespacesInQualifiedNames.Add(qualifiedName.Left);
                }
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);

            ProcessType(node.Type);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            base.VisitFieldDeclaration(node);

            ProcessType(node.Declaration.Type);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);

            ProcessType(node.ReturnType);

            foreach (var arg in node.ParameterList.Parameters)
            {
                ProcessType(arg.Type);
            }
        }

        private void ProcessType(TypeSyntax type)
        {
            var qualifiedNames = new List<QualifiedNameSyntax>();

            if (type is IdentifierNameSyntax)
            {
                // (type as IdentifierNameSyntax).Identifier
            }
            else if (type is ArrayTypeSyntax)
            {
                var elementType = (type as ArrayTypeSyntax).ElementType;
                if (elementType is QualifiedNameSyntax)
                {
                    qualifiedNames.Add(elementType as QualifiedNameSyntax);
                }
            }
            else if (type is QualifiedNameSyntax)
            {
                qualifiedNames.Add(type as QualifiedNameSyntax);
            }

            foreach (var qualifiedName in qualifiedNames)
            {
                if (!namespacesInQualifiedNames.Any(e => e.ToFullString() == qualifiedName.Left.ToFullString()))
                {
                    namespacesInQualifiedNames.Add(qualifiedName.Left);
                }
            }
        }

    }
}
