using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StormFog.Agent.Entity;
using System;
using System.Globalization;

namespace StormFog.Agent.Data.Configurations
{
    public class JobConfiguration : IEntityTypeConfiguration<SFJob>
    {
        public void Configure(EntityTypeBuilder<SFJob> builder)
        {
            builder.ToTable("Jobs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Class)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(x => x.CronExpression)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(x => x.CreatedAt)
                .IsRequired();
        }
    }
}
