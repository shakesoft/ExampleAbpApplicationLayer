using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.TextTemplating;
using Xunit;

namespace Volo.Abp.AuditLogging;

public class AuditLogEmailTemplates_Tests : AbpAuditLoggingTestBase<AbpAuditLoggingTestBaseModule>
{
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ITemplateDefinitionManager _templateDefinitionManager;

    public AuditLogEmailTemplates_Tests()
    {
        _templateRenderer = GetRequiredService<ITemplateRenderer>();
        _templateDefinitionManager = GetRequiredService<ITemplateDefinitionManager>();
    }

    [Fact]
    public async Task Should_Define_AuditLogExportCompleted_Template()
    {
        // Act
        var templateDefinition = await _templateDefinitionManager.GetOrNullAsync(AuditLoggingEmailTemplates.AuditLogExportCompleted);

        // Assert
        templateDefinition.ShouldNotBeNull();
        templateDefinition.Name.ShouldBe(AuditLoggingEmailTemplates.AuditLogExportCompleted);
    }

    [Fact]
    public async Task Should_Define_AuditLogExportFailed_Template()
    {
        // Act
        var templateDefinition = await _templateDefinitionManager.GetOrNullAsync(AuditLoggingEmailTemplates.AuditLogExportFailed);

        // Assert
        templateDefinition.ShouldNotBeNull();
        templateDefinition.Name.ShouldBe(AuditLoggingEmailTemplates.AuditLogExportFailed);
    }

    [Fact]
    public async Task Should_Define_EntityChangeExportCompleted_Template()
    {
        // Act
        var templateDefinition = await _templateDefinitionManager.GetOrNullAsync(AuditLoggingEmailTemplates.EntityChangeExportCompleted);

        // Assert
        templateDefinition.ShouldNotBeNull();
        templateDefinition.Name.ShouldBe(AuditLoggingEmailTemplates.EntityChangeExportCompleted);
    }

    [Fact]
    public async Task Should_Define_EntityChangeExportFailed_Template()
    {
        // Act
        var templateDefinition = await _templateDefinitionManager.GetOrNullAsync(AuditLoggingEmailTemplates.EntityChangeExportFailed);

        // Assert
        templateDefinition.ShouldNotBeNull();
        templateDefinition.Name.ShouldBe(AuditLoggingEmailTemplates.EntityChangeExportFailed);
    }

    [Fact]
    public async Task Should_Render_AuditLogExportCompleted_Template()
    {
        // Arrange
        var model = new
        {
            downloadLink = "https://example.com/download/file.xlsx",
            recordCount = 100,
            linkExpirationUtcTime = "2023-01-01 12:00:00"
        };

        // Act
        var result = await _templateRenderer.RenderAsync(
            AuditLoggingEmailTemplates.AuditLogExportCompleted,
            model
        );

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain(model.downloadLink);
    }

    [Fact]
    public async Task Should_Render_AuditLogExportFailed_Template()
    {
        // Arrange
        var model = new
        {
            errorMessage = "Test error message"
        };

        // Act
        var result = await _templateRenderer.RenderAsync(
            AuditLoggingEmailTemplates.AuditLogExportFailed,
            model
        );

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain(model.errorMessage);
    }

    [Fact]
    public async Task Should_Render_EntityChangeExportCompleted_Template()
    {
        // Arrange
        var model = new
        {
            downloadLink = "https://example.com/download/file.xlsx",
            recordCount = 100,
            linkExpirationUtcTime = "2023-01-01 12:00:00"
        };

        // Act
        var result = await _templateRenderer.RenderAsync(
            AuditLoggingEmailTemplates.EntityChangeExportCompleted,
            model
        );

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain(model.downloadLink);
    }

    [Fact]
    public async Task Should_Render_EntityChangeExportFailed_Template()
    {
        // Arrange
        var model = new
        {
            errorMessage = "Test error message"
        };

        // Act
        var result = await _templateRenderer.RenderAsync(
            AuditLoggingEmailTemplates.EntityChangeExportFailed,
            model
        );

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain(model.errorMessage);
    }
} 