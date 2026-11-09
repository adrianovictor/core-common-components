using Core.Common.Domain.Entities;

namespace Core.Common.Domain.Tests.Entities;

public class EntityTests
{
    private sealed class Vehicle : Entity<int>
    {
        public Vehicle(int id = 0) => Id = id;
    }

    private sealed class Driver : Entity<int>
    {
        public Driver(int id) => Id = id;
    }

    [Fact]
    public void EntitiesWithSameTypeAndIdAreEqual()
    {
        var first = new Vehicle(7);
        var second = new Vehicle(7);

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void EntitiesWithDifferentIdsAreNotEqual()
    {
        Assert.NotEqual(new Vehicle(7), new Vehicle(8));
        Assert.True(new Vehicle(7) != new Vehicle(8));
    }

    [Fact]
    public void EntitiesOfDifferentTypesAreNotEqualEvenWithSameId()
    {
        Assert.False(new Vehicle(7).Equals(new Driver(7)));
    }

    [Fact]
    public void TransientEntitiesAreOnlyEqualToThemselves()
    {
        // todas começam com Id 0: comparar pelo Id juntaria entidades diferentes
        var first = new Vehicle();
        var second = new Vehicle();

        Assert.True(first.IsTransient());
        Assert.NotEqual(first, second);
        Assert.Equal(first, first);
    }

    [Fact]
    public void EqualEntitiesAreFoundInCollections()
    {
        // é o caso do "esta posição já foi adicionada?" ao carregar a entidade de novo do banco
        var locations = new List<Vehicle> { new(7) };

        Assert.Contains(new Vehicle(7), locations);
    }

    [Fact]
    public void NullComparisonsWork()
    {
        Vehicle? none = null;

        Assert.False(new Vehicle(7).Equals(null));
        Assert.True(none == null);
        Assert.False(new Vehicle(7) == none);
    }
}
