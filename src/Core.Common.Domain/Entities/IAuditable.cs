namespace Core.Common.Domain.Entities;

/// <summary>Entidade que registra quando foi criada e quando foi alterada pela última vez (UTC).</summary>
public interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
}
