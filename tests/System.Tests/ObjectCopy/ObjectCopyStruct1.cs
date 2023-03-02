﻿// <Auto-Generated></Auto-Generated>

namespace System.Tests.ObjectCopy;

internal struct ObjectCopyStruct1
{
    private string _privateFields;

    public string PublicFields;

    public string casefields;

    public string PublicProperty { get; set; }

    public string caseproperty { get; set; }

    private string PrivateProperty { get; set; }

    public string SpecialProperty { get; }

    public ObjectCopyStruct1(string privateFields, string publicFields, string casefields, string publicProperty, string caseproperty, string privateProperty, string specialProperty)
    {
        _privateFields = privateFields;
        PublicFields = publicFields;
        this.casefields = casefields;
        PublicProperty = publicProperty;
        this.caseproperty = caseproperty;
        PrivateProperty = privateProperty;
        SpecialProperty = specialProperty;
    }

    public string GetPrivateFieldsValue() => _privateFields;

    public string GetPrivatePropertyValue() => PrivateProperty;

    public override bool Equals(object obj)
    {
        return obj is ObjectCopyStruct1 @class &&
               _privateFields == @class._privateFields &&
               PublicFields == @class.PublicFields &&
               casefields == @class.casefields &&
               PublicProperty == @class.PublicProperty &&
               caseproperty == @class.caseproperty &&
               PrivateProperty == @class.PrivateProperty &&
               SpecialProperty == @class.SpecialProperty;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_privateFields, PublicFields, casefields, PublicProperty, caseproperty, PrivateProperty, SpecialProperty);
    }
}
