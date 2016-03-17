using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace StackWarden.Core.Serialization
{
    public class PrivateMemberContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var members = properties.Cast<MemberInfo>()
                                    .Union(fields.Cast<MemberInfo>())
                                    .Where(x => !x.Name.Contains("__BackingField"))
                                    .Select(x =>
                                    {
                                        var createdProperty = CreateProperty(x, memberSerialization);
                                        createdProperty.Writable = true;
                                        createdProperty.Readable = true;

                                        return createdProperty;
                                    })
                                    .ToList();

            return members;   
        }

        //protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        //{
        //    var inheritedMembers = new List<MemberInfo>(base.GetSerializableMembers(objectType));
        //    var nonPublicFields = objectType.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField);
        //    var nonPublicProperties = objectType.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty);

        //    var result = inheritedMembers.Concat(nonPublicFields)
        //                                 .Concat(nonPublicProperties)
        //                                 .Distinct()
        //                                 .Select(x => base.CreateProperty(x, Newtonsoft.Json.MemberSerialization))
        //                                 .ToList();

        //    foreach (var currentItem in result)
        //    {
        //        currentItem.
        //    }

        //    return result;
        //}
    }
}