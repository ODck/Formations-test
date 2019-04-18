using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    internal interface IUnitsBehaviour
    {
        /// <summary>
        ///     Assign positions for every unit
        /// </summary>
        List<Vector3> CalculatePositions(Leader allLeader);
    }
}