using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Terrain.Phases
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class PhaseDependencyAttribute : System.Attribute
    {
        private Type type;
        private DependencyOrder order;

        public PhaseDependencyAttribute(Type type, DependencyOrder order)
        {
            this.type = type;
            this.order = order;
        }

        public Type Type
        {
            get => type;
            set => type = value;
        }

        public DependencyOrder Order
        {
            get => order;
            set => order = value;
        }

        public static IEnumerable<PhaseDependencyAttribute> GetDependencies(IGenerationPhase generationPhase)
        {
            return generationPhase.GetType().GetCustomAttributes(true).OfType<PhaseDependencyAttribute>();
        }
    }

    public enum DependencyOrder
    {
        Before,
        After
    }
}