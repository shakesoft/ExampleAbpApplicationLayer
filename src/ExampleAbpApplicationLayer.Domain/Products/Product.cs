using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace ExampleAbpApplicationLayer.Products
{
    public abstract class ProductBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string? Desc { get; set; }

        public virtual float Price { get; set; }

        public virtual bool IsActive { get; set; }

        protected ProductBase()
        {

        }

        public ProductBase(Guid id, string name, float price, bool isActive, string? desc = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            Price = price;
            IsActive = isActive;
            Desc = desc;
        }

    }
}