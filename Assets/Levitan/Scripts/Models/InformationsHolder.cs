using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Levitan {
    public class InformationsHolder : MonoBehaviour {
        public List<Information> informations;
        public Transform TransitionsHolder;

        public void Init() {
            informations = new List<Information>();
        }

        public List<NewInformationData> CollectInformationsData() {
            List<NewInformationData> datas = new();
            foreach (var information in informations) {
                try {
                    NewInformationData data = new() {
                        thought = information.ThoughtConnected.Name,
                        line = information._line
                    };
                    datas.Add(data);
                }
                catch {
                    AppManager.Instance._LogManager.Log("Information " + information._line + " has no thought attached.");
                }
            }

            return datas;
        }

        public void RedrawConnections() {
            foreach (var information in informations) {
                information.RedrawConnections();
            }
        }

        public void AddEmptyInformation(IDraggable draggable) {
            draggable.transform.SetParent(transform);
            Information information = draggable as Information;
            information.transform.localPosition = new Vector3(0, informations.Count * -1, 0);
            information.Init(draggable._data._dialogData.from, draggable._data._dialogData.allText);
            information.OnDestroyPressed += RemoveInformation;
            informations.Add(information);
            TransitionsHolder.localPosition = new Vector3(0, informations.Count * -1, 0);
        }

        public void RemoveInformation(Information information) {
            informations.Remove(information);
            for (int i = 0; i < informations.Count; i++) {
                informations[i].transform.localPosition = new Vector3(0, i * -1, 0);
            }

            TransitionsHolder.localPosition = new Vector3(0, informations.Count * -1, 0);
            RedrawConnections();
        }

        public void DestroyAll() {
            List<Information> infs = new List<Information>(informations);
            foreach (var information in infs) {
                information.DestroyDraggable();
            }
        }
    }
}