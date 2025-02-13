namespace KedemMarket.Factories.Directory;

public class DirectoryFactory : IDirectoryFactory
{
    public string ProcessPhoneNumber(string sourcePhone)
    {
        if (sourcePhone == null)
            return null;
        //remove non number characters
        var res = new List<char>();
        for (var i = 0; i < sourcePhone.Length; i++)
        {
            var nv = char.GetNumericValue(sourcePhone[i]);
            if (nv < 0 || nv > 9)
                continue;
            res.Add(sourcePhone[i]);
        }
        if (res.ElementAt(0) != 0)
            res.Insert(0, '+');

        return new string(res.ToArray());
    }

}
