using System.Collections.Generic;
using UnityEngine;

namespace Levitan {
    public class RuntimeParser : MonoBehaviour {
        public static Dialog ParseDialogData(DialogData dialogData) {
            string text = dialogData.allText;
            string[] lines = text.Split('\n');
            Dialog res = ParseDialogLines(lines);
            res.Id = dialogData.ID;
            res.requireTags = dialogData.requireTags;
            res.forbiddenTags = dialogData.forbiddenTags;
            res.produceTags = dialogData.produceTags;
            res.SayToOperator = dialogData.SayToOperator;
            res.requirementFrom.roomNumber = dialogData.from;
            res.requirementTo.roomNumber = dialogData.to;
            res.Transitions = dialogData.transitions;
            res.Informations = dialogData.informations;
            return res;
        }

        private static Dialog ParseDialogLines(string[] lines) {
            Dialog dialog = ScriptableObject.CreateInstance<Dialog>();

            dialog.lines = new List<string>();
            dialog.requireTags = new List<string>();
            dialog.bubbleLines = new List<BubbleLine>();
            dialog.requirementFrom = PersonShablon.GenerateEmptyShablon();
            dialog.requirementTo = PersonShablon.GenerateEmptyShablon();

            foreach (string t in lines) {
                if (t.IndexOf("//") == 0)
                    continue;

                try {
                    if (t.Length > 0) {
                        if (t[0] == '-')
                            dialog.lines[^1] += "\n" + t.Remove(0, 1);
                        else
                            dialog.lines.Add(t);
                    }
                }
                catch {
                    Debug.Log("Crash at: dialog, line: " + dialog.lines.Count);
                }
            }

            return dialog;
        }
    }
}