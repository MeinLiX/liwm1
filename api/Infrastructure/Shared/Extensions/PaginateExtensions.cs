namespace Shared.Extensions;

public static class PaginateExtensions
{
    public static int ToStart(this int start, int count) => start > count - 1 ? count - 1 : start - 1;

    public static int ToCount(this int countToTake, int start, int count) => countToTake - start > count - 1 - start ? count - start : countToTake;
}