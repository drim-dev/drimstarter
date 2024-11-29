using FluentAssertions;

namespace Drimstarter.Common.Tests.Assertions;

public static class AssertionExtensions
{
    public static void ShouldBeEquivalentTo<TActual, TExpected>(this IEnumerable<TActual> actual,
        IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> selector, Action<TActual, TExpected> assertion)
    {
        actual.Should().HaveCount(expected.Count());
        foreach (var expectedItem in expected)
        {
            var actualItem = actual.SingleOrDefault(x => selector(x, expectedItem));
            actualItem.Should().NotBeNull();

            assertion(actualItem!, expectedItem);
        }
    }
}
