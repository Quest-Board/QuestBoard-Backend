using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuestBoard.Models;
using System;

namespace QuestBoard.Context
{
    public class QuestboardContext : IdentityDbContext<User>
    {
        public QuestboardContext(DbContextOptions<QuestboardContext> options) : base(options) { }

        public DbSet<Board> Boards { get; set; }
        public DbSet<MemberOf> MemberOf { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Column> Columns { get; set; }

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

            base.OnModelCreating(builder);
        }
    }
}
