
public static class UnitUtils
{
    public static bool ContainsTag(UnitTags tagSet, UnitTags targetTags) {
        return (tagSet & targetTags) != 0;
    }
}
