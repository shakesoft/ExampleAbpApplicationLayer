using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using ExampleAbpApplicationLayer.EntityFrameworkCore;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public class EfCoreOrderItemRepository : EfCoreOrderItemRepositoryBase, IOrderItemRepository
    {
        public EfCoreOrderItemRepository(IDbContextProvider<ExampleAbpApplicationLayerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}