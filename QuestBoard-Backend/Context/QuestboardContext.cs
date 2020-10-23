using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuestBoard.Models;
using QuestBoard_Backend.Models.BoardModels;

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

            builder.Entity<Board>().HasMany(u => u.TeamMembers);
            builder.Entity<Board>().HasOne(e => e.BoardOwner).WithOne().HasForeignKey<Board>(e => e.BoardOwnerEmail);

            base.OnModelCreating(builder);
        }
    }
}
