using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasData(
                          new User
                          {
                              Id = 1,
                              Name = "Admin",
                              Email = "admin@gmail.com",
                              PasswordHash = "123456",
                              Role = "Admin",
                              IsActived = true
                          },
                          new User
                          {
                              Id = 2,
                              Name = "User",
                              Email = "user@gmail.com",
                              PasswordHash = "654321",
                              Role = "User",
                              IsActived = true
                          }
                      );
        }
    }
}
