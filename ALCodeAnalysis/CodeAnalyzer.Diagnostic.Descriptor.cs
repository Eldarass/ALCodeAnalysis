

using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;

namespace ALCodeAnalysis
{
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule001VariablesNameInPascalCase = new DiagnosticDescriptor("AA001",
            "Variable name should be declarated in Pascal Case",
            "'{0}' variable may be renamed. Names should be in Pascal Case.",
            "Naming",
            DiagnosticSeverity.Warning, true,
            "Variable name should be declarated in Pascal Case",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule002VariableMayNotContainWhiteSpace = new DiagnosticDescriptor("AA002",
            "Variable name may not contain whitespace",
            "'{0}' cannot be used as a variable name. Names may not contain whitespace.",
            "Naming",
            DiagnosticSeverity.Warning, true,
            "Variable name may not contain whitespace",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule003VariableMayNotContainWildcardSymbols = new DiagnosticDescriptor("AA003",
            "Variable name may not contain wildcard symbols such as % and &",
            "'{0}' cannot be used as a variable name. Names may not contain wildcard symbols such as % and &.",
            "Naming",
            DiagnosticSeverity.Warning, true,
            "Variable name may not contain wildcard symbols such as % and &",
            (string)null, Array.Empty<string>());

       public static readonly DiagnosticDescriptor Rule004GlobalVariablesMayBeAboveTriggersProcedures = new DiagnosticDescriptor("AA004",
            "Global variables section should be above triggers and procedures",
            "Move global variables section, it may be above triggers and procedures.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Global variables section should be above triggers and procedures",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule005ObjectsMustHaveCaptionProperty = new DiagnosticDescriptor("AA005",
            "Objects which type is: Table, Page, XmlPort, Report, Query must have caption property",
            "'{0}', must have caption property. Objects which type is: Table, Page, XmlPort, Report, Query must have captions.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Objects which type is: Table, Page, XmlPort, Report, Query must have caption property",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule006TableFieldsMustHaveCaptionProperty = new DiagnosticDescriptor("AA006",
            "Table Fields must have caption property",
            "'{0}', may have caption property. Table Fields must have caption property.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Table Fields must have caption property",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule007EnumValuesMustHaveCaptionProperty = new DiagnosticDescriptor("AA007",
            "Enum Value must have caption property",
            "'{0}', may have caption property. Enum Value must have caption property.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Enum Value must have caption property",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule008PagePartsMustHaveCaptionProperty = new DiagnosticDescriptor("AA008",
           "Page parts: Request Page, Page Group, Page Part, Page Action, Page Action Group must have caption property",
            "'{0}', must have caption property. Page parts: Page Group, Page Part, Page Action, Page Action Group must have caption property.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Page parts: Request Page, Page Group, Page Part, Page Action, Page Action Group must have caption property",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule009MethodsNameInPascalCase = new DiagnosticDescriptor("AA009",
            "Procedure Name should be declarated in Pascal Case",
            "'{0}' procedure may be renamed. Procedure name should be in Pascal Case.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Procedure Name should be declarated in Pascal Case",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule010MethodsNameMayNotContainWhiteSpace = new DiagnosticDescriptor("AA010",
             "Procedure name may not contain whitespace",
            "'{0}' cannot be used as a procedure name. Procedure name may not contain whitespace.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Procedure name may not contain whitespace",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule011First19TableFieldsIDReservedToPrimaryKey = new DiagnosticDescriptor("AA011",
            "First 19 Field IDs are reserved to primary key fields.",
            "Renumber '{0}' field. First 19 Field IDs are reserved to primary key fields.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "First 19 Field IDs are reserved to primary key fields.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule012TableExtensionFieldsNumberedInDedicatedRange = new DiagnosticDescriptor("AA012",
            "All fields in table extensions should be numbered in the dedicated extension or PTE range.",
            "Renumber '{0}' field. All fields in table extensions should be numbered in the dedicated extension or PTE range.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "All fields in table extensions should be numbered in the dedicated extension or PTE range.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule013EnumExtensionsValuesNumberedInDedicatedRange = new DiagnosticDescriptor("AA013",
            "All values in enum extensions should be numbered in the dedicated extension or PTE range.",
            "Renumber '{0}' value. All values in enum extensions should be numbered in the dedicated extension or PTE range.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "All values in enum extensions should be numbered in the dedicated extension or PTE range.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule014SystemKeywordsInLowerCase = new DiagnosticDescriptor("AA0241",
            "All system keywords should be written in lower case",
            "'{0}' should be written in lower case.",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "All system keywords should be written in lower case",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule015EmptyFieldGroupsSection = new DiagnosticDescriptor("AA015",
            "Empty FieldGroups sections should be remved",
            "Remove empty FieldGroup section",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Empty FieldGroups sections should be remved",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule016EmptyActionsSection = new DiagnosticDescriptor("AA016",
         "Empty Actions sections should be remved",
         "Remove empty Actions section",
         "Readability",
         DiagnosticSeverity.Warning, true,
         "Empty Actions sections should be remved",
         (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule017EmptyOnRunTrigger = new DiagnosticDescriptor("AA017",
            "Empty OnRun triggers should be removed",
            "Remove empty OnRun trigger",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Empty OnRun triggers should be removed",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule018HardcodedIpAddress = new DiagnosticDescriptor("AA018",
            "Hardcoding IP addresses is security-sensitive",
            "Make sure using this hardcoded IP address '{0}' is safe here.",
            "Security",
            DiagnosticSeverity.Warning, true,
            "Hardcoding IP addresses is security - sensitive",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule019UseParenthesisForFunctionCall = new DiagnosticDescriptor("AA0008",
            "Use parenthesis in a function call even if the function does not have any parameters.",
            "You must specify open and close parenthesis after '{0}'",
            "Readability",
            DiagnosticSeverity.Warning, true,
            "Use parenthesis in a function call even if the function does not have any parameters.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule020InitializedVariablesMustAlwaysBeUsed = new DiagnosticDescriptor("AA020",
            "The value assigned to a variable must be used, otherwise the variable is not necessary.",
            "The variable '{0}' is initialized but not used.",
            "Reliability",
            DiagnosticSeverity.Warning, true,
            "The value assigned to a variable must be used.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule021VariablesMustAlwaysBeInitialized = new DiagnosticDescriptor("AA021",
           "Always initialize a variable before usage. This can improve readability and make debugging easier.",
            "Use of unassigned variable '{0}'.",
            "Reliability",
            DiagnosticSeverity.Warning, true,
            "Variables must be initialized before usage.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule022GlobalLocalVariablesShouldNotHaveSameName = new DiagnosticDescriptor("AA022",
            "Do not use identical names for local and global variables.",
            "The name of the local variable '{0}' is identical to a global variable.",
            "Reliability",
            DiagnosticSeverity.Warning, true,
            "Do not use identical names for local and global variables.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule023DoNotNameLocalVarAsFieldOrMethod = new DiagnosticDescriptor("AA023",
            "To avoid confusion, do not give local variables the same name as fields, methods or actions in the same scope.",
            "The name of the local variable '{0}' is identical to a field, method or action in the same scope.",
            "Reliability",
            DiagnosticSeverity.Warning, true,
            "To avoid confusion, do not give local variables the same name as fields, methods or actions in the same scope.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule024DoNotNameMethodAsField = new DiagnosticDescriptor("AA024",
            "To avoid confusion, do not give methods the same name as fields or actions in the same scope.",
            "The name of the method '{0}' is identical to a field or action in the same scope.",
            "Reliability",
            DiagnosticSeverity.Warning, true,
            "To avoid confusion, do not give methods the same name as fields or actions in the same scope.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule025DoNotNameGlobalVarAsFieldOrMethod = new DiagnosticDescriptor("AA025",
            "To avoid confusion, do not give global variables the same name as fields, methods or actions in the same scope.",
            "The name of the global variable '{0}' is identical to a field, method or action in the same scope.",
            "Reliability",
            DiagnosticSeverity.Warning, true,
            "To avoid confusion, do not give global variables the same name as fields, methods or actions in the same scope.",
            (string)null, Array.Empty<string>());

        public static readonly DiagnosticDescriptor Rule026EmailAndPhoneNoMustNotBePresentInTheSource = new DiagnosticDescriptor("AA026",
            "Email and Phone No must not be present in any part of the source code.",
            "The {0} '{1}' must not be contain Email or Phone No.",
            "Design",
            DiagnosticSeverity.Warning, true,
            "Email and Phone No must not be present in any part of the source code that might be collected as telemetry data.",
            (string)null, Array.Empty<string>());
    }
}
