namespace Core.Common.Domain.Entities;

/// <summary>
/// Entidade com identidade: duas instâncias são iguais quando têm o mesmo tipo e o mesmo <see cref="Id"/>.
/// </summary>
/// <remarks>
/// Entidades ainda não persistidas (<see cref="IsTransient"/>) só são iguais a si mesmas,
/// porque todas começam com o mesmo Id padrão.
/// </remarks>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    /// <summary>Indica se a entidade ainda não recebeu um Id (não foi persistida).</summary>
    public bool IsTransient() => EqualityComparer<TId>.Default.Equals(Id, default!);

    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        if (IsTransient() || other.IsTransient()) return false;

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Entity<TId>);

    // Entidade transitória usa o hash da referência. Evite colocá-la num HashSet antes de persistir,
    // porque o hash muda quando ela recebe o Id.
    public override int GetHashCode() =>
        IsTransient() ? base.GetHashCode() : HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
}
