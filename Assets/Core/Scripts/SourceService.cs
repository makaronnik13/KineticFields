using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceService
{
    private List<Source> Sources;

    public void RegisterSource(Source source)
    {
        Sources.Add(source);
    }

    public void RemoveSource(Source source)
    {
        Sources.Remove(source);
    }
}
