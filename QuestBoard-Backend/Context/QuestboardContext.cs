﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuestBoard.Models;

namespace QuestBoard.Context
{
    public class QuestboardContext : IdentityDbContext<User>
    {
        public QuestboardContext(DbContextOptions<QuestboardContext> options) : base(options) { }
    }
}
