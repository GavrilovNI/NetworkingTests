using LiteNetLib.Utils;
using UnityEngine;

namespace Network.Utils.NetPacketProcessorUnityTypes
{
    public static class NetPacketProcessorUnityTypes
    {
        public static void RegisterUnityTypes(this NetPacketProcessor netPacketProcessor)
        {
            netPacketProcessor.RegisterNestedType<Vector3>(
                    (writer, value) =>
                    {
                        writer.Put(value.x);
                        writer.Put(value.y);
                        writer.Put(value.z);
                    },
                    (reader) =>
                    {
                        return new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
                    }
                );
        }

    }
}
