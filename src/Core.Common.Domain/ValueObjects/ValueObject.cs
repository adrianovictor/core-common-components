namespace Core.Common.Domain.ValueObjects;

/// <summary>
/// Objeto definido pelos seus valores: dois value objects do mesmo tipo com os mesmos componentes são iguais.
/// </summary>
/// <remarks>
/// Valide os valores no construtor e não exponha setters. Para casos simples, um <c>record</c> já resolve.
/// Esta base serve quando a igualdade precisa ignorar campos ou normalizar valores.
/// </remarks>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>Componentes que definem a igualdade, sempre na mesma ordem.</summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other) =>
        other is not null
        && GetType() == other.GetType()
        && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override bool Equals(object? obj) => Equals(obj as ValueObject);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(GetType());
        foreach (var component in GetEqualityComponents())
        {
            hash.Add(component);
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
