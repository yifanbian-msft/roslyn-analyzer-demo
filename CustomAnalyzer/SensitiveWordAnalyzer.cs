using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CustomAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SensitiveWordAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "SW0001";
    private const string Category = "Usage";
    private const string Title = "Sensitive word";
    private const string Description = "The word '{0}' might be offensive to some people. Please use a neutral word instead, for example, '{1}'.";
    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, Description, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
    private static readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> WordBlockList = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase)
    {
        ["whitelist"] = new[] { "allowlist" },
        ["blacklist"] = new[] { "disallow list", "blocklist" },
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.StringLiteralExpression);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var syntax = context.Node as LiteralExpressionSyntax;
        if (syntax == null)
        {
            return;
        }

        var value = syntax.Token.Value as string;
        if (value == null)
        {
            return;
        }

        var tokens = value.Split(new[] { ',', ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            if (WordBlockList.TryGetValue(token, out var alternatives))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), token, string.Join("', '", alternatives)));
            }
        }
    }
}
