﻿// <Auto-Generated></Auto-Generated>

namespace System.Tests.ObjectCopy;

internal struct ObjectCopyStruct2
{
    private string _specialPropertyValue;

    private string _privateFields;

    public string PublicFields;

    public string CaseFields;

    public string PublicProperty { get; set; }

    public string CaseProperty { get; set; }

    private string PrivateProperty { get; set; }

    public string SpecialProperty { set => _specialPropertyValue = value; }

    public string GetPrivateFieldsValue() => _privateFields;

    public string GetPrivatePropertyValue() => PrivateProperty;

    public string GetSpecialPropertyValue() => _specialPropertyValue;
}
