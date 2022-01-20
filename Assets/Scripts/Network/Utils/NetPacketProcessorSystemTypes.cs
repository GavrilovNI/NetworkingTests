using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Utils.NetPacketProcessorSystemTypes
{
    public static class NetPacketProcessorSystemTypes
    {
        public static void RegisterSystemTypes(this NetPacketProcessor netPacketProcessor)
        {
            netPacketProcessor.RegisterNestedType<Type>(
                    (writer, value) =>
                    {
                        writer.Put(value.FullName);
                    },
                    (reader) =>
                    {
                        return Type.GetType(reader.GetString());
                    }
                );
        }

    }
}
