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

namespace ExampleAbpApplicationLayer.Orders
{
    public class EfCoreOrderRepository : EfCoreOrderRepositoryBase, IOrderRepository
    {
        public EfCoreOrderRepository(IDbContextProvider<ExampleAbpApplicationLayerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}