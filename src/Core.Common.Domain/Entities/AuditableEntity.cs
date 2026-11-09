namespace Core.Common.Domain.Entities;

/// <summary>
/// Entidade com datas de criação e de última alteração, em UTC.
/// </summary>
/// <remarks>
/// O domínio não preenche essas datas. Quem preenche é a persistência, por exemplo um
/// <c>SaveChangesInterceptor</c> do EF Core:
/// <code>
/// entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = DateTime.UtcNow;
/// </code>
/// </remarks>
public abstract class AuditableEntity<TId> : Entity<TId>, IAuditable
    where TId : notnull
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
}
