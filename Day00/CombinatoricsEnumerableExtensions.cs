namespace Day00;

public static partial class CombinatoricsEnumerableExtensions
{
    /// <summary>
    /// Generate all combinations of size k from the input list.
    /// </summary>
    public static IEnumerable<List<T>> Combinations<T>(this IReadOnlyList<T> items, int k)
    {
        if (k == 0)
            yield return new List<T>();
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                var head = items[i];
                var tail = items.Skip(i + 1).ToList();
                foreach (var tailCombo in Combinations(tail, k - 1))
                {
                    tailCombo.Insert(0, head);
                    yield return tailCombo;
                }
            }
        }
    }

    /// <summary>
    /// Generate all permutations of a list.
    /// </summary>
    public static IEnumerable<List<T>> Permutations<T>(this IReadOnlyList<T> items)
    {
        if (items.Count == 1)
        {
            yield return new List<T>(items);
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                // item at i
                var first = items[i];
                var remaining = items.Where((x, idx) => idx != i).ToList();

                // permute the remainder
                foreach (var permOfRemainder in Permutations(remaining))
                {
                    permOfRemainder.Insert(0, first);
                    yield return permOfRemainder;
                }
            }
        }
    }

}
