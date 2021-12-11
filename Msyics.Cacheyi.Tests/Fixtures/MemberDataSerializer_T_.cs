using System.Text.Json;
using Xunit.Abstractions;

namespace Msyics.Cacheyi.Tests;

public class MemberDataSerializer<T> : IXunitSerializable
{
    public T? Value { get; private set; }

    public MemberDataSerializer() { }

    public MemberDataSerializer(T value) => Value = value;

    public void Deserialize(IXunitSerializationInfo info) => Value = JsonSerializer.Deserialize<T>(info.GetValue<string>("value"));

    public void Serialize(IXunitSerializationInfo info) => info.AddValue("value", JsonSerializer.Serialize(Value));
}
