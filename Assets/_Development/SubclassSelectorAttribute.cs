using UnityEngine;

public class SubclassSelectorAttribute : PropertyAttribute
{
}

public class SubclassSelectorListedAttribute : PropertyAttribute
{
    public SubclassSelectorListedAttribute() : base(true)
    {
    }
}
