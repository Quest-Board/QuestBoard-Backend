using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using QuestBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestBoard.Context
{
    public class QuestboardContext : IdentityDbContext<User>
    {
        public QuestboardContext(DbContextOptions<QuestboardContext> options) : base(options) { }

        public DbSet<Board> Boards { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Entity<Board>()
                .Property(b => b.Columns)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<string>>(v)
                );

            builder.Entity<Board>()
                .Property(b => b.Columns)
                .Metadata
                .SetValueComparer(
                    new ValueComparer<List<string>>(
                        (a, b) => a.SequenceEqual(b),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );

            base.OnModelCreating(builder);
        }
    }
}
