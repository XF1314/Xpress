using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.EntityFramework;

namespace Xpress.Demo.EntityFramework
{
    public class DemoDbContext : EfDbContextBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; private set; }

        public DemoDbContext(DbContextOptions<DemoDbContext> options)
            : base(options)
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
