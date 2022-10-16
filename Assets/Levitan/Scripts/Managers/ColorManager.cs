using System;
using UnityEngine;

namespace Levitan {
    public class ColorManager : MonoBehaviour {
        public Color DialogColor, TagColor, ThoughtColor, TransitionColor, InfoColor;

        public Color GetColorByDraggable(DraggableType type) {
            return type switch {
                DraggableType.Dialog => DialogColor,
                DraggableType.Tag => TagColor,
                DraggableType.Thought => ThoughtColor,
                DraggableType.Transition => TransitionColor,
                DraggableType.Information => InfoColor,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}