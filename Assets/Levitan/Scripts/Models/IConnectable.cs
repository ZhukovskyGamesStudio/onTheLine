using UnityEngine;

namespace Levitan {
    public interface IConnectable {
        public string ID { get; }

        public string Name { get; }

        public Vector3 Position { get; }

        public bool CanAddConnection(IConnectable start);

        public void AddConnection(Connection connection);

        public Vector3 GetRectEdgeForPosition(Vector3 position);

        public void RemoveConnection(Connection connection);

        public bool HasSameConnection(string target);
    }
}