using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomAnalyzer.Tests;

using VerifyCS = CSharpAnalyzerVerifier<SensitiveWordAnalyzer, MSTestVerifier>;

[TestClass]
public class SensitiveWordAnalyzerTests
{
    [TestMethod]
    public async Task TestMethod1Async()
    {
        string code = @"using System;
class Program
{
    static void Main()
    {
        Console.WriteLine(""Hello"");
        Console.WriteLine(""whitelist"");
        Console.WriteLine(""blacklist"");
    }
}".Trim();
        Console.WriteLine(code);
        await VerifyCS.VerifyAnalyzerAsync(
            code,
            VerifyCS.Diagnostic("SW0001")
                .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
                .WithSpan(7, 27, 7, 38)
                .WithMessage("The word 'whitelist' might be offensive to some people. Please use a neutral word instead, for example, 'allowlist'."),
            VerifyCS.Diagnostic("SW0001")
                .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
                .WithSpan(8, 27, 8, 38)
                .WithMessage("The word 'blacklist' might be offensive to some people. Please use a neutral word instead, for example, 'disallow list', 'blocklist'."));
    }
}