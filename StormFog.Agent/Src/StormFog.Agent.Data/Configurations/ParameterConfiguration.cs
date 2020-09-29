using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StormFog.Agent.Entity;

namespace StormFog.Agent.Data.Configurations
{
    public class ParameterConfiguration : IEntityTypeConfiguration<SFParameter>
    {
        public void Configure(EntityTypeBuilder<SFParameter> builder)
        {
            builder.ToTable("Parameters");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(x => x.CreatedAt)
                .IsRequired();
        }
    }
}
