using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Utils.VectorMath
{
    public static class VectorExtensions
    {
        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
        {
            Vector3 result = value;

            if (result.x < min.x)
                result.x = min.x;
            else if (result.x > max.x)
                result.x = max.x;

            if (result.y < min.y)
                result.y = min.y;
            else if (result.y > max.y)
                result.y = max.y;

            if (result.z < min.z)
                result.z = min.z;
            else if (result.z > max.z)
                result.z = max.z;

            return result;
        }
    }
}
