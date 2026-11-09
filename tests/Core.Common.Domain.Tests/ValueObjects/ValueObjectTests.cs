using Core.Common.Domain.ValueObjects;

namespace Core.Common.Domain.Tests.ValueObjects;

public class ValueObjectTests
{
    private sealed class Plate(string value) : ValueObject
    {
        // a igualdade ignora maiúsculas e hífen: "ABC-1234" e "abc1234" são a mesma placa
        public string Value { get; } = value.Replace("-", "").ToUpperInvariant();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }

    private sealed class Coordinates(double latitude, double longitude) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return latitude;
            yield return longitude;
        }
    }

    private sealed class Code(string? value) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return value;
        }
    }

    [Fact]
    public void ValueObjectsWithSameComponentsAreEqual()
    {
        var first = new Plate("ABC-1234");
        var second = new Plate("abc1234");

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ComponentOrderMatters()
    {
        Assert.NotEqual(new Coordinates(-23.5, -46.6), new Coordinates(-46.6, -23.5));
    }

    [Fact]
    public void DifferentTypesAreNeverEqual()
    {
        Assert.False(new Plate("ABC1234").Equals(new Code("ABC1234")));
    }

    [Fact]
    public void NullComponentsAreCompared()
    {
        Assert.Equal(new Code(null), new Code(null));
        Assert.NotEqual(new Code(null), new Code("A"));
    }

    [Fact]
    public void WorksAsDictionaryKey()
    {
        var owners = new Dictionary<Plate, string> { [new Plate("ABC-1234")] = "Ana" };

        Assert.Equal("Ana", owners[new Plate("abc1234")]);
    }
}
