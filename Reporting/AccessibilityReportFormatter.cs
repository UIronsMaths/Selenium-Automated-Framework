using CSharpAxeAccessibility;
using CSharpAxeAccessibility.Utilities;
using Deque.AxeCore.Commons;
using Microsoft.CodeAnalysis;
using System.Text;
namespace CSharpAxeAccessibility.Utilities;

public static class AccessibilityReportFormatter
{
    public static string Format(AxeResult result)
    {
        if (result.Violations == null || result.Violations.Length == 0) return "No accessibility violations were found.";
        
        StringBuilder report = new();
        report.AppendLine();
        report.AppendLine($"Accessibility violations found: " + $"{result.Violations.Length}");
        int violationNumber = 1;
        foreach (AxeResultItem violation in result.Violations)
        {
            report.AppendLine();
            report.AppendLine("==================================================");
            report.AppendLine($"Violation {violationNumber++}");
            report.AppendLine($"Rule ID: {violation.Id}");
            report.AppendLine($"Impact: {violation.Impact}");
            report.AppendLine($"Description: {violation.Description}");
            report.AppendLine($"Help: {violation.Help}");
            report.AppendLine($"Help URL: {violation.HelpUrl}");
            report.AppendLine($"Affected elements: {violation.Nodes.Length}");
        
            int nodeNumber = 1;
            foreach (AxeResultNode node in violation.Nodes)
            {
                report.AppendLine();
                report.AppendLine($"Element {nodeNumber++}");
                report.AppendLine($"Target: {string.Join(", ", node.Target)}");
                report.AppendLine($"HTML: {node.Html}");

                /*
                var props = node.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var prop in props)
                {
                    if (prop.Name == "Html" || prop.Name == "Target") continue;
                    object? val;
                    try { val = prop.GetValue(node); } catch { val = null; }
                    if (val == null) continue;
                    if (val is System.Collections.IEnumerable enumerable && !(val is string))
                    {
                        var items = new System.Collections.Generic.List<string>();
                        foreach (var item in enumerable) items.Add(item?.ToString() ?? "");
                        report.AppendLine($"{prop.Name}: {string.Join(", ", items)}");
                    }
                    else
                    {
                        report.AppendLine($"{prop.Name}: {val}");
                    }
                }
                */
            }
        }
        return report.ToString();
    }
}
