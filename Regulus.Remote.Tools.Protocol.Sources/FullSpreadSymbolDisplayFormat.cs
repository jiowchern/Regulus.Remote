using Microsoft.CodeAnalysis;


namespace Regulus.Remote.Tools.Protocol.Sources
{
    class FullSpreadSymbolDisplayFormat
    {
        public static readonly SymbolDisplayFormat Default = new SymbolDisplayFormat(
                SymbolDisplayGlobalNamespaceStyle.Omitted,

                SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,

                SymbolDisplayGenericsOptions.IncludeTypeParameters |
                SymbolDisplayGenericsOptions.IncludeTypeConstraints |
                SymbolDisplayGenericsOptions.IncludeVariance |
                SymbolDisplayGenericsOptions.None,


                SymbolDisplayMemberOptions.IncludeType |
                SymbolDisplayMemberOptions.IncludeModifiers |
                SymbolDisplayMemberOptions.IncludeAccessibility |
                SymbolDisplayMemberOptions.IncludeExplicitInterface |
                SymbolDisplayMemberOptions.IncludeParameters |
                SymbolDisplayMemberOptions.IncludeContainingType |
                SymbolDisplayMemberOptions.IncludeConstantValue |
                SymbolDisplayMemberOptions.IncludeRef |
                SymbolDisplayMemberOptions.None,

                    SymbolDisplayDelegateStyle.NameOnly,

                    SymbolDisplayExtensionMethodStyle.InstanceMethod,

                    /* SymbolDisplayParameterOptions.IncludeExtensionThis |
                     SymbolDisplayParameterOptions.IncludeParamsRefOut |
                     SymbolDisplayParameterOptions.IncludeType |
                     SymbolDisplayParameterOptions.IncludeName |
                     SymbolDisplayParameterOptions.IncludeDefaultValue |
                     SymbolDisplayParameterOptions.IncludeOptionalBrackets|*/
                    SymbolDisplayParameterOptions.None,

                    SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,

                    SymbolDisplayLocalOptions.IncludeRef |
                    SymbolDisplayLocalOptions.IncludeType |
                    SymbolDisplayLocalOptions.IncludeConstantValue |
                    SymbolDisplayLocalOptions.None,

                    //SymbolDisplayKindOptions.IncludeTypeKeyword | 
                    //SymbolDisplayKindOptions.IncludeNamespaceKeyword | 
                    //SymbolDisplayKindOptions.IncludeTypeKeyword|
                    SymbolDisplayKindOptions.None
                    ,

                    /*SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
                    SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                    SymbolDisplayMiscellaneousOptions.UseAsterisksInMultiDimensionalArrays |
                    SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName |
                    SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix |
                    SymbolDisplayMiscellaneousOptions.ExpandNullable |
                    SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
                    SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral |
                    SymbolDisplayMiscellaneousOptions.IncludeNotNullableReferenceTypeModifier|*/

                    SymbolDisplayMiscellaneousOptions.None

                );
    }
}