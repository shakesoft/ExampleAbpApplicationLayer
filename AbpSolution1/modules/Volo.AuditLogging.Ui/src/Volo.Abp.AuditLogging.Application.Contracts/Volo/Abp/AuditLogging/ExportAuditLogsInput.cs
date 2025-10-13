using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Volo.Abp.AuditLogging;

public class ExportAuditLogsInput : IValidatableObject
{
    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    [DynamicStringLength(typeof(AuditLogDtoCommonConsts), nameof(AuditLogDtoCommonConsts.UrlFilterMaxLength))]
    public string Url { get; set; }

    public string ClientId { get; set; }

    [DynamicStringLength(typeof(AuditLogDtoCommonConsts), nameof(AuditLogDtoCommonConsts.UserNameFilterMaxLength))]
    public string UserName { get; set; }

    public string ApplicationName { get; set; }

    public string ClientIpAddress { get; set; }

    public string CorrelationId { get; set; }

    [DynamicStringLength(typeof(AuditLogDtoCommonConsts), nameof(AuditLogDtoCommonConsts.HttpMethodFilterMaxLength))]
    public string HttpMethod { get; set; }

    public HttpStatusCode? HttpStatusCode { get; set; }

    public int? MaxExecutionDuration { get; set; }

    public int? MinExecutionDuration { get; set; }

    public bool? HasException { get; set; }

    public string Sorting { get; set; }

    public System.Collections.Generic.IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime.HasValue && EndTime.HasValue && StartTime.Value > EndTime.Value)
        {
            yield return new ValidationResult("Start time cannot be greater than end time.", new[] { nameof(StartTime), nameof(EndTime) });
        }
    }
} 