using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Validation;

namespace Volo.Abp.AuditLogging;

public class ExportEntityChangesInput : IValidatableObject
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public EntityChangeType? EntityChangeType { get; set; }

    public string EntityId { get; set; }

    public string EntityTypeFullName { get; set; }

    public string Sorting { get; set; }

    public System.Collections.Generic.IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
        {
            yield return new ValidationResult("Start date cannot be greater than end date.", new[] { nameof(StartDate), nameof(EndDate) });
        }
    }
} 