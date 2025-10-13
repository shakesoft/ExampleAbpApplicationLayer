using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.TextTemplating;
using Volo.Abp.TextTemplating.Scriban;

namespace Volo.Abp.AuditLogging;

public class AuditLoggingEmailTemplateDefinitionProvider : TemplateDefinitionProvider
{
    public override void Define(ITemplateDefinitionContext context)
    {
        // Entity Change Export Templates
        context.Add(
            new TemplateDefinition(
                AuditLoggingEmailTemplates.EntityChangeExportCompleted,
                localizationResource: typeof(AuditLoggingResource),
                defaultCultureName: "en"
            )
            .WithScribanEngine()
            .WithVirtualFilePath(
                "/Volo/Abp/AuditLogging/Templates/EntityChangeExportCompleted.tpl",
                isInlineLocalized: true
            )
        );

        context.Add(
            new TemplateDefinition(
                AuditLoggingEmailTemplates.EntityChangeExportFailed,
                localizationResource: typeof(AuditLoggingResource),
                defaultCultureName: "en"
            )
            .WithScribanEngine()
            .WithVirtualFilePath(
                "/Volo/Abp/AuditLogging/Templates/EntityChangeExportFailed.tpl",
                isInlineLocalized: true
            )
        );

        // Audit Log Export Templates
        context.Add(
            new TemplateDefinition(
                AuditLoggingEmailTemplates.AuditLogExportCompleted,
                localizationResource: typeof(AuditLoggingResource),
                defaultCultureName: "en"
            )
            .WithScribanEngine()
            .WithVirtualFilePath(
                "/Volo/Abp/AuditLogging/Templates/AuditLogExportCompleted.tpl",
                isInlineLocalized: true
            )
        );

        context.Add(
            new TemplateDefinition(
                AuditLoggingEmailTemplates.AuditLogExportFailed,
                localizationResource: typeof(AuditLoggingResource),
                defaultCultureName: "en"
            )
            .WithScribanEngine()
            .WithVirtualFilePath(
                "/Volo/Abp/AuditLogging/Templates/AuditLogExportFailed.tpl",
                isInlineLocalized: true
            )
        );
    }
} 