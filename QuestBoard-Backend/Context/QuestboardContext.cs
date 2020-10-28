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
        public DbSet<MemberOf> MemberOf { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Entity<MemberOf>().HasKey(mb => new { mb.BoardId, mb.MemberID });

            builder.Entity<MemberOf>()
                .HasOne(u => u.Member)
                .WithMany(s => s.Boards)
                .HasForeignKey(t => t.MemberID);

            builder.Entity<MemberOf>()
                .HasOne(b => b.Board)
                .WithMany(r => r.Members)
                .HasForeignKey(t => t.BoardId);

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
